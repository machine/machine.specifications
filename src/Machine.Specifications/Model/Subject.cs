using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
    public class Subject
    {
        readonly string _description;
        readonly Type _type;

        public Type Type
        {
            get { return _type; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string FullConcern
        {
            get
            {
                if (_type == null)
                {
                    return _description;
                }

                if (_description == null)
                {
                    return _type.Name;
                }

                return _type.Name + " " + _description;
            }
        }

        public Subject(Type type, string description)
        {
            _type = type;
            _description = description;
        }
    }
}
