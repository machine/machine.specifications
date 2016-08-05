## Documentation

The documentation is here: https://github.com/machine/machine.specifications/wiki

[![Build status](https://ci.appveyor.com/api/projects/status/wtk1ch0ix6i47epu/branch/master?svg=true)](https://ci.appveyor.com/project/machine-specifications/machine-specifications) [![Build Status](https://travis-ci.org/einari/machine.specifications.svg?branch=master)](https://travis-ci.org/einari/machine.specifications) [![Travis](https://img.shields.io/travis/rust-lang/rust.svg?maxAge=2592000)](https://travis-ci.org/einari/machine.specifications)

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
