// ============================================================
// LifeSync Load Tests - Users Service
// ============================================================

import http from 'k6/http';
import { sleep, group } from 'k6';
import { baseUrl, getProfile, DEFAULT_THRESHOLDS, USE_GATEWAY } from '../lib/config.js';
import {
    jsonHeaders, authenticate, registerUser,
    uniqueEmail, checkOk, checkCreated, checkSuccess,
} from '../lib/helpers.js';

const USERS_URL = baseUrl('users');

// Use env vars or defaults for test credentials
const TEST_EMAIL    = __ENV.TEST_EMAIL    || 'loadtest@test.com';
const TEST_PASSWORD = __ENV.TEST_PASSWORD || 'LoadTest@123';

export const options = {
    ...getProfile('average'),
    thresholds: DEFAULT_THRESHOLDS,
    tags: { service: 'users' },
};

// Setup: register a shared test user
export function setup() {
    const regRes = registerUser(USERS_URL, {
        firstName: 'Load',
        lastName: 'Tester',
        email: TEST_EMAIL,
        password: TEST_PASSWORD,
    });

    // User might already exist — that's fine
    const token = authenticate(USERS_URL, TEST_EMAIL, TEST_PASSWORD);
    if (!token) {
        console.error('Setup failed: could not authenticate test user');
    }
    return { token };
}

export default function (data) {
    const params = jsonHeaders(data.token);

    group('Auth - Register', () => {
        const email = uniqueEmail('reg');
        const res = registerUser(USERS_URL, {
            firstName: 'User',
            lastName: `Test_${Date.now()}`,
            email: email,
            password: 'Test@12345',
        });
        checkCreated(res, 'register');
    });

    sleep(0.5);

    group('Auth - Login', () => {
        const res = http.post(
            `${USERS_URL}/api/auth/login`,
            JSON.stringify({ email: TEST_EMAIL, password: TEST_PASSWORD }),
            jsonHeaders()
        );
        checkOk(res, 'login');
    });

    sleep(0.5);

    group('Users - Get All', () => {
        const res = http.get(`${USERS_URL}/api/users`, params);
        checkOk(res, 'get-all-users');
    });

    sleep(0.3);
}
