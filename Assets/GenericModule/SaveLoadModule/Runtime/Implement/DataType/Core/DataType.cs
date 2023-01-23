using System;

namespace DataType
{
    public abstract class DataType
    {
        //public variable

        public Type Type { get; }
        public bool IsCollection { get; protected set; } = false;
        public bool IsDictionary { get; protected set; } = false;


        //public method
        public DataType(Type type) =>
            Type = type;

        public abstract void Write(object obj, IWriter writer);
        public abstract object Read<T>(IReader reader);

        public virtual void ReadInto<T>(IReader reader, object obj)
        {
            throw new NotImplementedException("Self-assigning Read is not implemented or supported on this type.");
        }
    }
}