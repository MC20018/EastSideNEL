function renderOverview(userData) {
    const avatarUrl = userData.avatar || 'assets/logo.png';
    const cached = PageCache.announcement;
    if (!cached) setTimeout(() => loadAnnouncement(), 0);
    setTimeout(() => loadRecentPlay(), 0);
    return `
    <h2>概括</h2>
    <div class="overview-grid">
        <div class="card card-announcement">
            <div id="announcement-content"${cached ? '' : ' class="announcement-loading"'}>${cached || '加载中...'}</div>
        </div>
        <div class="card card-userinfo">
            <div class="userinfo-avatar">
                <img src="${escapeHtml(avatarUrl)}" alt="" onerror="this.src='assets/logo.png'" />
            </div>
            <div class="userinfo-detail">
                <div class="userinfo-name">${escapeHtml(userData.username || '未知用户')}</div>
                <div class="userinfo-id">ID: ${userData.userId || 0}</div>
            </div>
            <div class="userinfo-rank">${parseMcColor(userData.rank || '暂无称号')}</div>
        </div>
    </div>
    <div id="recent-play"></div>
    `;
}

async function loadAnnouncement() {
    const el = document.getElementById('announcement-content');
    if (!el) return;

    if (PageCache.announcement) {
        el.innerHTML = PageCache.announcement;
        el.classList.remove('announcement-loading');
        return;
    }

    try {
        const result = await Bridge.send('system:announcement');
        if (result.success && result.data) {
            const d = result.data;
            const dateStr = d.updated ? formatDate(d.updated) : '';
            const html = `
                <div class="announcement-item">
                    ${d.title ? `<div class="announcement-title">${escapeHtml(d.title)}</div>` : ''}
                    <p class="announcement-text">${escapeHtml(d.content || '暂无公告')}</p>
                    ${dateStr ? `<span class="announcement-date">${dateStr}</span>` : ''}
                </div>
            `;
            PageCache.announcement = html;
            el.innerHTML = html;
        } else {
            el.textContent = result.data?.message || '获取公告失败';
        }
    } catch (e) {
        el.textContent = '获取公告失败: ' + e.message;
    }
    el.classList.remove('announcement-loading');
}

async function loadRecentPlay() {
    const container = document.getElementById('recent-play');
    if (!container) return;

    try {
        const result = await Bridge.send('overview:recent');
        if (!result.success || !result.data?.items?.length) {
            container.innerHTML = '';
            return;
        }

        const items = result.data.items;
        const listHtml = items.map(item => {
            const typeLabel = item.type === 'rental' ? '租赁服' : '网络服';
            const typeCls = item.type === 'rental' ? 'type-rental' : 'type-network';
            const time = formatDate(item.playTime);
            return `
                <div class="recent-play-item" data-id="${escapeHtml(item.serverId)}"
                     data-name="${escapeHtml(item.serverName)}" data-type="${escapeHtml(item.type)}"
                     data-version="${escapeHtml(item.mcVersion || '')}"
                     data-pwd="${item.hasPassword ? '1' : '0'}">
                    <div class="recent-play-info">
                        <span class="recent-play-name">${escapeHtml(item.serverName)}</span>
                        <span class="recent-play-type ${typeCls}">${typeLabel}</span>
                    </div>
                    <div class="recent-play-meta">
                        <span class="recent-play-time">${time}</span>
                        <button class="recent-play-join">加入</button>
                    </div>
                </div>`;
        }).join('');

        container.innerHTML = `
            <div class="recent-play-section">
                <h3 class="recent-play-title">最近游玩</h3>
                <div class="recent-play-list">${listHtml}</div>
            </div>`;

        container.querySelectorAll('.recent-play-join').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                const item = btn.closest('.recent-play-item');
                handleRecentJoin(item.dataset);
            });
        });
    } catch (e) {
        container.innerHTML = '';
    }
}

function handleRecentJoin(data) {
    if (!_currentUserData || !_currentUserData.userId) {
        document.querySelector('.nav-item[data-page="account"]')?.click();
        showToast('请先登录账号', 'warning');
        return;
    }

    const page = data.type === 'rental' ? 'rental' : 'network';
    document.querySelector(`.nav-item[data-page="${page}"]`)?.click();

    setTimeout(() => {
        if (data.type === 'rental') {
            openRentalDetail(data.id, data.name, data.version, data.pwd === '1');
        } else {
            openNetworkDetail(data.id, data.name);
        }
    }, 300);
}
