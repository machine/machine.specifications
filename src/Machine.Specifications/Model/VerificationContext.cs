using System;

namespace Machine.Specifications.Model
{
    public class VerificationContext
    {
        public object Instance { get; set; }
        public Exception ThrownException { get; set; }

        public VerificationContext(object instance)
        {
            Instance = instance;
        }
    }
}