namespace Machine.Specifications.Sdk
{
    public class BehaviorAttributeFullName : AttributeFullName
    {
        private static readonly string AttributeFullName = typeof(BehaviorsAttribute).FullName;

        public static implicit operator string(BehaviorAttributeFullName fullName)
        {
            return AttributeFullName;
        }

        public override string FullName => this;
    }
}
