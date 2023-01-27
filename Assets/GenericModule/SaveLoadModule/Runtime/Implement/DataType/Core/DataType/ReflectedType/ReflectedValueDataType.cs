using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ES3Internal;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    internal class ReflectedValueDataType : DataType
    {
        public ReflectedValueDataType(Type type, IReflectionHelper reflectionHelper) : base(type, reflectionHelper)
        {
            GetProperties(true);
        }

        public override void Write(object obj, IWriter writer)
        {
            WriteProperties(obj, writer);
        }

        public override object Read<T>(IReader reader)
        {
            var obj = _reflectionHelper.CreateInstance(Type);

            if (obj == null)
                throw new NotSupportedException("Cannot create an instance of " + Type +
                                                ". However, you may be able to add support for it using a custom ES3Type file. For more information see: http://docs.moodkie.com/easy-save-3/es3-guides/controlling-serialization-using-es3types/");
            // Make sure we return the result of ReadProperties as properties aren't assigned by reference.
            return ReadProperties(reader, obj);
        }

        public override void ReadInto<T>(IReader reader, object obj)
        {
            throw new NotSupportedException("Cannot perform self-assigning load on a value type.");
        }
    }
}