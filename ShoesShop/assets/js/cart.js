// cart.js
class CartManager {
    static getCart() {
        return JSON.parse(localStorage.getItem('kinetic_cart')) || [];
    }

    static saveCart(cart) {
        localStorage.setItem('kinetic_cart', JSON.stringify(cart));
        this.updateUI();
    }

    static addItem(product, quantity = 1) {
        const cart = this.getCart();
        const existing = cart.find(item => item.id === product.id);
        if (existing) {
            existing.quantity += quantity;
        } else {
            cart.push({ ...product, quantity });
        }
        this.saveCart(cart);
        // Dispatch event for other listeners
        window.dispatchEvent(new Event('cartUpdated'));
    }

    static removeItem(productId) {
        let cart = this.getCart();
        cart = cart.filter(item => item.id !== productId);
        this.saveCart(cart);
        window.dispatchEvent(new Event('cartUpdated'));
    }

    static updateQuantity(productId, quantity) {
        const cart = this.getCart();
        const existing = cart.find(item => item.id === productId);
        if (existing) {
            existing.quantity = parseInt(quantity);
            if (existing.quantity <= 0) {
                this.removeItem(productId);
                return;
            }
        }
        this.saveCart(cart);
        window.dispatchEvent(new Event('cartUpdated'));
    }

    static clearCart() {
        localStorage.removeItem('kinetic_cart');
        this.saveCart([]);
        window.dispatchEvent(new Event('cartUpdated'));
    }

    static updateUI() {
        const cart = this.getCart();
        const countSpan = document.getElementById('cart-count');
        if (countSpan) {
            const count = cart.reduce((sum, item) => sum + item.quantity, 0);
            countSpan.textContent = count > 0 ? count : '';
        }
    }
}

window.Cart = CartManager;

document.addEventListener('DOMContentLoaded', () => {
    CartManager.updateUI();
});
