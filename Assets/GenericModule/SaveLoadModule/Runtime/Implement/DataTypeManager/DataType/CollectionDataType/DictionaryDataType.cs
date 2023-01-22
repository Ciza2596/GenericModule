using System;
using System.Collections;
using System.Collections.Generic;

namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class DictionaryDataType : DataType
	{
		public DataType _keyDataType;
		public DataType _valueDataType;

		protected ES3Reflection.ES3ReflectedMethod readMethod = null;
		protected ES3Reflection.ES3ReflectedMethod readIntoMethod = null;

		public DictionaryDataType(Type type) : base(type)
		{
			var types = ES3Reflection.GetElementTypes(type);
			_keyDataType = DataTypeManager.GetOrCreateDataType(types[0], false);
			_valueDataType = DataTypeManager.GetOrCreateDataType(types[1], false);

			// If either the key or value type is unsupported, make this type NULL.
			if(_keyDataType == null || _valueDataType == null)
				isUnsupported = true;;

			IsDictionary = true;
		}

        public DictionaryDataType(Type type, DataType keyDataType, DataType valueDataType) : base(type)
        {
            this._keyDataType = keyDataType;
            this._valueDataType = valueDataType;

            // If either the key or value type is unsupported, make this type NULL.
            if (keyDataType == null || valueDataType == null)
                isUnsupported = true; ;

            IsDictionary = true;
        }

        public override void Write(object obj, IWriter writer)
		{
			Write(obj, writer, writer.settings.memberReferenceMode);
		}

		public void Write(object obj, IWriter writer, ES3.ReferenceMode memberReferenceMode)
		{
			var dict = (IDictionary)obj;

			//writer.StartWriteDictionary(dict.Count);

			int i=0;
			foreach(System.Collections.DictionaryEntry kvp in dict)
			{
				writer.StartWriteDictionaryKey(i);
				writer.Write(kvp.Key, _keyDataType, memberReferenceMode);
				writer.EndWriteDictionaryKey(i);
				writer.StartWriteDictionaryValue(i);
				writer.Write(kvp.Value, _valueDataType, memberReferenceMode);
				writer.EndWriteDictionaryValue(i);
				i++;
			}

			//writer.EndWriteDictionary();
		}

		public override object Read<T>(IReader reader)
		{
			return Read(reader);
		}

		public override void ReadInto<T>(IReader reader, object obj)
		{
            ReadInto(reader, obj);
		}

		/*
		 * 	Allows us to call the generic Read method using Reflection so we can define the generic parameter at runtime.
		 * 	It also caches the method to improve performance in later calls.
		 */
		public object Read(IReader reader)
		{
			if(reader.StartReadDictionary())
				return null;

			var dict = (IDictionary)ES3Reflection.CreateInstance(Type);

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadDictionaryKey())
					return dict;
				var key = reader.Read<object>(_keyDataType);
				reader.EndReadDictionaryKey();

				reader.StartReadDictionaryValue();
				var value = reader.Read<object>(_valueDataType);

				dict.Add(key,value);

				if(reader.EndReadDictionaryValue())
					break;
			}

			reader.EndReadDictionary();

			return dict;
		}

		public void ReadInto(IReader reader, object obj)
		{
			if(reader.StartReadDictionary())
				throw new NullReferenceException("The Dictionary we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			var dict = (IDictionary)obj;

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadDictionaryKey())
					return;
				var key = reader.Read<object>(_keyDataType);

				if(!dict.Contains(key))
					throw new KeyNotFoundException("The key \"" + key + "\" in the Dictionary we are loading does not exist in the Dictionary we are loading into");
				var value = dict[key];
				reader.EndReadDictionaryKey();

				reader.StartReadDictionaryValue();

				reader.ReadInto<object>(value, _valueDataType);

				if(reader.EndReadDictionaryValue())
					break;
			}

			reader.EndReadDictionary();
		}
	}
}