using System;

namespace Machine.Specifications.Model
{
    public class VerificationContext
    {
        public VerificationContext(object instance)
        {
            Instance = instance;
        }

        public object Instance { get; }

        public Exception ThrownException { get; set; }
    }
}
