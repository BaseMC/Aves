## Usage / How to run it
* Supported versions: 1.14.4 or ``>=``19w36a (1.15+)

### Example
* Basic example for commandline/shell:<br/>
```Aves.exe -v 1.14.4```<br/>
Description: Creates source code for 1.14.4 (by default: client only)
#### Folder structure

    .
    ├─── javgent-standalone.jar          # Deobfuscator → see https://github.com/BaseMC/javgent
    ├─── vineflower.jar                  # Decompiler → see https://github.com/Vineflower/vineflower
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
##### 1. Run it over commandline (only)
:point_right: ``--help`` 

|Argument|Meaning|Example|
|--------|-------|-------|
|``-l`` ``--logfile``|logs additionally to a logfile<br/> generated under ``logs`` |``-l``|
|``--version``| Shows the current Aves version and does nothing else |``--version``<br/>Example output: ``Aves 1.0.7364.40087``<br/> Format:  ``<Name> <MainVersion>.<SubVersion>.<DaysSince2000>.<SecondsSinceMidnight/2>`` → [see also](https://stackoverflow.com/questions/356543/can-i-automatically-increment-the-file-build-version-when-using-visual-studio) |
|``--genconf <value>``|only generates a json-Config file<br/> value = JSON-Config file to generate | ``--genconf config.json`` |
|``-c <value>`` ``--conf <value>``|load a json-Config file<br/> value = JSON-Config file (see below) |``-c config.json`` (uses a file called config.json for configuration)|
|``-v`` ``--mcversion``|Required (if using no json file for configuration)<br /> Version that should be downloaded|``-v 1.14.4`` (generates files for 1.14.4)<br/>``-v LATEST`` (generates files for latest version)|
|``-j`` ``--java``|Path to ``java.exe`` (Java21+)<br/><i>default:</i> path to included ``jre``<br/><i>experimental:</i><br/>If not set, will be automatically searched in either the Environment-Variable ``%JAVA_HOME%``(Windows) / ``$JAVA_HOME``(Linux/Mac) or over the command ``where java`` (Windows, Linux, Mac) | ``-j "C:\Program Files\Java\openjdk-21.0.2\bin\java.exe"`` |
|``-p`` ``--profiles``|Given profiles/variants, that should be used<br/>Overrides the ``Enabled``-property in the json |``-p client server`` ``-p client``|

##### 2. Run it over JSON-Configuration
To get more customizable profiles or templates you can use a json file as configuration.

If you want to generate a config file, use ``--genconf <Path> <optional:additionalParameters>``.<br/>
It's also possible to combine it with some parameters from above, e.g. ``-j`` or ``-v``.

<details>
<summary>auto-generated file with <i>--genconf config.json</i></summary>

```JS
{
  "ResolveOverNetwork": true,
  "RemoteManifestJsonURL": "https://launchermeta.mojang.com/mc/game/version_manifest.json",
  "SuppressManifestDownload": "00:05:00",
  "ManifestJsonFilePath": "version_manifest.json",
  "TryKeepExisting": true,
  "NetworkIncludeClientLibs": false,
  "NetworkIncludeLogging": false,
  "Version": null, // if null: must be set manually over CLI when running it next time
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
  "BaseDeobfuscatorCommand": "-jar \"{Deobfuscator}\" -s \"{SrcJar}\" -m \"{PatchFile}\" -o \"{DeObfuscatedFile}\"",
  "Decompiler": null,
  "DecompilerTimeout": "00:30:00",
  "BaseDecompilerCommand": "-jar \"{Decompiler}\" -dgs=1 -rsy=1 -lit=1 -mpm=60 \"{SrcFile}\" \"{TargetDir}\"",
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

</details>

If you also want to set ``server`` to enabled (by default it isn't), use ``--genconf ... -p client server ``

:point_right: For more detailed description of the configuration options take a look at the documentation of the [corresponding source files](/src/Aves/Config)

##### 3. Run it as hybrid :twisted_rightwards_arrows:
You can also use a json-Configuration with some commandline arguments, e.g. if you like to use a different version.<br/>
Note that only the `variants` in the `.json`-file are used

Example using the json from above:<br/>
``-c config.json -v 19w36a``
