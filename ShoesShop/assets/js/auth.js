// auth.js
class AuthManager {
    static getCurrentUser() {
        return JSON.parse(localStorage.getItem('kinetic_currentUser'));
    }

    static login(email, password) {
        const users = window.db.getUsers();
        const user = users.find(u => u.email === email && u.pass === password);
        if (user) {
            localStorage.setItem('kinetic_currentUser', JSON.stringify({ email: user.email, name: user.name, role: user.role }));
            return true;
        }
        return false;
    }

    static logout() {
        localStorage.removeItem('kinetic_currentUser');
        window.location.href = 'login.html';
    }

    static requireLogin() {
        if (!this.getCurrentUser()) {
            const returnUrl = encodeURIComponent(window.location.href);
            window.location.href = `login.html?returnUrl=${returnUrl}`;
        }
    }

    static requireRole(role) {
        this.requireLogin();
        const user = this.getCurrentUser();
        if (user && user.role !== role) {
            alert('Access Denied');
            window.location.href = 'index.html';
        }
    }
}

window.Auth = AuthManager;

// Initial UI updates based on Auth state if elements exist
document.addEventListener('DOMContentLoaded', () => {
    const user = AuthManager.getCurrentUser();
    const loginLink = document.getElementById('login-nav-link');
    const profileLink = document.getElementById('profile-nav-link');
    if (user) {
        if (loginLink) loginLink.style.display = 'none';
        if (profileLink) {
            profileLink.style.display = 'inline-block';
            profileLink.textContent = `Hi, ${user.name}`;
        }
    } else {
        if (loginLink) loginLink.style.display = 'inline-block';
        if (profileLink) profileLink.style.display = 'none';
    }
});
