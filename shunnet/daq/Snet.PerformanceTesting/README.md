<h1 align="center">🧪 Snet.PerformanceTesting</h1>

<p align="center">
  <b>NetMQ PUB/SUB · Snet IDaq · 性能测试数据输入插件</b>
</p>

<p align="center">
  <a href="#简体中文"><b>简体中文</b></a> | <a href="#english"><b>English</b></a>
</p>


## 🇨🇳 简体中文

### ✨ 插件简介

`Snet.PerformanceTesting` 是一个**只支持数据订阅**的 DAQ 插件。它使用 NetMQ `SubscriberSocket` 连接外部发布端，接收指定主题的 JSON 点位数据，再通过 Snet 地址处理流程供兼容的宿主（例如 Daq）读取。

> 🎯 **适用目的：** 使用模拟数据或压测程序验证订阅、地址处理和数据转发链路。  
> ⚠️ **不是实际 PLC / 设备驱动；不支持写入。**

### 🧭 插件信息

| 项目 | 信息 |
| --- | --- |
| 插件类型 | `daq` · `IDaq` |
| 继承基类 | `DaqAbstract<PerformanceTestingOperate, Basics>` |
| 目标框架 | `.NET 8.0`、`.NET 10.0` |
| Snet.Core | `26.271.1` |
| NetMQ | `4.0.4.3` |
| 插件版本 | `26.271.1` |
| 许可证 | MIT（仓库默认许可） |
| 命名空间 | 操作类：`YSAI.PerformanceTesting`；数据模型：`Snet.PerformanceTesting` |

### 🔄 数据流

```text
NetMQ 发布端 (PUB)
      │  两帧消息：主题 + JSON 数组
      ▼
SubscriberSocket (本插件)
      │  按主题筛选并解析点位
      ▼
Snet AddressHandler
      │
      ▼
地址最新值 → 兼容的 Snet 宿主读取 / 后续处理
```

### 🎛️ 配置参数

| 参数 | 默认值 | 说明 |
| --- | --- | --- |
| `Uri` | `tcp://127.0.0.1:8866` | 发布端连接地址。插件调用 `Connect`，发布端应在对应地址监听/绑定。 |
| `Topic` | `PerformanceTesting` | 订阅主题；消息第一帧必须与主题完全一致。 |
| `TimeOut` | `1000` ms | SUB 套接字每次等待接收消息的超时时间。 |

在宿主中配置地址点位，并调用 `Subscribe` 启动订阅。消息中的 `AddressName` 必须与宿主配置并订阅的地址名相匹配。

### 📨 消息格式

发布端发送一个**两帧 NetMQ 消息**：

| 帧 | 内容 |
| --- | --- |
| 第 1 帧 | 主题字符串，例如 `PerformanceTesting` |
| 第 2 帧 | `PerformanceTestingData.Pack[]` 对应的 JSON 数组 |

```json
[
  { "AddressName": "Temperature", "Value": 23.6 },
  { "AddressName": "Pressure", "Value": 101.3 }
]
```

`AddressName` 需要匹配宿主中的地址名称；`Value` 应为 Snet 地址处理器能够处理的值。插件收到数据后会处理并缓存对应地址的最新值。

### 🛠️ 构建与部署

使用 .NET SDK，并按项目文件中声明的目标框架和包版本构建：

```bash
dotnet restore Snet.PerformanceTesting.csproj
dotnet build Snet.PerformanceTesting.csproj -c Release -f net8.0
dotnet build Snet.PerformanceTesting.csproj -c Release -f net10.0
```

> 📌 项目文件通过相对路径引用打包用的 `README.md` 和 `icon.png`。打包 NuGet 或调整目录时，请确认文件位置与 `.csproj` 路径相符，必要时更新路径。部署到 Daq 或其他 Snet 宿主时，应按宿主要求包含程序集及依赖。

### ⚠️ 限制与代码检查提示

- `WriteAsync` 明确返回“不支持此功能”；本插件是订阅输入，不是可写设备驱动。
- `GetStatusAsync` 通过 `SubscriberSocket` 对象是否为空判断状态，**不验证发布端是否实际可达**；显示“已连接”不一定代表已收到数据。
- 接收与队列处理任务以后台任务方式启动。停止时会取消令牌并释放资源，但当前代码没有等待后台任务完全结束；正式发布前建议保存并等待任务句柄，并验证反复启停时的清理行为。
- 本 README 根据提供的两个 `.cs` 文件和 `.csproj` 静态检查编写。当前沙箱未能运行 .NET 构建，尚未验证编译、运行或与具体宿主的集成结果。

### 📥 Plugins 仓库提交

建议目录：

```text
shunnet/daq/Snet.PerformanceTesting/README.md
```

提交到 Plugins 仓库的内容须经管理员审核，通过后方可合并或发布。审核不代表对插件安全性或所有宿主兼容性的保证。

### 👤 作者与许可

作者：**Shun** · [NuGet 插件与组件](https://www.nuget.org/profiles/Shun)  
本插件默认采用 **MIT** 许可证，详见仓库根目录的 [`LICENSE`](../../../LICENSE)。

---

## 🇬🇧 English

### ✨ Overview

`Snet.PerformanceTesting` is a **subscription-only** DAQ plugin. It connects to an external publisher through a NetMQ `SubscriberSocket`, receives JSON point data on a configured topic, and passes the data through the Snet address-processing pipeline for a compatible host such as Daq to read.

> 🎯 **Intended for:** validating subscription, address-processing, and data-forwarding pipelines with simulated data or a load generator.  
> ⚠️ **Not a physical PLC/device driver; write operations are not supported.**

### 🧭 Plugin Information

| Item | Details |
| --- | --- |
| Category / interface | `daq` · `IDaq` |
| Base class | `DaqAbstract<PerformanceTestingOperate, Basics>` |
| Target frameworks | `.NET 8.0`, `.NET 10.0` |
| Snet.Core | `26.271.1` |
| NetMQ | `4.0.4.3` |
| Plugin version | `26.271.1` |
| License | MIT (repository default) |
| Namespaces | Operation: `YSAI.PerformanceTesting`; data model: `Snet.PerformanceTesting` |

### 🔄 Data Flow

```text
NetMQ publisher (PUB)
      │  Two-frame message: topic + JSON array
      ▼
SubscriberSocket (this plugin)
      │  Filter topic and parse points
      ▼
Snet AddressHandler
      │
      ▼
Latest address values → read / process in a compatible Snet host
```

### 🎛️ Configuration

| Parameter | Default | Description |
| --- | --- | --- |
| `Uri` | `tcp://127.0.0.1:8866` | Publisher endpoint. The plugin calls `Connect`; the publisher should listen/bind at the corresponding address. |
| `Topic` | `PerformanceTesting` | Subscription topic; the first message frame must match it exactly. |
| `TimeOut` | `1000` ms | Timeout for each receive wait on the SUB socket. |

Configure address points in the host and start subscription with `Subscribe`. Each message's `AddressName` must match an address configured and subscribed in the host.

### 📨 Message Format

The publisher sends a **two-frame NetMQ message**:

| Frame | Content |
| --- | --- |
| Frame 1 | Topic string, for example `PerformanceTesting` |
| Frame 2 | JSON array corresponding to `PerformanceTestingData.Pack[]` |

```json
[
  { "AddressName": "Temperature", "Value": 23.6 },
  { "AddressName": "Pressure", "Value": 101.3 }
]
```

`AddressName` must match an address configured in the host. `Value` should be a value that the Snet address handler can process. The plugin processes the received data and caches the latest value for each address.

### 🛠️ Build and Deployment

Build with the .NET SDK using the target frameworks and package versions declared in the project file:

```bash
dotnet restore Snet.PerformanceTesting.csproj
dotnet build Snet.PerformanceTesting.csproj -c Release -f net8.0
dotnet build Snet.PerformanceTesting.csproj -c Release -f net10.0
```

> 📌 The project file references packaging `README.md` and `icon.png` via relative paths. When packing the NuGet package or changing the directory layout, ensure the files are where the `.csproj` expects them, or update the paths. When deploying to Daq or another Snet host, include the assemblies and dependencies required by that host.

### ⚠️ Limitations and Code Review Notes

- `WriteAsync` explicitly returns “not supported”; this is a subscription input plugin, not a writable device driver.
- `GetStatusAsync` checks whether the `SubscriberSocket` object is non-null; it **does not verify that the publisher is reachable**. A “connected” status does not necessarily mean data has arrived.
- The receive and queue-processing loops run as background tasks. Shutdown cancels the token and releases resources, but the current code does not await those tasks to finish. Before production release, consider retaining and awaiting task handles, and test cleanup during repeated start/stop cycles.
- This README is based on static inspection of the two supplied `.cs` files and `.csproj`. A .NET build could not be run in the current sandbox, so compilation, runtime behavior, and integration with a specific host have not been verified.

### 📥 Submitting to the Plugins Repository

Suggested path:

```text
shunnet/daq/Snet.PerformanceTesting/README.md
```

Contributions to the Plugins repository require administrator review and may be merged or published only after approval. Approval does not guarantee plugin security or compatibility with every host.

### 👤 Author and License

Author: **Shun** · [NuGet packages and components](https://www.nuget.org/profiles/Shun)  
This plugin is **MIT-licensed by default**; see the repository-root [`LICENSE`](../../../LICENSE).
