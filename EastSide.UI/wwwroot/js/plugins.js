let _pluginView = 'installed';

function renderPlugins(userData) {
    setTimeout(() => {
        bindPluginEvents();
        loadInstalledPlugins();
    }, 0);
    return `
    <div class="network-header">
        <div>
            <h2>插件</h2>
            <p class="page-desc">管理已安装的插件</p>
        </div>
        <div class="network-actions">
            <button class="btn-secondary" id="btn-plugin-store">插件商店</button>
            <button class="btn-secondary" id="btn-plugin-restart-always">重启</button>
        </div>
    </div>
    <div id="plugin-list-wrap"></div>
    `;
}

function bindPluginEvents() {
    document.getElementById('btn-plugin-store')?.addEventListener('click', () => {
        if (_pluginView === 'installed') {
            loadAvailablePlugins();
        } else {
            loadInstalledPlugins();
        }
    });
    document.getElementById('btn-plugin-restart-always')?.addEventListener('click', async () => {
        const btn = document.getElementById('btn-plugin-restart-always');
        if (btn) btn.classList.add('btn-loading');
        try {
            await Bridge.send('system:restart');
        } catch {
            showToast('重启失败，请手动重启', 'warning');
        }
    });
}
async function loadInstalledPlugins() {
    _pluginView = 'installed';
    const storeBtn = document.getElementById('btn-plugin-store');
    if (storeBtn) storeBtn.textContent = '插件商店';

    const wrap = document.getElementById('plugin-list-wrap');
    if (!wrap) return;
    wrap.innerHTML = '<div class="network-loading">加载中...</div>';

    try {
        const r = await Bridge.send('plugin:installed');
        if (!r.success) {
            wrap.innerHTML = `<div class="network-empty">${escapeHtml(r.data?.message || '获取失败')}</div>`;
            return;
        }

        const plugins = r.data?.plugins || [];
        if (plugins.length === 0) {
            wrap.innerHTML = '<div class="network-empty">暂无已安装的插件</div>';
            return;
        }

        let html = '';
        for (const p of plugins) html += renderInstalledPlugin(p);
        wrap.innerHTML = `<div class="plugin-list">${html}</div>`;

        wrap.querySelectorAll('.plugin-uninstall-btn').forEach(btn => {
            btn.addEventListener('click', async () => {
                btn.classList.add('btn-loading');
                try {
                    const r = await Bridge.send('plugin:uninstall', { pluginId: btn.dataset.id });
                    if (r.success) {
                        const card = btn.closest('.plugin-card');
                        if (card) {
                            const wrap = card.querySelector('.plugin-install-wrap');
                            if (wrap) wrap.innerHTML = '<span class="plugin-pending-badge">待重启</span>';
                        }
                        showToast('卸载成功，重启后生效', 'warning');
                    } else {
                        showToast(r.data?.message || '卸载失败', 'error');
                    }
                } catch (e) {
                    showToast('卸载失败', 'error');
                } finally {
                    btn.classList.remove('btn-loading');
                }
            });
        });
    } catch (e) {
        wrap.innerHTML = `<div class="network-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }
}

function renderInstalledPlugin(p) {
    return `
    <div class="plugin-card" data-plugin-id="${escapeHtml(p.id)}">
        <div class="plugin-icon">${pluginIconSvg}</div>
        <div class="plugin-info">
            <div class="plugin-name">${escapeHtml(p.name)}</div>
            <div class="plugin-desc">${escapeHtml(p.description || '暂无描述')}</div>
            <div class="plugin-meta">
                <span>作者: ${escapeHtml(p.author || '未知')}</span>
                <span>版本: ${escapeHtml(p.version)}</span>
                <span>状态: ${escapeHtml(p.status)}</span>
            </div>
        </div>
        <div class="plugin-install-wrap">
            <button class="btn-danger plugin-uninstall-btn" data-id="${escapeHtml(p.id)}">卸载</button>
        </div>
    </div>`;
}

async function loadAvailablePlugins() {
    _pluginView = 'store';
    const storeBtn = document.getElementById('btn-plugin-store');
    if (storeBtn) storeBtn.textContent = '已安装';

    const wrap = document.getElementById('plugin-list-wrap');
    if (!wrap) return;
    wrap.innerHTML = '<div class="network-loading">加载中...</div>';

    try {
        const r = await Bridge.send('plugin:available');
        if (!r.success) {
            wrap.innerHTML = `<div class="network-empty">${escapeHtml(r.data?.message || '获取失败')}</div>`;
            return;
        }

        const plugins = r.data?.plugins || [];
        if (plugins.length === 0) {
            wrap.innerHTML = '<div class="network-empty">暂无可用插件</div>';
            return;
        }

        let html = '';
        for (const p of plugins) html += renderAvailablePlugin(p);
        wrap.innerHTML = `<div class="plugin-list">${html}</div>`;

        wrap.querySelectorAll('.plugin-install-btn').forEach(btn => {
            btn.addEventListener('click', async () => {
                btn.classList.add('btn-loading');
                try {
                    const r = await Bridge.send('plugin:install', { pluginId: btn.dataset.id });
                    if (r.success) {
                        showToast(r.data?.message || '安装成功，重启后生效', 'success');
                        const wrap = btn.closest('.plugin-card')?.querySelector('.plugin-install-wrap');
                        if (wrap) wrap.innerHTML = '<span class="plugin-installed-badge">已安装</span>';
                    } else {
                        showToast(r.data?.message || '安装失败', 'error');
                    }
                } catch (e) {
                    showToast('安装失败', 'error');
                } finally {
                    btn.classList.remove('btn-loading');
                }
            });
        });
    } catch (e) {
        wrap.innerHTML = `<div class="network-empty">加载失败: ${escapeHtml(e.message)}</div>`;
    }

    wrap.querySelectorAll('.plugin-logo[data-logo]').forEach(el => {
        fetch(el.dataset.logo)
            .then(r => r.text())
            .then(svg => {
                if (svg.includes('<svg')) {
                    el.innerHTML = svg;
                    const svgEl = el.querySelector('svg');
                    if (svgEl) {
                        svgEl.setAttribute('width', '48');
                        svgEl.setAttribute('height', '48');
                    }
                }
            })
            .catch(() => {});
    });
}

function renderAvailablePlugin(p) {
    const actionHtml = p.isInstalled
        ? '<span class="plugin-installed-badge">已安装</span>'
        : `<button class="btn-accent plugin-install-btn" data-id="${escapeHtml(p.id)}">安装</button>`;
    return `
    <div class="plugin-card">
        <div class="plugin-logo" ${p.logoUrl ? `data-logo="${escapeHtml(p.logoUrl)}"` : ''}>${pluginIconSvg}</div>
        <div class="plugin-info">
            <div class="plugin-name">${escapeHtml(p.name)}</div>
            <div class="plugin-desc">${escapeHtml(p.shortDescription || '暂无描述')}</div>
            <div class="plugin-meta">
                <span>发布者: ${escapeHtml(p.publisher || '未知')}</span>
                <span>版本: ${escapeHtml(p.version)}</span>
            </div>
        </div>
        <div class="plugin-install-wrap">${actionHtml}</div>
    </div>`;
}

const pluginIconSvg = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M12 2L2 7l10 5 10-5-10-5z"/><path d="M2 17l10 5 10-5"/><path d="M2 12l10 5 10-5"/></svg>';
