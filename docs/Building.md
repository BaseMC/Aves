## Building the project
Don't want to use the [official releases](https://github.com/BaseMC/Aves/releases)?<br/>
Build the project yourself:

### Requirements
* [.NET 8.0](https://dotnet.microsoft.com/en-us/download)

### Build an executable
* Open a new commandline / shell in the repository-root
* Move into ``src``
* Build the project ``dotnet build``
* Select the subfolder ``Aves`` (main project) and create an executable with ``dotnet publish -r <TargetedRuntime(OS)Identifier> -p:PublishSingleFile=true``
  * for the targeted ``RuntimeIdentfier`` see https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
  * example for windows: ``dotnet publish -r win10-x64 -p:PublishSingleFile=true``
* Go back into the root, select [ADB](/src/ADB) and publish it ``dotnet publish -c Release``
* Copy the generated content (e.g. ``ADB/bin/Debug/net8.0/publish``) into the [``build``](/build) folder
* Run ``ADB.exe -r <TargetedRuntime(OS)Identifier> --bc <YourConfiguration(Release/Debug)>``
