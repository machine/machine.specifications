# Machine.Specifications

[![Docs](https://img.shields.io/badge/docs-wiki-blue.svg?style=for-the-badge)](https://github.com/machine/machine.specifications/wiki) [![Nuget](https://img.shields.io/nuget/dt/Machine.Specifications?style=for-the-badge)](https://www.nuget.org/packages/Machine.Specifications) [![Discussions](https://img.shields.io/badge/DISCUSS-ON%20GITHUB-orange?style=for-the-badge)](https://github.com/machine/machine.specifications/discussions) [![License](https://img.shields.io/github/license/machine/machine.specifications?style=for-the-badge)](https://github.com/machine/machine.specifications/blob/master/LICENSE)

<img src="https://github.com/machine/machine.specifications/raw/master/src/Machine.Specifications/Resources/Machine.png" alt="MSpec logo" title="Machine.Specifications" align="right" height="100" />

MSpec is called a "context/specification" test framework because of the "grammar" that is used in describing and coding the
tests or "specs". The grammar reads roughly like this

> When the system is in such a state, and a certain action occurs, it should do such-and-such or be in some end state.

You should be able to see the components of the traditional `Arrange-Act-Assert` model in there. To support readability
and remove as much "noise" as possible, MSpec eschews the traditional attribute-on-method model of test construction.
Instead it uses custom delegates that you assign anonymous methods, and asks you to name them following a certain convention.

```csharp
using Machine.Specifications;

[Subject("Authentication")]
class When_authenticating_an_admin_user
{
    static SecurityService subject;
    static UserToken user_token;

    Establish context = () => 
        subject = new SecurityService();

    Because of = () =>
        user_token = subject.Authenticate("username", "password");

    It should_indicate_the_users_role = () =>
        user_token.Role.ShouldEqual(Roles.Admin);

    It should_have_a_unique_session_id = () =>
        user_token.SessionId.ShouldNotBeNull();
}
```

## Getting Started
1. Install Nuget packages as follows:

```ps1
dotnet add package Machine.Specifications
```

2. Optionally, install the mocking libraries:

```powershell
dotnet add package Machine.Specifications.Fakes
```

## Documentation
For project documentation, please visit the [wiki](https://github.com/machine/machine.specifications/wiki).

## Get in touch
Discuss with us on [Discussions](https://github.com/machine/machine.specifications/discussions), or raise an [issue](https://github.com/machine/machine.specifications/issues).

[![Discussions](https://img.shields.io/badge/DISCUSS-ON%20GITHUB-orange?style=for-the-badge)](https://github.com/machine/machine.specifications/discussions)

## Packages

Project | Build | NuGet
-- | -- | --
`Machine.Specifications` | [![Build](https://img.shields.io/github/workflow/status/machine/machine.specifications/build?style=flat-square)](https://github.com/machine/machine.specifications/actions?query=workflow:build) | [![](https://img.shields.io/nuget/v/Machine.Specifications.svg?style=flat-square)](https://www.nuget.org/packages/machine.specifications)
`Machine.Specifications.Should` | [![Build](https://img.shields.io/github/workflow/status/machine/machine.specifications.should/build?style=flat-square)](https://github.com/machine/machine.specifications.should/actions?query=workflow:build) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Should.svg?style=flat-square)](https://www.nuget.org/packages/machine.specifications.should)
`Machine.Specificaitons.Fakes` | [![Build](https://img.shields.io/github/workflow/status/machine/machine.specifications.fakes/build?style=flat-square)](https://github.com/machine/machine.specifications.fakes/actions?query=workflow:build) | [![](https://img.shields.io/nuget/v/Machine.Specifications.Fakes.svg?style=flat-square)](https://www.nuget.org/packages/machine.specifications.fakes)
