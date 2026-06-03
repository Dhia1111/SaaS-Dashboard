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

    WriteNormal: {
        maxRetries: 3,
        baseDelayMs: 250
    },

    Critical: {
        maxRetries: 5,
        baseDelayMs: 1000
    }
 };

 export async function executeWithRetry(
    operation,
    {
        maxRetries = 0,
        baseDelayMs = 0
    } = {}
) {

    let attempt = 0;

    while (true) {

        try {

            return await operation();

        } catch (error) {

            const status = error?.response?.status;

            // Client errors (except 408 and 429)
            if (
                status >= 400 &&
                status < 500 &&
                status !== 408 &&
                status !== 429
            )
             {
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

            // Handle Retry-After header for 429
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

            await sleep(delay);
        }
    }
 }
 