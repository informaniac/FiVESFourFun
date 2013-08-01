using System;

namespace FIVES
{
    public enum AttributeType {
        INT,
        FLOAT,
        STRING,
        BOOL
    }

    internal class Attribute
    {
        private Guid Id { get; set; }
        public AttributeType type { get; set; }
        public object value { get; set; }

        public Attribute()
        {
        }

        public Attribute(AttributeType type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }
}

