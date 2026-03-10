function renderSession(userData) {
    setTimeout(() => loadSessions(), 0);
    return `
    <div class="network-header">
        <div>
            <h2>游戏会话</h2>
            <p class="page-desc">查看和管理当前游戏会话</p>
        </div>
        <div class="network-actions">
            <button class="btn-secondary" id="btn-session-refresh">刷新</button>
        </div>
    </div>
    <div id="session-list-wrap"></div>
    `;
}

async function loadSessions() {
    const wrap = document.getElementById('session-list-wrap');
    if (!wrap) return;
    wrap.innerHTML = '<div class="network-loading">加载中...</div>';

    try {
        const r = await Bridge.send('session:list');
        if (!r.success) {
            wrap.innerHTML = `<div class="network-empty">${escapeHtml(r.data?.message || '获取失败')}</div>`;
            return;
        }

        const sessions = r.data?.sessions || [];
        if (sessions.length === 0) {
            wrap.innerHTML = '<div class="network-empty">暂无会话</div>';
            return;
        }

        let html = '';
        for (const s of sessions) html += renderSessionCard(s);
        wrap.innerHTML = `<div class="session-list">${html}</div>`;

        wrap.querySelectorAll('.session-copy-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const addr = btn.dataset.addr;
                if (addr) {
                    navigator.clipboard?.writeText(addr);
                    showToast('已复制: ' + addr, 'success');
                }
            });
        });
        wrap.querySelectorAll('.session-shutdown-btn').forEach(btn => {
            btn.addEventListener('click', async () => {
                btn.classList.add('btn-loading');
                try {
                    const r = await Bridge.send('session:shutdown', { identifier: btn.dataset.id });
                    if (r.success) {
                        showToast('已关闭通道', 'success');
                        loadSessions();
                    } else {
                        showToast(r.data?.message || '关闭失败', 'error');
                    }
                } catch (e) {
                    showToast('关闭失败', 'error');
                } finally {
                    btn.classList.remove('btn-loading');
                }
            });
        });
    } catch (e) {
        wrap.innerHTML = `<div class="network-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }

    document.getElementById('btn-session-refresh')?.addEventListener('click', () => loadSessions());
}

function renderSessionCard(s) {
    const typeLabel = s.type === 'Launcher' ? '白端' : '代理';
    return `
    <div class="session-card">
        <div class="session-info">
            <div class="session-name">${escapeHtml(s.serverName)}</div>
            <div class="session-meta">${escapeHtml(s.characterName)} · ${escapeHtml(typeLabel)}</div>
            <div class="session-status">${escapeHtml(s.statusText)}</div>
        </div>
        <div class="session-actions">
            <button class="btn-secondary session-copy-btn" data-addr="${escapeHtml(s.localAddress)}">复制地址</button>
            <button class="btn-danger session-shutdown-btn" data-id="${escapeHtml(s.identifier)}">关闭通道</button>
        </div>
    </div>`;
}
