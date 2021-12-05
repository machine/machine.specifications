namespace Machine.Specifications.Comparers
{
    internal class ComparisionResult
    {
        public ComparisionResult(int result)
        {
            FoundResult = true;
            Result = result;
        }

        protected ComparisionResult()
        {
        }

        public bool FoundResult { get; protected set; }

        public int Result { get; }
    }
}
