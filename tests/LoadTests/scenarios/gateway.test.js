// ============================================================
// LifeSync Load Tests - YARP API Gateway (Cross-Service)
// ============================================================
// Tests the gateway routing, latency overhead, and end-to-end
// flow across multiple microservices.
// ============================================================

import http from 'k6/http';
import { sleep, group } from 'k6';
import { SERVICES, getProfile, DEFAULT_THRESHOLDS } from '../lib/config.js';
import {
    jsonHeaders, randomInt, randomChoice,
    futureDate, checkOk, checkCreated, checkSuccess,
} from '../lib/helpers.js';

const GW = SERVICES.gateway; // Always hit the gateway

const TEST_EMAIL    = __ENV.TEST_EMAIL    || 'loadtest@test.com';
const TEST_PASSWORD = __ENV.TEST_PASSWORD || 'LoadTest@123';

export const options = {
    ...getProfile('average'),
    thresholds: {
        ...DEFAULT_THRESHOLDS,
        // Gateway adds routing overhead — slightly relaxed thresholds
        http_req_duration: ['p(95)<800', 'p(99)<2000'],
    },
    tags: { service: 'gateway' },
};

export function setup() {
    // Login through the gateway auth route
    const loginRes = http.post(
        `${GW}/auth/login`,
        JSON.stringify({ email: TEST_EMAIL, password: TEST_PASSWORD }),
        jsonHeaders()
    );

    let token = null;
    try {
        const body = loginRes.json();
        token = body.accessToken || (body.data && body.data.accessToken);
    } catch { /* ignore */ }

    if (!token) {
        console.error('Gateway setup: login failed');
    }
    return { token };
}

export default function (data) {
    const params = jsonHeaders(data.token);

    // --- Auth through gateway ---
    group('Gateway - Auth Login', () => {
        const res = http.post(
            `${GW}/auth/login`,
            JSON.stringify({ email: TEST_EMAIL, password: TEST_PASSWORD }),
            jsonHeaders()
        );
        checkOk(res, 'gw-login');
    });

    sleep(0.3);

    // --- TaskManager through gateway ---
    group('Gateway - TaskManager: Get Tasks', () => {
        const res = http.get(`${GW}/taskmanager-service/api/task-items`, params);
        checkOk(res, 'gw-get-tasks');
    });

    sleep(0.3);

    group('Gateway - TaskManager: Create Task', () => {
        const res = http.post(
            `${GW}/taskmanager-service/api/task-items`,
            JSON.stringify({
                title: `GW_Task_${Date.now()}`,
                description: 'Gateway load test',
                priority: randomChoice([1, 2, 3, 4]),
                dueDate: futureDate(30),
                taskLabelsId: [],
            }),
            params
        );
        checkCreated(res, 'gw-create-task');
    });

    sleep(0.3);

    // --- Financial through gateway ---
    group('Gateway - Financial: Get Transactions', () => {
        const res = http.get(`${GW}/financial-service/api/transactions`, params);
        checkOk(res, 'gw-get-transactions');
    });

    sleep(0.3);

    group('Gateway - Financial: Get Categories', () => {
        const res = http.get(`${GW}/financial-service/api/categories`, params);
        checkOk(res, 'gw-get-categories');
    });

    sleep(0.3);

    // --- Gym through gateway ---
    group('Gateway - Gym: Get Routines', () => {
        const res = http.get(`${GW}/gym-service/api/routines`, params);
        checkOk(res, 'gw-get-routines');
    });

    sleep(0.3);

    group('Gateway - Gym: Get Exercises', () => {
        const res = http.get(`${GW}/gym-service/api/exercises`, params);
        checkOk(res, 'gw-get-exercises');
    });

    sleep(0.3);

    // --- Nutrition through gateway ---
    group('Gateway - Nutrition: Get Diaries', () => {
        const res = http.get(`${GW}/nutrition-service/api/diaries`, params);
        checkOk(res, 'gw-get-diaries');
    });

    sleep(0.3);

    group('Gateway - Nutrition: Get Foods', () => {
        const res = http.get(`${GW}/nutrition-service/api/foods`, params);
        checkOk(res, 'gw-get-foods');
    });

    sleep(0.3);

    // --- Users through gateway ---
    group('Gateway - Users: Get Users', () => {
        const res = http.get(`${GW}/users-service/api/users`, params);
        checkOk(res, 'gw-get-users');
    });

    sleep(0.5);
}
