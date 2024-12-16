namespace Machine.Specifications.Runner.VisualStudio.Navigation
{
    public class NavigationData
    {
        public static NavigationData Unknown { get; } = new NavigationData(null, 0);

        public NavigationData(string codeFile, int lineNumber)
        {
            CodeFile = codeFile;
            LineNumber = lineNumber;
        }

        public string CodeFile { get; }

        public int LineNumber { get; }
    }
}
