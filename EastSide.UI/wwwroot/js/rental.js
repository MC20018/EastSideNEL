let _rentalLoading = false;
let _rentalScrollHandler = null;
let _rentalOffset = 0;
let _rentalInDetail = false;

function renderRental(userData) {
    _rentalLoading = false;
    _rentalOffset = 0;
    _rentalInDetail = false;
    if (_rentalScrollHandler) {
        document.getElementById('content')?.removeEventListener('scroll', _rentalScrollHandler);
        _rentalScrollHandler = null;
    }
    setTimeout(() => {
        bindRentalEvents();
        loadRentalServers(false);
        const scrollEl = document.getElementById('content');
        if (scrollEl) {
            _rentalScrollHandler = () => {
                if (_rentalInDetail) return;
                if (_rentalLoading) return;
                if (scrollEl.scrollTop + scrollEl.clientHeight >= scrollEl.scrollHeight - 100) {
                    loadRentalServers(true);
                }
            };
            scrollEl.addEventListener('scroll', _rentalScrollHandler);
        }
    }, 0);
    return `
    <div class="network-header" id="rental-header">
        <div>
            <h2>租赁服务器</h2>
            <p class="page-desc">浏览和加入租赁服务器</p>
        </div>
        <div class="network-actions">
            <button class="btn-secondary" id="btn-rental-specify">指定服务器</button>
            <button class="btn-secondary" id="btn-rental-refresh">刷新</button>
        </div>
    </div>
    <div id="rental-grid-wrap"></div>
    `;
}

function bindRentalEvents() {
    document.getElementById('btn-rental-specify')?.addEventListener('click', specifyRentalServer);
    document.getElementById('btn-rental-refresh')?.addEventListener('click', () => {
        _rentalOffset = 0;
        loadRentalServers(false);
    });
}

async function loadRentalServers(append) {
    if (_rentalLoading) return;
    _rentalLoading = true;
    const wrap = document.getElementById('rental-grid-wrap');
    if (!wrap) { _rentalLoading = false; return; }

    if (!append) {
        _rentalOffset = 0;
        wrap.innerHTML = '<div class="network-loading">加载中...</div>';
    }

    try {
        const r = await Bridge.send('rental:list', { offset: _rentalOffset, pageSize: 20 });
        if (!r.success) {
            if (!append) wrap.innerHTML = `<div class="network-empty">${escapeHtml(r.data?.message || '获取失败')}</div>`;
            _rentalLoading = false;
            return;
        }

        const items = r.data?.items || [];
        if (!append && items.length === 0) {
            wrap.innerHTML = '<div class="network-empty">暂无租赁服务器</div>';
            _rentalLoading = false;
            return;
        }

        _rentalOffset += items.length;

        let html = '';
        for (const s of items) html += renderRentalCard(s);

        if (!append) {
            wrap.innerHTML = `<div class="server-grid" id="rental-grid">${html}</div>`;
        } else {
            document.getElementById('rental-grid')?.insertAdjacentHTML('beforeend', html);
        }

        wrap.querySelectorAll('.server-card:not([data-bound])').forEach(card => {
            card.setAttribute('data-bound', '1');
            card.addEventListener('click', () => {
                openRentalDetail(card.dataset.id, card.dataset.name, card.dataset.version, card.dataset.pwd === '1');
            });
        });
    } catch (e) {
        if (!append) wrap.innerHTML = `<div class="network-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }
    _rentalLoading = false;
}

function renderRentalCard(s) {
    const imgHtml = s.imageUrl
        ? `<img class="server-card-img" src="${escapeHtml(s.imageUrl)}" alt="" loading="lazy" onerror="this.style.display='none';this.nextElementSibling.style.display='block'" /><div class="server-card-placeholder" style="display:none"></div>`
        : `<div class="server-card-placeholder"></div>`;
    const lockHtml = s.hasPassword ? '<span class="rental-lock"><svg width="11" height="13" viewBox="0 0 11 13" fill="none"><rect x="1.5" y="5.5" width="8" height="6" rx="1" stroke="rgba(255,255,255,0.7)" stroke-width="1.2"/><path d="M3.5 5.5V3.5a2 2 0 014 0v2" stroke="rgba(255,255,255,0.7)" stroke-width="1.2" stroke-linecap="round"/></svg> 需要密码</span>' : '';
    return `
    <div class="server-card" data-id="${escapeHtml(s.entityId)}" data-name="${escapeHtml(s.name)}" data-version="${escapeHtml(s.mcVersion || '')}" data-pwd="${s.hasPassword ? '1' : '0'}">
        ${imgHtml}
        <div class="server-card-gradient"></div>
        <div class="server-card-body">
            <div class="server-card-name">${escapeHtml(s.name)}</div>
            <div class="server-card-online">在线: ${escapeHtml(String(s.playerCount || '0'))}</div>
            <div class="server-card-id">${escapeHtml(s.entityId)}</div>
            ${lockHtml}
        </div>
    </div>`;
}

async function specifyRentalServer() {
    document.getElementById('dialog-overlay')?.remove();
    const overlay = document.createElement('div');
    overlay.id = 'dialog-overlay';
    overlay.className = 'dialog-overlay';
    overlay.innerHTML = `
        <div class="dialog-box">
            <div class="dialog-header">
                <h3>指定租赁服务器</h3>
                <button class="dialog-close" id="dialog-close-btn">&times;</button>
            </div>
            <div class="dialog-body">
                <div class="form-group">
                    <input id="dlg-server-id" type="text" placeholder="输入服务器号" />
                </div>
                <div id="dialog-error" class="dialog-error"></div>
            </div>
            <div class="dialog-footer">
                <button class="btn-secondary" id="dialog-cancel-btn">取消</button>
                <button class="btn-accent" id="dialog-confirm-btn">进入</button>
            </div>
        </div>
    `;
    document.body.appendChild(overlay);

    const closeDialog = () => {
        overlay.classList.add('closing');
        overlay.addEventListener('animationend', () => overlay.remove(), { once: true });
    };
    overlay.querySelector('#dialog-close-btn').addEventListener('click', closeDialog);
    overlay.querySelector('#dialog-cancel-btn').addEventListener('click', closeDialog);
    overlay.addEventListener('click', (e) => { if (e.target === overlay) closeDialog(); });

    overlay.querySelector('#dialog-confirm-btn').addEventListener('click', () => {
        const id = overlay.querySelector('#dlg-server-id').value.trim();
        if (!id) { overlay.querySelector('#dialog-error').textContent = '请输入服务器号'; return; }
        closeDialog();
        openRentalDetail(id, id, '', false);
    });
}

function openRentalDetail(serverId, serverName, mcVersion, hasPassword) {
    _rentalInDetail = true;
    const header = document.getElementById('rental-header');
    const wrap = document.getElementById('rental-grid-wrap');
    if (header) header.style.display = 'none';

    openServerDetailPage({
        serverId, serverName, wrap,
        detailAction: 'rental:detail',
        accountsAction: 'rental:accounts',
        selectAction: 'rental:selectAccount',
        rolesAction: 'rental:roles',
        createAction: 'rental:createRole',
        deleteAction: 'rental:deleteRole',
        joinAction: 'rental:join',
        hasPassword,
        mcVersion,
        onBack() {
            _rentalInDetail = false;
            if (header) header.style.display = '';
            _rentalOffset = 0;
            loadRentalServers(false);
        }
    });
}
