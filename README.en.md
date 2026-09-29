<h1 align="center">🔌 Snet.Plugins</h1>

<p align="center">
  <img width="120" height="120" src="https://api.snet.cn/pic/nuget.png" alt="Snet Logo"/>
</p>

<p align="center">
  <b>Community plugins · Snet interfaces · Share and reuse</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Snet-Plugins-blue" alt="Snet Plugins"/>
  <img src="https://img.shields.io/badge/Interfaces-IDAq%20%7C%20IMq-green" alt="IDaq and IMq"/>
  <img src="https://img.shields.io/github/stars/shunnet/Plugins?style=social" alt="GitHub stars"/>
</p>

<p align="center">
  <a href="https://snet.cn"><b>🌐 Snet Website</b></a> ·
  <a href="https://www.nuget.org/profiles/Shun"><b>📦 NuGet Plugins</b></a> ·
  <a href="https://github.com/shunnet/Plugins"><b>📦 Plugin Repository</b></a> ·
  <a href="https://github.com/shunnet/Daq"><b>🔌 Daq Tool</b></a> ·
  <a href="https://github.com/shunnet/SKILLS/tree/main/PluginDev-Skill"><b>📚 Plugin Development Guide</b></a>
</p>

<p align="center">
  📖 <a href="README.md"><b>简体中文</b></a> | English
</p>

## ✨ About

**Snet.Plugins** is a community repository for individual developers to share plugins built against **Snet plugin interfaces**. It helps other users discover, learn from, and reuse plugin implementations in compatible Snet applications.

The repository accepts two plugin categories:

| Category | Interface family | Purpose |
| --- | --- | --- |
| `daq` | `IDaq` | Data acquisition, device integration, or custom protocol implementations |
| `mq` | `IMq` | Message middleware producers, consumers, or custom message integrations |

Plugin contracts, dependencies, and host compatibility may vary by Snet version. Refer to each plugin's README and the applicable version of [PluginDev-Skill](https://github.com/shunnet/SKILLS/tree/main/PluginDev-Skill).

## 📦 Plugins Published on NuGet

In addition to source plugins in this repository, **Snet plugins and related ecosystem packages developed and published by Shun** are available on NuGet: [NuGet — Shun](https://www.nuget.org/profiles/Shun). These NuGet packages are part of the Snet plugin ecosystem; this repository is for sharing plugin source code and documentation.

## 🗂️ Directory Convention

Organize plugins as **GitHub username → plugin category → plugin name**. Each plugin must have its own directory and a `README.md` inside that directory.

```text
<GitHub-username>/
├── daq/
│   └── <PluginName>/
│       ├── README.md          # Required: purpose, use cases, configuration, and usage
│       └── src/               # Minimum source required for the plugin
└── mq/
    └── <PluginName>/
        ├── README.md
        └── src/
```

For example, the GitHub user `shunnet` could submit one DAQ plugin and one MQ plugin as follows:

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

- Use the contributor's GitHub login as `<GitHub-username>`; keep that user's plugins under the same directory.
- `daq` and `mq` are fixed, lowercase category directories. Choose the category that matches the interface your plugin implements.
- Give each plugin its own directory with a short, stable, recognizable name.
- **Every plugin directory must contain its own `README.md`.** Providing both Chinese and English is recommended; at minimum, explain what the plugin does and the scenarios it is intended for.

## 📄 Required Plugin README Content

Each plugin README should describe at least:

1. **Name and category**: `daq` / `mq`, and the Snet interface implemented (for example, `IDaq` / `IMq`).
2. **Purpose**: what the plugin does and the devices, protocols, or message systems it supports.
3. **Use cases**: when to use it and any scenarios it does not support.
4. **Compatibility**: target .NET runtime, Snet.Core version or range, and verified host/platform where applicable.
5. **Configuration and dependencies**: required parameters, external dependencies, permissions, network/device prerequisites. Never include real credentials.
6. **Build, test, and usage**: reproducible steps to build, verify, and integrate the plugin; note any host-specific setup.
7. **Limitations and known issues**.
8. **Author and license**: maintainer's GitHub username. All plugins submitted to this repository are licensed under MIT by default; no alternative license selection is needed.

Suggested template:

```markdown
# Plugin Name

- Category: daq or mq
- Interface: IDaq or IMq
- Author: @GitHub-username
- Compatibility: .NET / Snet.Core / host version
- License: MIT (default)

## Purpose
Describe what the plugin implements.

## Use Cases
Describe the devices, protocols, and scenarios it supports, including scope boundaries.

## Configuration and Dependencies
List required parameters, dependencies, and prerequisites. Do not include real passwords, tokens, or keys.

## Build, Test, and Usage
Provide reproducible build, test, and integration steps.

## Limitations and Known Issues
Describe known limitations.
```

## 🧩 Plugin Code Scope (Important)

This repository is for **plugin implementations**, not personal projects or complete business systems. All contributions must follow these rules:

- **Submit only the minimum code needed to implement the Snet plugin interface and the plugin's stated functionality.** Keep only the necessary protocol communication, data parsing, parameter definitions, lifecycle handling, and error handling.
- Keep code focused on the plugin itself, such as an `IDaq` acquisition implementation or an `IMq` messaging implementation.
- **Do not upload your own system/business application code**, including complete personal or company applications, business workflows or rules, backend services, Web/API or UI layers, user/permission systems, database business modules, or project code unrelated to the plugin.
- Do not include unrelated frameworks, deployment environments, project configuration, internal tools, or private business dependencies. Small helpers required by the plugin are acceptable when their purpose is documented.
- Never commit secrets, passwords, tokens, certificates, real production connection details, personal data, or other confidential information. Use placeholders or document environment-variable configuration instead.
- Do not commit `bin/`, `obj/`, caches, temporary files, build artifacts, or host runtime data. By default, submit source code, necessary project files, sanitized example configuration, and documentation only.
- Submit only code and dependencies that you are authorized to publish; third-party dependencies remain subject to their own licenses and must be identified.

## 🚀 Contributing

1. Fork this repository and add your plugin under a directory named for your GitHub username, following the structure above.
2. Add a complete `README.md` for every plugin, describing its purpose, use cases, compatibility, and usage.
3. Check that the contribution contains only the minimum plugin implementation; remove business-system code, confidential information, and build artifacts.
4. Build and verify the interface implementation where possible, and state the test environment and results.
5. Open a Pull Request and list the category, interface, supported scenarios, dependencies, and verification results in its description. Submitting a contribution signifies acceptance of this repository's default MIT licensing rule.

Maintainers may request changes to directory layout, documentation, code scope, or compatibility notes. Merging a plugin does not guarantee its security, correctness, or compatibility with every Snet host.

## 📜 License

**All plugin source code uploaded to this repository is licensed under the MIT License by default.** By submitting a plugin, you confirm that you have the right to contribute the code and agree to license it under MIT, allowing others to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the code. Original authors retain their copyright. Do not submit code that you cannot or do not wish to license under MIT.

The root [`LICENSE`](LICENSE) file contains the full MIT License text. Third-party dependency licenses are not changed by this policy; contributors must comply with and identify their respective license requirements.

## 🔗 Resources

| Resource | Description |
| --- | --- |
| [Snet.Iot.Daq](https://github.com/shunnet/Daq) | Plugin-based data acquisition tool; compatible plugins can be loaded by supported versions |
| [Snet.SKILLS — PluginDev-Skill](https://github.com/shunnet/SKILLS/tree/main/PluginDev-Skill) | Development contract and guidance for `IDaq` / `IMq` plugins |
| [NuGet — Shun](https://www.nuget.org/profiles/Shun) | Snet plugins and related ecosystem packages developed and published by Shun |
| [Snet Website](https://snet.cn/) | Snet information |

## ⚠️ Security Notice

All submissions are subject to administrator review; they may be merged or published only after approval. Review covers repository structure, documentation, code scope, licensing, and basic compatibility. It is not a full security audit and does not guarantee that a plugin is completely safe or compatible with every Snet host. Plugins run in the user's environment, so inspect the source, dependencies, permissions, and network behavior before use. Do not load unreviewed code.