// API Service for authentication
const auth = {
    token: null,

    setToken(token) {
        this.token = token;
        localStorage.setItem('jwt_token', token);
    },

    getToken() {
        if (!this.token) {
            this.token = localStorage.getItem('jwt_token');
        }
        return this.token;
    },

    clearToken() {
        this.token = null;
        localStorage.removeItem('jwt_token');
    },

    async request(url, options = {}) {
        const token = this.getToken();
        if (token) {
            options.headers = {
                ...options.headers,
                'Authorization': `Bearer ${token}`
            };
        }
        const response = await fetch(url, {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            }
        });
        if (response.status === 401) {
            this.clearToken();
            window.location.href = '/login';
            return;
        }
        return response;
    },

    async login(username, password, rememberMe = false) {
        try {
            const response = await this.request('/api/login', {
                method: 'POST',
                body: JSON.stringify({ username, password, rememberMe })
            });

            if (response.ok) {
                const data = await response.json();
                this.setToken(data.token);
                return { success: true };
            } else {
                const error = await response.json();
                return { success: false, error: error.message || 'Login failed' };
            }
        } catch (error) {
            return { success: false, error: 'Network error occurred' };
        }
    },

    async register(username, email, password) {
        try {
            const response = await this.request('/api/login/register', {
                method: 'POST',
                body: JSON.stringify({ username, email, password })
            });

            if (response.ok) {
                const data = await response.json();
                this.setToken(data.token);
                return { success: true };
            } else {
                const error = await response.json();
                return { success: false, error: error.message || 'Registration failed' };
            }
        } catch (error) {
            return { success: false, error: 'Network error occurred' };
        }
    },

    async logout() {
        this.clearToken();
        window.location.href = '/login';
    },

    init() {
        // Check if user is authenticated
        const token = this.getToken();
        if (!token && !window.location.pathname.includes('/login') && !window.location.pathname.includes('/register')) {
            window.location.href = '/login';
        }
    }
};
