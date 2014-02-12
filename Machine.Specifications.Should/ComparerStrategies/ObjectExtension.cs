namespace Machine.Specifications.ComparerStrategies
{
    static class ObjectExtension
    {
        public static bool IsEqualToDefault<T>(this T obj)
        {
            return Equals(obj, default(T));
        }
    }
}