using System;
using System.Collections.Generic;
using DataType;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	[Preserve]
	public abstract class BaseReader : IReader, DataType.IReader
	{
		//private variable
		[Preserve]
		protected readonly IDataTypeController _dataTypeController;

		[Preserve]
		protected int _serializationDepth = 0;

		//public variable

		[Preserve]
		public IEnumerable<string> PropertyNames => new ReaderPropertyNameEnumerator(this);

		[Preserve]
		public string OverridePropertyName { get; private set; }

		//constructor
		[Preserve]
		protected BaseReader(IDataTypeController dataTypeController) =>
			_dataTypeController = dataTypeController;

		//SaveLoadModule IReader
		[Preserve]
		public T Read<T>(string key)
		{
			if (!TryGoTo(key))
				Debug.LogError($"[BaseReader::Read] Cant find key: {key}");
			else
				Debug.Log($"[BaseReader::Read] Key: {key} is found.");

			var type     = ReadTypeFromHeader<T>();
			var dataType = _dataTypeController.GetOrCreateDataType(type);
			T   obj      = Read<T>(dataType);

			return obj;
		}

		[Preserve]
		public abstract void Dispose();

		//DataType IReader
		[Preserve]
		public abstract Type ReadType();

		[Preserve]
		public abstract int ReadInt();

		[Preserve]
		public abstract bool ReadBool();

		[Preserve]
		public abstract byte ReadByte();

		[Preserve]
		public abstract char ReadChar();

		[Preserve]
		public abstract decimal ReadDecimal();

		[Preserve]
		public abstract double ReadDouble();

		[Preserve]
		public abstract float ReadFloat();

		[Preserve]
		public abstract long ReadLong();

		[Preserve]
		public abstract sbyte ReadSbyte();

		[Preserve]
		public abstract short ReadShort();

		[Preserve]
		public abstract uint ReadUint();

		[Preserve]
		public abstract ulong ReadUlong();

		[Preserve]
		public abstract ushort ReadUshort();

		[Preserve]
		public abstract string ReadString();

		[Preserve]
		public void ReadInto<T>(object obj, BaseDataType dataType)
		{
			if (dataType.IsCollection)
				((CollectionDataType)dataType).ReadInto(this, obj);

			else if (dataType.IsDictionary)
				((DictionaryDataType)dataType).ReadInto(this, obj);

			else
				ReadObject<T>(obj, dataType);
		}

		[Preserve]
		public abstract string ReadPropertyName();

		[Preserve]
		public T ReadProperty<T>(BaseDataType dataType)
		{
			ReadPropertyName();
			return Read<T>(dataType);
		}

		[Preserve]
		public T Read<T>(BaseDataType dataType)
		{
			if (dataType.IsPrimitive)
				return (T)dataType.Read<T>(this);

			if (dataType.IsCollection)
				return (T)((CollectionDataType)dataType).Read(this);

			if (dataType.IsDictionary)
				return (T)((DictionaryDataType)dataType).Read(this);

			return ReadObject<T>(dataType);
		}

		[Preserve]
		public abstract bool StartReadCollection();

		[Preserve]
		public abstract void EndReadCollection();

		[Preserve]
		public abstract bool StartReadCollectionItem();

		[Preserve]
		public abstract bool EndReadCollectionItem();

		[Preserve]
		public abstract bool StartReadDictionary();

		[Preserve]
		public abstract void EndReadDictionary();

		[Preserve]
		public abstract bool StartReadDictionaryKey();

		[Preserve]
		public abstract void EndReadDictionaryKey();

		[Preserve]
		public abstract void StartReadDictionaryValue();

		[Preserve]
		public abstract bool EndReadDictionaryValue();

		[Preserve]
		public void SetOverridePropertyName(string overridePropertyName) =>
			OverridePropertyName = overridePropertyName;

		[Preserve]
		public void Skip() => ReadElement(true);

		//protected method
		[Preserve]
		protected abstract byte[] ReadElement(bool skip = false);

		[Preserve]
		protected abstract Type ReadKeyPrefix();

		[Preserve]
		protected abstract void ReadKeySuffix();

		[Preserve]
		protected virtual bool StartReadObject()
		{
			_serializationDepth++;
			return false;
		}

		[Preserve]
		protected virtual void EndReadObject() =>
			_serializationDepth--;

		//private method
		[Preserve]
		private bool TryGoTo(string key)
		{
			Assert.IsTrue(key != null, "[BaseReader::TryGoTo] Key cannot be NULL when loading data.");

			string currentKey;
			while ((currentKey = ReadPropertyName()) != key)
			{
				if (currentKey is null)
					return false;

				Skip();
			}

			return true;
		}

		[Preserve]
		private void ReadObject<T>(object obj, BaseDataType dataType)
		{
			// Check for null.
			if (StartReadObject())
				return;

			dataType.ReadInto<T>(this, obj);

			EndReadObject();
		}

		[Preserve]
		private T ReadObject<T>(BaseDataType dataType)
		{
			if (StartReadObject())
				return default;

			var obj = dataType.Read<T>(this);

			EndReadObject();
			return (T)obj;
		}

		[Preserve]
		private Type ReadTypeFromHeader<T>()
		{
			var type = ReadKeyPrefix();
			return type;
		}
	}
}
