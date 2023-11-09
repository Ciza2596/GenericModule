using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace DataType
{
    public class DictionaryDataType : BaseDataType
    {
        private readonly BaseDataType _keyDataType;
        private readonly BaseDataType _valueDataType;

        [Preserve]
        public DictionaryDataType(Type type, BaseDataType keyDataType, BaseDataType valueDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(type, dataTypeController, reflectionHelper)
        {
            _keyDataType = keyDataType;
            _valueDataType = valueDataType;

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
                writer.Write(kvp.Key, _keyDataType);
                writer.EndWriteDictionaryKey(i);
                writer.StartWriteDictionaryValue(i);
                writer.Write(kvp.Value, _valueDataType);
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

            var dict = (IDictionary)_reflectionHelper.CreateInstance(Type);

            // Iterate through each character until we reach the end of the array.
            while (true)
            {
                if (!reader.StartReadDictionaryKey())
                    return dict;
                var key = reader.Read<object>(_keyDataType);
                reader.EndReadDictionaryKey();

                reader.StartReadDictionaryValue();
                var value = reader.Read<object>(_valueDataType);

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
                    "[DictionaryDataType::ReadInto] The Dictionary we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

            var dict = (IDictionary)obj;

            // Iterate through each character until we reach the end of the array.
            while (true)
            {
                if (!reader.StartReadDictionaryKey())
                    return;
                var key = reader.Read<object>(_keyDataType);

                if (!dict.Contains(key))
                    throw new KeyNotFoundException($"[DictionaryDataType::ReadInto] The key \"{key}\" in the Dictionary we are loading does not exist in the Dictionary we are loading into.");
                var value = dict[key];
                reader.EndReadDictionaryKey();

                reader.StartReadDictionaryValue();

                reader.ReadInto<object>(value, _valueDataType);

                if (reader.EndReadDictionaryValue())
                    break;
            }

            reader.EndReadDictionary();
        }
    }
}