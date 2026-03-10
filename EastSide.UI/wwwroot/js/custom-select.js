class CustomSelect {
    constructor(el) {
        this.el = el;
        this.trigger = el.querySelector('.custom-select-trigger');
        this.dropdown = el.querySelector('.custom-select-dropdown');
        this.options = [];
        this.value = '';
        this.onChange = null;

        this.trigger.addEventListener('click', (e) => {
            e.stopPropagation();
            this.toggle();
        });

        document.addEventListener('click', (e) => {
            if (!this.el.contains(e.target)) this.close();
        });
    }

    toggle() {
        this.el.classList.toggle('open');
    }

    close() {
        this.el.classList.remove('open');
    }

    setOptions(opts) {
        this.options = opts;
        this.dropdown.innerHTML = '';
        if (opts.length === 0) {
            this.dropdown.innerHTML = '<div class="custom-select-empty">无选项</div>';
            this.value = '';
            this.trigger.textContent = '无选项';
            return;
        }
        for (let i = 0; i < opts.length; i++) {
            const div = document.createElement('div');
            div.className = 'custom-select-option';
            div.textContent = opts[i].label;
            div.dataset.value = opts[i].value;
            div.dataset.index = i;
            div.addEventListener('click', (e) => {
                e.stopPropagation();
                this.selectIndex(parseInt(div.dataset.index));
                this.close();
                if (this.onChange) this.onChange(this.value);
            });
            this.dropdown.appendChild(div);
        }
    }

    selectIndex(idx) {
        if (idx < 0 || idx >= this.options.length) return;
        this.value = this.options[idx].value;
        this.trigger.textContent = this.options[idx].label;
        this.dropdown.querySelectorAll('.custom-select-option').forEach((el, i) => {
            el.classList.toggle('selected', i === idx);
        });
    }
}
