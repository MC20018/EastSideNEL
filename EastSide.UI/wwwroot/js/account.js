function renderAccount(userData) {
    setTimeout(() => {
        loadAccountList();
        bindAccountButtons();
    }, 0);
    return `
    <div class="account-header">
        <div>
            <h2>账号管理</h2>
            <p class="page-desc">管理您的游戏账号</p>
        </div>
        <div class="account-actions">
            <button class="btn-secondary" id="btn-random-login">随机登录</button>
            <button class="btn-accent" id="btn-add-account">添加账号</button>
        </div>
    </div>
    <div id="account-table-wrap">
        <div class="account-loading">加载中...</div>
    </div>
    `;
}

function bindAccountButtons() {
    document.getElementById('btn-random-login')?.addEventListener('click', accountRandomLogin);
    document.getElementById('btn-add-account')?.addEventListener('click', accountAddPC);
}

async function accountRandomLogin() {
    const btn = document.getElementById('btn-random-login');
    if (btn) btn.classList.add('btn-loading');
    try {
        const r = await Bridge.send('account:randomLogin');
        if (r.success) {
            showToast(r.data?.message || '登录成功', 'success');
            loadAccountList();
        } else {
            showToast(r.data?.message || '登录失败', 'error');
        }
    } catch (e) {
        showToast('操作失败: ' + e.message, 'error');
    } finally {
        if (btn) btn.classList.remove('btn-loading');
    }
}

async function accountAddPC() {
    showAddAccountDialog();
}

function showAddAccountDialog() {
    document.getElementById('dialog-overlay')?.remove();

    const overlay = document.createElement('div');
    overlay.id = 'dialog-overlay';
    overlay.className = 'dialog-overlay';
    overlay.innerHTML = `
        <div class="dialog-box">
            <div class="dialog-header">
                <h3>添加账号</h3>
                <button class="dialog-close" id="dialog-close-btn">&times;</button>
            </div>
            <div class="dialog-body">
                <div class="dialog-tabs">
                    <button class="dialog-tab active" data-method="4399">4399</button>
                    <button class="dialog-tab" data-method="netease">网易邮箱</button>
                    <button class="dialog-tab" data-method="cookie">Cookie</button>
                </div>
                <div id="dialog-form">
                    <div class="form-group">
                        <input id="dlg-account" type="text" placeholder="4399 账号" />
                    </div>
                    <div class="form-group">
                        <input id="dlg-password" type="password" placeholder="密码" />
                    </div>
                </div>
                <div id="dialog-error" class="dialog-error"></div>
            </div>
            <div class="dialog-footer">
                <button class="btn-secondary" id="dialog-cancel-btn">取消</button>
                <button class="btn-accent" id="dialog-confirm-btn">登录</button>
            </div>
        </div>
    `;
    document.body.appendChild(overlay);

    let currentMethod = '4399';

    overlay.querySelectorAll('.dialog-tab').forEach(tab => {
        tab.addEventListener('click', () => {
            overlay.querySelectorAll('.dialog-tab').forEach(t => t.classList.remove('active'));
            tab.classList.add('active');
            currentMethod = tab.dataset.method;
            updateDialogForm(currentMethod);
        });
    });

    const closeDialog = () => {
        overlay.classList.add('closing');
        overlay.addEventListener('animationend', () => overlay.remove(), { once: true });
    };
    overlay.querySelector('#dialog-close-btn').addEventListener('click', closeDialog);
    overlay.querySelector('#dialog-cancel-btn').addEventListener('click', closeDialog);
    overlay.addEventListener('click', (e) => { if (e.target === overlay) closeDialog(); });

    overlay.querySelector('#dialog-confirm-btn').addEventListener('click', async () => {
        const confirmBtn = overlay.querySelector('#dialog-confirm-btn');
        const errorEl = overlay.querySelector('#dialog-error');
        errorEl.textContent = '';
        confirmBtn.classList.add('btn-loading');

        try {
            let payload = { method: currentMethod };

            if (currentMethod === 'cookie') {
                payload.cookie = overlay.querySelector('#dlg-cookie').value.trim();
                if (!payload.cookie) { errorEl.textContent = '请输入 Cookie'; confirmBtn.classList.remove('btn-loading'); return; }
            } else {
                payload.account = overlay.querySelector('#dlg-account').value.trim();
                payload.password = overlay.querySelector('#dlg-password').value.trim();
                if (!payload.account || !payload.password) { errorEl.textContent = '请输入账号和密码'; confirmBtn.classList.remove('btn-loading'); return; }
            }

            const r = await Bridge.send('account:loginAdd', payload);
            if (r.success) {
                showToast(r.data?.message || '登录成功', 'success');
                closeDialog();
                loadAccountList();
            } else {
                errorEl.textContent = r.data?.message || '登录失败';
            }
        } catch (e) {
            errorEl.textContent = '请求失败: ' + e.message;
        } finally {
            confirmBtn.classList.remove('btn-loading');
        }
    });
}

function updateDialogForm(method) {
    const form = document.getElementById('dialog-form');
    const error = document.getElementById('dialog-error');
    if (error) error.textContent = '';

    if (method === 'cookie') {
        form.innerHTML = `
            <div class="form-group">
                <textarea id="dlg-cookie" rows="4" placeholder="粘贴 Cookie 内容..."></textarea>
            </div>
        `;
    } else {
        const placeholder = method === 'netease' ? '网易邮箱' : '4399 账号';
        form.innerHTML = `
            <div class="form-group">
                <input id="dlg-account" type="text" placeholder="${placeholder}" />
            </div>
            <div class="form-group">
                <input id="dlg-password" type="password" placeholder="密码" />
            </div>
        `;
    }
}

async function loadAccountList() {
    const wrap = document.getElementById('account-table-wrap');
    if (!wrap) return;

    try {
        const result = await Bridge.send('account:list');
        if (!result.success) {
            wrap.innerHTML = `<div class="account-empty">获取失败: ${escapeHtml(result.data?.message || '')}</div>`;
            return;
        }

        const accounts = result.data?.accounts || [];
        if (accounts.length === 0) {
            wrap.innerHTML = `
                <div class="account-empty">
                    <div class="account-empty-title">暂无账号</div>
                    <div class="account-empty-desc">点击上方按钮添加您的第一个账号</div>
                </div>
            `;
            return;
        }

        let rows = '';
        for (const a of accounts) {
            const statusClass = a.status === 'online' ? 'status-online' : 'status-offline';
            const statusText = a.status === 'online' ? '在线' : '离线';
            const loginType = formatLoginType(a.type);

            rows += `
            <div class="account-row" data-entity-id="${escapeHtml(a.entityId)}">
                <div class="account-cell cell-id">${escapeHtml(a.entityId)}</div>
                <div class="account-cell cell-status"><span class="status-badge ${statusClass}">${statusText}</span></div>
                <div class="account-cell cell-type">${escapeHtml(loginType)}</div>
                <div class="account-cell cell-alias">
                    <input type="text" class="alias-input" value="${escapeHtml(a.alias || '')}" placeholder="添加备注..." data-entity-id="${escapeHtml(a.entityId)}" />
                </div>
                <div class="account-cell cell-actions">
                    ${a.status === 'online'
                        ? `<button class="btn-sm btn-secondary" data-action="logout" data-id="${escapeHtml(a.entityId)}">注销</button>`
                        : `<button class="btn-sm btn-accent" data-action="activate" data-id="${escapeHtml(a.entityId)}">登录</button>`
                    }
                    <button class="btn-sm btn-danger" data-action="delete" data-id="${escapeHtml(a.entityId)}">删除</button>
                </div>
            </div>`;
        }

        wrap.innerHTML = `
            <div class="account-table">
                <div class="account-row account-header-row">
                    <div class="account-cell cell-id">账号ID</div>
                    <div class="account-cell cell-status">状态</div>
                    <div class="account-cell cell-type">登录方式</div>
                    <div class="account-cell cell-alias">备注</div>
                    <div class="account-cell cell-actions">操作</div>
                </div>
                ${rows}
            </div>
        `;

        wrap.querySelectorAll('[data-action]').forEach(btn => {
            btn.addEventListener('click', () => {
                const id = btn.dataset.id;
                const action = btn.dataset.action;
                if (action === 'activate') accountActivate(id);
                else if (action === 'logout') accountLogout(id);
                else if (action === 'delete') accountDelete(id);
            });
        });

        wrap.querySelectorAll('.alias-input').forEach(input => {
            input.addEventListener('blur', () => {
                accountUpdateAlias(input.dataset.entityId, input.value.trim());
            });
        });
    } catch (e) {
        wrap.innerHTML = `<div class="account-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }
}

function formatLoginType(type) {
    switch ((type || '').toLowerCase()) {
        case 'cookie': return 'Cookie';
        case '4399': return '4399账号';
        case 'x19': case 'netease': return '网易邮箱';
        case 'password': return '密码登录';
        default: return type || '未知';
    }
}

async function accountActivate(entityId) {
    const btn = document.querySelector(`[data-action="activate"][data-id="${entityId}"]`);
    if (btn) btn.classList.add('btn-loading');
    try {
        const r = await Bridge.send('account:activate', { entityId });
        if (r.success) {
            showToast('激活成功', 'success');
            loadAccountList();
        } else {
            showToast(r.data?.message || '激活失败', 'error');
        }
    } catch (e) {
        showToast('激活失败: ' + e.message, 'error');
    } finally {
        if (btn) btn.classList.remove('btn-loading');
    }
}

async function accountLogout(entityId) {
    try {
        const r = await Bridge.send('account:logout', { entityId });
        if (r.success) {
            showToast('已注销', 'success');
            loadAccountList();
        } else {
            showToast(r.data?.message || '注销失败', 'error');
        }
    } catch (e) {
        showToast('注销失败: ' + e.message, 'error');
    }
}

async function accountDelete(entityId) {
    try {
        const r = await Bridge.send('account:delete', { entityId });
        if (r.success) {
            showToast('已删除', 'success');
            loadAccountList();
        } else {
            showToast(r.data?.message || '删除失败', 'error');
        }
    } catch (e) {
        showToast('删除失败: ' + e.message, 'error');
    }
}

async function accountUpdateAlias(entityId, alias) {
    try {
        await Bridge.send('account:updateAlias', { entityId, alias });
    } catch (e) {
        showToast('保存备注失败', 'error');
    }
}
