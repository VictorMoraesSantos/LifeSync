// ============================================================
// LifeSync Load Tests - TaskManager Service
// ============================================================

import http from 'k6/http';
import { sleep, group } from 'k6';
import { baseUrl, getProfile, DEFAULT_THRESHOLDS } from '../lib/config.js';
import {
    jsonHeaders, authenticate, randomInt, randomChoice,
    futureDate, checkOk, checkCreated, checkSuccess,
} from '../lib/helpers.js';

const TM_URL    = baseUrl('taskManager');
const USERS_URL = baseUrl('users');

const TEST_EMAIL    = __ENV.TEST_EMAIL    || 'loadtest@test.com';
const TEST_PASSWORD = __ENV.TEST_PASSWORD || 'LoadTest@123';

const PRIORITIES = [1, 2, 3, 4]; // Low, Medium, High, Urgent
const STATUSES   = [1, 2, 3];    // Pending, InProgress, Completed

export const options = {
    ...getProfile('average'),
    thresholds: DEFAULT_THRESHOLDS,
    tags: { service: 'taskmanager' },
};

export function setup() {
    const token = authenticate(USERS_URL, TEST_EMAIL, TEST_PASSWORD);
    if (!token) {
        console.error('Setup failed: could not authenticate');
    }
    return { token };
}

export default function (data) {
    const params = jsonHeaders(data.token);
    let createdTaskId = null;
    let createdLabelId = null;

    // --- Task Labels ---
    group('TaskLabels - Create', () => {
        const res = http.post(
            `${TM_URL}/api/task-labels`,
            JSON.stringify({
                name: `Label_${Date.now()}_${randomInt(1, 9999)}`,
                labelColor: randomInt(0, 8),
            }),
            params
        );
        if (checkCreated(res, 'create-label')) {
            try {
                const body = res.json();
                createdLabelId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('TaskLabels - Get All', () => {
        const res = http.get(`${TM_URL}/api/task-labels`, params);
        checkOk(res, 'get-all-labels');
    });

    sleep(0.3);

    // --- Task Items ---
    group('TaskItems - Create', () => {
        const payload = {
            title: `Task_${Date.now()}_${randomInt(1, 9999)}`,
            description: 'Load test task description',
            priority: randomChoice(PRIORITIES),
            dueDate: futureDate(30),
            taskLabelsId: createdLabelId ? [createdLabelId] : [],
        };
        const res = http.post(
            `${TM_URL}/api/task-items`,
            JSON.stringify(payload),
            params
        );
        if (checkCreated(res, 'create-task')) {
            try {
                const body = res.json();
                createdTaskId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('TaskItems - Get All', () => {
        const res = http.get(`${TM_URL}/api/task-items`, params);
        checkOk(res, 'get-all-tasks');
    });

    sleep(0.3);

    group('TaskItems - Search', () => {
        const res = http.get(`${TM_URL}/api/task-items/search?priority=2&status=1`, params);
        checkOk(res, 'search-tasks');
    });

    sleep(0.3);

    if (createdTaskId) {
        group('TaskItems - Get By Id', () => {
            const res = http.get(`${TM_URL}/api/task-items/${createdTaskId}`, params);
            checkOk(res, 'get-task-by-id');
        });

        sleep(0.2);

        group('TaskItems - Update', () => {
            const res = http.put(
                `${TM_URL}/api/task-items/${createdTaskId}`,
                JSON.stringify({
                    title: `Updated_${Date.now()}`,
                    description: 'Updated by load test',
                    status: randomChoice(STATUSES),
                    priority: randomChoice(PRIORITIES),
                    dueDate: futureDate(60),
                }),
                params
            );
            checkSuccess(res, 'update-task');
        });

        sleep(0.2);

        group('TaskItems - Delete', () => {
            const res = http.del(`${TM_URL}/api/task-items/${createdTaskId}`, null, params);
            checkSuccess(res, 'delete-task');
        });
    }

    // Cleanup label
    if (createdLabelId) {
        group('TaskLabels - Delete', () => {
            const res = http.del(`${TM_URL}/api/task-labels/${createdLabelId}`, null, params);
            checkSuccess(res, 'delete-label');
        });
    }

    sleep(0.5);
}
