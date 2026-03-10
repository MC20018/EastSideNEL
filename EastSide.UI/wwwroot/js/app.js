
const PageCache = {};

const AuthCache = {
    _dbName: 'EastSideCache',
    _storeName: 'auth',

    _open() {
        return new Promise((resolve, reject) => {
            const req = indexedDB.open(this._dbName, 1);
            req.onupgradeneeded = () => req.result.createObjectStore(this._storeName);
            req.onsuccess = () => resolve(req.result);
            req.onerror = () => reject(req.error);
        });
    },

    async get() {
        try {
            const db = await this._open();
            return new Promise((resolve) => {
                const tx = db.transaction(this._storeName, 'readonly');
                const req = tx.objectStore(this._storeName).get('authData');
                req.onsuccess = () => resolve(req.result || null);
                req.onerror = () => resolve(null);
            });
        } catch { return null; }
    },

    async set(data) {
        try {
            const db = await this._open();
            const tx = db.transaction(this._storeName, 'readwrite');
            tx.objectStore(this._storeName).put(data, 'authData');
        } catch {}
    }
};

document.addEventListener('DOMContentLoaded', async () => {
    bindWindowControls();

    try {
        const settingsRes = await Bridge.send('settings:get');
        if (settingsRes.success && settingsRes.data) {
            _settingsData = settingsRes.data;
            applyTheme(settingsRes.data.themeMode || 'system');
            if (settingsRes.data.themeColor) applyThemeColor(settingsRes.data.themeColor);
            applyBackdrop(settingsRes.data.backdrop || 'none');
            if (settingsRes.data.customBackgroundPath) applyCustomBg(settingsRes.data.customBackgroundPath);
        }
    } catch (_) {}

    Splash.init();

    window.addEventListener('bridge:push', (e) => {
        const msg = e.detail;
        if (msg.action === 'notify' && msg.data) {
            showToast(msg.data.message, msg.data.level);
        }
        if (msg.action === 'auth:updated' && msg.data) {
            onAuthUpdated(msg.data);
        }
    });

    const content = document.getElementById('content');

    try {
        const cached = sessionStorage.getItem('auth_ready') ? await AuthCache.get() : null;
        let authData;

        if (cached) {
            authData = cached;
        } else {
            const auth = await Bridge.send('auth:getStatus');
            await Splash.waitAndHide(1500);

            if (!auth.success || !auth.data) {
                return;
            }
            authData = auth.data;

            if (authData.isLoggedIn && authData.avatar) {
                await Promise.race([
                    new Promise(resolve => {
                        const img = new Image();
                        img.onload = resolve;
                        img.onerror = resolve;
                        img.src = authData.avatar;
                    }),
                    sleep(3000)
                ]);
            }

            await AuthCache.set(authData);
            sessionStorage.setItem('auth_ready', '1');
        }

        if (authData.isLoggedIn) {
            showMainLayout(authData);
        } else {
            showLoginPage(content);
        }
    } catch (e) {
        await Splash.waitAndHide(1500);
        content.innerHTML = `<div class="page"><p>加载失败: ${escapeHtml(e.message)}</p></div>`;
    }
});

const NAV_ITEMS = [
    { id: 'overview',    label: '概括',       icon: 'home' },
    { id: 'account',     label: '账号管理',   icon: 'user' },
    { id: 'divider1' },
    { id: 'network',     label: '网络服务器', icon: 'server' },
    { id: 'rental',      label: '租赁服务器', icon: 'cloud' },
    { id: 'lobby',       label: '联机大厅(失效)',   icon: 'users' },
    { id: 'divider2' },
    { id: 'plugins',     label: '插件',       icon: 'puzzle' },
    { id: 'session',     label: '游戏会话',   icon: 'gamepad' },
    { id: 'skin',        label: '皮肤',       icon: 'palette' },
    { id: 'divider3' },
    { id: 'about',       label: '关于',       icon: 'info' },
    { id: 'settings',    label: '设置',       icon: 'gear' },
];

const NAV_ICONS = {
    home:    '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><path d="M2.5 6.5L8 2l5.5 4.5V13a1 1 0 01-1 1h-3V10H6.5v4h-3a1 1 0 01-1-1V6.5z"/></svg>',
    user:    '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><circle cx="8" cy="5" r="2.5"/><path d="M3 14c0-2.8 2.2-5 5-5s5 2.2 5 5"/></svg>',
    server:  '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="2" width="12" height="5" rx="1"/><rect x="2" y="9" width="12" height="5" rx="1"/><circle cx="5" cy="4.5" r="0.7" fill="currentColor" stroke="none"/><circle cx="5" cy="11.5" r="0.7" fill="currentColor" stroke="none"/></svg>',
    cloud:   '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><path d="M4 12.5a3 3 0 01-.5-5.96 4.5 4.5 0 018.94-.58A2.5 2.5 0 0112.5 12.5H4z"/></svg>',
    users:   '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><circle cx="6" cy="5" r="2"/><path d="M1.5 14c0-2.5 2-4.5 4.5-4.5s4.5 2 4.5 4.5"/><circle cx="11" cy="5.5" r="1.5"/><path d="M14.5 14c0-2-1.5-3.5-3.5-3.5"/></svg>',
    puzzle:  '<svg viewBox="0 0 1024 1024" fill="currentColor"><path d="M640 932H256c-90.44 0-164-73.56-164-164v-96a36 36 0 0 1 36-36h32c33.08 0 60-26.92 60-60s-26.92-60-60-60h-32a36 36 0 0 1-36-36V384c0-90.44 73.56-164 164-164h60.04c2.12-70.96 60.48-128 131.96-128s129.8 57.04 131.96 128H640c90.44 0 164 73.56 164 164v60.08c70.96 2.12 128 60.48 128 131.92s-57.04 129.84-128 131.92V768c0 90.44-73.56 164-164 164zM164 707.92V768c0 50.72 41.28 92 92 92h384c50.72 0 92-41.28 92-92v-96a36 36 0 0 1 36-36h32c33.08 0 60-26.92 60-60s-26.92-60-60-60h-32a36 36 0 0 1-36-36V384c0-50.72-41.28-92-92-92h-96a36 36 0 0 1-36-36v-32c0-33.08-26.92-60-60-60s-60 26.92-60 60v32a36 36 0 0 1-36 36H256c-50.72 0-92 41.28-92 92v96.08A132.12 132.12 0 0 1 260 576c0 72.76-59.24 132-132 132z"/></svg>',
    gamepad: '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><rect x="1.5" y="4" width="13" height="8" rx="2"/><line x1="6" y1="7" x2="6" y2="9"/><line x1="5" y1="8" x2="7" y2="8"/><circle cx="10.5" cy="7.5" r="0.5" fill="currentColor"/><circle cx="10.5" cy="9" r="0.5" fill="currentColor"/></svg>',
    palette: '<svg viewBox="0 0 1024 1024" fill="currentColor"><path d="M617.813333 165.205333l-2.730666 31.232c-3.925333 45.056-42.496 80.213333-87.722667 80.213334-45.909333 0-83.626667-34.474667-87.722667-80.384l-2.730666-31.061334H116.736v326.144h118.784v317.781334c0 56.490667 45.909333 102.4 102.4 102.4h379.050667c56.490667 0 102.4-45.909333 102.4-102.4V491.349333h118.784V165.205333H617.813333z m252.074667 257.877334h-118.784v386.048c0 18.773333-15.36 34.133333-34.133333 34.133333H337.92c-18.773333 0-34.133333-15.36-34.133333-34.133333V423.082667h-118.784V233.472h192.682666c19.285333 65.365333 79.018667 111.445333 149.845334 111.445333s130.56-46.08 149.845333-111.445333h192.682667v189.610667z"/></svg>',
    gear:    '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><circle cx="8" cy="8" r="2"/><path d="M6.7 1.5h2.6l.4 1.9c.4.2.8.4 1.1.7l1.8-.6.13.23 1.17 2.07-.02.02-1.38 1.28c.05.4.05.8 0 1.2l1.4 1.3-1.3 2.3-1.8-.6c-.3.3-.7.5-1.1.7l-.4 1.9H6.7l-.4-1.9c-.4-.2-.8-.4-1.1-.7l-1.8.6-1.3-2.3 1.4-1.3c-.05-.4-.05-.8 0-1.2L2.1 5.8l1.3-2.3 1.8.6c.3-.3.7-.5 1.1-.7l.4-1.9z"/></svg>',
    info:    '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><circle cx="8" cy="8" r="6.5"/><line x1="8" y1="7" x2="8" y2="11"/><circle cx="8" cy="5" r="0.5" fill="currentColor" stroke="none"/></svg>',
};

function showMainLayout(userData) {
    setCurrentUserData(userData);
    const sidebar = document.getElementById('sidebar');
    const content = document.getElementById('content');

    let navHtml = '';
    for (const item of NAV_ITEMS) {
        if (item.id.startsWith('divider')) {
            navHtml += '<div class="nav-divider"></div>';
            continue;
        }
        const activeClass = item.id === 'overview' ? ' active' : '';
        navHtml += `
            <button class="nav-item${activeClass}" data-page="${item.id}">
                <span class="nav-icon">${NAV_ICONS[item.icon] || ''}</span>
                ${item.label}
            </button>
        `;
    }
    sidebar.innerHTML = navHtml;
    sidebar.classList.remove('hidden');
    sidebar.classList.add('sidebar-enter');

    const allChildren = sidebar.querySelectorAll('.nav-item, .nav-divider');
    allChildren.forEach((el, i) => {
        el.classList.add('nav-item-enter');
        el.style.animationDelay = (0.08 + i * 0.04) + 's';
    });

    sidebar.addEventListener('animationend', () => {
        sidebar.classList.remove('sidebar-enter');
    }, { once: true });

    SidebarIndicator.init(sidebar);

    content.classList.add('content-main', 'content-enter');
    content.innerHTML = '<div id="page-content"></div>';
    content.addEventListener('animationend', () => {
        content.classList.remove('content-enter');
    }, { once: true });

    const navItems = sidebar.querySelectorAll('.nav-item');
    let currentNavIndex = 0;
    navItems.forEach((btn, idx) => {
        btn.addEventListener('click', () => {
            if (btn.classList.contains('active')) return;
            const direction = idx > currentNavIndex ? 'down' : 'up';
            currentNavIndex = idx;
            navItems.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            SidebarIndicator.moveTo(btn, sidebar);
            loadPage(btn.dataset.page, userData, direction);
        });
    });

    loadPage('overview', userData);
}

function loadPage(pageId, userData, direction) {
    const pageContent = document.getElementById('page-content');

    if (_networkScrollHandler) {
        document.getElementById('content')?.removeEventListener('scroll', _networkScrollHandler);
        _networkScrollHandler = null;
    }
    if (_rentalScrollHandler) {
        document.getElementById('content')?.removeEventListener('scroll', _rentalScrollHandler);
        _rentalScrollHandler = null;
    }
    if (_skinScrollHandler) {
        document.getElementById('content')?.removeEventListener('scroll', _skinScrollHandler);
        _skinScrollHandler = null;
    }

    const pages = {
        overview: () => renderOverview(userData),
        account:  () => renderAccount(userData),
        network:  () => renderNetwork(userData),
        rental:   () => renderRental(userData),
        lobby:    () => renderLobby(userData),
        plugins:  () => renderPlugins(userData),
        session:  () => renderSession(userData),
        skin:     () => renderSkin(userData),
        settings: () => renderSettings(),
        about:    () => renderAbout(userData),
    };

    const render = pages[pageId];
    if (!render) return;

    let html;
    try {
        html = render();
    } catch (e) {
        html = `<div class="network-empty">页面渲染错误: ${escapeHtml(e.message)}</div>`;
    }

    if (!direction) {
        pageContent.innerHTML = html;
        return;
    }

    const animClass = direction === 'down' ? 'page-slide-down' : 'page-slide-up';
    pageContent.classList.remove('page-slide-down', 'page-slide-up');
    pageContent.innerHTML = html;
    void pageContent.offsetWidth;
    pageContent.classList.add(animClass);
    pageContent.addEventListener('animationend', () => {
        pageContent.classList.remove(animClass);
    }, { once: true });
}

function bindWindowControls() {
    document.addEventListener('keydown', (e) => {
        const blocked = ['F3', 'F5', 'F7', 'F12'];
        if (blocked.includes(e.key)) { e.preventDefault(); return; }

        if (e.ctrlKey && e.shiftKey) {
            // Ctrl+Shift+I/J/C: DevTools, Console, Inspector
            if (['I','J','C'].includes(e.key.toUpperCase())) { e.preventDefault(); return; }
        }
        if (e.ctrlKey && !e.shiftKey) {
            // Ctrl+U: View Source, Ctrl+S: Save, Ctrl+P: Print
            // Ctrl+G: Find, Ctrl+F: Find, Ctrl+H: History, Ctrl+J: Downloads
            if (['U','S','P','G','F','H','J'].includes(e.key.toUpperCase())) { e.preventDefault(); return; }
        }
    });

    const dragArea = document.querySelector('.titlebar-drag');
    if (dragArea) {
        dragArea.addEventListener('mousedown', (e) => {
            if (e.button === 0) Bridge.send('window:drag');
        });
        dragArea.addEventListener('dblclick', () => {
            Bridge.send('window:maximize');
        });
    }

    document.getElementById('btn-minimize')?.addEventListener('click', () => {
        Bridge.send('window:minimize');
    });
    document.getElementById('btn-maximize')?.addEventListener('click', () => {
        Bridge.send('window:maximize');
    });
    document.getElementById('btn-close')?.addEventListener('click', () => {
        Bridge.send('window:close');
    });
}

function escapeHtml(str) {
    if (!str) return '';
    const div = document.createElement('div');
    div.textContent = str;
    return div.innerHTML;
}

function parseMcColor(str) {
    if (!str) return '';

    const MC_COLORS = {
        '0': '#000000', '1': '#0000AA', '2': '#00AA00', '3': '#00AAAA',
        '4': '#AA0000', '5': '#AA00AA', '6': '#FFAA00', '7': '#AAAAAA',
        '8': '#555555', '9': '#5555FF', 'a': '#55FF55', 'b': '#55FFFF',
        'c': '#FF5555', 'd': '#FF55FF', 'e': '#FFFF55', 'f': '#FFFFFF',
    };

    const MC_FORMATS = {
        'l': 'font-weight:bold',
        'o': 'font-style:italic',
        'n': 'text-decoration:underline',
        'm': 'text-decoration:line-through',
    };

    let result = '';
    let styles = [];
    let hasSpan = false;
    let i = 0;

    while (i < str.length) {
        if ((str[i] === '§' || str[i] === '&') && i + 1 < str.length) {
            const code = str[i + 1].toLowerCase();

            if (code === 'r') {
                if (hasSpan) result += '</span>';
                hasSpan = false;
                styles = [];
                i += 2;
                continue;
            }

            if (MC_COLORS[code]) {
                if (hasSpan) result += '</span>';
                styles = styles.filter(s => !s.startsWith('color:'));
                styles.push(`color:${MC_COLORS[code]}`);
                result += `<span style="${styles.join(';')}">`;
                hasSpan = true;
                i += 2;
                continue;
            }

            if (MC_FORMATS[code]) {
                if (hasSpan) result += '</span>';
                styles.push(MC_FORMATS[code]);
                result += `<span style="${styles.join(';')}">`;
                hasSpan = true;
                i += 2;
                continue;
            }
        }

        result += escapeHtml(str[i]);
        i++;
    }

    if (hasSpan) result += '</span>';
    return result;
}

function formatDate(isoStr) {
    try {
        const d = new Date(isoStr);
        if (isNaN(d.getTime())) return isoStr;
        const y = d.getFullYear();
        const m = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        const h = String(d.getHours()).padStart(2, '0');
        const min = String(d.getMinutes()).padStart(2, '0');
        return `${y}/${m}/${day} ${h}:${min}`;
    } catch {
        return isoStr;
    }
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

let _currentUserData = null;

function setCurrentUserData(data) {
    _currentUserData = data;
}

function onAuthUpdated(data) {
    if (!_currentUserData) return;
    Object.assign(_currentUserData, data);
    AuthCache.set(_currentUserData);

    const activeNav = document.querySelector('.nav-item.active');
    if (activeNav) {
        loadPage(activeNav.dataset.page, _currentUserData);
    }
}
