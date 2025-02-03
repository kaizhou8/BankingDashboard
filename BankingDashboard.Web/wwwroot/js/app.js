// API Service
const api = {
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
            window.location.href = '/login.html';
            return;
        }
        return response;
    },

    async login(username, password) {
        const response = await this.request('/api/login/login', {
            method: 'POST',
            body: JSON.stringify({ username, password })
        });
        if (response.ok) {
            const data = await response.json();
            this.setToken(data.token);
            return true;
        }
        return false;
    },

    async register(username, email, password) {
        const response = await this.request('/api/login/register', {
            method: 'POST',
            body: JSON.stringify({ username, email, password })
        });
        if (response.ok) {
            const data = await response.json();
            this.setToken(data.token);
            return true;
        }
        return false;
    },

    async getDashboardData() {
        const response = await this.request('/api/dashboard');
        if (response.ok) {
            return await response.json();
        }
        return null;
    }
};

// Dashboard Component
const dashboard = {
    async init() {
        if (!api.getToken()) {
            window.location.href = '/login.html';
            return;
        }

        const data = await api.getDashboardData();
        if (data) {
            this.render(data);
        }
    },

    render(data) {
        document.querySelector('.welcome-section').innerHTML = `
            <h2>Welcome back, ${data.userName}!</h2>
            <p>Last login: ${data.lastLoginTime}</p>
        `;

        const accountsHtml = data.accounts.map(account => `
            <div class="account-summary">
                <h2>${account.accountType} Account</h2>
                <div class="account-balance">$${account.balance.toFixed(2)}</div>
                <div class="account-details">
                    <div class="detail-item">
                        <h3>Account Number</h3>
                        <p>${account.accountNumber}</p>
                    </div>
                </div>
            </div>
        `).join('');
        document.querySelector('.accounts').innerHTML = accountsHtml;

        const transactionsHtml = data.recentTransactions.map(transaction => `
            <li class="transaction-item">
                <div class="transaction-info">
                    <span class="transaction-date">${new Date(transaction.transactionDate).toLocaleDateString()}</span>
                    <span class="transaction-description">${transaction.description}</span>
                </div>
                <span class="transaction-amount ${transaction.amount > 0 ? 'positive' : 'negative'}">
                    ${transaction.amount > 0 ? '+' : ''}$${Math.abs(transaction.amount).toFixed(2)}
                </span>
            </li>
        `).join('');
        document.querySelector('.transaction-list').innerHTML = transactionsHtml;
    }
};
