// Dashboard functionality
const dashboard = {
    async init() {
        if (!auth.getToken()) {
            window.location.href = '/login';
            return;
        }

        try {
            const response = await auth.request('/api/dashboard');
            if (response.ok) {
                const data = await response.json();
                this.render(data);
            } else {
                console.error('Failed to load dashboard data');
            }
        } catch (error) {
            console.error('Error loading dashboard:', error);
        }
    },

    render(data) {
        // Update welcome section
        document.querySelector('.welcome-section').innerHTML = `
            <h2>Welcome back, ${data.userName}!</h2>
            <p>Last login: ${data.lastLoginTime}</p>
        `;

        // Update accounts section
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

        // Update transactions section
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
