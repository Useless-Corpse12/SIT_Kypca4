import api from '/src/api/api.js';

const KEYS = {
    ACCESS: 'accessToken',
    REFRESH: 'refreshToken',
    USER: 'user',
};

function decodeToken(token) {
    const payload = token.split('.')[1];
    const decoded = JSON.parse(atob(payload));

    return {
        id: decoded.sub || decoded.nameid,
        email: decoded.email || decoded.unique_name,
        role: decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
    };
}

function getFingerprint() {
    return navigator.userAgent + '|' + screen.width + '|' + screen.height;
}

function saveTokens(accessToken, refreshToken) {
    localStorage.setItem(KEYS.ACCESS, accessToken);
    localStorage.setItem(KEYS.REFRESH, refreshToken);
    localStorage.setItem(KEYS.USER, JSON.stringify(decodeToken(accessToken)));
}

function clearTokens() {
    Object.values(KEYS).forEach((key) => localStorage.removeItem(key));
}

export function getCurrentUser() {
    const user = localStorage.getItem(KEYS.USER);
    return user ? JSON.parse(user) : null;
}

export function isAuthenticated() {
    return !!localStorage.getItem(KEYS.ACCESS);
}

export function hasRole(role) {
    const user = getCurrentUser();
    return user?.role === role;
}

export async function login(email, password) {
    const res = await api.post('/auth/login', {
        email,
        password,
        fingerprint: getFingerprint()});

    const { status, refreshToken, accessToken } = res.data;

    if (status === 'Success' && refreshToken && accessToken) {
        saveTokens(accessToken, refreshToken);
        return { success: true, user: getCurrentUser() };
    }

    if (status === 'Requires2FA') {
        return { success: false, requires2FA: true };
    }

    if (status === 'Unverified') {
        return { success: false, unverified: true };
    }

    return { success: false, message: res.data.message };
}

export async function register(login, email, password) {
    const res = await api.post('/auth/registration', { login, email, password });
    return { success: res.data.isSuccess, userId: res.data.userId };
}

export async function verifyEmail(userId, code) {
    const res = await api.post('/auth/verify-email', { userId, code });
    return { success: res.data.isSuccess };
}

export async function refreshAccessToken() {
    const refreshToken = localStorage.getItem(KEYS.REFRESH);
    if (!refreshToken) return false;

    try {
        const res = await api.post('/auth/refresh', {
            refreshToken,
            fingerprint: getFingerprint(),
        });
        const { accessToken, refreshToken: newRefresh } = res.data;
        saveTokens(accessToken, newRefresh);
        return true;
    } catch {
        clearTokens();
        return false;
    }
}

export async function logout() {
    const refreshToken = localStorage.getItem(KEYS.REFRESH);
    try {
        await api.post('/auth/logout', { refreshToken });
    } catch (ex) {
        console.error(ex);
    } finally {
        clearTokens();
    }

}

export async function resendCode(userId) {
    const res = await api.post('/auth/resend-code', { userId });
    return res.data;
}

export { clearTokens, getFingerprint };