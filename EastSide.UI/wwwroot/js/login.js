function showLoginPage(content) {
    content.innerHTML = `
        <div class="page login-page">
            <h2>EastSide</h2>
            <p class="subtitle">请登录以继续</p>
            <div class="form-group">
                <input id="login-user" type="text" placeholder="用户名 / 邮箱" />
            </div>
            <div class="form-group">
                <input id="login-pass" type="password" placeholder="密码" />
            </div>
            <button id="login-btn" class="btn-primary">登录</button>
        </div>
    `;
    bindLogin();
}

function bindLogin() {
    const btn = document.getElementById('login-btn');
    btn.addEventListener('click', async () => {
        const username = document.getElementById('login-user').value.trim();
        const password = document.getElementById('login-pass').value.trim();
        if (!username || !password) { showToast('请输入用户名和密码', 'warning'); return; }

        btn.disabled = true;
        btn.textContent = '登录中...';

        try {
            const result = await Bridge.send('auth:login', { username, password });
            if (result.success) {
                showToast('登录成功', 'success');
                showMainLayout(result.data);
            } else {
                showToast(result.data?.message || '登录失败', 'error');
            }
        } catch (e) {
            showToast('请求失败: ' + e.message, 'error');
        } finally {
            btn.disabled = false;
            btn.textContent = '登录';
        }
    });
}
