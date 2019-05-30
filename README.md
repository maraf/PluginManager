# PluginManager
NuGet based universal plugin manager

![Preview](/assets/screenshot-search.png)

Nightly builds of PluginManager available at Appveyor - https://ci.appveyor.com/nuget/pluginmanager.

### Appveyor

[![Build status](https://ci.appveyor.com/api/projects/status/k3y0frp1pgsyepwh?svg=true)](https://ci.appveyor.com/project/neptuo/pluginmanager)

### Command line arguments
PluginManager is designed to be a reusable tool to manage plugins distributed as nuget packages.
As a standalone application, PluginManager supports these command line arguments:

- `--path` (required) - A root path to a directory where to install packages.
- `--selfpackageid` (optional) - A package id to indicate which package should be treated as a package for self update. 
- `--dependencies` (optional) - A comma separated list of package ids and versions that are required in package to be compatible (Eg. `PluginContracts-v3.0,TestA,TestB-v1`).
- `--monikers` (optional) - A comma separated list of .NET framework monikers to filter package content during extraction (Eg. `net461,netstandard2.0`).
- `--processnamestokillbeforechange` - A comma separated list of process names to be killed before any changes being processed (it is used to kill all instances of target application before installing/uninstalling dlls, that might be locked). 

### Icons

Some icons by Yusuke [Kamiyamane](http://p.yusukekamiyamane.com).<br>
Some other by [Material Design](https://material.io/tools/icons).
