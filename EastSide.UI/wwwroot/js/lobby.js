let _lobbyLoading = false;
let _lobbyKeyword = '';
let _lobbySearchTimer = null;

function renderLobby(userData) {
    _lobbyLoading = false;
    _lobbyKeyword = '';
    setTimeout(() => {
        bindLobbyEvents();
        loadLobbyRooms();
    }, 0);
    return `
    <div class="network-header">
        <div>
            <h2>联机大厅</h2>
            <p class="page-desc">浏览和加入联机房间</p>
        </div>
        <div class="network-actions">
            <button class="btn-secondary" id="btn-lobby-refresh">刷新</button>
            <input type="text" class="network-search" id="lobby-search" placeholder="搜索房间..." />
        </div>
    </div>
    <div id="lobby-grid-wrap"></div>
    `;
}

function bindLobbyEvents() {
    document.getElementById('btn-lobby-refresh')?.addEventListener('click', () => loadLobbyRooms());
    document.getElementById('lobby-search')?.addEventListener('input', (e) => {
        clearTimeout(_lobbySearchTimer);
        _lobbySearchTimer = setTimeout(() => {
            _lobbyKeyword = e.target.value.trim();
            loadLobbyRooms();
        }, 400);
    });
}

async function loadLobbyRooms() {
    if (_lobbyLoading) return;
    _lobbyLoading = true;
    const wrap = document.getElementById('lobby-grid-wrap');
    if (!wrap) return;
    wrap.innerHTML = '<div class="network-loading">加载中...</div>';

    try {
        const payload = { count: 50 };
        if (_lobbyKeyword) payload.keyword = _lobbyKeyword;
        const r = await Bridge.send('lobby:list', payload);
        if (!r.success) {
            wrap.innerHTML = `<div class="network-empty">${escapeHtml(r.data?.message || '获取失败')}</div>`;
            _lobbyLoading = false;
            return;
        }

        const items = r.data?.items || [];
        if (items.length === 0) {
            wrap.innerHTML = '<div class="network-empty">暂无房间</div>';
            _lobbyLoading = false;
            return;
        }

        let html = '';
        for (const room of items) {
            html += renderLobbyRoomCard(room);
        }
        wrap.innerHTML = `<div class="lobby-grid" id="lobby-grid">${html}</div>`;

        wrap.querySelectorAll('.lobby-card').forEach(card => {
            card.addEventListener('click', (e) => {
                if (e.target.closest('.btn-lobby-members') || e.target.closest('.lobby-members-panel')) return;
                const srv = card.dataset.srv;
                if (srv) {
                    navigator.clipboard?.writeText(srv);
                    showToast('已复制连接地址: ' + srv, 'success');
                }
            });
        });

        wrap.querySelectorAll('.btn-lobby-members').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                loadRoomMembers(btn);
            });
        });

        wrap.querySelectorAll('.btn-lobby-join').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                const card = btn.closest('.lobby-card');
                openLobbyJoinDialog(card);
            });
        });
    } catch (e) {
        wrap.innerHTML = `<div class="network-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }
    _lobbyLoading = false;
}

function parseTips(tips) {
    try { return JSON.parse(tips); } catch { return null; }
}

function formatVersion(ver) {
    if (!ver) return '';
    const major = Math.floor(ver / 1000000);
    const minor = Math.floor((ver % 1000000) / 1000);
    const patch = ver % 1000;
    return patch === 0 ? `${major}.${minor}` : `${major}.${minor}.${patch}`;
}

function renderLobbyRoomCard(room) {
    const info = parseTips(room.tips);
    const hostName = info?.NickName || room.name;
    const version = info?.Version ? formatVersion(info.Version) : '';
    const players = room.cap - room.free;
    const full = room.free === 0;
    const fullClass = full ? 'lobby-status-full' : 'lobby-status-open';
    const fullText = full ? '已满' : '空闲';
    return `
    <div class="lobby-card" data-hid="${room.hid}" data-rid="${room.rid}" data-srv="${escapeHtml(room.srv || '')}"
         data-room-name="${escapeHtml(hostName)}" data-version="${escapeHtml(version)}" data-tips="${escapeHtml(room.tips || '')}">
        <div class="lobby-card-header">
            <span class="lobby-card-name">${escapeHtml(hostName)}</span>
            <span class="lobby-badge ${fullClass}">${fullText}</span>
        </div>
        <div class="lobby-card-bottom">
            <span class="lobby-card-players">${players}/${room.cap} 人</span>
            <span class="lobby-card-rid">#${room.rid}</span>
            ${version ? `<span class="lobby-card-version">${escapeHtml(version)}</span>` : ''}
            <span class="lobby-card-srv">${escapeHtml(room.srv || '')}</span>
        </div>
        <div class="lobby-card-actions">
            <button class="btn-lobby-members" data-hid="${room.hid}" data-rid="${room.rid}">成员列表</button>
            <button class="btn-lobby-join" data-hid="${room.hid}" data-rid="${room.rid}">加入房间</button>
        </div>
        <div class="lobby-members-panel" id="members-${room.hid}-${room.rid}" style="display:none;"></div>
    </div>`;
}

async function loadRoomMembers(btn) {
    const hid = parseInt(btn.dataset.hid);
    const rid = parseInt(btn.dataset.rid);
    const panel = document.getElementById(`members-${hid}-${rid}`);
    if (!panel) return;

    if (panel.style.display !== 'none') {
        panel.style.display = 'none';
        return;
    }

    panel.style.display = 'block';
    panel.innerHTML = '<span class="lobby-members-loading">加载中...</span>';

    try {
        const r = await Bridge.send('lobby:members', { hid, rid });
        if (!r.success) {
            panel.innerHTML = `<span class="lobby-members-error">${escapeHtml(r.data?.message || '获取失败')}</span>`;
            return;
        }
        const uids = r.data?.uidList || [];
        if (uids.length === 0) {
            panel.innerHTML = '<span class="lobby-members-empty">暂无成员</span>';
            return;
        }
        panel.innerHTML = uids.map(uid => `<span class="lobby-member-uid">${uid}</span>`).join('');
    } catch (e) {
        panel.innerHTML = `<span class="lobby-members-error">${escapeHtml(e.message)}</span>`;
    }
}

async function openLobbyJoinDialog(card) {
    const hid = parseInt(card.dataset.hid);
    const rid = parseInt(card.dataset.rid);
    const srv = card.dataset.srv;
    const roomName = card.dataset.roomName || '';
    const version = card.dataset.version || '';
    const gameId = String(hid);

    document.getElementById('dialog-overlay')?.remove();
    const overlay = document.createElement('div');
    overlay.id = 'dialog-overlay';
    overlay.className = 'dialog-overlay';
    overlay.innerHTML = `
        <div class="dialog-box">
            <div class="dialog-header">
                <h3>加入房间</h3>
                <button class="dialog-close" id="dialog-close-btn">&times;</button>
            </div>
            <div class="dialog-body">
                <div class="join-info-row">
                    <span>${escapeHtml(roomName)}</span>
                    ${version ? `<span class="lobby-badge lobby-status-open">${escapeHtml(version)}</span>` : ''}
                </div>
                <div class="join-form-group">
                    <label>账号</label>
                    <div class="custom-select" id="cs-account">
                        <button class="custom-select-trigger" type="button">选择账号</button>
                        <svg class="custom-select-arrow" width="10" height="6" viewBox="0 0 10 6"><path d="M0 0l5 6 5-6z" fill="currentColor"/></svg>
                        <div class="custom-select-dropdown"></div>
                    </div>
                </div>
                <div class="join-form-group">
                    <label>角色</label>
                    <div class="join-role-actions">
                        <div class="custom-select" id="cs-role" style="flex:1">
                            <button class="custom-select-trigger" type="button">选择角色</button>
                            <svg class="custom-select-arrow" width="10" height="6" viewBox="0 0 10 6"><path d="M0 0l5 6 5-6z" fill="currentColor"/></svg>
                            <div class="custom-select-dropdown"></div>
                        </div>
                        <button class="join-add-role-btn" id="btn-add-role">添加</button>
                    </div>
                </div>
                <div id="dialog-error" class="dialog-error"></div>
            </div>
            <div class="dialog-footer">
                <button class="btn-secondary" id="dialog-cancel-btn">关闭</button>
                <button class="btn-accent" id="dialog-launch-btn">启动</button>
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

    const csAccount = new CustomSelect(overlay.querySelector('#cs-account'));
    const csRole = new CustomSelect(overlay.querySelector('#cs-role'));
    const errorEl = overlay.querySelector('#dialog-error');

    overlay.querySelector('.dialog-box').addEventListener('click', (e) => {
        if (!e.target.closest('#cs-account')) csAccount.close();
        if (!e.target.closest('#cs-role')) csRole.close();
    });

    try {
        const ar = await Bridge.send('lobby:accounts');
        if (!ar.success || !ar.data?.accounts?.length) {
            errorEl.textContent = '没有已登录的游戏账号，请先在账号管理中激活';
            return;
        }
        csAccount.setOptions(ar.data.accounts.map(a => ({ value: a.id, label: a.label })));
        csAccount.selectIndex(0);
    } catch (e) {
        errorEl.textContent = '获取账号失败';
        return;
    }

    async function loadRoles(accountId) {
        csRole.setOptions([{ value: '', label: '加载中...' }]);
        csRole.selectIndex(0);
        try {
            if (accountId) await Bridge.send('lobby:selectAccount', { accountId });
            const rr = await Bridge.send('lobby:roles', { gameId, accountId });
            if (rr.success && rr.data?.roles?.length) {
                csRole.setOptions(rr.data.roles.map(r => ({ value: r.id, label: r.name })));
                csRole.selectIndex(0);
            } else {
                csRole.setOptions([{ value: '', label: '暂无角色，请添加' }]);
                csRole.selectIndex(0);
            }
        } catch {
            csRole.setOptions([{ value: '', label: '获取失败' }]);
            csRole.selectIndex(0);
        }
    }

    await loadRoles(csAccount.value);
    csAccount.onChange = (val) => loadRoles(val);

    overlay.querySelector('#btn-add-role').addEventListener('click', () => {
        showAddRoleDialog(gameId, (roles) => {
            csRole.setOptions(roles.map(r => ({ value: r.id, label: r.name })));
            csRole.selectIndex(roles.length - 1);
        }, 'lobby:createRole', csAccount.value);
    });

    overlay.querySelector('#dialog-launch-btn').addEventListener('click', async () => {
        const accountId = csAccount.value;
        const roleId = csRole.value;
        errorEl.textContent = '';
        if (!accountId) { errorEl.textContent = '请选择账号'; return; }
        if (!roleId) { errorEl.textContent = '请选择角色'; return; }

        const btn = overlay.querySelector('#dialog-launch-btn');
        btn.classList.add('btn-loading');

        try {
            const r = await Bridge.send('lobby:join', {
                accountId, hid, rid, roomName, roleId, gameId, srv, mcVersion: version
            });
            if (r.success) {
                showToast(`启动成功 ${r.data.ip}:${r.data.port}`, 'success');
                closeDialog();
            } else {
                errorEl.textContent = r.data?.message || '启动失败';
            }
        } catch (e) {
            errorEl.textContent = '启动失败: ' + e.message;
        } finally {
            btn.classList.remove('btn-loading');
        }
    });
}
