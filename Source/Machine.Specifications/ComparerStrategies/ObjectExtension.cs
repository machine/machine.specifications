namespace Machine.Specifications.ComparerStrategies
{
    public static class ObjectExtension
    {
        public static bool IsEqualToDefault<T>(this T obj)
        {
            return object.Equals(obj, default(T));
        }
    }
}