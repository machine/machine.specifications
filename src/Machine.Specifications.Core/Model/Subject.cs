using System;

namespace Machine.Specifications.Model
{
    public class Subject
    {
        public Subject(Type type, string description)
        {
            Type = type;
            Description = description;
        }

        public Type Type { get; }

        public string Description { get; }

        public string FullConcern
        {
            get
            {
                if (Type == null)
                {
                    return Description;
                }

                if (Description == null)
                {
                    return Type.Name;
                }

                return Type.Name + " " + Description;
            }
        }
    }
}
