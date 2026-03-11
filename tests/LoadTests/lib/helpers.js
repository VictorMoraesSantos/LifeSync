// ============================================================
// LifeSync Load Tests - Helper Functions
// ============================================================

import http from 'k6/http';
import { check, fail } from 'k6';

/**
 * Standard JSON headers with optional auth token.
 */
export function jsonHeaders(token) {
    const headers = { 'Content-Type': 'application/json' };
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }
    return { headers };
}

/**
 * Authenticate and return the access token.
 */
export function authenticate(usersBaseUrl, email, password) {
    const res = http.post(
        `${usersBaseUrl}/api/auth/login`,
        JSON.stringify({ email, password }),
        jsonHeaders()
    );

    const success = check(res, {
        'login: status 200': (r) => r.status === 200,
        'login: has accessToken': (r) => {
            try {
                const body = r.json();
                return body && (body.accessToken || (body.data && body.data.accessToken));
            } catch {
                return false;
            }
        },
    });

    if (!success) {
        console.error(`Login failed: ${res.status} - ${res.body}`);
        return null;
    }

    const body = res.json();
    return body.accessToken || (body.data && body.data.accessToken);
}

/**
 * Register a new user and return the response.
 */
export function registerUser(usersBaseUrl, userData) {
    return http.post(
        `${usersBaseUrl}/api/auth/register`,
        JSON.stringify(userData),
        jsonHeaders()
    );
}

/**
 * Generate a unique email for load test users.
 */
export function uniqueEmail(prefix) {
    const ts = Date.now();
    const rand = Math.floor(Math.random() * 100000);
    return `${prefix || 'loadtest'}_${ts}_${rand}@test.com`;
}

/**
 * Generate a random integer between min and max (inclusive).
 */
export function randomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

/**
 * Pick a random element from an array.
 */
export function randomChoice(arr) {
    return arr[Math.floor(Math.random() * arr.length)];
}

/**
 * Generate a random date string (YYYY-MM-DD) within the next N days.
 */
export function futureDate(daysAhead) {
    const d = new Date();
    d.setDate(d.getDate() + randomInt(1, daysAhead || 30));
    return d.toISOString().split('T')[0];
}

/**
 * Generate a random past datetime ISO string within the last N days.
 */
export function pastDateTime(daysBack) {
    const d = new Date();
    d.setDate(d.getDate() - randomInt(0, daysBack || 30));
    return d.toISOString();
}

/**
 * Standard check for successful creation (201).
 */
export function checkCreated(res, label) {
    return check(res, {
        [`${label}: status 201`]: (r) => r.status === 201,
    });
}

/**
 * Standard check for successful read (200).
 */
export function checkOk(res, label) {
    return check(res, {
        [`${label}: status 200`]: (r) => r.status === 200,
    });
}

/**
 * Standard check for successful update/delete (200 or 204).
 */
export function checkSuccess(res, label) {
    return check(res, {
        [`${label}: status 2xx`]: (r) => r.status >= 200 && r.status < 300,
    });
}
