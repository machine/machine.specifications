using System.Threading.Tasks;
using Machine.Specifications.Analyzers.Naming;
using Microsoft.CodeAnalysis.Testing;
using Verify = Machine.Specifications.Analyzers.Tests.CodeFixVerifier<
    Machine.Specifications.Analyzers.Maintainability.AccessModifierShouldNotBeUsedAnalyzer,
    Machine.Specifications.Analyzers.Maintainability.AccessModifierShouldNotBeUsedCodeFixProvider>;

namespace Machine.Specifications.Analyzers.Tests.Maintainability;

public class AccessModifierShouldNotBeUsedTests
{
    public class NoErrorsInValidSource
    {
        const string source = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        static string value;

        It should_do_something = () =>
            true.ShouldBeTrue();

        class inner_specs
        {
            Establish context = () =>
                value = string.Empty;
        }
    }
}";
        It should_verify = async () =>
            await Verify.VerifyAnalyzerAsync(source);
    }

    public class RemovesClassAccessModifier()
    {
        const string source = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    public class {|#0:SpecsClass|}
    {
        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        const string fixedSource = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        static DiagnosticResult expected;

        Because of = () =>
            expected = Verify.Diagnostic(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed)
                .WithLocation(0)
                .WithArguments("SpecsClass");

        It should_verify = async () => 
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    public class RemovesFieldAccessModifier
    {
        const string source = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        private It {|#0:should_do_something|} = () =>
            true.ShouldBeTrue();
    }
}";

        const string fixedSource = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        static DiagnosticResult expected;

        Because of = () =>
            expected = Verify.Diagnostic(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed)
                .WithLocation(0)
                .WithArguments("should_do_something");

        It should_verify = async () =>
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    public class RemovesFieldAndClassAccessModifiers
    {
        const string source = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    public class {|#0:SpecsClass|}
    {
        private It {|#1:should_do_something|} = () =>
            true.ShouldBeTrue();
    }
}";

        const string fixedSource = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        static DiagnosticResult[] expected;

        Because of = () => expected =
        [
            Verify.Diagnostic(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed)
                .WithLocation(0)
                .WithArguments("SpecsClass"),

            Verify.Diagnostic(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed)
                .WithLocation(1)
                .WithArguments("should_do_something")
        ];

        It should_verify = async () =>
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    public class RemovesInnerFieldAndClassAccessModifiers
    {
        const string source = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        private static string {|#0:value|};

        It should_do_something = () =>
            true.ShouldBeTrue();

        internal class {|#1:InnerClass|}
        {
            private Establish {|#2:context|} = () =>
                value = string.Empty;
        }
    }
}";

        const string fixedSource = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        static string value;

        It should_do_something = () =>
            true.ShouldBeTrue();

        class InnerClass
        {
            Establish context = () =>
                value = string.Empty;
        }
    }
}";

        static DiagnosticResult[] expected;

        Because of = () => expected =
        [
            Verify.Diagnostic(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed)
                .WithLocation(0)
                .WithArguments("value"),

            Verify.Diagnostic(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed)
                .WithLocation(1)
                .WithArguments("InnerClass"),

            Verify.Diagnostic(DiagnosticIds.Maintainability.AccessModifierShouldNotBeUsed)
                .WithLocation(2)
                .WithArguments("context")
        ];

        It should_verify = async () => 
            await Verify.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    public class RemovesFieldAccessModifierWithLeadingTrivia
    {
        const string source = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        static private string [|value|];

        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        const string fixedSource = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        static string value;

        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        It should_verify = async () => 
            await Verify.VerifyCodeFixAsync(source, fixedSource);
    }

    public class RemovesMultipleAccessModifiers
    {
        const string source = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        private protected static string [|value|];

        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        const string fixedSource = @"
using System;
using Machine.Specifications;

namespace ConsoleApplication1
{
    class SpecsClass
    {
        static string value;

        It should_do_something = () =>
            true.ShouldBeTrue();
    }
}";

        It should_verify = async () =>
            await Verify.VerifyCodeFixAsync(source, fixedSource);
    }
}
