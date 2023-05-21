namespace Machine.Specifications.Reflection
{
    public interface ITypeComparer
    {
        void Compare(CompareContext context, Node node);
    }
}
