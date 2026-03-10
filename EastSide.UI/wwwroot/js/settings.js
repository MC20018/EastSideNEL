let _settingsData = null;
let _settingsTab = 'appearance';
let _settingsTabIndex = 0;
const _settingsSelects = {};
const _settingsTabOrder = ['appearance', 'function', 'network', 'other'];

function renderSettings() {
    if (!_settingsData) {
        loadSettingsData();
        return `<div class="page"><p>加载设置中...</p></div>`;
    }
    setTimeout(bindSettingsEvents, 0);
    return buildSettingsHtml(_settingsData);
}

async function loadSettingsData() {
    try {
        const res = await Bridge.send('settings:get');
        if (res.success && res.data) {
            _settingsData = res.data;
            const pc = document.getElementById('page-content');
            if (pc) pc.innerHTML = buildSettingsHtml(_settingsData);
            bindSettingsEvents();
        }
    } catch (e) {
        const pc = document.getElementById('page-content');
        if (pc) pc.innerHTML = `<div class="network-empty">加载设置失败: ${escapeHtml(e.message)}</div>`;
    }
}

function settingsCustomSelect(id, options, selectedValue) {
    const selOpt = options.find(o => o.value === selectedValue) || options[0];
    let optsHtml = '';
    for (const o of options) {
        optsHtml += `<div class="custom-select-option${o.value === selectedValue ? ' selected' : ''}" data-value="${o.value}">${escapeHtml(o.label)}</div>`;
    }
    return `<div class="custom-select" id="${id}">
        <button class="custom-select-trigger" type="button">${escapeHtml(selOpt.label)}</button>
        <svg class="custom-select-arrow" width="10" height="6" viewBox="0 0 10 6"><path d="M0 0l5 6 5-6z" fill="currentColor"/></svg>
        <div class="custom-select-dropdown">${optsHtml}</div>
    </div>`;
}

function buildSettingsHtml(s) {
    const tabs = [
        { id: 'appearance', label: '外观' },
        { id: 'function',   label: '功能' },
        { id: 'network',    label: '网络' },
        { id: 'other',      label: '其他' },
    ];

    let html = '<div class="settings-tabs">';
    for (const t of tabs) {
        html += `<button class="settings-tab${t.id === _settingsTab ? ' active' : ''}" data-tab="${t.id}">${t.label}</button>`;
    }
    html += '</div>';

    html += `<div class="settings-section${_settingsTab === 'appearance' ? ' active' : ''}" data-section="appearance">
        <div class="settings-title">外观</div>
        ${settingsCard('主题', '选择应用的配色主题',
            settingsCustomSelect('set-theme',
                [{ value: 'light', label: '浅色' }, { value: 'dark', label: '深色' }, { value: 'system', label: '跟随系统' }],
                s.themeMode || 'system'))}
        ${settingsCard('主题颜色', '选择应用的强调色',
            `<input type="color" class="settings-color-input" id="set-color" value="${s.themeColor || '#0078D4'}">`)}
        ${settingsCard('背景特效', '系统级窗口模糊效果（需要重启生效）',
            settingsCustomSelect('set-backdrop',
                [{ value: 'none', label: '无' }, { value: 'acrylic', label: '亚克力' }, { value: 'mica', label: 'Mica' }],
                s.backdrop || 'none') +
            `<div class="settings-hint">亚克力：毛玻璃透明效果 / Mica：柔和模糊效果</div>`)}
        ${settingsToggleRow('自定义背景', '使用自定义图片作为窗口背景', 'set-custombg-toggle', !!s.customBackgroundPath)}
        <div id="set-custombg-panel" style="display:${s.customBackgroundPath ? 'block' : 'none'}">
            ${settingsCard('背景图片', '设置背景图片路径',
                `<div class="settings-input-row">
                    <input class="settings-input" id="set-custom-bg" value="${escapeHtml(s.customBackgroundPath || '')}" placeholder="输入图片路径（支持 jpg, png, webp）">
                    <button class="btn-secondary btn-sm" id="set-custom-bg-browse">浏览</button>
                </div>`)}
        </div>
    </div>`;

    html += `<div class="settings-section${_settingsTab === 'function' ? ' active' : ''}" data-section="function">
        <div class="settings-title">功能</div>
        ${settingsToggleRow('启动成功后自动复制连接IP', '当角色成功连接服务器后，自动将连接地址复制到剪贴板', 'set-autocopy', s.autoCopyIpOnStart)}
        ${settingsCard('检测封禁后的操作', '当检测到角色被服务器封禁时执行的操作',
            settingsCustomSelect('set-ban',
                [{ value: 'none', label: '无操作' }, { value: 'close', label: '关闭此通道' }, { value: 'switch', label: '关闭通道并启动其他角色' }],
                s.autoDisconnectOnBan || 'none'))}
        ${settingsToggleRow('调试模式', '启用后会在日志中输出更详细的调试信息', 'set-debug', s.debug)}
    </div>`;

    html += `<div class="settings-section${_settingsTab === 'network' ? ' active' : ''}" data-section="network">
        <div class="settings-title">网络</div>
        ${settingsToggleRow('Socks5 代理', '启用 Socks5 代理后，所有网络请求将通过代理服务器转发', 'set-socks5', s.socks5Enabled)}
        <div id="set-socks5-panel" style="display:${s.socks5Enabled ? 'block' : 'none'}">
            <div class="settings-card">
                <div class="settings-card-title">代理服务器配置</div>
                <div style="margin-bottom:10px">
                    <div class="settings-card-desc">服务器地址</div>
                    <input class="settings-input" id="set-socks5-addr" value="${escapeHtml(s.socks5Address || '')}" placeholder="例如: 127.0.0.1">
                </div>
                <div style="margin-bottom:10px">
                    <div class="settings-card-desc">端口</div>
                    <input class="settings-input" id="set-socks5-port" type="number" value="${s.socks5Port || 1080}" placeholder="1080">
                </div>
                <div style="margin-bottom:10px">
                    <div class="settings-card-desc">用户名（可选）</div>
                    <input class="settings-input" id="set-socks5-user" value="${escapeHtml(s.socks5Username || '')}" placeholder="如果代理需要认证，请输入用户名">
                </div>
                <div>
                    <div class="settings-card-desc">密码（可选）</div>
                    <input class="settings-input" id="set-socks5-pass" type="password" value="${escapeHtml(s.socks5Password || '')}" placeholder="如果代理需要认证，请输入密码">
                </div>
                <div class="settings-hint" style="margin-top:12px">修改代理设置后，需要重启应用程序才能生效</div>
            </div>
        </div>
    </div>`;

    html += `<div class="settings-section${_settingsTab === 'other' ? ' active' : ''}" data-section="other">
        <div class="settings-title">其他</div>
        ${settingsToggleRow('混合登录 (Mixed)', '开启后 4399 登录使用 PE Cookie（混合登录），关闭则使用 PC Cookie（纯PC登录）', 'set-mixed-login', s.useMixedLogin !== false)}
    </div>`;

    return html;
}

function settingsCard(title, desc, inner) {
    return `<div class="settings-card">
        <div class="settings-card-title">${title}</div>
        <div class="settings-card-desc">${desc}</div>
        ${inner}
    </div>`;
}

function settingsToggleRow(title, desc, id, checked) {
    return `<div class="settings-card">
        <div class="settings-row">
            <div class="settings-row-info">
                <div class="settings-row-title">${title}</div>
                <div class="settings-row-desc">${desc}</div>
            </div>
            <label class="settings-toggle">
                <input type="checkbox" id="${id}" ${checked ? 'checked' : ''}>
                <span class="settings-toggle-slider"></span>
            </label>
        </div>
    </div>`;
}

function initSettingsSelect(id, onChange) {
    const el = document.getElementById(id);
    if (!el) return null;
    const cs = new CustomSelect(el);
    const opts = [];
    el.querySelectorAll('.custom-select-option').forEach(opt => {
        opts.push({ value: opt.dataset.value, label: opt.textContent });
    });
    cs.setOptions(opts);
    const selectedOpt = el.querySelector('.custom-select-option.selected');
    if (selectedOpt) {
        const idx = opts.findIndex(o => o.value === selectedOpt.dataset.value);
        if (idx >= 0) cs.selectIndex(idx);
    }
    cs.onChange = onChange;
    _settingsSelects[id] = cs;
    return cs;
}

function bindSettingsEvents() {
    document.querySelectorAll('.settings-tab').forEach(btn => {
        btn.addEventListener('click', () => {
            const newTab = btn.dataset.tab;
            if (newTab === _settingsTab) return;
            const newIdx = _settingsTabOrder.indexOf(newTab);
            const direction = newIdx > _settingsTabIndex ? 'right' : 'left';
            _settingsTabIndex = newIdx;
            _settingsTab = newTab;

            document.querySelectorAll('.settings-tab').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            document.querySelectorAll('.settings-section').forEach(s => s.classList.remove('active'));
            const sec = document.querySelector(`.settings-section[data-section="${_settingsTab}"]`);
            if (sec) {
                sec.classList.remove('settings-slide-left', 'settings-slide-right');
                sec.classList.add('active');
                void sec.offsetWidth;
                sec.classList.add(direction === 'right' ? 'settings-slide-left' : 'settings-slide-right');
                sec.addEventListener('animationend', () => {
                    sec.classList.remove('settings-slide-left', 'settings-slide-right');
                }, { once: true });
            }
        });
    });

    let saveTimer = null;
    function saveSettings(patch) {
        Object.assign(_settingsData, patch);
        clearTimeout(saveTimer);
        saveTimer = setTimeout(() => {
            Bridge.send('settings:update', patch);
        }, 400);
    }

    initSettingsSelect('set-theme', v => {
        applyTheme(v);
        saveSettings({ themeMode: v });
    });
    initSettingsSelect('set-backdrop', v => {
        applyBackdrop(v, _settingsData.customBackgroundPath);
        Bridge.send('system:setBackdrop', { style: v });
        saveSettings({ backdrop: v });
    });
    on('set-color', 'input', e => {
        applyThemeColor(e.target.value);
        saveSettings({ themeColor: e.target.value });
    });
    on('set-custombg-toggle', 'change', e => {
        const v = e.target.checked;
        const panel = document.getElementById('set-custombg-panel');
        if (panel) panel.style.display = v ? 'block' : 'none';
        if (!v) {
            applyCustomBg('');
            saveSettings({ customBackgroundPath: '' });
        } else {
            const path = _settingsData.customBackgroundPath || '';
            if (path) applyCustomBg(path);
        }
    });
    on('set-custom-bg', 'change', e => {
        const path = e.target.value;
        applyCustomBg(path);
        saveSettings({ customBackgroundPath: path });
    });
    on('set-custom-bg-browse', 'click', async () => {
        const res = await Bridge.send('system:browseFile', { title: '选择背景图片', filter: '图片文件|*.jpg;*.jpeg;*.png;*.webp;*.bmp|所有文件|*.*' });
        if (res.success && res.data?.path) {
            const input = document.getElementById('set-custom-bg');
            if (input) input.value = res.data.path;
            applyCustomBg(res.data.path);
            saveSettings({ customBackgroundPath: res.data.path });
        }
    });
    on('set-music', 'change', e => {
        const v = e.target.checked;
        const panel = document.getElementById('set-music-panel');
        if (panel) panel.style.display = v ? 'block' : 'none';
        saveSettings({ musicPlayerEnabled: v });
    });
    on('set-music-path', 'change', e => saveSettings({ musicPath: e.target.value }));
    on('set-music-path-browse', 'click', async () => {
        const res = await Bridge.send('system:browseFile', { title: '选择音乐文件', filter: '音频文件|*.mp3;*.wav;*.ogg;*.flac;*.aac|所有文件|*.*' });
        if (res.success && res.data?.path) {
            const input = document.getElementById('set-music-path');
            if (input) input.value = res.data.path;
            saveSettings({ musicPath: res.data.path });
        }
    });
    on('set-volume', 'input', e => {
        const v = parseFloat(e.target.value);
        const label = document.getElementById('set-volume-val');
        if (label) label.textContent = Math.round(v * 100) + '%';
        saveSettings({ musicVolume: v });
    });

    on('set-autocopy', 'change', e => saveSettings({ autoCopyIpOnStart: e.target.checked }));
    initSettingsSelect('set-ban', v => saveSettings({ autoDisconnectOnBan: v }));
    on('set-debug', 'change', e => saveSettings({ debug: e.target.checked }));

    on('set-socks5', 'change', e => {
        const v = e.target.checked;
        const panel = document.getElementById('set-socks5-panel');
        if (panel) panel.style.display = v ? 'block' : 'none';
        saveSettings({ socks5Enabled: v });
    });
    on('set-socks5-addr', 'change', e => saveSettings({ socks5Address: e.target.value }));
    on('set-socks5-port', 'change', e => saveSettings({ socks5Port: parseInt(e.target.value) || 1080 }));
    on('set-socks5-user', 'change', e => saveSettings({ socks5Username: e.target.value }));
    on('set-socks5-pass', 'change', e => saveSettings({ socks5Password: e.target.value }));

    on('set-mixed-login', 'change', e => saveSettings({ useMixedLogin: e.target.checked }));
}

function on(id, evt, fn) {
    const el = document.getElementById(id);
    if (el) el.addEventListener(evt, fn);
}

function applyTheme(mode) {
    _appliedThemeMode = mode;
    if (mode === 'system') {
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        document.documentElement.setAttribute('data-theme', prefersDark ? 'dark' : 'light');
    } else {
        document.documentElement.setAttribute('data-theme', mode === 'dark' ? 'dark' : 'light');
    }
}

let _appliedThemeMode = 'system';
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
    if (_appliedThemeMode === 'system') applyTheme('system');
});

function applyThemeColor(hex) {
    if (!hex) return;
    document.documentElement.style.setProperty('--accent', hex);
    const r = parseInt(hex.slice(1,3),16), g = parseInt(hex.slice(3,5),16), b = parseInt(hex.slice(5,7),16);
    const darker = '#' + [r,g,b].map(c => Math.max(0, Math.round(c * 0.85)).toString(16).padStart(2,'0')).join('');
    document.documentElement.style.setProperty('--accent-hover', darker);
}

function applyBackdrop(style) {
    const root = document.documentElement;
    root.removeAttribute('data-backdrop');

    if (style === 'acrylic' || style === 'mica') {
        root.setAttribute('data-backdrop', style);
    }
}

function applyCustomBg(path) {
    const root = document.documentElement;
    if (path) {
        root.setAttribute('data-custom-bg', '');
        root.style.setProperty('--custom-bg-image', `url("${path.replace(/\\/g, '/')}")`);
    } else {
        root.removeAttribute('data-custom-bg');
        root.style.removeProperty('--custom-bg-image');
    }
}
