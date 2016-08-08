## Documentation

The documentation is here: https://github.com/machine/machine.specifications/wiki

## Training

There is a course made by [@kevinkuebler](https://github.com/kevinkuebler) - you can find it here:

[![Training](https://www.pluralsight.com/content/dam/pluralsight/newsroom/brand-assets/logos/pluralsight-logo-hor-black-1@2x.png)](https://www.pluralsight.com/courses/expressive-testing-dotnet-mspec)

## Build status

[![Build status](https://ci.appveyor.com/api/projects/status/wtk1ch0ix6i47epu/branch/master?svg=true)](https://ci.appveyor.com/project/machine-specifications/machine-specifications) [![Travis](https://img.shields.io/travis/machine/machine.specifications.svg?label=travis-ci)](https://travis-ci.org/machine/machine.specifications) [![Gitter](https://img.shields.io/gitter/room/nwjs/nw.js.svg?maxAge=2592000)](https://gitter.im/machine/specifications) [![Waffle.io](https://img.shields.io/waffle/label/evancohen/smart-mirror/in%20progress.svg?maxAge=2592000)](https://waffle.io/machine/machine.specifications)


## Overview
MSpec is called a "context/specification" test framework because of the "grammar" that is used in describing and coding the tests or "specs". That grammar reads roughly like this

> When the system is in such a state, and a certain action occurs, it should do such-and-such or be in some end state.

You should be able to see the components of the traditional Arrange-Act-Assert model in there. To support readability and remove as much "noise" as possible, MSpec eschews the traditional attribute-on-method model of test construction. It instead uses custom .NET delegates that you assign anonymous methods and asks you to name them following a certain convention.

```csharp
[Subject("Authentication")]
public class When_authenticating_an_admin_user
{
    Establish context = () => {
        Subject = new SecurityService();
    };

    Because of = () => {
        Token = Subject.Authenticate("username", "password");
    };

    It should_indicate_the_users_role = () => {
        Token.Role.ShouldEqual(Roles.Admin);
    };

    It should_have_a_unique_session_id = () => {
        Token.SessionId.ShouldNotBeNull();
    };

    static SecurityService Subject;
    static UserToken Token;
}
```
