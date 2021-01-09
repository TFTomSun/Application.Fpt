# Demo App

## Getting Started

### Prerequsites
* Latest released version of Visual Studio 2019
* Latest version of .NET SDK (normally comes with Visual Studio 2019 installation)
* To be able to use the apps and run the tests you need to configure the weather stack API key as an environment variable 'WeatherStackApiKey'.

### Build
#### Visual Studio
Open and build the solution in the repository root in the latest released version of Visual Studio 2019 

#### Dotnet CLI
Open any console, navigate to the repository root and type:
```
dotnet build
```

### Run

#### Visual Studio
* Select one of the app projects as startup project
  * Thomas.Demo.Client.WpfApp
  * Thomas.Demo.Client.ConsoleApp
* Press F5 / Start to run the application(s)


## Disclaimer
All code in the solution is written by me. All referenced nuget packages (e.g. MaterialDesign) are 3rd Party components.

## Notes
The underlying framework '**Thomas.Apis.***' is not feature complete and not cleaned up, because I just reused and implemented parts that I needed for that Demo. In the same time it contains some functionality that is not used, because it was a bit difficulty to only integrate the parts that I needed.
The framework is also not fully compatible with the new C# Nullables feature, which leads to some compiler warnings.
