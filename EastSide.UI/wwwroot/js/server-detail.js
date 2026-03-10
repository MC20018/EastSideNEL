/**
 * 服务器详情页 - 网络服和租赁服共用
 *
 * openServerDetailPage(options) 替换当前页面内容为详情页
 * options:
 *   serverId, serverName,
 *   detailAction   - 'network:detail' | 'rental:detail'
 *   accountsAction - 'network:accounts' | 'rental:accounts'
 *   selectAction   - 'network:selectAccount' | 'rental:selectAccount'
 *   rolesAction    - 'network:roles' | 'rental:roles'
 *   createAction   - 'network:createRole' | 'rental:createRole'
 *   deleteAction   - 'network:deleteRole' | 'rental:deleteRole'
 *   joinAction     - 'network:join' | 'rental:join'
 *   onBack         - 返回回调
 *   hasPassword     - (可选) 是否需要密码
 *   mcVersion       - (可选) MC 版本
 *   extraJoinParams - (可选) 额外 join 参数的函数 () => ({})
 */
async function openServerDetailPage(opts) {
    const {
        serverId, serverName, detailAction, accountsAction, selectAction,
        rolesAction, createAction, deleteAction, joinAction, onBack,
        hasPassword, mcVersion, extraJoinParams
    } = opts;

    const wrap = opts.wrap;
    if (!wrap) return;

    wrap.innerHTML = '<div class="network-loading">加载详情...</div>';

    let images = [];
    let description = '';
    try {
        const r = await Bridge.send(detailAction, { serverId });
        if (r.success) {
            images = r.data?.images || [];
            description = (r.data?.description || '').replace(/<img[^>]*>/gi, '');
        }
    } catch {}

    const imgHtml = images.length > 0
        ? `<div class="server-detail-images"><img src="${escapeHtml(images[0])}" alt="" onerror="this.style.display='none';this.nextElementSibling.style.display='block'" /><div class="detail-img-placeholder" style="display:none"></div></div>`
        : '';

    const pwdHtml = hasPassword ? `
        <div class="detail-password-group">
            <label>服务器密码</label>
            <input id="detail-pwd" type="password" placeholder="输入服务器密码" />
        </div>` : '';

    wrap.innerHTML = `
    <div class="server-detail">
        <div class="server-detail-header">
            <button class="server-detail-back" id="detail-back-btn">
                <svg width="16" height="16" viewBox="0 0 16 16" fill="none"><path d="M10 12L6 8l4-4" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/></svg>
            </button>
            <div class="server-detail-title">${escapeHtml(serverName)}</div>
        </div>
        ${imgHtml}
        ${description ? `<div class="server-detail-desc">${description}</div>` : ''}
        ${pwdHtml}
        <div class="server-detail-section">
            <div class="server-detail-section-title">账号</div>
            <div class="custom-select" id="detail-cs-account">
                <button class="custom-select-trigger" type="button">选择账号</button>
                <svg class="custom-select-arrow" width="10" height="6" viewBox="0 0 10 6"><path d="M0 0l5 6 5-6z" fill="currentColor"/></svg>
                <div class="custom-select-dropdown"></div>
            </div>
        </div>
        <div class="server-detail-section">
            <div class="server-detail-section-title">角色列表</div>
            <div class="server-detail-roles" id="detail-roles">
                <div class="detail-no-roles">加载中...</div>
            </div>
            <button class="detail-add-role-btn" id="detail-add-role" style="margin-top:8px">+ 添加角色</button>
        </div>
    </div>`;

    document.getElementById('detail-back-btn').addEventListener('click', handleBack);

    const csAccount = new CustomSelect(wrap.querySelector('#detail-cs-account'));
    wrap.querySelector('.server-detail').addEventListener('click', (e) => {
        if (!e.target.closest('#detail-cs-account')) csAccount.close();
    });

    try {
        const ar = await Bridge.send(accountsAction);
        if (!ar.success || !ar.data?.accounts?.length) {
            document.getElementById('detail-roles').innerHTML = '<div class="detail-no-roles">没有已登录的游戏账号，请先在账号管理中激活</div>';
            return;
        }
        csAccount.setOptions(ar.data.accounts.map(a => ({ value: a.id, label: a.label })));
        csAccount.selectIndex(0);
    } catch {
        document.getElementById('detail-roles').innerHTML = '<div class="detail-no-roles">获取账号失败</div>';
        return;
    }

    async function loadRoles(accountId) {
        const rolesEl = document.getElementById('detail-roles');
        if (!rolesEl) return;
        rolesEl.innerHTML = '<div class="detail-no-roles">加载中...</div>';
        try {
            if (accountId) await Bridge.send(selectAction, { accountId });
            const rr = await Bridge.send(rolesAction, { serverId, accountId });
            if (rr.success && rr.data?.roles?.length) {
                renderRoles(rolesEl, rr.data.roles, accountId);
            } else {
                rolesEl.innerHTML = '<div class="detail-no-roles">暂无角色，请添加</div>';
            }
        } catch {
            rolesEl.innerHTML = '<div class="detail-no-roles">获取角色失败</div>';
        }
    }

    let _banTimers = [];

    function clearBanTimers() {
        _banTimers.forEach(id => clearInterval(id));
        _banTimers = [];
    }

    function formatCountdown(ms) {
        if (ms <= 0) return '已解封';
        const s = Math.floor(ms / 1000);
        const d = Math.floor(s / 86400);
        const h = Math.floor((s % 86400) / 3600);
        const m = Math.floor((s % 3600) / 60);
        const sec = s % 60;
        if (d > 0) return `${d}天 ${h}时 ${m}分 ${sec}秒`;
        if (h > 0) return `${h}时 ${m}分 ${sec}秒`;
        if (m > 0) return `${m}分 ${sec}秒`;
        return `${sec}秒`;
    }

    function renderRoles(container, roles, accountId) {
        clearBanTimers();

        container.innerHTML = roles.map((r, i) => {
            let banHtml = '';
            if (r.banned) {
                if (r.permanent) {
                    banHtml = '<div class="role-ban-info role-ban-permanent">永久封禁</div>';
                } else if (r.unbanTime) {
                    const ut = new Date(r.unbanTime);
                    const dateStr = ut.toLocaleString('zh-CN', { year:'numeric', month:'2-digit', day:'2-digit', hour:'2-digit', minute:'2-digit', second:'2-digit' });
                    banHtml = `<div class="role-ban-info"><span class="role-ban-countdown" data-unban="${r.unbanTime}" data-idx="${i}"></span><span class="role-ban-date">解封: ${dateStr}</span></div>`;
                }
            }
            return `
            <div class="role-row${r.banned ? ' role-row-banned' : ''}" data-role-id="${escapeHtml(r.id)}" data-role-name="${escapeHtml(r.name)}">
                <div class="role-row-left">
                    <div class="role-row-name">${escapeHtml(r.name)}</div>
                    ${banHtml}
                </div>
                <div class="role-row-actions">
                    <button class="role-launch-btn">启动</button>
                    <button class="role-delete-btn">删除</button>
                </div>
            </div>`;
        }).join('');

        container.querySelectorAll('.role-row').forEach(row => {
            const roleId = row.dataset.roleId;
            const roleName = row.dataset.roleName;

            row.querySelector('.role-launch-btn').addEventListener('click', async () => {
                if (hasPassword) {
                    const pwd = document.getElementById('detail-pwd')?.value?.trim() || '';
                    if (!pwd) { showToast('请输入服务器密码', 'error'); return; }
                }
                const btn = row.querySelector('.role-launch-btn');
                btn.classList.add('btn-loading');
                try {
                    const joinParams = { accountId, serverId, serverName, roleId };
                    if (mcVersion) joinParams.mcVersion = mcVersion;
                    if (hasPassword) joinParams.password = document.getElementById('detail-pwd')?.value?.trim() || '';
                    if (extraJoinParams) Object.assign(joinParams, extraJoinParams());
                    const r = await Bridge.send(joinAction, joinParams);
                    if (r.success) {
                        showToast(`启动成功 ${r.data.ip}:${r.data.port}`, 'success');
                    } else {
                        showToast(r.data?.message || '启动失败', 'error');
                    }
                } catch (e) {
                    showToast('启动失败: ' + e.message, 'error');
                } finally {
                    btn.classList.remove('btn-loading');
                }
            });

            row.querySelector('.role-delete-btn').addEventListener('click', async () => {
                const btn = row.querySelector('.role-delete-btn');
                btn.classList.add('btn-loading');
                try {
                    const r = await Bridge.send(deleteAction, { serverId, roleName, roleId, accountId });
                    if (r.success) {
                        showToast('角色已删除', 'success');
                        if (r.data?.roles?.length) {
                            renderRoles(container, r.data.roles, accountId);
                        } else {
                            container.innerHTML = '<div class="detail-no-roles">暂无角色，请添加</div>';
                        }
                    } else {
                        showToast(r.data?.message || '删除失败', 'error');
                    }
                } catch (e) {
                    showToast('删除失败: ' + e.message, 'error');
                } finally {
                    btn.classList.remove('btn-loading');
                }
            });
        });

        // 启动倒计时
        const countdownEls = container.querySelectorAll('.role-ban-countdown[data-unban]');
        if (countdownEls.length > 0) {
            function tick() {
                let anyExpired = false;
                countdownEls.forEach(el => {
                    const unban = new Date(el.dataset.unban);
                    const remaining = unban - Date.now();
                    if (remaining <= 0) anyExpired = true;
                    el.textContent = remaining > 0 ? '剩余: ' + formatCountdown(remaining) : '已解封';
                });
                if (anyExpired) {
                    clearBanTimers();
                    loadRoles(accountId);
                }
            }
            tick();
            _banTimers.push(setInterval(tick, 1000));
        }
    }

    await loadRoles(csAccount.value);
    csAccount.onChange = (val) => loadRoles(val);

    document.getElementById('detail-add-role').addEventListener('click', () => {
        showAddRoleDialog(serverId, (roles) => {
            const rolesEl = document.getElementById('detail-roles');
            if (rolesEl) renderRoles(rolesEl, roles, csAccount.value);
        }, createAction, csAccount.value);
    });

    function onBanPush(e) {
        if (!document.getElementById('detail-roles')) {
            window.removeEventListener('bridge:push', onBanPush);
            clearBanTimers();
            return;
        }
        const msg = e.detail;
        if (msg.action === 'ban:updated' && msg.data?.serverId === serverId) {
            loadRoles(csAccount.value);
        }
    }
    window.addEventListener('bridge:push', onBanPush);

    function handleBack() {
        window.removeEventListener('bridge:push', onBanPush);
        clearBanTimers();
        onBack();
    }
}
