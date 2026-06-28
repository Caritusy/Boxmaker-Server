# Boxmaker 服务端

**Boxmaker** 的 ASP.NET Core 服务端实现，适用于私有部署、兼容客户端测试、停服后的保存研究，以及后续自定义开发。

English documentation is available at [README.md](./README.md).

## 项目简介

本项目实现了 Boxmaker 游戏 HTTP 协议、本地账号持久化、服务端地图存储、任务流程、回放/排行数据，以及一个浏览器端运维控制台。

该服务端面向兼容的 Boxmaker 客户端和受控部署环境。本项目与原发行方没有官方从属关系。

## 主要特性

- 基于 ASP.NET Core，目标框架为 `.NET 9.0`。
- 支持 protobuf 客户端数据包解析与响应序列化。
- 支持兼容旧客户端流量的 DES 数据解码。
- 支持账号登录、游客创建、注册、切换账号、资料编辑和密码更新。
- 使用本地文件保存账号、地图、任务、最近游玩、收藏和玩家状态数据。
- 支持服务端地图上传、下载、搜索、列表、点赞、评论、通关、回放和排行。
- 支持编辑器地图流程：创建、保存、改名、上传、删除、下载和查看。
- 支持任务/挑战接口：开始、继续、重玩、成功、失败、结算和放弃。
- 地图排行与回放/录像数据关联存储。
- 启动时重建玩家状态，用于资料页和 Web 页面展示。
- 对非关键持久化工作使用 IO 队列，减少请求处理中的无必要阻塞。
- 已现代化控制台输出，运行诊断信息更清楚。
- 内置 Razor Web 控制台，支持登录、资料编辑、国家/地区预设、头像预设、密码更新和公开地图搜索。
- 内置国家/地区配置文件 `Resources/config/t_guojia.txt`。
- 静态网页资源位于 `wwwroot`。

## 当前安全说明

已针对当前代码路径做过一次文件穿越风险检查。没有发现可由 HTTP 参数触发的账号、地图、任务、资料或 Web 控制台文件穿越暴露。

关键边界如下：

- 账号目录来自经过校验的 `openid`。注册会拒绝路径分隔符和 Windows 非法文件名字符。
- 登录和 Web 登录通过账号缓存查找路径，不会把请求里的字符串直接拼进文件路径。
- 服务端地图文件通过整数地图 ID 定位。
- 用户地图列表、任务数据、玩家状态和资料文件都使用账号目录下的固定文件名。
- `BOXMAKER_COUNTRY_CONFIG` 是部署时可选环境变量，不来自 HTTP 输入。

这不是完整安全审计，也不能替代认证加固、权限模型检查或恶意数据包 fuzzing；这里只覆盖当前实现里的文件穿越风险。

## 优点

- 可以用标准 .NET 工具链运行和部署，适合私有服维护。
- 协议 DTO 与主要业务逻辑相对分离。
- `AccountManager` 和 `BoxmakerProxy` 按业务域拆成 partial class，便于维护。
- 本地文件持久化便于备份、迁移和人工检查。
- 同时提供游戏客户端接口和浏览器端管理页面。
- 提供玩家统计、最近游玩、收藏地图和更可读的地图搜索展示。
- 国家/地区数据已随项目发布，不再只依赖开发机绝对路径。

## 缺点与限制

- 持久化仍是本地文件方式，不是数据库。
- 目前还没有自动化测试套件。
- 部分协议和遗留辅助代码沿用原项目命名风格，并仍会产生 nullable warning。
- 一些旧路径仍存在同步文件 IO。
- 数据模型变更需要谨慎处理，因为已有账号和地图文件就是线上存档。
- 该项目面向兼容的 Boxmaker 客户端，不是通用公开 API。

## 环境要求

- .NET SDK `9.0` 或更新版本。
- 兼容的 Boxmaker 客户端构建。
- Windows、Linux，或任何可以运行 ASP.NET Core 的环境。
- 对服务端数据目录具有读写权限。

## 快速开始

还原并构建：

```powershell
dotnet restore
dotnet build
```

运行：

```powershell
dotnet run
```

仓库中的 Kestrel 配置监听：

```text
http://0.0.0.0:13500
```

浏览器控制台地址：

```text
http://localhost:13500/
```

## 配置说明

国家/地区预设按以下顺序加载：

1. `BOXMAKER_COUNTRY_CONFIG`
2. 发布目录旁的 `Resources/config/t_guojia.txt`
3. 当前工作目录下的 `Resources/config/t_guojia.txt`
4. Unity 导出工程使用的旧开发路径

仓库已包含 `Resources/config/t_guojia.txt`，项目文件会在 build/publish 时复制它。

## 项目结构

```text
AccountManager/      账号文件、地图缓存、任务、玩家状态、编辑器地图和 IO 队列逻辑
BoxmakerProxy/       按认证、地图、编辑器和任务分组的 HTTP 路由处理
Infrastructure/      控制台输出、文件辅助和 DES 辅助
Models/              服务端地图、任务和地图数据模型
Networking/          数据包辅助、操作码和网络消息容器
Pages/               Razor Web 控制台
protocol.game/       游戏协议 DTO
protocol.map/        地图协议 DTO
Resources/           随项目发布的配置文件
Services/            运行期服务辅助
Utilities/           通用工具和版本辅助
wwwroot/             静态网页资源
```

## 开发说明

- 除非正在处理协议兼容，否则尽量不要修改 `protocol.game` 和 `protocol.map`。
- 新增游戏 HTTP 接口优先放在 `BoxmakerProxy/`。
- 账号、地图、任务和玩家状态逻辑优先放在 `AccountManager/`。
- 热路径优先使用地图缓存辅助，不要在请求处理中扫描地图文件。
- 修改数据模型时要考虑已有存档兼容。

## 配套客户端资源

- [Boxmaker 客户端源码](https://github.com/Caritusy/Boxmaker)
- [Boxmaker 客户端发布页](https://github.com/Caritusy/Boxmaker-Release/releases)

## 许可证

请查看仓库内包含的许可证文件。
