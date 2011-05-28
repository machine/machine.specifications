namespace Machine.Specifications
{
    public class ComparisionResult
    {
        public bool FoundResult { get; set; }
        public int Result { get; set; }

        public ComparisionResult(int result)
        {
            this.FoundResult = true;
            this.Result = result;
        }

        protected ComparisionResult()
        {
        }
    }
}