// ============================================================
// LifeSync Load Tests - Financial Service
// ============================================================

import http from 'k6/http';
import { sleep, group } from 'k6';
import { baseUrl, getProfile, DEFAULT_THRESHOLDS } from '../lib/config.js';
import {
    jsonHeaders, authenticate, randomInt, randomChoice,
    pastDateTime, checkOk, checkCreated, checkSuccess,
} from '../lib/helpers.js';

const FIN_URL   = baseUrl('financial');
const USERS_URL = baseUrl('users');

const TEST_EMAIL    = __ENV.TEST_EMAIL    || 'loadtest@test.com';
const TEST_PASSWORD = __ENV.TEST_PASSWORD || 'LoadTest@123';

const PAYMENT_METHODS   = [1, 2, 3, 4, 5, 6]; // Cash..Other
const TRANSACTION_TYPES = [1, 2];               // Income, Expense

export const options = {
    ...getProfile('average'),
    thresholds: DEFAULT_THRESHOLDS,
    tags: { service: 'financial' },
};

export function setup() {
    const token = authenticate(USERS_URL, TEST_EMAIL, TEST_PASSWORD);
    return { token };
}

export default function (data) {
    const params = jsonHeaders(data.token);
    let categoryId = null;
    let transactionId = null;

    // --- Categories ---
    group('Categories - Create', () => {
        const res = http.post(
            `${FIN_URL}/api/categories`,
            JSON.stringify({
                name: `Cat_${Date.now()}_${randomInt(1, 9999)}`,
                description: 'Load test category',
            }),
            params
        );
        if (checkCreated(res, 'create-category')) {
            try {
                const body = res.json();
                categoryId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('Categories - Get All', () => {
        const res = http.get(`${FIN_URL}/api/categories`, params);
        checkOk(res, 'get-all-categories');
    });

    sleep(0.3);

    group('Categories - Search', () => {
        const res = http.get(`${FIN_URL}/api/categories/search?name=Cat`, params);
        checkOk(res, 'search-categories');
    });

    sleep(0.3);

    // --- Transactions ---
    group('Transactions - Create', () => {
        const res = http.post(
            `${FIN_URL}/api/transactions`,
            JSON.stringify({
                categoryId: categoryId,
                paymentMethod: randomChoice(PAYMENT_METHODS),
                transactionType: randomChoice(TRANSACTION_TYPES),
                amount: {
                    amount: randomInt(10, 5000),
                    currency: 'BRL',
                },
                description: `Transaction_${Date.now()}`,
                transactionDate: pastDateTime(30),
                isRecurring: false,
            }),
            params
        );
        if (checkCreated(res, 'create-transaction')) {
            try {
                const body = res.json();
                transactionId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('Transactions - Get All', () => {
        const res = http.get(`${FIN_URL}/api/transactions`, params);
        checkOk(res, 'get-all-transactions');
    });

    sleep(0.3);

    group('Transactions - Search', () => {
        const res = http.get(`${FIN_URL}/api/transactions/search?transactionType=1`, params);
        checkOk(res, 'search-transactions');
    });

    sleep(0.3);

    if (transactionId) {
        group('Transactions - Get By Id', () => {
            const res = http.get(`${FIN_URL}/api/transactions/${transactionId}`, params);
            checkOk(res, 'get-transaction-by-id');
        });

        sleep(0.2);

        group('Transactions - Update', () => {
            const res = http.put(
                `${FIN_URL}/api/transactions/${transactionId}`,
                JSON.stringify({
                    categoryId: categoryId,
                    paymentMethod: randomChoice(PAYMENT_METHODS),
                    transactionType: randomChoice(TRANSACTION_TYPES),
                    amount: {
                        amount: randomInt(10, 10000),
                        currency: 'BRL',
                    },
                    description: `Updated_${Date.now()}`,
                    transactionDate: pastDateTime(15),
                }),
                params
            );
            checkSuccess(res, 'update-transaction');
        });

        sleep(0.2);

        group('Transactions - Delete', () => {
            const res = http.del(`${FIN_URL}/api/transactions/${transactionId}`, null, params);
            checkSuccess(res, 'delete-transaction');
        });
    }

    // Cleanup category
    if (categoryId) {
        group('Categories - Delete', () => {
            const res = http.del(`${FIN_URL}/api/categories/${categoryId}`, null, params);
            checkSuccess(res, 'delete-category');
        });
    }

    sleep(0.5);
}
