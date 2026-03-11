// ============================================================
// LifeSync Load Tests - Gym Service
// ============================================================

import http from 'k6/http';
import { sleep, group } from 'k6';
import { baseUrl, getProfile, DEFAULT_THRESHOLDS } from '../lib/config.js';
import {
    jsonHeaders, authenticate, randomInt, randomChoice,
    checkOk, checkCreated, checkSuccess,
} from '../lib/helpers.js';

const GYM_URL   = baseUrl('gym');
const USERS_URL = baseUrl('users');

const TEST_EMAIL    = __ENV.TEST_EMAIL    || 'loadtest@test.com';
const TEST_PASSWORD = __ENV.TEST_PASSWORD || 'LoadTest@123';

const MUSCLE_GROUPS   = ['Chest', 'Back', 'Shoulders', 'Biceps', 'Triceps', 'Forearms', 'Abs', 'Quadriceps', 'Hamstrings', 'Calves', 'Glutes', 'LowerBack', 'Traps', 'Core'];
const EXERCISE_TYPES  = ['Strength', 'Hypertrophy', 'Endurance', 'Power', 'Flexibility', 'Cardio', 'HIIT', 'Recovery'];
const EQUIPMENT_TYPES = ['Barbell', 'Dumbbell', 'Machine', 'Cable', 'Bodyweight', 'ResistanceBand', 'Kettlebell', 'Bench'];

export const options = {
    ...getProfile('average'),
    thresholds: DEFAULT_THRESHOLDS,
    tags: { service: 'gym' },
};

export function setup() {
    const token = authenticate(USERS_URL, TEST_EMAIL, TEST_PASSWORD);
    return { token };
}

export default function (data) {
    const params = jsonHeaders(data.token);
    let routineId = null;
    let exerciseId = null;

    // --- Routines ---
    group('Routines - Create', () => {
        const res = http.post(
            `${GYM_URL}/api/routines`,
            JSON.stringify({
                name: `Routine_${Date.now()}_${randomInt(1, 9999)}`,
                description: 'Load test routine',
            }),
            params
        );
        if (checkCreated(res, 'create-routine')) {
            try {
                const body = res.json();
                routineId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('Routines - Get All', () => {
        const res = http.get(`${GYM_URL}/api/routines`, params);
        checkOk(res, 'get-all-routines');
    });

    sleep(0.3);

    group('Routines - Search', () => {
        const res = http.get(`${GYM_URL}/api/routines/search?name=Routine`, params);
        checkOk(res, 'search-routines');
    });

    sleep(0.3);

    // --- Exercises ---
    group('Exercises - Create', () => {
        const res = http.post(
            `${GYM_URL}/api/exercises`,
            JSON.stringify({
                name: `Exercise_${Date.now()}_${randomInt(1, 9999)}`,
                description: 'Load test exercise',
                muscleGroup: randomChoice(MUSCLE_GROUPS),
                exerciseType: randomChoice(EXERCISE_TYPES),
                equipmentType: randomChoice(EQUIPMENT_TYPES),
            }),
            params
        );
        if (checkCreated(res, 'create-exercise')) {
            try {
                const body = res.json();
                exerciseId = body.id || (body.data && body.data.id);
            } catch { /* ignore */ }
        }
    });

    sleep(0.3);

    group('Exercises - Get All', () => {
        const res = http.get(`${GYM_URL}/api/exercises`, params);
        checkOk(res, 'get-all-exercises');
    });

    sleep(0.3);

    // --- Routine Exercises (link exercise to routine) ---
    if (routineId && exerciseId) {
        let routineExerciseId = null;

        group('RoutineExercises - Create', () => {
            const res = http.post(
                `${GYM_URL}/api/routine-exercises`,
                JSON.stringify({
                    routineId: routineId,
                    exerciseId: exerciseId,
                    sets: randomInt(2, 5),
                    repetitions: randomInt(6, 15),
                    restBetweenSets: randomInt(30, 120),
                    recommendedWeight: randomInt(10, 100),
                    instructions: 'Load test instructions',
                }),
                params
            );
            if (checkCreated(res, 'create-routine-exercise')) {
                try {
                    const body = res.json();
                    routineExerciseId = body.id || (body.data && body.data.id);
                } catch { /* ignore */ }
            }
        });

        sleep(0.3);

        group('RoutineExercises - Get All', () => {
            const res = http.get(`${GYM_URL}/api/routine-exercises`, params);
            checkOk(res, 'get-all-routine-exercises');
        });

        sleep(0.2);

        // --- Training Sessions ---
        let sessionId = null;

        group('TrainingSessions - Create', () => {
            const now = new Date();
            const start = new Date(now.getTime() - 3600000); // 1 hour ago
            const res = http.post(
                `${GYM_URL}/api/training-sessions`,
                JSON.stringify({
                    routineId: routineId,
                    startTime: start.toISOString(),
                    endTime: now.toISOString(),
                    notes: 'Load test session',
                }),
                params
            );
            if (checkCreated(res, 'create-session')) {
                try {
                    const body = res.json();
                    sessionId = body.id || (body.data && body.data.id);
                } catch { /* ignore */ }
            }
        });

        sleep(0.3);

        group('TrainingSessions - Get All', () => {
            const res = http.get(`${GYM_URL}/api/training-sessions`, params);
            checkOk(res, 'get-all-sessions');
        });

        // Cleanup
        if (sessionId) {
            http.del(`${GYM_URL}/api/training-sessions/${sessionId}`, null, params);
        }
        if (routineExerciseId) {
            http.del(`${GYM_URL}/api/routine-exercises/${routineExerciseId}`, null, params);
        }
    }

    // Cleanup
    if (exerciseId) {
        http.del(`${GYM_URL}/api/exercises/${exerciseId}`, null, params);
    }
    if (routineId) {
        http.del(`${GYM_URL}/api/routines/${routineId}`, null, params);
    }

    sleep(0.5);
}
