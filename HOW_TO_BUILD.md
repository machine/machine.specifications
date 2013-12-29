# How to build

These instructions are *only* for building with Rake, which includes compilation, test execution and packaging. This is the simplest way to build.

You can also build the solution using Visual Studio 2012 or later.

*Don't be put off by the prerequisites!* It only takes a few minutes to set them up and only needs to be done once. If you haven't used [Rake](http://rake.rubyforge.org/ "RAKE -- Ruby Make") before then you're in for a real treat!

At the time of writing the build is only confirmed to work on Windows using the Microsoft .NET framework.

## Prerequisites

1. Ensure you have .NET framework 3.5 and 4.0/4.5 installed.

1. Install Ruby 2.0.0 or later.

 For Windows we recommend using [RubyInstaller](http://rubyinstaller.org/) and selecting 'Add Ruby executables to your PATH' when prompted. For alternatives see the [Ruby download page](http://www.ruby-lang.org/en/downloads/). Be sure to use the 32 bit version.
 
1. Install Ruby DevKit for 2.0.0 or later.

 For Windows we recommend using [RubyInstaller](http://rubyinstaller.org/), follow the instructions for [DevKit](https://github.com/oneclick/rubyinstaller/wiki/Development-Kit) . For alternatives see the [Ruby download page](http://www.ruby-lang.org/en/downloads/). Be sure to use the 32 bit version.
 
1. Using a command prompt, install bundler:

    `gem install bundler`

1. Install/update necessary build tools, navigate to your clone root folder and execute:

    `bundle install`

## Building

Using a command prompt, navigate to your clone root folder and execute:

`rake`

or use the provided build batch files.

This executes the default build tasks. After the build has completed, the build artifacts will be located in `Build`.

## Extras

* View the full list of build tasks:

    `rake -T`

* Run a specific task:

    `rake spec`

* Run multiple tasks:

    `rake spec pack`

* View the full list of rake options:

    `rake -h`
