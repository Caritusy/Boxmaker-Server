# Boxmaker 服务端

本仓库是 **Boxmaker（盒子制造）** 的 ASP.NET Core 服务端实现，适用于本地搭建、私服部署、停服后的保存研究，以及兼容客户端的自定义服务端实验。

English documentation is available in [README.md](./README.md).

## 项目简介

Boxmaker 原为 **银月网络** 发行的网络游戏，官方服务目前已经停止运营。本项目基于原有服务端行为进行适配、整理和扩展，用于配合兼容客户端继续运行游戏核心功能。

本项目与原发行方无从属或官方关联。

## 主要特性

- 基于 ASP.NET Core，目标框架为 `.NET 9.0`。
- 支持 protobuf 客户端数据包解析和响应。
- 支持兼容客户端所需的 DES 数据解码。
- 支持账号登录、注册、Token 校验、资料更新和密码修改。
- 使用本地文件保存玩家账号数据。
- 支持服务端地图存储、读取、上传、下载、搜索、点赞、评论和排行榜数据。
- 支持编辑器地图流程：创建、保存、改名、上传、删除和单图查看。
- 支持任务和挑战相关接口，包括开始、继续、重玩、失败、成功和放弃流程。
- 支持录像/回放数据保存，并与地图排行榜关联。
- 启动时重建玩家状态，供资料页和 Web 页面使用。
- 支持最近游玩地图和收藏地图记录。
- 内置简单 Razor Web 控制台，支持登录、资料编辑、密码修改和公开地图搜索。
- 静态网页资源位于 `wwwroot`。

## 优点

- 可以作为普通 ASP.NET Core 项目运行，部署方式清晰。
- 协议类与业务逻辑目录分离。
- `AccountManager` 和 `BoxmakerProxy` 使用 partial class 按业务域拆分，便于维护。
- 数据以本地文件保存，适合私有测试、备份和迁移。
- 同时包含客户端游戏接口和浏览器端运维页面。
- 当前目录结构已经按模型、网络、服务、基础设施、代理路由、账号逻辑、页面和协议文件分区。

## 缺点和已知限制

- `protocol.game` 和 `protocol.map` 属于旧式/生成式协议代码，仍然存在较多 nullable warning。
- 持久化是本地文件方式，不是数据库。
- 部分辅助类仍沿用原项目命名风格，并非完全现代 C# 命名。
- 目前还没有自动化测试套件。
- 部署时需要准备或让程序创建账号、地图等运行数据目录。
- 服务端面向兼容 Boxmaker 客户端，不是通用公开 API。

## 环境要求

- .NET SDK `9.0` 或更新版本。
- 兼容的 Boxmaker 客户端。
- Windows、Linux，或任何可以运行 ASP.NET Core 并访问配置数据目录的环境。

## 快速开始

还原并构建：

```powershell
dotnet restore
dotnet build
```

使用默认项目配置运行：

```powershell
dotnet run
```

仓库中的 Kestrel 配置默认监听：

```text
http://0.0.0.0:13500
```

开发启动配置监听：

```text
http://localhost:5226
```

## 客户端配套仓库

推荐搭配以下客户端源码或完整客户端资源使用：

- [Boxmaker 客户端源码](https://github.com/Caritusy/Boxmaker)
- [Boxmaker 客户端发布页](https://github.com/Caritusy/Boxmaker-Release/releases)

## 项目结构

```text
AccountManager/      账号文件、地图缓存、任务、玩家状态、编辑地图和 IO 队列逻辑
BoxmakerProxy/       按认证、地图、编辑器、任务分组的 HTTP 路由处理
Infrastructure/      控制台输出、文件锁辅助、DES 辅助
Models/              服务端地图、任务和地图数据模型
Networking/          数据包辅助、操作码、网络消息容器
Pages/               Razor Web 控制台页面
protocol.game/       游戏协议 DTO
protocol.map/        地图协议 DTO
Services/            运行期服务辅助
Utilities/           通用工具和版本辅助
wwwroot/             静态网页资源
```

## 开发说明

- 除非正在做协议兼容工作，否则尽量不要修改 `protocol.game` 和 `protocol.map`。
- 新增游戏接口优先放在 `BoxmakerProxy/` 下。
- 账号、地图、任务、玩家状态逻辑优先放在 `AccountManager/` 下。
- 数据模型变更需要谨慎，因为已有本地存档文件可能需要迁移。

## 许可证

请查看仓库内包含的许可证文件。
