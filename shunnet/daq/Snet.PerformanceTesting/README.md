# Snet.PerformanceTesting

> 基于 NetMQ PUB/SUB 的 Snet `IDaq` 性能测试数据输入插件。  
> A NetMQ PUB/SUB data-input plugin for Snet `IDaq` performance testing.

[简体中文](#简体中文) | [English](#english)

## 简体中文

### 插件简介

`Snet.PerformanceTesting` 是一个只支持数据订阅的 DAQ 插件。它通过 NetMQ `SubscriberSocket` 连接到外部发布端，订阅指定主题，接收 JSON 点位数据，并将数据转换为 Snet 地址值，供兼容的 Snet 宿主（例如 Daq）读取和处理。

它适用于使用模拟数据或压测程序验证订阅、地址处理和数据转发链路；**它不是实际 PLC 或设备通信驱动，也不提供写入能力。**

### 类型与兼容性

| 项目 | 信息 |
| --- | --- |
| 插件类型 | `daq` |
| Snet 接口 | `IDaq`（继承 `DaqAbstract<PerformanceTestingOperate, Basics>`） |
| 目标框架 | `.NET 8.0`、`.NET 10.0`（项目文件声明） |
| `Snet.Core` | `26.271.1` |
| NetMQ | `4.0.4.3` |
| 许可证 | MIT（按仓库默认许可） |

### 功能与适用场景

- 通过 NetMQ SUB 套接字接收外部发布端发来的点位数据。
- 只处理与配置主题完全匹配的消息。
- 将点位名和值传入 Snet 地址处理流程，并缓存最新接收的数据供读取。
- 可用于模拟采集数据、验证订阅链路和进行性能/压力测试。
- 不适用于真实设备协议接入，也不支持通过该插件向设备或发布端写入数据。

### 配置项

| 参数 | 默认值 | 说明 |
| --- | --- | --- |
| `Uri` | `tcp://127.0.0.1:8866` | NetMQ 发布端的连接地址。插件端调用 `Connect`；发布端应在对应地址监听/绑定。 |
| `Topic` | `PerformanceTesting` | 要订阅的主题。消息第一帧必须与此主题一致。 |
| `TimeOut` | `1000` ms | SUB 套接字每次等待接收消息的超时时间。 |

请按宿主使用方式配置地址点位，并使用 `Subscribe` 启动订阅。点位名称需要与消息中的 `AddressName` 对应。

### 消息格式

发布端应发送一个包含 **两帧** 的 NetMQ 消息：第一帧为主题，第二帧为 JSON 数组。JSON 数组元素对应 `PerformanceTestingData.Pack`：

1. 第一帧：与 `Topic` 完全一致，例如 `PerformanceTesting`。
2. 第二帧：包含 `AddressName` 和 `Value` 的 JSON 数组。

示例第二帧：

```json
[
  { "AddressName": "Temperature", "Value": 23.6 },
  { "AddressName": "Pressure", "Value": 101.3 }
]
```

`AddressName` 必须匹配宿主中配置并订阅的地址名。`Value` 应为该 Snet 地址可处理的值类型。插件收到消息后使用 Snet 地址处理器转换数据，并更新对应地址的最新值。

### 构建

项目使用 .NET SDK，目标框架和包版本以项目文件为准：

```bash
dotnet restore Snet.PerformanceTesting.csproj
dotnet build Snet.PerformanceTesting.csproj -c Release -f net8.0
dotnet build Snet.PerformanceTesting.csproj -c Release -f net10.0
```

项目文件还通过相对路径引用打包用的 `README.md` 和 `icon.png`。打包 NuGet 包或调整目录时，请确认这两个文件位于项目文件所预期的位置，或相应更新 `.csproj` 中的路径。部署到 Daq 或其他 Snet 宿主时，应按宿主支持的插件打包方式包含所需程序集和依赖。

### 限制与代码检查提示

- `WriteAsync` 明确返回“不支持此功能”；本插件是订阅输入，不是可写设备驱动。
- 当前 `GetStatusAsync` 通过 `SubscriberSocket` 对象是否为空判断状态，并不验证发布端是否实际可达；显示“已连接”不一定代表已收到数据。
- 当前接收/队列处理任务以后台任务方式启动，停止时取消令牌并释放资源，但代码没有等待这些后台任务完全结束。建议在正式发布前保存并等待任务句柄，并验证反复启停时的清理行为。
- 本次检查基于提供的两个 `.cs` 文件和 `.csproj` 静态阅读；当前沙箱未能运行 .NET 构建，因此尚未验证编译、运行或与具体宿主的集成结果。


## English

### Overview

`Snet.PerformanceTesting` is a DAQ plugin that supports subscription-only input. It connects to an external publisher through a NetMQ `SubscriberSocket`, subscribes to a configured topic, receives JSON point data, and converts it into Snet address values for a compatible Snet host (such as Daq) to read and process.

It is intended for validating subscription, address-processing, and data-forwarding pipelines with simulated data or a load generator. **It is not a driver for physical PLCs/devices and does not support writes.**

### Type and Compatibility

| Item | Details |
| --- | --- |
| Plugin category | `daq` |
| Snet interface | `IDaq` (inherits `DaqAbstract<PerformanceTestingOperate, Basics>`) |
| Target frameworks | `.NET 8.0` and `.NET 10.0` (as declared in the project file) |
| `Snet.Core` | `26.271.1` |
| NetMQ | `4.0.4.3` |
| Plugin version | `26.271.1` |
| License | MIT (repository default) |

The operation class is in the `YSAI.PerformanceTesting` namespace; the data model is in `Snet.PerformanceTesting`.

### Features and Use Cases

- Receives point data from an external publisher over a NetMQ SUB socket.
- Processes only messages whose topic exactly matches the configured topic.
- Passes point names and values through the Snet address-processing pipeline and caches the latest received values for reads.
- Suitable for simulated acquisition data, subscription-pipeline validation, and performance/load testing.
- Not intended for physical-device protocol integration; it cannot write data to a device or publisher.

### Configuration

| Parameter | Default | Description |
| --- | --- | --- |
| `Uri` | `tcp://127.0.0.1:8866` | Connection address for the NetMQ publisher. The plugin calls `Connect`; the publisher should listen/bind at the corresponding endpoint. |
| `Topic` | `PerformanceTesting` | Topic to subscribe to. The first message frame must match it exactly. |
| `TimeOut` | `1000` ms | Timeout for each receive wait on the SUB socket. |

Configure the address points in the host and start subscription using `Subscribe`. Point names must correspond to the message's `AddressName` values.

### Message Format

The publisher must send one NetMQ message containing **two frames**: the first frame is the topic and the second is a JSON array. Each JSON item corresponds to `PerformanceTestingData.Pack`:

1. Frame 1: exactly the configured topic, for example `PerformanceTesting`.
2. Frame 2: a JSON array with `AddressName` and `Value` fields.

Example second frame:

```json
[
  { "AddressName": "Temperature", "Value": 23.6 },
  { "AddressName": "Pressure", "Value": 101.3 }
]
```

`AddressName` must match an address configured and subscribed in the host. `Value` must be a value type that the Snet address can process. On receipt, the plugin uses the Snet address handler to process the data and update the latest value for that address.

### Build

Build with the .NET SDK. Target frameworks and package versions are declared in the project file:

```bash
dotnet restore Snet.PerformanceTesting.csproj
dotnet build Snet.PerformanceTesting.csproj -c Release -f net8.0
dotnet build Snet.PerformanceTesting.csproj -c Release -f net10.0
```

The project file also references packaging `README.md` and `icon.png` using relative paths. When packaging the NuGet package or changing the directory layout, ensure both files are where the `.csproj` expects them to be, or update those paths. When deploying to Daq or another Snet host, include the required assemblies and dependencies using that host's supported plugin packaging method.

### Limitations and Code Review Notes

- `WriteAsync` explicitly returns “not supported”; this is a subscription input plugin, not a writable device driver.
- `GetStatusAsync` currently checks whether the `SubscriberSocket` object is non-null; it does not verify that the publisher is reachable. A displayed “connected” status does not necessarily mean data has been received.
- The receive and queue-processing loops are started as background tasks. Shutdown cancels the token and releases resources, but the code does not await those background tasks to finish. Before production release, consider retaining and awaiting task handles, and test cleanup during repeated start/stop cycles.
- This review is based on static inspection of the two supplied `.cs` files and `.csproj`. A .NET build could not be run in the current sandbox, so compilation, runtime behavior, and integration with a specific host have not been verified.
