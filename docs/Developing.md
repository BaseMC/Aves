## Developing
### Tools for developing
* [Visual Studio 2019](https://visualstudio.microsoft.com/en/vs/)
* [SonarLint VS](https://www.sonarlint.org/visualstudio/)

### Get required external files for ``Aves``
Aves requires some external files, e.g. a [JVM](https://adoptopenjdk.net/), a [deobfuscator](https://github.com/BaseMC/javgent) and a [decompiler](https://github.com/Vineflower/vineflower)
* Build ``ADB`` with configuration ``Debug`` (with ``Release`` it would need a ``GITHUB_TOKEN``)
* Copy [config-dev.json](/build/config-dev.json) into your ``ADB`` build output folder (e.g. ``src/ADB/bin/Debug/net8.0``)
* Run in the ``ADB`` build output folder: ``ADB.exe -c config-dev.json -r <yourSystemRID>``
  * You can get the corresponding RID [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)<br/>Most common:
    * ``win-x64`` Windows 64bit
    * ``linux-x64`` Linux 64bit
    * ``osx-x64`` Mac OS X 64bit
* Copy all files from the generated ``dev`` folder directly into your ``Aves`` build output folder (e.g. ``Aves/bin/Debug/net8.0``)

#### :warning: Notes
* The files are usually kept until a (forced) rebuild is carried out 
* You should update this files regularly


## Related
* [Building](Building.md)
