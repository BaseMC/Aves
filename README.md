# Aves [![Build](https://img.shields.io/github/workflow/status/BaseMC/Aves/Master%20CI)](https://github.com/BaseMC/Aves/actions?query=workflow%3A%22Master+CI%22)  [![Latest Version](https://img.shields.io/github/v/release/BaseMC/Aves)](https://github.com/BaseMC/Aves/releases)
A Minecraft deobfusctor / code generator

Generates deobfuscated code based on the [emitted obfuscation files from mojang](https://www.minecraft.net/article/minecraft-snapshot-19w36a) (since 19w36a) in about 5 minutes

``platform independent`` ``no local installation of Minecraft required``

### [Download](https://github.com/BaseMC/Aves/releases/latest)

#### Requirements
* ```# (c) 2020 Microsoft Corporation. These mappings are provided "as-is" and you bear the risk of using them. You may copy and use the mappings for development purposes, but you may not redistribute the mappings complete and unmodified. Microsoft makes no warranties, express or implied, with respect to the mappings provided here.  Use and modification of this document or the source code (in any form) of Minecraft: Java Edition is governed by the Minecraft End User License Agreement available at https://account.mojang.com/documents/minecraft_eula.```
* Recommended: 1.5-2 GB of RAM for the [decompiler](https://github.com/BaseMC/avesflower)
* Supported MC-versions: 1.14.4 or ``>=``19w36a (1.15+)

### Why?
This project is designed for people that want to quickly take a look at a part of the code and understand what is going on.

This is very useful e.g.<br/>- for quickly inspecting new versions<br/>- if you find a bug and want to know what causes it <br/>- if you are just interested in how things work :smile:<br/>
No need to wait for someone to release new mappings or a version.

The generated code is not perfect, but it's readable and that's what it's meant for.

The project trys to work as automated as possible.<br/>
So if Mojang doesn't do any (breaking) changes, it should work a long time :t-rex:

### [Usage](docs/Usage.md)

### [Developing](docs/Developing.md) [![Build develop Azure](https://dev.azure.com/BaseMC/Aves/_apis/build/status/Develop?label=build%20develop)](https://dev.azure.com/BaseMC/Aves/_build/latest?definitionId=1) [![Build Develop](https://img.shields.io/github/workflow/status/BaseMC/Aves/Check%20Build/develop?label=build%20develop)](https://github.com/BaseMC/Aves/actions?query=workflow%3A%22Check+Build%22+branch%3Adevelop)

### [Building](docs/Building.md)
