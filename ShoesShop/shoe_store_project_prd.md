# Product Requirements Document: Shoe Store System

## Project Overview
A comprehensive shoe commerce platform featuring three main interfaces: User (Customer), POS (Sales Staff), and Admin (Management).

## Core Modules

### 1. Authentication & Security
- **Login/Register:** Standard flow with redirect back to previous page after validation.
- **Forgot Password:** Email-based OTP verification.
- **Access Control:** "Validate" pages require login, otherwise redirect to login.

### 2. User Interface (Customer)
- **Home:** Header (search/menu), auto-playing Banners, Body (infinite scroll, filters, sale/featured sections), Footer.
- **Product Detail:** Standard layout with images, descriptions, and a review system.
- **Cart:** List of selected items.
- **Checkout:** Shipping info, payment selection, discount codes.
- **Orders & History:** List of orders and detailed order views.
- **Profile:** User info management with cloud-based avatar upload.

### 3. POS (Point of Sale)
- **Product Search:** Quick search and scan-to-add functionality.
- **Transaction:** Real-time cart calculation.
- **Payment Modes:** 
    - Bank Transfer (QR Code generation).
    - Cash (Input amount, calculate change).
- **Post-purchase:** Print receipt and optional electronic invoice to email.

### 4. Admin Interface
- **Dashboard:** Visual statistics (Revenue by month/quarter/year, total orders, cancellations).
- **Product Management:** Full CRUD (Create, Read, Update, Delete) with cloud image storage.
- **User Management:** Manage customer and staff accounts.

## Technical Notes
- Desktop-first design.
- Infinite scroll for product listings.
- Cloud storage for all media (avatars, product images).