// ============================================================
// LifeSync Load Tests - Shared Configuration
// ============================================================

// Base URLs for each microservice (direct access)
export const SERVICES = {
    gateway:      __ENV.GATEWAY_URL      || 'http://localhost:5006',
    users:        __ENV.USERS_URL        || 'http://localhost:5001',
    taskManager:  __ENV.TASKMANAGER_URL  || 'http://localhost:5000',
    financial:    __ENV.FINANCIAL_URL    || 'http://localhost:5003',
    gym:          __ENV.GYM_URL          || 'http://localhost:5004',
    nutrition:    __ENV.NUTRITION_URL    || 'http://localhost:5002',
    notification: __ENV.NOTIFICATION_URL || 'http://localhost:5126',
};

// Whether to route through the YARP API Gateway
export const USE_GATEWAY = typeof __ENV !== 'undefined' && (__ENV.USE_GATEWAY || 'false') === 'true';

// Gateway route prefixes (matches YARP config)
export const GATEWAY_PREFIXES = {
    users:       '/users-service',
    auth:        '/auth',
    taskManager: '/taskmanager-service',
    financial:   '/financial-service',
    gym:         '/gym-service',
    nutrition:   '/nutrition-service',
};

/**
 * Returns the base URL for a given service.
 * If USE_GATEWAY is true, routes through the YARP gateway.
 */
export function baseUrl(service) {
    if (USE_GATEWAY) {
        return SERVICES.gateway + (GATEWAY_PREFIXES[service] || '');
    }
    return SERVICES[service];
}

// ============================================================
// Load test profiles
// ============================================================

export const PROFILES = {
    // Smoke test: verify endpoints work under minimal load
    smoke: {
        vus: 1,
        duration: '30s',
    },

    // Low load: baseline performance
    low: {
        stages: [
            { duration: '30s', target: 10 },
            { duration: '1m',  target: 10 },
            { duration: '30s', target: 0 },
        ],
    },

    // Average load: typical usage
    average: {
        stages: [
            { duration: '1m',  target: 50 },
            { duration: '3m',  target: 50 },
            { duration: '1m',  target: 0 },
        ],
    },

    // Stress test: find breaking points
    stress: {
        stages: [
            { duration: '1m',  target: 50 },
            { duration: '2m',  target: 100 },
            { duration: '2m',  target: 200 },
            { duration: '2m',  target: 300 },
            { duration: '2m',  target: 0 },
        ],
    },

    // Spike test: sudden burst of traffic
    spike: {
        stages: [
            { duration: '30s', target: 10 },
            { duration: '10s', target: 500 },
            { duration: '1m',  target: 500 },
            { duration: '10s', target: 10 },
            { duration: '30s', target: 0 },
        ],
    },

    // Soak test: sustained load over time
    soak: {
        stages: [
            { duration: '2m',  target: 100 },
            { duration: '30m', target: 100 },
            { duration: '2m',  target: 0 },
        ],
    },
};

/**
 * Returns k6 options for a given profile name.
 * Reads from K6_PROFILE env var or defaults to 'average'.
 */
export function getProfile(defaultProfile) {
    const name = __ENV.K6_PROFILE || defaultProfile || 'average';
    const profile = PROFILES[name];
    if (!profile) {
        throw new Error(`Unknown profile: ${name}. Available: ${Object.keys(PROFILES).join(', ')}`);
    }
    return profile;
}

// ============================================================
// Thresholds (shared across all tests)
// ============================================================

export const DEFAULT_THRESHOLDS = {
    http_req_duration: ['p(95)<500', 'p(99)<1500'],   // 95% < 500ms, 99% < 1.5s
    http_req_failed:   ['rate<0.05'],                   // < 5% errors
    http_reqs:         ['rate>10'],                      // > 10 rps minimum
};

export const STRICT_THRESHOLDS = {
    http_req_duration: ['p(95)<200', 'p(99)<500'],
    http_req_failed:   ['rate<0.01'],
    http_reqs:         ['rate>50'],
};
