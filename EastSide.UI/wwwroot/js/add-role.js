function showAddRoleDialog(serverId, onSuccess, action, accountId) {
    const createAction = action || 'network:createRole';
    const overlay = document.createElement('div');
    overlay.className = 'dialog-overlay';
    overlay.style.zIndex = '5100';
    overlay.innerHTML = `
        <div class="dialog-box" style="width:360px">
            <div class="dialog-header">
                <h3>添加角色</h3>
                <button class="dialog-close" id="role-close-btn">&times;</button>
            </div>
            <div class="dialog-body">
                <div class="form-group">
                    <input id="role-name-input" type="text" placeholder="输入角色名称" maxlength="8" />
                </div>
                <button class="btn-random-name" id="role-random-btn">随机名字</button>
                <div id="role-error" class="dialog-error"></div>
            </div>
            <div class="dialog-footer">
                <button class="btn-secondary" id="role-cancel-btn">取消</button>
                <button class="btn-accent" id="role-confirm-btn">添加</button>
            </div>
        </div>
    `;
    document.body.appendChild(overlay);

    const close = () => {
        overlay.classList.add('closing');
        overlay.addEventListener('animationend', () => overlay.remove(), { once: true });
    };
    overlay.querySelector('#role-close-btn').addEventListener('click', close);
    overlay.querySelector('#role-cancel-btn').addEventListener('click', close);
    overlay.addEventListener('click', (e) => { if (e.target === overlay) close(); });

    setTimeout(() => overlay.querySelector('#role-name-input')?.focus(), 50);

    overlay.querySelector('#role-random-btn').addEventListener('click', () => {
        overlay.querySelector('#role-name-input').value = generateRandomName();
    });

    overlay.querySelector('#role-confirm-btn').addEventListener('click', async () => {
        const name = overlay.querySelector('#role-name-input').value.trim();
        const errorEl = overlay.querySelector('#role-error');
        if (!name) { errorEl.textContent = '请输入角色名称'; return; }

        const btn = overlay.querySelector('#role-confirm-btn');
        btn.classList.add('btn-loading');

        try {
            const payload = { serverId, roleName: name };
            if (accountId) payload.accountId = accountId;
            const cr = await Bridge.send(createAction, payload);
            if (cr.success && cr.data?.roles) {
                showToast('角色创建成功', 'success');
                close();
                if (onSuccess) onSuccess(cr.data.roles);
            } else {
                errorEl.textContent = cr.data?.message || '创建失败';
            }
        } catch (e) {
            errorEl.textContent = '创建角色失败';
        } finally {
            btn.classList.remove('btn-loading');
        }
    });
}

const _rnAdj = [
    'Dark','Ice','Red','Sky','Sun','Ash','Dew','Fog','Grim','Hex',
    'Ivy','Jet','Kin','Lux','Neo','Odd','Raw','Sly','Zen','Arc',
    'Dim','Elm','Fey','Haze','Cold','Hot','Dry','Wet','Old','New',
    'Big','Sly','Mad','Sad','Bad','Shy','Coy','Wry','Icy','Orc',
    'Elf','Gem','Nyx','Pax','Rex','Rox','Zap','Ace','Axe','Bay',
    'Blu','Cob','Dax','Eve','Fin','Gil','Hal','Ion','Jax','Koi',
    'Leo','Max','Nix','Oak','Pix','Qin','Rio','Sol','Tao','Uma',
    'Vox','Wiz','Xen','Yew','Zed','Oni','Boa','Cub','Doe','Emu',
    'Fig','Gnu','Hop','Imp','Jay','Kit','Lac','Mew','Nap','Orb',
    'Pug','Rum','Sap','Tin','Urn','Van','Wax','Yak','Zip','Ale'
];
const _rnNoun = [
    'Wolf','Fox','Hawk','Bear','Lynx','Crow','Deer','Owl','Pike','Wren',
    'Moth','Fang','Claw','Bone','Star','Moon','Fire','Sage','Rune','Vex',
    'Bolt','Mist','Gale','Dusk','Dawn','Void','Flux','Haze','Jade','Onyx',
    'Opal','Ruby','Tusk','Vine','Wave','Yeti','Apex','Bane','Core','Doom',
    'Edge','Fury','Glow','Horn','Iron','Jinx','Knot','Lore','Maze','Nuke',
    'Omen','Pyre','Rift','Scar','Tide','Urge','Vale','Wisp','Xray','Yawn',
    'Zeal','Axle','Bark','Cave','Dune','Echo','Fern','Grit','Husk','Isle',
    'Jolt','Kelp','Lava','Mane','Nest','Oath','Palm','Quay','Root','Silk',
    'Twig','Vent','Warp','Yarn','Zinc','Arch','Blot','Coil','Dart','Fist',
    'Gust','Helm','Iris','Kite','Lamp','Mace','Node','Oryx','Pond','Reef'
];

function generateRandomName() {
    const pick = arr => arr[Math.floor(Math.random() * arr.length)];
    for (let i = 0; i < 50; i++) {
        const adj = pick(_rnAdj);
        const noun = pick(_rnNoun);
        const useSuffix = Math.random() > 0.4;
        const suffix = useSuffix ? String(Math.floor(Math.random() * 99) + 1) : '';
        const name = adj + noun + suffix;
        if (name.length <= 8) return name;
    }
    return pick(_rnAdj) + pick(_rnNoun).slice(0, 3);
}
