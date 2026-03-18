EastSideNEL
===========

[![EastSideNEL Download](https://img.shields.io/github/downloads/MC20018/EastSideNEL/total?color=0&logo=github)](https://github.com/MC20018/EastSideNEL/releases/latest)
[![QQ](https://img.shields.io/badge/QQ_Unofficial-1090382774-blue)](https://qm.qq.com/q/yUpUFBbcLm)

## 本项目基于 [Codexus.OpenSDK](https://github.com/corpnetease/Codexus.OpenSDK) 以及 Codexus.Development.SDK 制作的第三方启动器

### 功能一览

#### 皮肤显示
- 自动获取网易皮肤并在 Java 版客户端中显示
- 通过 MineSkin API 生成 Mojang 签名的 textures property
- 服务端缓存 + 客户端内存缓存，已缓存皮肤秒加载
- 已收录 6000+ 玩家皮肤，持续增长中
- 支持普通皮肤和纤细模型（slim）
- 自动缩放非标准尺寸皮肤至 64x64
- 异步加载，不阻塞游戏

#### IRC 跨服聊天
- 内置 IRC 系统，支持跨服务器聊天
- TAB 列表显示 IRC 在线玩家（带 [ES] 前缀）
- 支持 1.20.6 / 1.12.2 / 1.8.9 / 1.7.x 等多版本
- 可配置在线提示频率，支持开关

#### 封禁检测
- 自动检测小黑屋封禁
- 检测到封禁后可自动关闭通道或切换角色
- 封禁记录管理

#### 协议支持
- 支持 1.20.6 协议（PlayerInfoUpdate / PlayerInfoRemove 拦截与重建）
- 支持 1.8.x / 1.7.x / 1.12.2 / 1.20.x / 1.21.x 聊天协议
- 兼容 ViaVersion 跨版本连接

#### 设置
- 外观：主题模式、主题色、背景、自定义壁纸
- 功能：自动复制 IP、封禁检测操作、调试模式、IRC 在线提示开关及频率
- 网络：Socks5 代理支持
- 其他：混合登录模式

#### 插件系统
- 内置插件商店
- 支持第三方插件安装

### 安装要求
- .NET 9.0

### 安装步骤
1. 在 QQ 群下载最新的构建结果
2. 双击打开即可

### 怎么装插件？
1. 找到插件页面
2. 点击插件商店
3. 安装想要的插件

### 数据目录在哪？
1. 网页数据位于 C:\ProgramData\EastSide
2. 用户的数据或软件的数据在软件同级目录的 data 文件夹

### 项目结构
- `EastSide` - 主项目（协议处理、管理器、事件处理）
- `EastSide.Core` - 核心库（网易 API、工具类、加密）
- `EastSide.IRC` - IRC 聊天系统（跨服聊天、TAB 列表、皮肤注入）
- `EastSide.UI` - 用户界面（Photino + Web 前端）

## 开源协议
本项目遵循 **[GNU GPLv3](https://www.gnu.org/licenses/gpl-3.0.html)** 协议开源
```text
EastSideNEL Copyright (C) 2026 FandMC Studio
本程序是自由软件，你可以重新发布或修改它，但必须：
- 保留原始版权声明
- 采用相同许可证分发
- 提供完整的源代码
```
详细条款请查阅 [LICENSE](LICENSE) 文件。

## 项目历史

EastSideNEL 经历了多次更名与重构，以下是完整的演变历程：

| 阶段 | 名称 | 提交数 | 备注 |
|------|------|--------|------|
| 1 | OpenSDK.NEL | 31 | 最初版本，基于 Codexus.OpenSDK 的早期探索 |
| 2 | OpenNEL | 86 | 功能逐步完善，提交最活跃的阶段 |
| 3 | RunNEL | 4 | 短暂过渡版本 |
| 4 | OxygenNEL | 15+ | 后期转为闭源，部分提交记录丢失 |
| 5 | NorthSideNEL | — | 未开源，无提交记录 |
| 6 | EastSideNEL | 进行中 | 当前版本，重新开源 |
