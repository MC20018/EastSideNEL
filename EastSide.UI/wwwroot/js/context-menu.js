const ContextMenu = (() => {
    let _menu = null;

    const ICONS = {
        copy:    '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><rect x="5.5" y="5.5" width="8" height="8" rx="1.5"/><path d="M10.5 5.5V3.5a1.5 1.5 0 00-1.5-1.5H3.5A1.5 1.5 0 002 3.5V9a1.5 1.5 0 001.5 1.5h2"/></svg>',
        paste:   '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="10" height="10" rx="1.5"/><path d="M6 4V2.5A.5.5 0 016.5 2h3a.5.5 0 01.5.5V4"/></svg>',
        cut:     '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><circle cx="4.5" cy="11.5" r="2"/><circle cx="11.5" cy="11.5" r="2"/><line x1="6.2" y1="10" x2="11" y2="2"/><line x1="9.8" y1="10" x2="5" y2="2"/></svg>',
        select:  '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="2" width="12" height="12" rx="2"/><path d="M5 8h6"/><path d="M5 5.5h6"/><path d="M5 10.5h6"/></svg>',
        refresh: '<svg viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"><path d="M2.5 8a5.5 5.5 0 019.3-3.9L13.5 2.5v4h-4l1.6-1.6A3.5 3.5 0 004.5 8"/><path d="M13.5 8a5.5 5.5 0 01-9.3 3.9L2.5 13.5v-4h4l-1.6 1.6A3.5 3.5 0 0011.5 8"/></svg>',
    };

    function isEditable(el) {
        if (!el) return false;
        const tag = el.tagName;
        if (tag === 'INPUT' || tag === 'TEXTAREA') return !el.readOnly && !el.disabled;
        return el.isContentEditable;
    }

    function hasSelection() {
        const sel = window.getSelection();
        return sel && sel.toString().length > 0;
    }

    function buildItems(target) {
        const editable = isEditable(target);
        const selected = hasSelection();
        const items = [];

        items.push({ id: 'cut',    label: '剪切', icon: ICONS.cut,    shortcut: 'Ctrl+X', disabled: !editable || !selected });
        items.push({ id: 'copy',   label: '复制', icon: ICONS.copy,   shortcut: 'Ctrl+C', disabled: !selected });
        items.push({ id: 'paste',  label: '粘贴', icon: ICONS.paste,  shortcut: 'Ctrl+V', disabled: !editable });
        items.push({ type: 'divider' });
        items.push({ id: 'selectAll', label: '全选', icon: ICONS.select, shortcut: 'Ctrl+A' });
        items.push({ type: 'divider' });
        items.push({ id: 'refresh', label: '刷新页面', icon: ICONS.refresh, shortcut: 'Ctrl+R' });

        return items;
    }

    function exec(id) {
        switch (id) {
            case 'cut':       document.execCommand('cut');       break;
            case 'copy':      document.execCommand('copy');      break;
            case 'paste':     document.execCommand('paste');     break;
            case 'selectAll': document.execCommand('selectAll'); break;
            case 'refresh':   location.reload();                 break;
        }
    }

    function show(x, y, target) {
        close(true);

        const items = buildItems(target);
        const menu = document.createElement('div');
        menu.className = 'ctx-menu';

        for (const item of items) {
            if (item.type === 'divider') {
                menu.appendChild(Object.assign(document.createElement('div'), { className: 'ctx-divider' }));
                continue;
            }
            const btn = document.createElement('button');
            btn.className = 'ctx-item' + (item.disabled ? ' disabled' : '');
            btn.innerHTML =
                `<span class="ctx-item-icon">${item.icon}</span>` +
                `<span class="ctx-item-label">${item.label}</span>` +
                (item.shortcut ? `<span class="ctx-item-shortcut">${item.shortcut}</span>` : '');
            btn.addEventListener('click', () => { close(true); exec(item.id); });
            menu.appendChild(btn);
        }

        document.body.appendChild(menu);
        _menu = menu;

        // 边界修正
        const rect = menu.getBoundingClientRect();
        const mx = Math.min(x, window.innerWidth  - rect.width  - 6);
        const my = Math.min(y, window.innerHeight - rect.height - 6);
        menu.style.left = Math.max(0, mx) + 'px';
        menu.style.top  = Math.max(0, my) + 'px';
    }

    function close(instant) {
        if (!_menu) return;
        if (instant) { _menu.remove(); _menu = null; return; }
        _menu.classList.add('closing');
        const m = _menu;
        m.addEventListener('animationend', () => m.remove(), { once: true });
        _menu = null;
    }

    document.addEventListener('contextmenu', (e) => {
        e.preventDefault();
        show(e.clientX, e.clientY, e.target);
    });

    document.addEventListener('click', () => close());
    document.addEventListener('keydown', (e) => { if (e.key === 'Escape') close(); });
    window.addEventListener('blur', () => close(true));
    window.addEventListener('resize', () => close(true));

    return { show, close };
})();
