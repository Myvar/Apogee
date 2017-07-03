using System;

namespace Apogee.Core
{
    public class UniformAttribute : Attribute
    {
        public UniformAttribute(string name)
        {
            this.Name = name;

        }

        public UniformAttribute()
        {
            
        }

        public string Name { get; set; }

    }
}