## Documentation

For project documentation, please visit the [wiki](https://github.com/machine/machine.specifications/wiki).

## Training

There is a PluralSight course made by [@kevinkuebler](https://github.com/kevinkuebler) - you can find it here:

[![Training](https://www.pluralsight.com/content/dam/pluralsight/newsroom/brand-assets/logos/pluralsight-logo-hor-black-1@2x.png)](https://www.pluralsight.com/courses/expressive-testing-dotnet-mspec)


## Overview
MSpec is called a "context/specification" test framework because of the "grammar" that is used in describing and coding the tests or "specs". That grammar reads roughly like this

> When the system is in such a state, and a certain action occurs, it should do such-and-such or be in some end state.

You should be able to see the components of the traditional Arrange-Act-Assert model in there. To support readability and remove as much "noise" as possible, MSpec eschews the traditional attribute-on-method model of test construction. It instead uses custom .NET delegates that you assign anonymous methods and asks you to name them following a certain convention.

```csharp
[Subject("Authentication")]
class When_authenticating_an_admin_user
{
    static SecurityService Subject;
    static UserToken Token;

    Establish context = () => 
        Subject = new SecurityService();

    Because of = () =>
        Token = Subject.Authenticate("username", "password");

    It should_indicate_the_users_role = () =>
        Token.Role.ShouldEqual(Roles.Admin);

    It should_have_a_unique_session_id = () =>
        Token.SessionId.ShouldNotBeNull();
}
```

## Build status

Project | CI | NuGet
-- | -- | --
Machine.Specifications | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-specifications.svg)](https://ci.appveyor.com/project/machine-specifications/machine-specifications) | [![](https://img.shields.io/nuget/v/Machine.Specifications.svg)](https://www.nuget.org/packages/machine.specifications)
Machine.Specifications.Reporting | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-specifications-reporting.svg)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-reporting) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Reporting.svg)](https://www.nuget.org/packages/machine.specifications.reporting)
Machine.Specifications.Runner.Console | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-specifications-runner-console.svg)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-runner-console) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Runner.Console.svg)](https://www.nuget.org/packages/machine.specifications.runner.console)
Machine.Specifications.Runner.ReSharper | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-specifications-runner-resharper.svg)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-runner.resharper) | [![](https://img.shields.io/resharper/v/Machine.Specifications.Runner.Resharper9.svg)](https://resharper-plugins.jetbrains.com/packages/Machine.Specifications.Runner.Resharper9)
Machine.Specifications.Runner.TDNet | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-specifications-runner-tdnet.svg)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-runner.tdnet) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Runner.TDNet.svg)](https://www.nuget.org/packages/machine.specifications.runner.tdnet)
Machine.Specifications.Runner.Utility | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-specifications-runner-utility.svg)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-runner-utility) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Runner.Utility.svg)](https://www.nuget.org/packages/machine.specifications.runner.utility)
Machine.Specifications.Runner.VisualStudio | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-vstestadapter.svg)](https://ci.appveyor.com/project/machine-specifications/machine-vstestadapter) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Runner.VisualStudio.svg)](https://www.nuget.org/packages/machine.specifications.runner.visualstudio)
Machine.Specifications.Should | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-specifications-should.svg)](https://ci.appveyor.com/project/machine-specifications/machine-specifications-should) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Should.svg)](https://www.nuget.org/packages/machine.specifications.should)
Machine.Fakes | [![](https://img.shields.io/appveyor/ci/machine-specifications/machine-fakes.svg)](https://ci.appveyor.com/project/machine-specifications/machine-fakes) | [![](https://img.shields.io/nuget/v/Machine.Fakes.svg)](https://www.nuget.org/packages/machine.fakes)
