#tool nuget:?package=Newtonsoft.Json&version=12.0.3
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var destDir = Argument("destDir", CakeEnvironment.WorkingDirectory);
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//May need GITHUB-SECRET-KEY
//Download javgent (version as parameter ?? latest)
//Download avesflower (version as parameter ?? latest)
//Download jre11 (javaVersion, os, arch, ...) (adotopenjdk)
// -> See https://api.adoptopenjdk.net/swagger-ui/#/Assets/get_v3_assets_feature_releases__feature_version___release_type_
//set a global option where to find jre (e.g. into a file), or do autodetect
Task("javgent")
    .Does(() =>
    {
        DownloadFile("https://github.com/BaseMC/javgent/releases/download/v0.1.0/javgent-standalone.jar", destDir+"/javgent-standalone.jar");
    });

Task("avesflower")
    .Does(() =>
    {

    });

Task("jre")
    .Does(() =>
    {

    });

Task("Default")
    .IsDependentOn("javgent")
    .IsDependentOn("avesflower")
    .IsDependentOn("jre");

RunTarget(target);