# 网易我的世界 皮肤 API 接口文档

> 基于网易我的世界启动器 (WPFLauncher) 反编译源码分析 + 实际接口验证
> 所有接口均为 `POST` 方法，请求体为 JSON，响应为 JSON

---

## 基础信息

### Base URL

```
https://x19mclobt.nie.netease.com
```

### 认证方式

每个请求需要携带以下 HTTP Header：

| Header | 说明 |
|--------|------|
| `user-id` | 登录用户的数字 ID |
| `user-token` | 基于请求路径和请求体动态计算的签名 |
| `X_TRACE_ID` | 随机 32 位字符串 (用于链路追踪) |
| `Content-Type` | `application/json` |

### Token 签名算法

`user-token` 由 `TokenUtil.ComputeHttpRequestToken` 计算，输入为请求路径、请求体、userId、userToken(登录凭证)。

```python
import hashlib, base64

TOKEN_SALT = "0eGsBkhl"

def compute_token(request_path, send_body, user_id, user_token):
    """计算 user-token 签名"""
    if not request_path.startswith('/'):
        request_path = '/' + request_path

    # 1. 拼接: MD5(userToken) + body + salt + path
    token_md5 = hashlib.md5(user_token.encode()).hexdigest().lower()
    buf = token_md5.encode() + send_body.encode() + TOKEN_SALT.encode() + request_path.encode()

    # 2. 整体 MD5
    h = hashlib.md5(buf).hexdigest().lower()

    # 3. 转二进制，循环左移 6 位
    binary = ''.join(format(ord(c), '08b') for c in h)
    secret = binary[6:] + binary[:6]

    # 4. XOR 处理
    t = bytearray(h.encode())
    for i in range(len(secret) // 8):
        chunk = secret[i*8:i*8+8]
        num = 0
        for j in range(len(chunk)):
            if chunk[7 - j] == '1':
                num |= (1 << j)
        t[i] ^= num

    # 5. Base64 前 12 字节 + "1"，替换特殊字符
    result = (base64.b64encode(bytes(t[:12])).decode() + "1").replace('+','m').replace('/','o')

    return {"user-id": user_id, "user-token": result}
```

### 通用响应结构

```json
{
  "code": 0,
  "message": "正常返回",
  "entities": [ ... ],
  "entity": { ... },
  "total": 0
}
```

| code | 含义 |
|------|------|
| 0 | 成功 |
| 10 | 请先登录 (认证失败) |
| 12 | 参数错误 / 签名不匹配 |
| 16 | 目标找不到 |

---

## 核心流程：通过玩家名获取任意玩家的皮肤图片

```
玩家名 ──①──> user_id ──②──> skin_id ──③──> res_url (皮肤图片)
```

> 已实际验证可查询任意玩家的皮肤，不限于自己。

### 第①步：玩家名 → user_id

```
POST /game-character/query/search-by-character
```

```json
{
  "game_id": "服务器/游戏实例ID",
  "game_type": 2,
  "name": "目标玩家名"
}
```

> `game_type` 需要遍历尝试 `[2, 7, 8, 9, 10]`。实测租赁服玩家角色注册在 `game_type=2` (NET_GAME) 下，`game_type=8` (SERVER_GAME) 反而查不到。建议按 `[2, 8, 9, 7, 10]` 顺序遍历，优先命中。

**响应：**

```json
{
  "code": 0,
  "entities": [
    {
      "user_id": "899673653",
      "game_id": "4661334467366178884",
      "game_type": 2,
      "name": "FogBear",
      "create_time": 1773570541
    }
  ]
}
```

---

### 第②步：user_id → skin_id

```
POST /user-game-skin/query/search-by-type
```

```json
{
  "user_id": "目标用户ID"
}
```

> 只需传 `user_id`，不需要 `game_type` 和 `client_type`，服务端会返回所有组合。

**响应：**

```json
{
  "code": 0,
  "entities": [
    {
      "user_id": "387075602",
      "game_type": 2,
      "skin_type": 31,
      "skin_id": "4630108215716945668",
      "skin_mode": 0,
      "client_type": "java"
    }
  ]
}
```

- `skin_mode`：0 = 默认模型，1 = 纤细模型 (slim)
- `skin_type`：31 = 普通皮肤，41 = 4D皮肤，42 = 特殊皮肤
- 如果 `entities` 为空，说明该玩家使用默认皮肤 (Steve/Alex)

---

### 第③步：skin_id → 皮肤图片 URL

**单个查询：**

```
POST /user-item-download-v2
```

```json
{
  "item_id": "皮肤物品ID"
}
```

**批量查询：**

```
POST /user-item-download-v2/get-list
```

```json
{
  "item_id_list": ["4630108215716945668", "4635701020889602309"]
}
```

**响应：**

```json
{
  "code": 0,
  "entities": [
    {
      "item_id": "4630108215716945668",
      "itype": 2,
      "mtypeid": 10,
      "stypeid": 31,
      "sub_entities": [
        {
          "res_url": "https://x19.fp.ps.netease.com/file/5c9780522786fdf93abcc6d9mSQm0AT002",
          "res_size": 1350,
          "res_md5": "e5a2e67dba2c2d6bb80817a1c9211036",
          "res_name": "steve.png",
          "res_version": 1
        }
      ]
    }
  ]
}
```

> `sub_entities[0].res_url` 就是皮肤图片 CDN 地址，直接 GET 下载即可。

---

## 实际查询示例

### 查询玩家 widii 的皮肤

```
① POST /game-character/query/search-by-character
   {"game_id":"4661334467366178884","game_type":2,"name":"widii"}
   => user_id: 387075602

② POST /user-game-skin/query/search-by-type
   {"user_id":"387075602"}
   => skin_ids: [4630108215716945668, 4635701020889602309]

③ POST /user-item-download-v2/get-list
   {"item_id_list":["4630108215716945668","4635701020889602309"]}
   => https://x19.fp.ps.netease.com/file/5c9780522786fdf93abcc6d9mSQm0AT002
   => https://x19.fp.ps.netease.com/file/5def646d143cfa9055cbd143rhOMFsHX02
```

---

## 其他接口

### 查询物品详情

```
POST /item/query/search-by-iid          # 单个
POST /item/query/search-by-ids          # 批量 {"item_id_list": [...]}
```

### 查询自己已购皮肤列表

```
POST /user-item-purchase/query/search-by-user
```

```json
{
  "user_id": "自己的用户ID",
  "item_type": 2,
  "master_type_id": 10,
  "length": 100,
  "offset": 0
}
```

> 此接口只能查自己的已购列表，传别人的 user_id 仍返回自己的数据。

### 批量设置游戏皮肤

```
POST /user-game-skin-multi
```

```json
{
  "skin_settings": [
    {"game_type": 7, "skin_type": 31, "skin_id": "ID", "skin_mode": 0, "client_type": "java"},
    {"game_type": 2, "skin_type": 31, "skin_id": "ID", "skin_mode": 0, "client_type": "java"}
  ]
}
```

### 本地自定义皮肤

```
POST /user-local-skin/query/search-by-type    # 查询 {"skin_type": 31}
POST /user-local-skin                          # 上传保存
POST /user-local-skin/delete                   # 删除
```

### 收藏管理

```
POST /user-favorite-item/add       # 收藏 {"iid": "物品ID"}
POST /user-favorite-item/delete    # 取消收藏
```

### 图片上传

```
POST /image-upload-token           # 获取上传凭证
POST /image-upload-token/delete    # 删除已上传图片
```

上传流程：获取 token → 向 token.url 发送 PUT 请求 (Header: `Authorization: {token}`, Body: 文件二进制)

---

## 枚举值参考

### TextureType (皮肤类型)

| 值 | 名称 | 说明 |
|----|------|------|
| 31 | `SKIN` | 普通皮肤 |
| 41 | `FOUR_DIMENSIONAL_SKIN` | 4D 皮肤 |
| 42 | `SPECIAL_SKIN` | 特殊皮肤 |

### GType (游戏类型)

| 值 | 名称 | 说明 |
|----|------|------|
| 2 | `NET_GAME` | 网络服务器 |
| 7 | `MC_GAME` | 本地存档游戏 |
| 8 | `SERVER_GAME` | 租赁服 |
| 9 | `LAN_GAME` | 局域网联机 |
| 10 | `ONLINE_LOBBY_GAME` | 在线大厅 (花雨庭等) |

### GameClientType

| 值 | 说明 |
|----|------|
| `all` | 全部 |
| `java` | Java 版 |
| `cpp` | 基岩版 |

---

## 客户端 Mod 侧 (0.jar)

客户端 mod (`SkinHandler.java`) 通过本地消息通道与启动器通信：

1. 客户端发送 SMID `2050`：`{gameid, 玩家名, UUID}`
2. 启动器调用上述 API 获取皮肤
3. 启动器返回：`{name, skinPath, capePath, isSlim}`
4. 客户端 SHA256→SHA1 哈希后存入 `./assets/skins/`
5. 通过 `http://127.0.0.1/{sha256_hash}` 本地加载

域名白名单：`.minecraft.net`, `.mojang.com`, `.163.com`, `.netease.com`

缓存：主缓存 30 分钟，加载缓存 15 秒，消息超时 60 秒
