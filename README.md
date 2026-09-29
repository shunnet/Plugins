<h1 align="center">🔌 Snet.Plugins</h1>

<p align="center">
  <img width="120" height="120" src="https://api.snet.cn/pic/nuget.png" alt="Snet Logo"/>
</p>

<p align="center">
  <b>社区插件 · Snet 接口 · 分享与复用</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Snet-Plugins-blue" alt="Snet Plugins"/>
  <img src="https://img.shields.io/badge/Interfaces-IDAq%20%7C%20IMq-green" alt="IDaq and IMq"/>
  <img src="https://img.shields.io/github/stars/shunnet/Plugins?style=social" alt="GitHub stars"/>
</p>

<p align="center">
  <a href="https://snet.cn"><b>🌐 Snet 官网</b></a> ·
  <a href="https://github.com/shunnet/Plugins"><b>📦 插件仓库</b></a> ·
  <a href="https://github.com/shunnet/Daq"><b>🔌 Daq 工具</b></a> ·
  <a href="https://github.com/shunnet/SKILLS/tree/main/PluginDev-Skill"><b>📚 插件开发规范</b></a>
</p>

<p align="center">
  简体中文 | 📖 <a href="README.en.md"><b>English</b></a>
</p>

## ✨ 仓库简介

**Snet.Plugins** 是一个面向个人开发者的社区插件仓库，用来分享按照 **Snet 插件接口**开发的插件实现，方便其他用户查找、学习、复用，并在兼容的 Snet 应用中使用。

本仓库主要收录两类插件：

| 类型 | 接口方向 | 用途 |
| --- | --- | --- |
| `daq` | `IDaq` | 数据采集、设备接入或自定义协议实现 |
| `mq` | `IMq` | 消息中间件的生产、消费或自定义消息接入 |

插件接口、依赖版本和宿主支持情况可能随 Snet 版本变化。请以插件自己的 README 和对应版本的 [PluginDev-Skill](https://github.com/shunnet/SKILLS/tree/main/PluginDev-Skill) 为准。

## 🗂️ 目录约定

按 **GitHub 用户名 → 插件类型 → 插件名称** 组织。每个插件单独一个目录，并且必须在插件目录内提供 `README.md`。

```text
<GitHub用户名>/
├── daq/
│   └── <插件名称>/
│       ├── README.md          # 必须：插件用途、适用场景、配置与使用说明
│       └── src/               # 插件最小必要源码
└── mq/
    └── <插件名称>/
        ├── README.md
        └── src/
```

例如，GitHub 用户 `shunnet` 可以按下面的方式提交一个 DAQ 插件和一个 MQ 插件：

```text
shunnet/
├── daq/
│   └── TemperatureSensor/
│       ├── README.md
│       └── src/
└── mq/
    └── CustomMqtt/
        ├── README.md
        └── src/
```

- `<GitHub用户名>` 使用贡献者的 GitHub 登录名；同一用户的插件集中放在自己的目录下。
- `daq` 和 `mq` 是固定的小写分类目录，请按插件实际实现的接口归类。
- 每个插件一个独立目录；目录名使用简洁、稳定、易识别的插件名称。
- **每个插件目录都必须有自己的 `README.md`**。建议插件说明同时提供中文和英文，至少要清楚说明插件做什么、适用于什么场景。

## 📄 插件 README 必须说明什么

每个插件至少应说明：

1. **插件名称与类型**：`daq` / `mq`，以及实现的 Snet 接口（如 `IDaq` / `IMq`）。
2. **功能简介**：这个插件具体负责什么，支持哪些设备、协议或消息系统。
3. **适用场景**：什么情况下应该使用它，以及明确不支持的情况。
4. **兼容性**：目标 .NET 运行时、Snet.Core 版本或范围、已验证的宿主/平台（如适用）。
5. **配置与依赖**：需要填写的参数、外部依赖、权限、网络或设备前置条件；不得放真实凭据。
6. **构建、测试与使用**：如何构建、验证和接入；若宿主有额外安装步骤，请注明。
7. **限制与已知问题**：已知边界、未实现功能及注意事项。
8. **作者与许可**：维护者 GitHub 用户名，以及源码适用的许可证或授权说明。

可参考下面的模板：

```markdown
# 插件名称

- 类型：daq 或 mq
- 接口：IDaq 或 IMq
- 作者：@GitHub用户名
- 兼容版本：.NET / Snet.Core / 宿主版本
- 许可证：许可证名称或授权说明

## 功能简介
说明插件实现了什么。

## 适用场景
说明适合使用的设备、协议、业务边界或场景。

## 配置与依赖
列出必要参数、依赖和前置条件；不要填写真实密码、Token 或密钥。

## 构建、测试与使用
提供可复现的构建、测试和接入步骤。

## 限制与已知问题
说明已知限制。
```

## 🧩 插件代码范围（重要）

本仓库收录的是**插件实现**，不是个人项目或完整业务系统。提交代码必须遵守以下范围：

- **只提交实现 Snet 插件接口及完成该插件功能所必需的最基础代码。** 保留必要的协议通信、数据解析、参数定义、生命周期处理和错误处理即可。
- 代码应聚焦于插件本身：例如 `IDaq` 数据采集实现，或 `IMq` 消息收发实现。
- **不要上传自己的系统业务代码**，包括个人/公司的完整应用、业务流程、业务规则、后台服务、Web/API、UI、用户/权限系统、数据库业务模块，或与插件运行无关的项目代码。
- 不要提交与插件无关的通用框架、部署环境、项目配置、内部工具或私有业务依赖。插件所必需的少量辅助代码可以保留，但应说明用途。
- 不要提交密钥、密码、Token、证书、真实生产连接信息、个人数据或其他机密内容；请使用示例值或环境变量说明。
- 不要提交 `bin/`、`obj/`、缓存、临时文件、编译产物或宿主运行数据。默认仅提交源码、必要的项目文件、示例配置（脱敏）和文档。
- 仅提交你有权公开和授权他人使用的代码与依赖；注明第三方依赖及其许可证。

## 🚀 如何贡献

1. Fork 本仓库，并在自己的 GitHub 用户名目录下按上述结构添加插件。
2. 为每个插件添加完整的 `README.md`，说明用途、适用场景、兼容性和使用方法。
3. 检查提交内容仅包含插件必要实现；移除业务系统代码、机密信息和构建产物。
4. 尽可能在本地构建并验证接口实现，注明测试环境和结果。
5. 提交 Pull Request，并在描述中列出插件类型、接口、适配场景、依赖/许可证及验证结果。

维护者可以要求修改目录、文档、代码范围或兼容性说明。合并不代表对插件安全性、正确性或与所有 Snet 宿主兼容性的保证。

## 🔗 相关资源

| 资源 | 说明 |
| --- | --- |
| [Snet.Iot.Daq](https://github.com/shunnet/Daq) | 插件化数据采集工具；可在兼容版本中加载相应插件 |
| [Snet.SKILLS — PluginDev-Skill](https://github.com/shunnet/SKILLS/tree/main/PluginDev-Skill) | `IDaq` / `IMq` 插件开发契约与指导 |
| [Snet 官网](https://snet.cn/) | Snet 相关信息 |

## ⚠️ 安全提示

插件代码会在使用者的环境中运行。使用前请自行检查源码、依赖、权限和网络行为，并确认其来源可信；不要直接加载未经审查的代码。本仓库是社区分享目录，不对第三方插件提供安全审计或适配保证。
