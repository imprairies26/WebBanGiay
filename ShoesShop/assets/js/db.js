// db.js: Mock Data initialization
const mockProducts = [
    {
        id: 'p1',
        title: 'Kinetic Quantum',
        category: 'Running',
        price: 180,
        originalPrice: null,
        tags: ['Featured'],
        image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuC-Z59r9b0vyyQJzgaLYRB6Ib8kMRiAg7yPUznIdRxusg0xsX3Rnp4GY5IUj5WL-qBAt1Y8IifoRj7RGC_94TQXfsJsI-fIX-QMZOBJHfZR_Nj42kcwWcV2ULKSN28dpD3twGVToH35WexsqQ8enob5avvYPYmoHt-Ckrg0wWg29eThLjkD1BqM9fFWPUpl-LoRJ2poo0pS4jPOB3QrGpNGpvYzE97PJowc1YAV4Wo0ntO-fF2U43RQQ2jUYPQCACoZy9T-EGNGPjWU',
        colors: ['#2a3026', '#ffffff']
    },
    {
        id: 'p2',
        title: 'Kinetic Shift V2',
        category: 'Lifestyle',
        price: 130,
        originalPrice: 160,
        tags: ['Sale'],
        image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCiv-dSomy1GRdVWCzkxAiqMU_QD9JQqwwwDHCouIGBSiRWk-6vRjgwvxzt8hFEOx3cIbNpNPXv1kFZxqW1XiODW6wYvrdsDOfkCxneIw_PJ18x-BULd80W0a0cbbvpOQ42l7EKYh0ZAzvK6UvFoSJdnCG69Tp1OVri3rQoFq1LHvXuG1eIv_vIkruE6CBwiRDWq8mMIaAp_I_V7QWrCuors0TMm0bI-_pUXRKiVT-35EPxIu_yEuoKPaZx5uOgYj2xbH8cjmMleohk',
        colors: ['#ff5a00']
    },
    {
        id: 'p3',
        title: 'Kinetic Stealth',
        category: 'Training',
        price: 150,
        originalPrice: null,
        tags: [],
        image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuD-EeJDitHu_Zre8nwdeC0hY6MfW8_CrvGBMaKI58WLU8LMtF-O_DNd8vUywIt_hkragbygjwk0nSskrXSx3iuczyiMcYPt7XRd3fepgdmvd06VzQw1FmB1lBoRdwq0vErCQ8WBrKgEBQR9atkw9VguboGud7m0sGUrFSe7holvrN3tKfjeL47WSz3xKaygaGQEA2o4GogmyeXnSU1W-x5yNbWcjl9emMKW7xwgSAe1nvkyNhSbMRf8qAiC1WVFWSPQC2StLX3I64R3',
        colors: ['#000000', '#71717a']
    },
    {
        id: 'p4',
        title: 'Kinetic Core',
        category: 'Running',
        price: 160,
        originalPrice: null,
        tags: [],
        image: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDu-RW-5Ztok81Ts88NzzVplXkJC9mon5NF-Rumw_beDVj45vn5xMQwEP-ifOzNpjbTRKfI9NlFx6iJvIZ3vAp1gPXlV1F08qvH-Lj6Qjce0FTJaQDkKSdUzJ6qjZOzdtQFlzWwJDfDl51YCfMwE2eLwXWxF0Sw_EG5MMbgEvRD_a8mRSIZ01GNE2t6tA3vlnxJpzQsVWMkV9MVrJhbbEk48Tv72aq3Paib356MP_Iw9_q1ClW_YS5NeZFj4vC4Pp8-9r9kJb3x1bdn',
        colors: ['#dbeafe', '#ff5a00']
    }
];

const mockUsers = [
    { email: 'user@test.com', pass: '123456', role: 'user', name: 'John Doe' },
    { email: 'admin@test.com', pass: '123456', role: 'admin', name: 'Admin Account' },
    { email: 'pos@test.com', pass: '123456', role: 'staff', name: 'POS Staff 1' }
];

function initDB() {
    if (!localStorage.getItem('kinetic_products')) {
        localStorage.setItem('kinetic_products', JSON.stringify(mockProducts));
    }
    if (!localStorage.getItem('kinetic_users')) {
        localStorage.setItem('kinetic_users', JSON.stringify(mockUsers));
    }
    if (!localStorage.getItem('kinetic_orders')) {
        localStorage.setItem('kinetic_orders', JSON.stringify([]));
    }
}

initDB();

window.db = {
    getProducts: () => JSON.parse(localStorage.getItem('kinetic_products')),
    getUsers: () => JSON.parse(localStorage.getItem('kinetic_users')),
    getOrders: () => JSON.parse(localStorage.getItem('kinetic_orders'))
};
