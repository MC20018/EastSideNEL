let _networkPage = 0;
let _networkHasMore = false;
let _networkLoading = false;
let _networkKeyword = '';
let _networkSearchTimer = null;
let _networkScrollHandler = null;
let _networkInDetail = false;

function renderNetwork(userData) {
    _networkPage = 0;
    _networkHasMore = false;
    _networkLoading = false;
    _networkKeyword = '';
    _networkInDetail = false;
    if (_networkScrollHandler) {
        document.getElementById('content')?.removeEventListener('scroll', _networkScrollHandler);
        _networkScrollHandler = null;
    }
    setTimeout(() => {
        bindNetworkEvents();
        loadServers(false);
        const scrollEl = document.getElementById('content');
        if (scrollEl) {
            _networkScrollHandler = () => {
                if (_networkInDetail) return;
                if (!_networkHasMore || _networkLoading) return;
                if (scrollEl.scrollTop + scrollEl.clientHeight >= scrollEl.scrollHeight - 100) {
                    _networkPage++;
                    loadServers(true);
                }
            };
            scrollEl.addEventListener('scroll', _networkScrollHandler);
        }
    }, 0);
    return `
    <div class="network-header" id="network-header">
        <div>
            <h2>网络服务器</h2>
            <p class="page-desc">浏览和加入网络服务器</p>
        </div>
        <div class="network-actions">
            <button class="btn-secondary" id="btn-specify-server">指定服务器</button>
            <input type="text" class="network-search" id="network-search" placeholder="搜索服务器..." />
        </div>
    </div>
    <div id="server-grid-wrap"></div>
    `;
}

function bindNetworkEvents() {
    document.getElementById('btn-specify-server')?.addEventListener('click', specifyServer);
    document.getElementById('network-search')?.addEventListener('input', (e) => {
        clearTimeout(_networkSearchTimer);
        _networkSearchTimer = setTimeout(() => {
            _networkKeyword = e.target.value.trim();
            _networkPage = 0;
            loadServers(false);
        }, 400);
    });
}

async function loadServers(append) {
    if (_networkLoading) return;
    _networkLoading = true;
    const wrap = document.getElementById('server-grid-wrap');
    if (!wrap) return;

    if (!append) {
        _networkPage = 0;
        wrap.innerHTML = '<div class="network-loading">加载中...</div>';
    }

    try {
        const offset = _networkPage * 20;
        const payload = { offset, pageSize: 20 };
        if (_networkKeyword) payload.keyword = _networkKeyword;

        const r = await Bridge.send('network:list', payload);
        if (!r.success) {
            if (!append) wrap.innerHTML = `<div class="network-empty">${escapeHtml(r.data?.message || '获取失败')}</div>`;
            _networkLoading = false;
            return;
        }

        const items = r.data?.items || [];
        _networkHasMore = r.data?.hasMore || false;

        if (!append && items.length === 0) {
            wrap.innerHTML = '<div class="network-empty">暂无服务器</div>';
            _networkLoading = false;
            return;
        }

        let html = '';
        for (const s of items) html += renderServerCard(s);

        if (!append) {
            wrap.innerHTML = `<div class="server-grid" id="server-grid">${html}</div>`;
        } else {
            document.getElementById('server-grid')?.insertAdjacentHTML('beforeend', html);
        }

        document.getElementById('network-load-more')?.remove();
        if (_networkHasMore) {
            wrap.insertAdjacentHTML('beforeend', `
                <div class="network-load-more" id="network-load-more">
                    <div class="network-scroll-loading">加载中...</div>
                </div>
            `);
        }

        wrap.querySelectorAll('.server-card:not([data-bound])').forEach(card => {
            card.setAttribute('data-bound', '1');
            card.addEventListener('click', () => {
                openNetworkDetail(card.dataset.id, card.dataset.name);
            });
        });
    } catch (e) {
        if (!append) wrap.innerHTML = `<div class="network-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }
    _networkLoading = false;
}

function renderServerCard(s) {
    const imgHtml = s.imageUrl
        ? `<img class="server-card-img" src="${escapeHtml(s.imageUrl)}" alt="" loading="lazy" onerror="this.style.display='none';this.nextElementSibling.style.display='block'" /><div class="server-card-placeholder" style="display:none"></div>`
        : `<div class="server-card-placeholder"></div>`;
    return `
    <div class="server-card" data-id="${escapeHtml(s.entityId)}" data-name="${escapeHtml(s.name)}">
        ${imgHtml}
        <div class="server-card-gradient"></div>
        <div class="server-card-body">
            <div class="server-card-name">${escapeHtml(s.name)}</div>
            <div class="server-card-online">在线: ${escapeHtml(s.onlineCount || '0')}</div>
            <div class="server-card-id">${escapeHtml(s.entityId)}</div>
        </div>
    </div>`;
}

async function specifyServer() {
    document.getElementById('dialog-overlay')?.remove();
    const overlay = document.createElement('div');
    overlay.id = 'dialog-overlay';
    overlay.className = 'dialog-overlay';
    overlay.innerHTML = `
        <div class="dialog-box">
            <div class="dialog-header">
                <h3>指定服务器</h3>
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
        openNetworkDetail(id, id);
    });
}

function openNetworkDetail(serverId, serverName) {
    _networkInDetail = true;
    const header = document.getElementById('network-header');
    const wrap = document.getElementById('server-grid-wrap');
    if (header) header.style.display = 'none';

    openServerDetailPage({
        serverId, serverName, wrap,
        detailAction: 'network:detail',
        accountsAction: 'network:accounts',
        selectAction: 'network:selectAccount',
        rolesAction: 'network:roles',
        createAction: 'network:createRole',
        deleteAction: 'network:deleteRole',
        joinAction: 'network:join',
        onBack() {
            _networkInDetail = false;
            if (header) header.style.display = '';
            _networkPage = 0;
            loadServers(false);
        }
    });
}
