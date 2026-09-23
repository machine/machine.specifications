# Machine.Specificaitons Agent Guide
**Machine.Specifications** is also known colloquially as **MSpec**.

## Project architecture
All code for MSpec lives in `src` or `tests`. Documentation is in `docs`, and logos for NuGet are in `assets`.

The below projects are in use by this project. You should **ignore** any other project that is not in this list.

- `Machine.Specifications` - Meta NuGet package that references the default set of dependencies
- `Machine.Specifications.Analyzers` - Code hints and fixes for writing tests "the MSpec way"
- `Machine.Specifications.Core` - Core testing engine and framwork
- `Machine.Specifications.Core.SourceGenerator` - Source generators to aid in test discovery and speed up execution
- `Machine.Specifications.Fakes` - Interface mocking library used by MSpec
- `Machine.Specifications.Fakes.Analyzers` - Code warnings and fixes for writing mocks using MSpec
- `Machine.Specifications.Fakes.SourceGenerator` - Source generator that creates code for mocks in MSpec
- `Machine.Specifications.Should` - Assertion library for MSpec
- `Machine.Specifications.Templates` - Dotnet 'new' templates for creating test projects using MSpec

All test projects end in `.Specs`, and match the libraries above.

**Important:** Any projects named with the words `Fixtures` or `Runner` should be ignored as these are legacy and will be removed.
Only use the projects referenced by the `.slnx` file in the repo root.

## Dev environment tips
- Use `dotnet build` to build the solution
- Use `dotnet test` to build and run all the tests in the solution

## Making changes
- Make the smallest, most surgical change you can make for the feature you are writing
- Tests should be focused and not verbose
- Follow the naming and styling conventions of this repository
- All changes, in general, should have accompanying tests
- Any API surface changes should have documentation added to `docs`

## PR instructions
- Title format: Use <Title> with a brief description
- Always run `dotnet test` before committing.
