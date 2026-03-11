// ============================================================
// LifeSync Load Tests - Nutrition Service
// ============================================================

import http from 'k6/http';
import { sleep, group } from 'k6';
import { baseUrl, getProfile, DEFAULT_THRESHOLDS } from '../lib/config.js';
import {
    jsonHeaders, authenticate, randomInt, futureDate,
    checkOk, checkCreated, checkSuccess,
} from '../lib/helpers.js';

const NUT_URL   = baseUrl('nutrition');
const USERS_URL = baseUrl('users');

const TEST_EMAIL    = __ENV.TEST_EMAIL    || 'loadtest@test.com';
const TEST_PASSWORD = __ENV.TEST_PASSWORD || 'LoadTest@123';

export const options = {
    ...getProfile('average'),
    thresholds: DEFAULT_THRESHOLDS,
    tags: { service: 'nutrition' },
};

export function setup() {
    const token = authenticate(USERS_URL, TEST_EMAIL, TEST_PASSWORD);
    return { token };
}

export default function (data) {
    const params = jsonHeaders(data.token);
    let diaryId = null;
    let mealId = null;

    // --- Diaries ---
    group('Diaries - Create', () => {
        const res = http.post(
            `${NUT_URL}/api/diaries`,
            JSON.stringify({
                date: futureDate(30),
            }),
            params
        );
        if (checkCreated(res, 'create-diary')) {
            try {
                const body = res.json();
                diaryId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('Diaries - Get All', () => {
        const res = http.get(`${NUT_URL}/api/diaries`, params);
        checkOk(res, 'get-all-diaries');
    });

    sleep(0.3);

    // --- Meals ---
    if (diaryId) {
        group('Meals - Create', () => {
            const res = http.post(
                `${NUT_URL}/api/meals`,
                JSON.stringify({
                    diaryId: diaryId,
                    name: `Meal_${Date.now()}_${randomInt(1, 9999)}`,
                    description: 'Load test meal',
                }),
                params
            );
            if (checkCreated(res, 'create-meal')) {
                try {
                    const body = res.json();
                    mealId = body.id || (body.data && body.data.id);
                } catch { /* ignore */ }
            }
        });

        sleep(0.3);

        group('Meals - Get All', () => {
            const res = http.get(`${NUT_URL}/api/meals`, params);
            checkOk(res, 'get-all-meals');
        });

        sleep(0.3);

        group('Meals - Get By Diary', () => {
            const res = http.get(`${NUT_URL}/api/meals/diary/${diaryId}`, params);
            checkOk(res, 'get-meals-by-diary');
        });

        sleep(0.3);
    }

    // --- Foods (read-only, seeded data) ---
    group('Foods - Get All', () => {
        const res = http.get(`${NUT_URL}/api/foods`, params);
        checkOk(res, 'get-all-foods');
    });

    sleep(0.3);

    group('Foods - Search', () => {
        const res = http.get(`${NUT_URL}/api/foods/search?name=rice`, params);
        checkOk(res, 'search-foods');
    });

    sleep(0.3);

    // --- Daily Progresses ---
    let progressId = null;

    group('DailyProgresses - Create', () => {
        const res = http.post(
            `${NUT_URL}/api/daily-progresses`,
            JSON.stringify({
                caloriesConsumed: randomInt(500, 3000),
                liquidsConsumedMl: randomInt(500, 3000),
                goal: randomInt(1500, 2500),
            }),
            params
        );
        if (checkCreated(res, 'create-progress')) {
            try {
                const body = res.json();
                progressId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('DailyProgresses - Get All', () => {
        const res = http.get(`${NUT_URL}/api/daily-progresses`, params);
        checkOk(res, 'get-all-progresses');
    });

    sleep(0.3);

    // --- Liquid Types ---
    let liquidTypeId = null;

    group('LiquidTypes - Create', () => {
        const res = http.post(
            `${NUT_URL}/api/liquid-types`,
            JSON.stringify({
                name: `Liquid_${Date.now()}_${randomInt(1, 9999)}`,
            }),
            params
        );
        if (checkCreated(res, 'create-liquid-type')) {
            try {
                const body = res.json();
                liquidTypeId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('LiquidTypes - Get All', () => {
        const res = http.get(`${NUT_URL}/api/liquid-types`, params);
        checkOk(res, 'get-all-liquid-types');
    });

    sleep(0.3);

    // --- Liquids ---
    if (diaryId && liquidTypeId) {
        let liquidId = null;

        group('Liquids - Create', () => {
            const res = http.post(
                `${NUT_URL}/api/liquids`,
                JSON.stringify({
                    liquidTypeId: liquidTypeId,
                    quantity: randomInt(100, 500),
                }),
                params
            );
            if (checkCreated(res, 'create-liquid')) {
                try {
                    const body = res.json();
                    liquidId = body.id || (body.data && body.data.id);
                } catch { /* ignore */ }
            }
        });

        sleep(0.2);

        // Cleanup liquid
        if (liquidId) {
            http.del(`${NUT_URL}/api/liquids/${liquidId}`, null, params);
        }
    }

    // Cleanup
    if (progressId) {
        http.del(`${NUT_URL}/api/daily-progresses/${progressId}`, null, params);
    }
    if (mealId) {
        http.del(`${NUT_URL}/api/meals/${mealId}`, null, params);
    }
    if (liquidTypeId) {
        http.del(`${NUT_URL}/api/liquid-types/${liquidTypeId}`, null, params);
    }
    if (diaryId) {
        http.del(`${NUT_URL}/api/diaries/${diaryId}`, null, params);
    }

    sleep(0.5);
}
