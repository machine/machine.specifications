MSpec, like other testing frameworks, provides a robust command-line runner that can be used to execute specs in one or more assemblies and allows a number of output formats to suit your needs. The runner is provided as a [separate package](http://www.nuget.org/packages/Machine.Specifications.Runner.Console/) and can be installed with the following commands:

```bash
cmd> nuget install Machine.Specifications.Runner.Console
```

Or use the Package Manager console in Visual Studio:

```powershell
PM> Install-Package Machine.Specifications.Runner.Console
```

The runner comes in different flavors:

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
--appveyor                  Reporting for AppVeyor CI integration (also auto-detected)
--no-appveyor-autodetect    Disables AppVeyor autodetection
--html <PATH>               Outputs the HTML report to path, one-per-assembly w/ index.html (if directory, otherwise all are in one file)
--xml <PATH>                Outputs the XML report to the file referenced by the path
-h, --help                  Shows this help message
Usage: mspec.exe [options] <assemblies>
```
