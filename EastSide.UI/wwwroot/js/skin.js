let _skinPage = 0;
let _skinHasMore = false;
let _skinLoading = false;
let _skinKeyword = '';
let _skinSearchTimer = null;
let _skinScrollHandler = null;

function renderSkin(userData) {
    _skinPage = 0;
    _skinHasMore = false;
    _skinLoading = false;
    _skinKeyword = '';
    if (_skinScrollHandler) {
        document.getElementById('content')?.removeEventListener('scroll', _skinScrollHandler);
        _skinScrollHandler = null;
    }
    setTimeout(() => {
        bindSkinEvents();
        loadSkins(false);
        const scrollEl = document.getElementById('content');
        if (scrollEl) {
            _skinScrollHandler = () => {
                if (!_skinHasMore || _skinLoading) return;
                if (scrollEl.scrollTop + scrollEl.clientHeight >= scrollEl.scrollHeight - 100) {
                    _skinPage++;
                    loadSkins(true);
                }
            };
            scrollEl.addEventListener('scroll', _skinScrollHandler);
        }
    }, 0);
    return `
    <div class="skin-header" id="skin-header">
        <div>
            <h2>皮肤</h2>
            <p class="page-desc">浏览和应用皮肤</p>
        </div>
        <div class="skin-actions">
            <input type="text" class="skin-search" id="skin-search" placeholder="搜索皮肤..." />
        </div>
    </div>
    <div id="skin-grid-wrap"></div>
    `;
}

function bindSkinEvents() {
    document.getElementById('skin-search')?.addEventListener('input', (e) => {
        clearTimeout(_skinSearchTimer);
        _skinSearchTimer = setTimeout(() => {
            _skinKeyword = e.target.value.trim();
            _skinPage = 0;
            loadSkins(false);
        }, 400);
    });
}

async function loadSkins(append) {
    if (_skinLoading) return;
    _skinLoading = true;
    const wrap = document.getElementById('skin-grid-wrap');
    if (!wrap) return;

    if (!append) {
        _skinPage = 0;
        wrap.innerHTML = '<div class="skin-loading">加载中...</div>';
    }

    try {
        const offset = _skinPage * 20;
        let r;
        if (_skinKeyword) {
            r = await Bridge.send('skin:search', { keyword: _skinKeyword });
        } else {
            r = await Bridge.send('skin:list', { offset, pageSize: 20 });
        }

        if (!r.success) {
            if (!append) wrap.innerHTML = `<div class="skin-empty">${escapeHtml(r.data?.message || '获取失败')}</div>`;
            _skinLoading = false;
            return;
        }

        const items = r.data?.items || [];
        _skinHasMore = r.data?.hasMore || false;

        if (!append && items.length === 0) {
            wrap.innerHTML = '<div class="skin-empty">暂无皮肤</div>';
            _skinLoading = false;
            return;
        }

        let html = '';
        for (const s of items) html += renderSkinCard(s);

        if (!append) {
            wrap.innerHTML = `<div class="skin-grid" id="skin-grid">${html}</div>`;
        } else {
            document.getElementById('skin-grid')?.insertAdjacentHTML('beforeend', html);
        }

        document.getElementById('skin-load-more')?.remove();
        if (_skinHasMore) {
            wrap.insertAdjacentHTML('beforeend', `
                <div class="skin-load-more" id="skin-load-more">
                    <div class="skin-scroll-loading">加载中...</div>
                </div>
            `);
        }

        wrap.querySelectorAll('.skin-card-apply:not([data-bound])').forEach(btn => {
            btn.setAttribute('data-bound', '1');
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                const card = btn.closest('.skin-card');
                showApplySkinDialog(card.dataset.id, card.dataset.name);
            });
        });
    } catch (e) {
        if (!append) wrap.innerHTML = `<div class="skin-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }
    _skinLoading = false;
}

function renderSkinCard(s) {
    const imgHtml = s.previewUrl
        ? `<img class="skin-card-img" src="${escapeHtml(s.previewUrl)}" alt="" loading="lazy" onerror="this.style.display='none';this.nextElementSibling.style.display='block'" /><div class="skin-card-placeholder" style="display:none"></div>`
        : `<div class="skin-card-placeholder"></div>`;
    return `
    <div class="skin-card" data-id="${escapeHtml(s.entityId)}" data-name="${escapeHtml(s.name)}">
        ${imgHtml}
        <div class="skin-card-gradient"></div>
        <div class="skin-card-body">
            <div class="skin-card-name">${escapeHtml(s.name)}</div>
            <button class="skin-card-apply">应用</button>
        </div>
    </div>`;
}
async function showApplySkinDialog(skinId, skinName) {
    document.getElementById('dialog-overlay')?.remove();

    let accounts = [];
    try {
        const r = await Bridge.send('skin:accounts');
        if (r.success) accounts = r.data?.accounts || [];
    } catch (e) { /* ignore */ }

    if (accounts.length === 0) {
        showToast('请先登录游戏账号', 'warning');
        return;
    }

    const overlay = document.createElement('div');
    overlay.id = 'dialog-overlay';
    overlay.className = 'dialog-overlay';
    overlay.innerHTML = `
        <div class="dialog-box">
            <div class="dialog-header">
                <h3>应用皮肤</h3>
                <button class="dialog-close" id="dialog-close-btn">&times;</button>
            </div>
            <div class="dialog-body">
                <p style="font-size:13px;color:var(--text-secondary);margin-bottom:12px;">
                    将 <strong>${escapeHtml(skinName)}</strong> 应用到:
                </p>
                <div class="join-form-group">
                    <label>选择账号</label>
                    <div class="custom-select" id="dlg-skin-account-select">
                        <button class="custom-select-trigger" type="button">请选择账号</button>
                        <span class="custom-select-arrow">▾</span>
                        <div class="custom-select-dropdown"></div>
                    </div>
                </div>
                <div id="dialog-error" class="dialog-error"></div>
                <div id="dialog-status" style="font-size:12px;color:var(--text-muted);margin-top:8px;"></div>
            </div>
            <div class="dialog-footer">
                <button class="btn-secondary" id="dialog-cancel-btn">取消</button>
                <button class="btn-accent" id="dialog-confirm-btn">购买并应用</button>
            </div>
        </div>
    `;
    document.body.appendChild(overlay);

    const accountSelect = new CustomSelect(overlay.querySelector('#dlg-skin-account-select'));
    accountSelect.setOptions(accounts.map(a => ({ value: a.id, label: a.label })));
    accountSelect.selectIndex(0);

    const closeDialog = () => {
        overlay.classList.add('closing');
        overlay.addEventListener('animationend', () => overlay.remove(), { once: true });
    };
    overlay.querySelector('#dialog-close-btn').addEventListener('click', closeDialog);
    overlay.querySelector('#dialog-cancel-btn').addEventListener('click', closeDialog);
    overlay.addEventListener('click', (e) => { if (e.target === overlay) closeDialog(); });

    overlay.querySelector('#dialog-confirm-btn').addEventListener('click', async () => {
        const accountId = accountSelect.value;
        if (!accountId) {
            overlay.querySelector('#dialog-error').textContent = '请选择账号';
            return;
        }
        const btn = overlay.querySelector('#dialog-confirm-btn');
        const errEl = overlay.querySelector('#dialog-error');
        const statusEl = overlay.querySelector('#dialog-status');
        btn.disabled = true;
        errEl.textContent = '';

        try {
            // 1. 购买皮肤
            statusEl.textContent = '正在购买皮肤...';
            btn.textContent = '购买中...';
            const purchaseRes = await Bridge.send('skin:purchase', { skinId, accountId });
            if (!purchaseRes.success) {
                errEl.textContent = purchaseRes.data?.message || '购买失败';
                statusEl.textContent = '';
                btn.disabled = false;
                btn.textContent = '购买并应用';
                return;
            }

            // 2. 确认购买结果
            const purchaseData = purchaseRes.data?.data;
            if (purchaseData?.orderId) {
                statusEl.textContent = '正在确认购买...';
                await Bridge.send('skin:buyResult', {
                    accountId,
                    orderId: purchaseData.orderId,
                    buyType: purchaseData.buyType || 0
                });
            }

            // 3. 应用皮肤
            statusEl.textContent = '正在应用皮肤...';
            btn.textContent = '应用中...';
            const applyRes = await Bridge.send('skin:apply', { skinId, accountId });
            if (applyRes.success) {
                showToast('皮肤应用成功', 'success');
                closeDialog();
            } else {
                errEl.textContent = applyRes.data?.message || '应用失败';
                statusEl.textContent = '';
                btn.disabled = false;
                btn.textContent = '购买并应用';
            }
        } catch (e) {
            errEl.textContent = '操作失败: ' + e.message;
            statusEl.textContent = '';
            btn.disabled = false;
            btn.textContent = '购买并应用';
        }
    });
}
