using System;
using System.Collections;
using System.Collections.Generic;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class DictionaryDataType : DataType
    {
        public DataType KeyDataType { get; }
        public DataType ValueDataType { get; }

        public DictionaryDataType(Type type, DataType keyDataType, DataType valueDataType) : base(type)
        {
            KeyDataType = keyDataType;
            ValueDataType = valueDataType;

            // If either the key or value type is unsupported, make this type NULL.
            if (keyDataType == null || valueDataType == null)
                IsUnsupported = true;

            IsDictionary = true;
        }

        public override void Write(object obj, IWriter writer)
        {
            var dict = (IDictionary)obj;
            
            var i = 0;
            foreach (DictionaryEntry kvp in dict)
            {
                writer.StartWriteDictionaryKey(i);
                writer.Write(kvp.Key, KeyDataType);
                writer.EndWriteDictionaryKey(i);
                writer.StartWriteDictionaryValue(i);
                writer.Write(kvp.Value, ValueDataType);
                writer.EndWriteDictionaryValue(i);
                i++;
            }
        }

        public override object Read<T>(IReader reader) =>
            Read(reader);
        

        public override void ReadInto<T>(IReader reader, object obj) =>
            ReadInto(reader, obj);
        

        /*
         * 	Allows us to call the generic Read method using Reflection so we can define the generic parameter at runtime.
         * 	It also caches the method to improve performance in later calls.
         */
        public object Read(IReader reader)
        {
            if (reader.StartReadDictionary())
                return null;

            var dict = new Dictionary<object, object>(); //(IDictionary)ES3Reflection.CreateInstance(Type);

            // Iterate through each character until we reach the end of the array.
            while (true)
            {
                if (!reader.StartReadDictionaryKey())
                    return dict;
                var key = reader.Read<object>(KeyDataType);
                reader.EndReadDictionaryKey();

                reader.StartReadDictionaryValue();
                var value = reader.Read<object>(ValueDataType);

                dict.Add(key, value);

                if (reader.EndReadDictionaryValue())
                    break;
            }

            reader.EndReadDictionary();

            return dict;
        }

        public void ReadInto(IReader reader, object obj)
        {
            if (reader.StartReadDictionary())
                throw new NullReferenceException(
                    "The Dictionary we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

            var dict = (IDictionary)obj;

            // Iterate through each character until we reach the end of the array.
            while (true)
            {
                if (!reader.StartReadDictionaryKey())
                    return;
                var key = reader.Read<object>(KeyDataType);

                if (!dict.Contains(key))
                    throw new KeyNotFoundException("The key \"" + key +
                                                   "\" in the Dictionary we are loading does not exist in the Dictionary we are loading into");
                var value = dict[key];
                reader.EndReadDictionaryKey();

                reader.StartReadDictionaryValue();

                reader.ReadInto<object>(value, ValueDataType);

                if (reader.EndReadDictionaryValue())
                    break;
            }

            reader.EndReadDictionary();
        }
    }
}