const SidebarIndicator = {
    _el: null,

    init(sidebar) {
        const indicator = document.createElement('div');
        indicator.className = 'nav-indicator';
        indicator.style.opacity = '0';
        sidebar.appendChild(indicator);
        this._el = indicator;

        const active = sidebar.querySelector('.nav-item.active');
        if (active) {
            this._jumpTo(active, sidebar);
        }
    },

    moveTo(navItem, sidebar) {
        if (!this._el || !navItem) return;

        const sidebarRect = sidebar.getBoundingClientRect();
        const itemRect = navItem.getBoundingClientRect();

        const top = itemRect.top - sidebarRect.top + sidebar.scrollTop;
        const height = itemRect.height;

        const padding = 8;

        this._el.style.top = (top + padding) + 'px';
        this._el.style.height = (height - padding * 2) + 'px';
        this._el.style.opacity = '1';
    },

    _jumpTo(navItem, sidebar) {
        if (!this._el || !navItem) return;

        this._el.style.transition = 'none';

        this.moveTo(navItem, sidebar);

        this._el.offsetHeight;
        this._el.style.transition = '';
    }
};
