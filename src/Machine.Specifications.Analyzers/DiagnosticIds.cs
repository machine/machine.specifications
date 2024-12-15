namespace Machine.Specifications.Analyzers;

public static class DiagnosticIds
{
    public static class Naming
    {
        public const string ClassMustBeUpper = "MSP1001";
    }

    public static class Maintainability
    {
        public const string AccessModifierShouldNotBeUsed = "MSP2001";

        public const string PrivateDelegateFieldWarning = "MSP2002";
    }
}
