namespace Machine.Specifications.ComparerStrategies
{
    class ComparisionResult
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
        public int Result { get; private set; }
    }
}