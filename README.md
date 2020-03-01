[![Build](https://img.shields.io/github/workflow/status/BaseMC/Aves/Master%20CI)](https://github.com/BaseMC/Aves/actions)
[![Latest Version](https://img.shields.io/github/v/release/BaseMC/Aves)](https://github.com/BaseMC/Aves/releases)
[![Build Develop](https://dev.azure.com/BaseMC/Aves/_apis/build/status/Develop?label=build%20develop)](https://dev.azure.com/BaseMC/Aves/_build/latest?definitionId=1)

# Aves
A Minecraft deobfusctor / code generator

Generates deobfuscated code based on the [emitted obfuscation files from mojang](https://www.minecraft.net/article/minecraft-snapshot-19w36a) (since 19w36a)

``platform independent`` ``no local installation of Minecraft required``

### Why?
This project is designed for people that want to quickly take a look at a part of the code and understand what is going on.

This is very useful e.g.<br/>- for quickly inspecting new versions <br/>- or if you find a bug and want to know where it comes from <br/>- or if you are just interested in how things work :smile:<br/>
No need to rely on someone else to release new mappings or versions.

The generated code is not perfect, but it's readable and that's what it's meant for.

The project trys to work as automated as possible.<br/>
So if mojang doesn't do any (breaking) changes, it should work a long time.

## Requirements
* Recommended: 1.5-2GB of RAM for the [decompiler](https://github.com/BaseMC/avesflower)
* You can only use the generated code under mojang's license: <br/> ``(c) 2019 Microsoft Corporation.  All rights reserved.  This information is provided "as-is" and you bear the risk of using it. This information does not provide you with any legal rights to any intellectual property in any Microsoft product. You may copy and use this information for your internal, reference purposes. Microsoft makes no warranties, express or implied, with respect to the information provided here.``

## Download
* https://github.com/BaseMC/Aves/releases

## Usage / How to run it
* Supported versions: 1.14.4 or ``>=``19w36a (1.15+)

### Example
* Basic example for commandline/shell:<br/>
```Aves.exe -v 1.14.4```<br/>
Description: Creates source code for 1.14.4 (by default: client only)
#### Folder structure

    .
    ├─── javgent-standalone.jar          # Deobfuscator → see https://github.com/BaseMC/javgent
    ├─── avesflower.jar                  # Decompiler → see https://github.com/BaseMC/avesflower
    ├─── jre                             # Embedded Java Runtime Environment → see https://adoptopenjdk.net/
    ├─── logs                            # Generated log files (if enabled)
    └─┬─ workingDir                      # Main Working Directory
      ├─── version_manifest.json         # Version Manifest (Lookup for all versions)
      └─┬─ 1.14.4                        # <version> Version Working Directory (here: 1.14.4)
        ├─┬─ raw                         # Raw files
        │ ├─── package.json              # Version-Manifest (looked up from version_manifest.json)
        │ └─┬─ client                    # <variant> variant (here: client)
        │   └── client.jar               # Executable jar file (looked up from package.json)
        ├─┬─ mappings                    # Mapping files
        │ └─┬─ client                    
        │   └── client.txt               # Client-Mappings (looked up from package.json)
        ├─┬─ patch                       # Generated patch files
        │ └─┬─ client                    
        │   ├── *.json                   # PatchFiles (generated from mapping-file: client.txt)
        ├─┬─ deof                        # Deobfuscated files
        │ └─┬─ client                   
        │   └── client.jar               # Deobfuscated file (Obfuscated names, fixed stuff like: avm → Zombie)
        ├─┬─ dec                         # Decompiled files
        │ └─┬─ client                    
        │   └── client.jar               # Decompiled file; files are getting decompiled to human-readable form
        └─┬─ output                      # human-readable Source-Code
          └─┬─ client                    
            ├── *                        # SourceFiles; unzipped disassmebled source.jar (client.jar)
        
### More detailed description
#### Execution-Modes
This is just an overview.<br/>
For more detailed description take a look at the documentation of the [corresponding source files](/Aves/Config)
##### 1. Run it over commandline (only)

|Argument|Meaning|Example|
|--------|-------|-------|
|``-l`` ``--logfile``|logs additionally to a logfile<br/> generated under ``/logs`` |``-l``|
|``--version``| Shows the current Aves version and does nothing else |``-version``<br/>Example ``Aves 1.0.7364.40087``<br/> Format:  ``<Name> <MainVersion>.<SubVersion>.<DaysSince2000>.<SecondsSinceMidnight/2>`` → [see also](https://stackoverflow.com/questions/356543/can-i-automatically-increment-the-file-build-version-when-using-visual-studio) |
|``--genconf <value>``|only generates a json-Config file<br/> value = JSON-Config file to generate | ``--genconf config.json`` |
|``-c <value>`` ``--conf <value>``|load a json-Config file<br/> value = JSON-Config file (see below) |``-c config.json`` (uses a file called config.json for configuration)|
|``-v`` ``--mcversion``|Required (if using no json file for configuration)<br /> Version that should be downloaded|``-v "1.14.4"`` (generates files for 1.14.4)|
|``-j`` ``--java``|Path to ``java.exe`` (Java11+)<br/><i>default:</i> path to included ``jre``<br/><i>experimental:</i><br/>If not set, will be automatically searched in either the Environment-Variable ``%JAVA_HOME%``(Windows) / ``$JAVA_HOME``(Linux/Mac) or over the command ``where java`` (Windows, Linux, Mac) | ``-j "C:\Program Files\Java\openjdk-11.0.2\bin\java.exe"`` |
|``-p`` ``--profiles``|Given profiles/variants, that should be used<br/>Overrides the ``Enabled``-property in the json |``-p client server`` ``-p client``|

see also :point_right: ``--help`` 

##### 2. Run it over JSON-Configuration
To get more customizable profiles or templates you can use a json file as configuration.

If you want to generate a config file, use ``--genconf <Path> <optional:additionalParameters>``.<br/>
It's also possible to combine it with some parameters from above, e.g. ``-j`` or ``-v``.

auto-generated file:
``--genconf config.json``
```JS
{
  "ResolveOverNetwork": true,
  "RemoteManifestJsonURL": "https://launchermeta.mojang.com/mc/game/version_manifest.json",
  "SuppressManifestDownload": "00:05:00",
  "ManifestJsonFilePath": "version_manifest.json",
  "TryKeepExisting": true,
  "NetworkIncludeClientLibs": false,
  "NetworkIncludeLogging": false,
  "Version": null, //this must be set by hand or calling the program next time with -v
  "VersionSrcJson": "package.json",
  "VariantConfigs": [
    {
      "Enabled": true,
      "Name": "client",
      "Key": "client",
      "SrcJar": "client.jar",
      "MappingKey": "client_mappings",
      "MappingFile": "client.txt",
      "PatchFile": "client.txt",
      "ExcludedComponents": [
        "net/minecraft/gametest/"
      ],
      "DeObfuscatedFile": "client.jar",
      "DecompiledFile": "client.jar",
      "OutputFilesDirFolder": "client"
    },
    {
      "Enabled": false,
      "Name": "server",
      "Key": "server",
      "SrcJar": "server.jar",
      "MappingKey": "server_mappings",
      "MappingFile": "server.txt",
      "PatchFile": "server.txt",
      "ExcludedComponents": [
        "net/minecraft/gametest/",
        "com/google/",
        "io/netty",
        "it/unimi/",
        "javax/",
        "joptsimple/",
        "org/apache"
      ],
      "DeObfuscatedFile": "server.jar",
      "DecompiledFile": "server.jar",
      "OutputFilesDirFolder": "server"
    }
  ],
  "MakeJavaCompatible": true,
  "JavaExePath": "jre\\bin\\java.exe",
  "Deobfuscator": null,
  "DeobfuscatorTimeout": "00:05:00",
  "BaseDeobfuscatorCommand": "-jar \"{0}\" -s \"{1}\" -m \"{2}\" -o \"{3}\"",
  "Decompiler": null,
  "DecompilerTimeout": "00:30:00",
  "BaseDecompileCommand": "-jar \"{0}\" -dgs=1 -rsy=1 -lit=1 -mpm=60 \"{1}\" \"{2}\"",
  "WorkingDirectory": "workingDir",
  "VersionWorkingDirectory": null,
  "RawDirectory": "raw",
  "MappingFilesDir": "mappings",
  "PatchFilesDir": "patch",
  "DeObfuscatedDirectory": "deof",
  "DecompiledDirectory": "dec",
  "OutputDirectory": "output",
  "OutputDirectoryMetaFiles": "output-meta",
  "OutputDirLibs": "libs",
  "OutputDirLogging": "logging"
}
```
If you also want to set ``server`` to enabled (by default it isn't), use ``--genconf ... -p client server ``

##### 3. Run it as hybrid
You can also use a json-Configuration with some commandline arguments, e.g. if you like to use a different version.<br/>
Note that only the `variants` in the `.json`-file are used

Example using the json from above:<br/>
``-c config.json -v 19w36a``

## Build
Don't want to use the [official releases](https://github.com/BaseMC/Aves/releases)?<br/>
Build the project yourself:

### Requirements
* [.NET CORE 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)

### Get required dependencies
* Build ``ADB`` with Configuration ``Debug``
* Copy [build-dev.json](build/build-dev.json) into your ``ADB`` build output folder (e.g. ``ADB/bin/Debug/netcoreapp3.1``)
* Run in the ``ADB`` build output folder: ``ADB.exe -c build-dev.json -r <yourSystemRID>``
** You can get the corresponding RID [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)<br/>Most common:
*** ``win-x64`` Windows 64bit
*** ``linux-x64`` Linux 64bit
*** ``osx-x64`` Mac OS X 64bit
* Copy all files from the generated ``dev`` folder into your ``Aves`` build output folder

### Build an executable
* Open a new commandline / shell in the repository-root
* Build the project ``dotnet build``
* Select the subfolder ``Aves`` (main project) and create an executable with ``dotnet publish -r <TargetedRuntime(OS)Identifier> -p:PublishSingleFile=true``
  * for the targeted ``RuntimeIdentfier`` see https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
  * example for windows: ``dotnet publish -r win10-x64 -p:PublishSingleFile=true``
* Go back into the root and publish ADB (select the corresponding subfolder) ``dotnet publish -c Release``
* Copy the generated content (e.g. ``ADB/bin/Debug/netcoreapp3.1/publish``) into the ``build`` folder under the directory rot
* Run ``ADB.exe -r <TargetedRuntime(OS)Identifier> --bc <YourConfiguration(Release/Debug)>``

### Nested projects
* [javgent](https://github.com/BaseMC/javgent)
* [avesflower](https://github.com/BaseMC/avesflower)

## Tools for developing
* [Visual Studio 2019](https://visualstudio.microsoft.com/de/vs/)
* [SonarLint VS](https://www.sonarlint.org/visualstudio/)
