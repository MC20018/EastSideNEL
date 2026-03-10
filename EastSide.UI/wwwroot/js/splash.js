const Splash = {
    _overlay: null,

    init() {
        this._overlay = document.getElementById('splash-overlay');
        if (sessionStorage.getItem('splash_done')) {
            if (this._overlay) {
                this._overlay.style.display = 'none';
            }
        }
    },

    hide() {
        if (this._overlay) {
            this._overlay.classList.add('hide');
            sessionStorage.setItem('splash_done', '1');
        }
    },

    async waitAndHide(minDisplayMs = 1500) {
        if (sessionStorage.getItem('splash_done')) return;
        await sleep(minDisplayMs);
        this.hide();
    }
};
