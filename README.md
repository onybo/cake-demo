# cake-demo
Short intro to how to use cake build

## About
Most of the demo code and sample is based on https://github.com/wieslawsoltes/Cake.CoreCLR.Runner/

### Create Cake tools directory in your project root

```cmd
mkdir tools
```

### Create empty `netcoreapp2.0` project in tool directory

`tools/tools.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>
</Project>
```

### Add Cake as NuGet dependency to tools project

```cmd
dotnet add tools/tools.csproj package Cake.CoreCLR -v 0.27.1 --package-directory tools/Cake.CoreCLR.0.27.1
```

### Run Cake script using .NET Core CLR version

```cmd
dotnet tools/Cake.CoreCLR.0.27.1/cake.coreclr/0.27.1/Cake.dll build.cake -target="Default" -configuration="Release"
```

# Scripts

### Windows

To run Cake build script `build.cake` on `Windows` using `.NET Core` create `build.cmd` script.

```cmd
@echo off
set CAKE_VERSION=0.27.1
mkdir tools
@echo ^<Project Sdk="Microsoft.NET.Sdk"^>^<PropertyGroup^>^<OutputType^>Exe^</OutputType^>^<TargetFramework^>netcoreapp2.0^</TargetFramework^>^</PropertyGroup^>^</Project^> > tools\tools.csproj
dotnet add tools/tools.csproj package Cake.CoreCLR -v %CAKE_VERSION% --package-directory tools/Cake.CoreCLR.%CAKE_VERSION%
dotnet tools/Cake.CoreCLR.%CAKE_VERSION%/cake.coreclr/%CAKE_VERSION%/Cake.dll build.cake -target="Default" -configuration="Release"
```


### Linux/OSX

To run Cake build script `build.cake` on `Linux/OSX` using `.NET Core` create `build.sh` script.

```sh
#!/usr/bin/env bash
CAKE_VERSION=0.27.1
mkdir tools
echo "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup></Project>" > tools/tools.csproj
dotnet add tools/tools.csproj package Cake.CoreCLR -v $CAKE_VERSION --package-directory tools/Cake.CoreCLR.$CAKE_VERSION
dotnet tools/Cake.CoreCLR.$CAKE_VERSION/cake.coreclr/$CAKE_VERSION/Cake.dll build.cake -target="Default" -configuration="Release"
```