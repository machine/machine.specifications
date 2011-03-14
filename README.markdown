Machine.Specifications (MSpec)
======================================================================

The most recent release is available at [CodeBetter's TeamCity server](http://teamcity.codebetter.com/project.html?projectId=project27):

  * Recommended: [Unsigned release](http://teamcity.codebetter.com/guestAuth/repository/download/bt342/.lastSuccessful/Machine.Specifications-Release.zip),
  * [Signed release](http://teamcity.codebetter.com/guestAuth/repository/download/bt345/.lastSuccessful/Machine.Specifications-Signed-Release.zip).
  
You can also install these using [NuGet](http://nuget.org/Packages/Search?packageType=Packages&searchCategory=All+Categories&searchTerm=machine.specifications):
<pre>
  PM> Install-Package Machine.Specifications
  PM> Install-Package Machine.Specifications-Signed
</pre>

On top of that we provide downloads for the latest successful build (possibly more current than the releases above):

  * [Unsigned build](http://teamcity.codebetter.com/guestAuth/repository/download/bt44/.lastSuccessful/Machine.Specifications-Release.zip),
  * [Signed build](http://teamcity.codebetter.com/guestAuth/repository/download/bt344/.lastSuccessful/Machine.Specifications-Signed-Release.zip).

Machine.Specifications is a Context/Specification framework geared towards removing language noise and simplifying tests. All it asks is that you accept the `=()=>`.

Below docs are a work in progress:

# Machine.Specifications

Machine.Specifications is a [Context/Specification](http://www.code-magazine.com/article.aspx?quickid=0805061) framework geared towards removing language noise and simplifying tests. All it asks is that you accept the `=()=>`.

The source code is available on GitHub at [http://github.com/machine/machine.specifications](http://github.com/machine/machine.specifications). It is released under the terms of the MIT license with some parts MS-PL. Information about this license is contained within the accompanying `License.txt` file.

## Getting started with MSpec

### Downloading/Building/Installing MSpec (would love to make this simpler)

MSpec is a tool that is constantly being worked on in order to fix bugs or add new features. As such, it does not have the classic model of milestone releases to represent "stable" version, although the version number of MSpec is periodically updated to reflect progress in the evolution of the codebase.

With this in mind, there are two common ways to obtain MSpec: build from source or get the latest build zip MSpec's Continuous Integration (CI) server.

#### Build from source

The easiest way to build MSpec from source is to clone the git repository on GitHub and build the MSpec solution. If you do not intend to fork/contribute changes MSpec, you can anonymously clone the GitHub repo with the following command. If terms like "git" and "clone the git repository" are moon language to you, you can learn more [here](http://book.git-scm.com).

`git clone git://github.com/machine/machine.specifications.git`

Start the build by running `build.cmd` right from the cloned directory.

The solution file is located, relative to the root of the repo, at `Source\Machine.Specifications.sln`.

#### Get the latest build from the CI server

MSpec has a Continuous Integration setup, provided by [CodeBetter](http://www.codebetter.com) and running on TeamCity.

If you'd like to skip the above steps and just want the binaries for MSpec, get the zip of the latest release

  * Recommended: [Unsigned release](http://teamcity.codebetter.com/guestAuth/repository/download/bt342/.lastSuccessful/Machine.Specifications-Release.zip),
  * [Signed release](http://teamcity.codebetter.com/guestAuth/repository/download/bt345/.lastSuccessful/Machine.Specifications-Signed-Release.zip).

### How stuff works

Subject/It/Because of/Establish context/Cleanup after

### Running your specs (Test Runners)

#### Command line

MSpec, like other testing frameworks, provides a robust command-line runner that can be used to execute specs in one or more assemblies and allows a number of output formats to suit your needs. The runner comes in different flavors:

  * `mspec.exe`, AnyCPU, runs on the CLR 2.0
  * `mspec-x86.exe`, x86, runs on the CLR 2.0
  * `mspec-clr4.exe`, AnyCPU, runs on the CLR 4.0
  * `mspec-x86-clr4.exe`, x86, runs on the CLR 4.0

Usage of the command-line runner is as follows (from `mspec.exe --help`):

<pre>
Usage: mspec.exe [options] <assemblies>

Options:

-i, --include  Executes all specifications in contexts with these comma delimited tags. Ex. -i "foo,bar,foo_bar"

-x, --exclude  Exclude specifications in contexts with these comma delimited tags. Ex. -x "foo,bar,foo_bar"

-t, --timeinfo Shows time-related information in HTML output

-s, --silent   Suppress console output

--teamcity     Reporting for TeamCity CI integration.

--html &lt;PATH&gt;  Outputs an HTML file(s) to path, one-per-assembly w/ index.html (if directory, otherwise all are in one file)

--xml &lt;PATH&gt;   Outputs an XML file(s) to path

-h, --help     Shows this help message

Usage: mspec.exe [options] &lt;assemblies&gt;
</pre>

#### Selenium Support in the command-line runner

The command-line runner provides support for [Selenium](http://seleniumhq.org/) web tests written using MSpec. When utilized, the MSpec HTML reports will show additional, Selenium-specific information, like screenshots and some useful debug info that can come in handy when trying to figure out why a web test has failed. Aaron Jensen has written a [blog post](http://codebetter.com/blogs/aaron.jensen/archive/2009/10/19/advanced-selenium-logging-with-mspec.aspx) on the topic that explains how to integrate this feature into your specs.

#### TeamCity support from the command-line runner

MSpec provides [TeamCity](http://www.jetbrains.com/teamcity/) integration via specialized output from the command-line runner to provide information and updates on overall test run status while the specs are running. This functionality can be enabled by passing the `--teamcity` option to the command-line runner.

#### HTML output from the command-line runner

Using the `--html` option from the command-line runner will cause the runner to output its test results in a "human readable" HTML document. If no file name is provided as an argument, it will use the name of the tested assembly(s) as the name of the output file. In the case of multiple assemblies, an `index.html` will be included with links to each assembly-specific HTML document. If a filename is provided, the output will be place in a file at that name/path, overwriting previous files. If multiple assemblies are being testing, the output for each will be grouped into a single file.

Using this option with a CI server that supports running the command-line runner and capturing the output HTML as an artifact, you can integrate the test results into your build report.

Also, this option is needed if you intend to capture Selenium-specific test information in your build results.

#### XML output from the command-line runner

XML output can be generated by MSpec's command-line runner for use with external tools that can consume the markup with the `--xml <filename(s)>` option.

This option behaves the same as the `--html` option, in terms of filename behavior and multiple assemblies.

#### ReSharper

##### Using InstallResharperRunner*.bat

MSpec provides a batch file for each of the four versions of ReSharper it supports, 4.1, 4.5, 5.0 and 5.1.

##### Preventing ReSharper from marking specifications as unused

By default, ReSharper will think that specification classes (those marked with the [Subject] attribute), and their internals are unused.  To change this behavior in Visual Studio:

1. Open the ReSharper Options (ReSharper -> Options...)
2. Select "Code Annotations"
3. Ensure that the namespace "Machine.Specifications.Annotations" is checked
4. Click "OK"

#### TestDriven.Net

##### Using InstallTDNetRunner.bat

*NOTE: If you obtained the latest successful binaries from CI build as indicated above, the InstallTDNetRunner.bat is already with the binaries and doesn't need to be copied.*

MSpec provides a `InstallTDNetRunner.bat` file which can be used to add support for MSpec to TestDriven.Net. The file (and another version which runs silently) are available in the `\Distribution\Specifications` directory of the source repository. To add TD.Net support:

- Copy one or more of the .bat files in the above-mentioned directory into a directory which contains the complete build output of the project (usually `\Build` in the source repo).

- Run one of them.

After following these steps, MSpec-based Contexts and Specifications can be ran from within Visual Studio in the same manner as tests from other frameworks are executed.

##### Using XCopy deploy in TD.NET 2.24+

TestDriven.Net versions 2.24 and newer support an XCopy deployment model that simplifies the plugin deployment process and negates the versioning issues that arise from using the registry-based scheme used in `InstallTDNetRunner.bat`.

All that needs to be done is to make sure that the `Machine.Specifications.dll.tdnet` file that is deployed as part of the zip downloads and `Machine.Specifications.TDNetRunner.csproj` is in the same directory as your MSpec binaries.

## Guidelines

### The utility of Inheritance in Context/Specification-style testing

aka When/how to use base classes.

### Intention revealing contexts

aka helper methods in base classes, fluent-fixture patterns, etc etc ad nauseum.

## Tips

### Make your Its/Becauses single-line statements

MSpec is designed to reduce noise in tests. You should generally only have one line in your `It` and `Because` statements. As such, you should probably leave out the `{` and `}`. Context/Specification testing, while a rethinking of "classic" TDD, still abides by the rules of [Arrange-Act-Assert](http://c2.com/cgi/wiki?ArrangeActAssert). As such, it is preferrable to keep the latter two as succinct as possible.

#### Because

If you're consistently finding that you need to have multiple lines in your `Because` statements, that may be a code smell that your Context is "too chunky". You want to be able to verify and "nail down" that the behavior that you're verifying with your `It` statements is because of a *single* action. Of course, most non-trivial contexts require multiple lines of setup to get into the preferred state that you're verifying, but you want to be able to say that there is a particular, final action that makes your specifications pass.

For example:

<pre>
public class because_example_goes_here : ExampleSpecs {

}
</pre>

#### It

Simultaneously, your `It` statements should be single-liners and reflect a single assertion. If you're stuffing multiple assertions into a single `It`, considering the wording of that `It` and how you may be able to break it up into two or more specifications, each containing a single statement.

<pre>
public class it_example_goes_here : ExampleSpecs {

}
</pre>

# Brief code documentation

## Definitions
* Specification (spec) - expected behavior of a class under specific context.
	* It is also sometimes known as exemplar.
* Specification class - executable examples of the specification.
* Observation - expected result of the class method described by the specification.

## Conventions
* One class per specification
* *Class name* - in lower case with underscore separating each word: `when_first_created`. Often starts with *when* to better describe condition/context under which specification applies (under which class is being tested).
* *Element names* - lower case with underscore separator, same as Class name: `should_do_stuff`
* *Observation name* - always begins with '*should*' to explicitly remind that this code describes the **expected** behavior.
	* An alternative naming convention is to drop shoulds, but the language then begins to appear like you're static facts, not theories of behavior: *System should say 'Hello'* vs. *System says 'Hello'*. Choose the language that works better for you. If in doubt, stick with *should* is recommended..
* *Code placement* - in Mspec all specification elements are C# delegates. Use Lamba expression to define code for each specification element: `It should_do_stuff =()=> stuff.DoIt();`
* Single `Establish context` and `Because of` per class
* Single line per `Because of` and `It should_...`

## Attributes
* `[Subject( [type], ["subject"] )]` Attribute - decorates every specification class. `type` or `subject` can be omitted, but not both.
* `[SetupForEachSpecification]` - forces the context to be re-established for each observation (It clause). However it is recommended for observation not to modify the state (thus by default a context needs to be established only once). Try to fix your code to work this way instead of overriding default behavior.
* `[Tags( "tag", ["optional tag"] ]` - adds tags to the specification class.
* `[Ignore( "reason", ["reason"] )]` - ignores class or observation.
* `[Behaviors]` - marks class as a shared behaviors group that can be included in other specification classes.
	* Use `Behaves_like<TBehavior>` in a specification class to include shared behaviors group.

## Class elements
All specification elements are delegates for code to run specific portion of the spec.  
Everything except `It` is optional.

* `Establish context` - initialization and set up.
* `Because of` - the operation specification describes. This is where method under test is executed.
* `It should_...` - observation. Operation (`Because of`) results are tested here.
	* Use `Should` extension methods to test assertions. For example: `someResult.ShouldBeFalse()`
* `Behaves_like<TBehavior> ...` - points to the group of shared behaviors. All observations from `TBehavior` class will be tested in the current class.
	* `TBehavior` class needs to be decorated with `[Behaviors]` attribute.
* `Cleanup after` - code to clean up afterwards.

## Special interfaces
* `IAssemblyContext` - implement to set up before or clean up things after all specs in the assembly are executed.
	* `OnAssemblyStart` - setup stuff before specs are run.
	* `OnAssemblyComplete` - clean up afterwards.
* `ICleanupAfterEveryContextInAssembly` - implement to do common clean up after every spec run in the assembly (database clean up, system clock reset, etc.)
	* `AfterContextCleanup` - clean up after every spec.
* `ISupplementSpecificationResults` - ??

## Shoulds (Assertions)
MSpec provides assertions methods to test that observations are correct. There's no need to use 3rd party TDD library (like xUnit or NUnit).

The main difference is, again, in the language. Instead of having static Assert methods you're provided with extension Should methods. For example: `Assert.True( someData );` becomes `someData.ShouldBeTrue()`.

List of *shoulds* (grouped by related input type):

* `ShouldBeFalse` & `ShouldBeTrue`
* `ShouldEqual` & `ShouldNotEqual` - compares using Equals methods.
* `ShouldBeNull` & `ShouldNotBeNull`
* `ShouldBeTheSameAs` & `ShouldNotBeTheSameAs` - compares references.
* `ShouldBeOfType`, S`houldBe` & `ShouldNotBeOfType`- confirms object is of specific type (or not). `ShouldBe` is the same as `ShouldBeOfType`.
* `ShouldEachConformTo` - confirms all items in collection confirm to the provided condition.
* `ShouldContain` & `ShouldNotContain` - confirms collection contains (or not) specified items. Can also confirm that string contains another string.
* `ShouldContainOnly` - confirms collections contains only provided items.
* `ShouldBeGreaterThan`, `ShouldBeGreaterThanOrEqualTo`, `ShouldBeLessThan` &  `ShouldBeLessThanOrEqualTo` - `IComparable.CompareTo` method is compare two objects.
* `ShouldBeCloseTo` - confirms two numbers or dates are within specified tolerance.
* `ShouldBeEmpty` & `ShouldNotBeEmpty` - confirms collection is empty (or not).
* `ShouldMatch` - confirms string matches provided pattern.
* `ShouldBeEqualIgnoringCase` - compares strings ignoring case.
* `ShouldStartWith`, `ShouldEndWith` & `ShouldBeSurroundedWith` - confirms provided string is before, after, or at both before and after of the tested string.
* `ShouldContainErrorMessage` - confirms exception contains provided message.
* `ShouldBeThrownBy` - confirms exception of specific type is thrown by provided action.

## Exceptions
When testing for exceptions it is recommended that you use the `Catch` class in your `Because` statement then validate the exception in subsequent `It` statements. This ensures that no validation of the `Exception` occurs in the `Because` statement. It is also recommended to include an `It` statement that explicitly determines the type of `Exception` if that is important to you. Doing so will improve the readability of your specifications clarifying how the system is intended to behave. 

<pre>
public class when_the_user_credentials_cannot_be_verified : ExampleSpecs {
  
  static Exception Exception;

  Because of = () => 
    Exception = Catch.Exception(() => SecurityService.Authenticate("user", "pass") );

  It should_fail = () => 
    Exception.ShouldBeOfType&lt;AuthenticationFailedException&gt;();

}
</pre>

## Reuse
If you're reusing behaviors, use *shared behaviors group*. If you're reusing context setup use *inheritance*.

### Shared behaviors group
Shared behaviors group allows reuse of expected behavior between different specifications. It's purpose is to confirm that in related specifications with similar contexts some of the behavior will be similar, but some of it will be different.

* Add `[Behaviors]` attribute to the class which contains shared behaviors.
* In each specification class where you want to reuse behaviors add (appropriately named) `Behaves_like<TBehavior>` member. `TBehavior` is your shared behaviors class.

### Inheritance 
Regular inheritance makes it much easier to reuse common context set up. `Establish context` from both base class and current class will be invoked.

NOTE: Observations in the base class won't be tested (run).

## Empty specs
Creating a specification class with empty `It` delegates helps to fully capture spec and expectations before writing the actual code.

All observations will show up as "unimplemented" in reports.

<pre>
[Subject("Recent Account Activity Summary page")]
public class when_a_customer_first_views_the_account_summary_page
{
	It should_display_all_account_transactions_for_the_past_thirty_days;
	It should_display_debit_amounts_in_red_text;
	It should_display_deposit_amounts_in_black_text;
}
</pre>

## Additional Links
* [MVC extensions](https://github.com/jamesbroome/Machine.Specifications.Mvc)
* [Automocking](https://github.com/jamesbroome/Machine.Specifications.AutoMocking)