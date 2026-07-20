 function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
 }

 function isRetryableStatus(status) {

    if (!status)
        return true; // network error

    switch (status) {

        case 408: // Request Timeout
        case 429: // Too Many Requests
        case 500: // Internal Server Error
        case 502: // Bad Gateway
        case 503: // Service Unavailable
        case 504: // Gateway Timeout
            return true;

        default:
            return false;
    }
 }

 export const RetryPolicies = {

    None: {
        maxRetries: 0,
        baseDelayMs: 0
    },

    ReadFast: {
        maxRetries: 1,
        baseDelayMs: 300
    },

    Write: {
        maxRetries: 3,
        baseDelayMs: 250
    },

    Critical: {
        maxRetries: 2,
        baseDelayMs: 100
    }
 };

  let currentController = null;

function createRetryExecutor() {

    return async function executeWithRetry(
        operation,
        {
            maxRetries = 0,
            baseDelayMs = 0
        } = {}
    ) {

        // Cancel previous request
        currentController?.abort();

        const controller = new AbortController();
        currentController = controller;

        let attempt = 0;

        while (true) {

            try {

                return await operation(controller.signal);

            } catch (error) {

                // Request was intentionally cancelled
                if (
                    error.name === "AbortError" ||
                    error.code === "ERR_CANCELED"
                ) {
                    throw error;
                }
                const isNetworkError = !error?.response ||error.code === "ERR_NETWORK";
if (isNetworkError) {

    if (attempt >= 1) {
        throw error;
    }

    attempt++;

    await sleep(1000);

    continue;
}

                const status = error?.response?.status;

                if (
                    status >= 400 &&
                    status < 500 &&
                    status !== 408 &&
                    status !== 429
                ) {
                    throw error;
                }

                if (!isRetryableStatus(status)) {
                    throw error;
                }

                if (attempt >= maxRetries) {
                    throw error;
                }

                attempt++;

                let delay;

                if (status === 429) {

                    const retryAfterHeader =
                        error.response?.headers?.["retry-after"];

                    const retryAfterSeconds =
                        Number(retryAfterHeader);

                    delay =
                        Number.isFinite(retryAfterSeconds)
                            ? retryAfterSeconds * 1000
                            : baseDelayMs * Math.pow(2, attempt - 1);

                } else {

                    delay =
                        baseDelayMs *
                        Math.pow(2, attempt - 1);
                }

                // Add jitter (0-30%)
                const jitter =
                    delay * (Math.random() * 0.3);

                delay += jitter;

                await sleep(delay);
            }
        }
    };
}

export const executeWithRetry = createRetryExecutor();
 