const Bridge = (() => {
    const pending = new Map();

    function generateId() {
        return crypto.randomUUID ? crypto.randomUUID() : Math.random().toString(36).slice(2) + Date.now().toString(36);
    }

    function send(action, data = null, timeout = 30000) {
        return new Promise((resolve, reject) => {
            const requestId = generateId();
            const message = JSON.stringify({ action, requestId, data });

            const timer = setTimeout(() => {
                pending.delete(requestId);
                reject(new Error(`请求超时: ${action}`));
            }, timeout);

            pending.set(requestId, { resolve, reject, timer });

            window.external.sendMessage(message);
        });
    }

    window.external.receiveMessage(raw => {
        try {
            const msg = JSON.parse(raw);
            const requestId = msg.requestId;

            if (requestId && pending.has(requestId)) {
                const { resolve, timer } = pending.get(requestId);
                clearTimeout(timer);
                pending.delete(requestId);
                resolve(msg);
            } else {
                window.dispatchEvent(new CustomEvent('bridge:push', { detail: msg }));
            }
        } catch (e) {
            console.error('Bridge: 解析消息失败', e, raw);
        }
    });

    return { send };
})();
