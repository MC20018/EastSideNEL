function showLoginPage(content) {
    content.innerHTML = `
        <div class="page login-page">
            <h2>EastSide</h2>
            <div id="login-form">
                <p class="subtitle">请登录以继续</p>
                <div class="form-group">
                    <input id="login-user" type="text" placeholder="用户名 / 邮箱" />
                </div>
                <div class="form-group">
                    <input id="login-pass" type="password" placeholder="密码" />
                </div>
                <button id="login-btn" class="btn-primary">登录</button>
                <p class="form-link">没有账号？<a href="#" id="show-register">注册</a></p>
            </div>
            <div id="register-form" style="display:none">
                <p class="subtitle">创建新账号</p>
                <div class="form-group">
                    <input id="reg-user" type="text" placeholder="用户名（3-20位，字母数字下划线中文）" />
                </div>
                <div class="form-group">
                    <input id="reg-email" type="email" placeholder="邮箱" />
                </div>
                <div class="form-group">
                    <input id="reg-pass" type="password" placeholder="密码（至少8位，含大小写/数字/特殊字符中3种）" />
                </div>
                <div class="form-group">
                    <input id="reg-pass2" type="password" placeholder="确认密码" />
                </div>
                <button id="register-btn" class="btn-primary">注册</button>
                <p class="form-link">已有账号？<a href="#" id="show-login">登录</a></p>
            </div>
        </div>
    `;
    bindLogin();
    bindRegister();
    document.getElementById('show-register').addEventListener('click', e => {
        e.preventDefault();
        document.getElementById('login-form').style.display = 'none';
        document.getElementById('register-form').style.display = 'block';
    });
    document.getElementById('show-login').addEventListener('click', e => {
        e.preventDefault();
        document.getElementById('register-form').style.display = 'none';
        document.getElementById('login-form').style.display = 'block';
    });
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

function bindRegister() {
    const btn = document.getElementById('register-btn');
    btn.addEventListener('click', async () => {
        const username = document.getElementById('reg-user').value.trim();
        const email = document.getElementById('reg-email').value.trim();
        const password = document.getElementById('reg-pass').value;
        const confirmPassword = document.getElementById('reg-pass2').value;

        if (!username || !email || !password || !confirmPassword) {
            showToast('请填写所有字段', 'warning'); return;
        }
        if (password !== confirmPassword) {
            showToast('两次密码不一致', 'warning'); return;
        }

        btn.disabled = true;
        btn.textContent = '注册中...';

        try {
            const result = await Bridge.send('auth:register', { username, email, password, confirmPassword });
            if (result.success) {
                showToast('注册成功，已自动登录', 'success');
                showMainLayout(result.data);
            } else {
                showToast(result.data?.message || '注册失败', 'error');
            }
        } catch (e) {
            showToast('请求失败: ' + e.message, 'error');
        } finally {
            btn.disabled = false;
            btn.textContent = '注册';
        }
    });
}
