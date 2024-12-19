namespace Machine.Specifications.Runner.VisualStudio.Reflection
{
    public class SequencePointData
    {
        public SequencePointData(string fileName, int startLine, int endLine, int offset, bool isHidden)
        {
            FileName = fileName;
            StartLine = startLine;
            EndLine = endLine;
            Offset = offset;
            IsHidden = isHidden;
        }

        public string FileName { get; }

        public int StartLine { get; }

        public int EndLine { get; }

        public int Offset { get; }

        public bool IsHidden { get; }
    }
}
