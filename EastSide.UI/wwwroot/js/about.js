function renderAbout(userData) {
    setTimeout(() => loadAboutInfo(), 0);
    return `
    <div class="about-page">
        <div class="about-hero">
            <img src="assets/logo.png" alt="" class="about-logo" onerror="this.style.display='none'" />
            <div class="about-hero-text">
                <h1 class="about-title">EastSide NEL</h1>
                <div class="about-version" id="about-version">...</div>
            </div>
        </div>
        <div class="about-card">
            <div class="about-section-title">版权信息</div>
            <p>Copyright &copy; 2025 EastSide. All Rights Reserved.</p>
            <p class="about-muted">Thank Codexus!</p>
        </div>
        <div class="about-card">
            <div class="about-section-title">链接</div>
            <div class="about-links">
                <a class="about-link" href="#" id="about-link-github">
                    <svg viewBox="0 0 16 16" fill="currentColor" width="16" height="16"><path d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.013 8.013 0 0016 8c0-4.42-3.58-8-8-8z"/></svg>
                    GitHub
                </a>
                <a class="about-link" href="#" id="about-link-qq2">
                    <svg viewBox="0 0 16 16" fill="currentColor" width="16" height="16"><path d="M8 1C4.13 1 1 3.58 1 6.75c0 1.83 1.03 3.47 2.65 4.55-.07.52-.26 1.26-.7 1.95.92-.16 1.77-.56 2.4-1.05.51.12 1.06.18 1.65.18 3.87 0 7-2.58 7-5.63S11.87 1 8 1z"/></svg>
                    官方2群
                </a>
            </div>
        </div>
    </div>
    `;
}

async function loadAboutInfo() {
    const versionEl = document.getElementById('about-version');
    if (!versionEl) return;
    try {
        const r = await Bridge.send('system:about');
        if (r.success && r.data) {
            versionEl.textContent = 'Version ' + (r.data.version || '未知');
        }
    } catch {}

    document.getElementById('about-link-github')?.addEventListener('click', (e) => {
        e.preventDefault();
        Bridge.send('system:openUrl', { url: 'https://github.com/EastSide/oxygen' });
    });

    document.getElementById('about-link-qq2')?.addEventListener('click', (e) => {
        e.preventDefault();
        Bridge.send('system:openUrl', { url: 'https://qm.qq.com/q/rx6zWIjPUW' });
    });
}
