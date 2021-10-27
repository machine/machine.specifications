namespace Machine.Specifications
{
    public interface IAssemblyContext
    {
        void OnAssemblyStart();
        void OnAssemblyComplete();
    }
}
