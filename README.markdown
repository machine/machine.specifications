# Machine.Specifications

Machine.Specifications (MSpec) is a [context/specification][5] framework that removes language noise and simplifies tests. All it asks is that you accept the `= () =>`. Keep up with the [latest news and discussions][8] or follow the maintainer, [@agross](https://twitter.com/agross).

![NDepend supports the Machine.Specifications project](http://www.ndepend.com/res/PoweredByNDepend.png)

# Installation

You can download the [unsigned binaries][1] (<strong>recommended</strong>) or the [signed binaries][2] directly from the [TeamCity server][3]. But, we recommended installing [the NuGet package][4]. Install on the command line from your solution directory:

```bash
cmd> nuget install Machine.Specifications

# or:
cmd> nuget install Machine.Specifications-Signed
```

Or use the Package Manager console in Visual Studio:

```powershell
PM> Install-Package Machine.Specifications

# or:
PM> Install-Package Machine.Specifications-Signed
```

# Usage
MSpec is called a "context/specification" test framework because of the "grammar" that is used in describing and coding the tests or "specs". That grammar reads roughly like this

> When the system is in such a state, and a certain action occurs, it should do such-and-such or be in some end state.

You should be able to see the components of the traditional [Arrange-Act-Assert][9] model in there. To support readability and remove as much "noise" as possible, MSpec eschews the traditional attribute-on-method model of test construction. It instead uses custom .NET Delegates that you assign anonymous methods and asks you to name them following a certain convention.

Read on to construct a simple MSpec styled specification class.

## Subject

The `Subject` attribute is the first part of a spec class. It describes the "context", which can be the literal `Type` under test or a broader description. The subject is not required, but it is good practice to add it. Also, the attribute allows [ReSharper](#resharper-integration) to detect context classes such that [delegate members](#establish) will not be regarded as unused.

The class naming convention is to use `Sentence_snake_case` and to start with the word "When".

```csharp
[Subject("Authentication")]                           // a description
[Subject(typeof(SecurityService))]                    // the type under test
[Subject(typeof(SecurityService), "Authentication")]  // or a combo!
public class When_authenticating_a_user { ... }       // remember: you can only use one Subject Attribute!
```

## Tags

The `Tags` attribute is used to organize your spec classes for inclusion or exclusion in test runs. You can identify tests that hit the database by tagging them "Slow" or tests for special reports by tagging them "AcceptanceTest".

Tags can be used to [include or exclude certain contexts during a spec run](#command-line-reference).

```csharp
[Tags("RegressionTest")]  // this attribute supports any number of tags via a params string[] argument!
[Subject(typeof(SecurityService), "Authentication")]
public class When_authenticating_a_user { ... }
```

## Establish

The `Establish` delegate is the "Arrange" part of the spec class. The establish will only run *once*, so your assertions should not mutate any state or you may be in trouble.

```csharp
[Subject("Authentication")]
public class When_authenticating_a_new_user
{
    Establish context = () =>
    {
        // ... any mocking, stubbing, or other setup ...
        Subject = new SecurityService(foo, bar);
    }

    static SecurityService Subject;
}
```

## Cleanup

The pair to Establish is `Cleanup`, which is also called *once* after all of the specs have been run.

```csharp
[Subject("Authentication")]
public class When_authenticating_a_user
{
    Establish context = () =>
    {
        Subject = new SecurityService(foo, bar);
    }

    Cleanup after = () =>
    {
        Subject.Dispose();
    }

    static SecurityService Subject;
}
```

## Because

The `Because` delegate is the "Act" part of the spec class. It should be the single action for this context, the only part that mutates state, against which all of the assertions can be made. Most `Because` statements are only *one* line, which allows you to leave off the squiggly brackets!

```csharp
[Subject("Authentication")]
public class When_authenticating_a_user
{
    Establish context = () =>
    {
        Subject = new SecurityService(foo, bar);
    }

    Because of = () => Subject.Authenticate("username", "password");

    static SecurityService Subject;
}
```

If you have a multi-line because statement, you probably need to identify which of those lines are actually setup and move them into the establish. Or, your spec may be concerned with too many contexts and needs to be split or the subject-under-test needs to be refactored.

## It

The `It` delegate is the "Assert" part of the spec class. It may appear one or more times in your spec class. Each statement should contain a single assertion, so that the intent and failure reporting is crystal clear. Like `Because` statements, `It` statements are usually one-liners and may not have squiggly brackets.

```csharp
[Subject("Authentication")]
public class When_authenticating_an_admin_user
{
    Establish context = () =>
    {
        Subject = new SecurityService(foo, bar);
    }

    Because of = () => Token = Subject.Authenticate("username", "password");

    It should_indicate_the_users_role = () => Token.Role.ShouldEqual(Roles.Admin);
    It should_have_a_unique_session_id = () => Token.SessionId.ShouldNotBeNull();

    static SecurityService Subject;
    static UserToken Token;
}
```

An `It` statement without an assignment will be reported by the test runner in the "Not implemented" state. You may find that "stubbing" your assertions like this helps you practice TDD.

```csharp
It should_list_your_authorized_actions;
```

### Assertion Extension Methods

As you can see above, the `It` assertions make use of these `Should` extension methods. They encourage readability and a good flow to your assertions when read aloud or on paper. You *should* use them wherever possible, just "dot" off of your object and browse the IntelliSense!

It's good practice to make your own `Should` assertion extension methods for complicated custom objects or domain concepts.

### Ignore

Every test framework lets you ignore incomplete or failing (we hope not) specs, MSpec provides the `Ignore` attribute for just that. Just leave a note describing the reason that you ignored this spec.

```csharp
[Ignore("We are switching out the session ID factory for a better implementation")]
It should_have_a_unique_session_id = () => Token.SessionId.ShouldNotBeNull();
```

## Catch

When testing that exceptions are thrown from the "action" you should use a `Catch` statement. This prevents thrown exceptions from escaping the spec and failing the test run. You can inspect the exception's expected properites in your assertions.

```csharp
[Subject("Authentication")]
public class When_authenticating_a_user_fails_due_to_bad_credentials
{
    Establish context = () =>
    {
        Subject = new SecurityService(foo, bar);
    }

    Because of = () => Exception = Catch.Exception(() => Subject.Authenticate("username", "password"));

    It should_fail = () => Exception.ShouldBeOfType<AuthenticationFailedException>();
    It should_have_a_specific_reason = () => Exception.Message.ShouldContain("credentials")

    static SecurityService Subject;
    static Exception Exception;
}
```

# Command Line Reference

MSpec, like other testing frameworks, provides a robust command-line runner that can be used to execute specs in one or more assemblies and allows a number of output formats to suit your needs. The runner comes in different flavors:

 * `mspec.exe`, AnyCPU, runs on the CLR 2.0
 * `mspec-x86.exe`, x86, runs on the CLR 2.0
 * `mspec-clr4.exe`, AnyCPU, runs on the CLR 4.0
 * `mspec-x86-clr4.exe`, x86, runs on the CLR 4.0

Usage of the command-line runner is as follows (from `mspec.exe --help`):

```text
Usage: mspec.exe [options] <assemblies>
Options:
-f, --filters               Filter file specifying contexts to execute (full type name, one per line). Takes precedence over tags
-i, --include               Executes all specifications in contexts with these comma delimited tags. Ex. -i "foo,bar,foo_bar"
-x, --exclude               Exclude specifications in contexts with these comma delimited tags. Ex. -x "foo,bar,foo_bar"
-t, --timeinfo              Shows time-related information in HTML output
-s, --silent                Suppress progress output (print fatal errors, failures and summary)
-p, --progress              Print dotted progress output
-c, --no-color              Suppress colored console output
-w, --wait                  Wait 15 seconds for debugger to be attached
--teamcity                  Reporting for TeamCity CI integration (also auto-detected)
--no-teamcity-autodetect    Disables TeamCity autodetection
--html <PATH>               Outputs the HTML report to path, one-per-assembly w/ index.html (if directory, otherwise all are in one file)
--xml <PATH>                Outputs the XML report to the file referenced by the path
-h, --help                  Shows this help message
Usage: mspec.exe [options] <assemblies>
```

### TeamCity Reports

MSpec can output [TeamCity](http://www.jetbrains.com/teamcity/) [service messages][7] to update the test run status in real time. This feature is enabled by passing the `--teamcity` switch, but the command-line runner *can* auto-detect that it is running in the TeamCity context.

### HTML Reports

MSpec can output human-readable HTML reports of the test run by passing the `--html` option. If a filename is provided, the output is placed at that path, overwriting existing files. If multiple assemblies are being testing, the output is grouped into a single file. If no filename is provided, it will use the name of the assembly(s). If multiple assemblies are being tested, an `index.html` is created with links to each assembly-specific report. You can use this option if your CI server supports capturing HTML as build reports.

### XML Reports

MSpec can output XML test run reports by passing the `--xml` option. This option behaves the same as the `--html` option, in terms of file naming.

### Selenium Reports

The MSpec HTML reports can show additional [Selenium](http://seleniumhq.org/)-specific information, like screenshots and debug statements. Instructions on [how to integrate this feature][6] into your specs is available on the web. There is also a [sample implementation][10] available.

# ReSharper Integration

MSpec provides a batch file to integrate with the ReSharper test runner, custom naming rules, and code annotations. MSpec currently supports ReSharper 6.1, 7.0, and 7.1.

### Code Annotations

By default, ReSharper thinks that specification classes (those with the `[Subject]` attribute) and their internals are unused. To change this behavior in Visual Studio:

 1. Open the ReSharper Options (ReSharper -> Options...)
 1. Select "Code Annotations"
 1. Ensure that the namespace "Machine.Specifications.Annotations" is checked
 1. Click "OK"

### Templates

The file, live, and surround templates can be imported from `Misc\ReSharper.*.DotSettings`. The single file template creates a basic context. The single surround template wraps a `Catch.Exception` call ([more information how to use them][11]). The live templates cover the major delegates:

 * `mse`, an `Establish` delegate
 * `msb`, a `Because` delegate
 * `msi`, an `It` delegate
 * `msf`, a failing `It` delegate, use in combination with the `Catch` surround template

# TestDriven.Net Integration

MSpec provides a batch file for setting up TD.NET integration. Newer versions (2.24+) support an xcopy integration that avoids the versioning issues arising from the registry-based scheme. If you use NuGet, you're already set. If you're not using NuGet, make sure to  copy `Machine.Specifications.dll.tdnet` and `Machine.Specifications.TDNetRunner.dll` to your project's output directory.

 [1]: http://teamcity.codebetter.com/guestAuth/repository/download/bt342/.lastSuccessful/Machine.Specifications-Release.zip
 [2]: http://teamcity.codebetter.com/guestAuth/repository/download/bt345/.lastSuccessful/Machine.Specifications-Signed-Release.zip
 [3]: http://teamcity.codebetter.com/project.html?projectId=project27
 [4]: http://nuget.org/packages/Machine.Specifications
 [5]: http://www.code-magazine.com/article.aspx?quickid=0805061
 [6]: http://codebetter.com/blogs/aaron.jensen/archive/2009/10/19/advanced-selenium-logging-with-mspec.aspx
 [7]: http://confluence.jetbrains.com/display/TCD7/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ReportingTests
 [8]: https://groups.google.com/forum/?fromgroups#!forum/machine_users
 [9]: http://c2.com/cgi/wiki?ArrangeActAssert
 [10]: https://github.com/agross/mspec-samples/tree/master/WebSpecs/LoginApp.Selenium.Specs
 [11]: http://therightstuff.de/2010/03/03/MachineSpecifications-Templates-For-ReSharper.aspx
