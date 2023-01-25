using System;
using System.Collections.Generic;
using DataType;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public abstract class BaseReader : IReader, DataType.IReader, IDisposable
    {
        //private variable
        protected readonly IDataTypeController _dataTypeController;
        
        private IEnumerable<string> _properties;
        protected int _serializationDepth = 0;
        
        
        //public variable
        public string OverridePropertiesName { get; }
        IEnumerable<string> DataType.IReader.Properties => _properties;


        public BaseReader(IDataTypeController dataTypeController) =>
            _dataTypeController = dataTypeController;
        


        //SaveLoadModule IReader
        public IReaderPropertyEnumerator Properties { get; }
        public IReaderRawEnumerator Raws { get; }
        
        public T Read<T>(string key)
        {
            Assert.IsTrue(TryGoTo(key), $"[BaseReader::Read] Cant find key: {key}");

            var type = ReadTypeFromHeader<T>();
            var dataType = _dataTypeController.GetOrCreateDataType(type);
            T obj = Read<T>(dataType);

            return obj;
        }


        //DataType IReader
        public abstract Type ReadType();

        public abstract int ReadInt();

        public abstract bool ReadBool();

        public abstract byte ReadByte();

        public abstract char ReadChar();

        public abstract decimal ReadDecimal();

        public abstract double ReadDouble();

        public abstract float ReadFloat();

        public abstract long ReadLong();

        public abstract sbyte ReadSbyte();

        public abstract short ReadShort();

        public abstract uint ReadUint();

        public abstract ulong ReadUlong();

        public abstract ushort ReadUshort();

        public abstract string ReadString();

        public abstract void Dispose();

        public void ReadInto<T>(object obj, DataType.DataType dataType)
        {
            if(dataType.IsCollection)
                ((CollectionDataType)dataType).ReadInto(this, obj);
            else if(dataType.IsDictionary)
                ((DictionaryDataType)dataType).ReadInto(this, obj);
            else
                ReadObject<T>(obj, dataType);
        }
        
        

        public abstract string ReadPropertyName();

        public T ReadProperty<T>(DataType.DataType dataType)
        {
            ReadPropertyName();
            return Read<T>(dataType);
        }

        public T Read<T>(DataType.DataType dataType)
        {
            if (dataType.IsPrimitive)
                return (T)dataType.Read<T>(this);

            if (dataType.IsCollection)
                return (T)((CollectionDataType)dataType).Read(this);
            
            if (dataType.IsCollection)
                return (T)((DictionaryDataType)dataType).Read(this);

            return ReadObject<T>(dataType);
        }


        public abstract bool StartReadCollection();
        public abstract void EndReadCollection();


        public abstract bool StartReadCollectionItem();
        public abstract bool EndReadCollectionItem();
        

        public abstract bool StartReadDictionary();
        public abstract void EndReadDictionary();
        
        
        public abstract bool StartReadDictionaryKey();
        public abstract void EndReadDictionaryKey();


        public abstract void StartReadDictionaryValue();
        public abstract bool EndReadDictionaryValue();
        

        public void Skip() => ReadElement(true);


        //protected method
        protected bool TryGoTo(string key)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(key), "[BaseReader::TryGoTo] Key cannot be NULL when loading data.");

            string currentKey;
            while ((currentKey = ReadPropertyName()) != key)
            {
                if (currentKey is null)
                    return false;
                Skip();
            }

            return true;
        }

        private void ReadObject<T>(object obj, DataType.DataType dataType)
        {
            // Check for null.
            if(StartReadObject())
                return;

            dataType.ReadInto<T>(this, obj);

            EndReadObject();
        }
        
        private T ReadObject<T>(DataType.DataType dataType)
        {
            if(StartReadObject())
                return default;

            var obj = dataType.Read<T>(this);

            EndReadObject();
            return (T)obj;
        }

        protected abstract byte[] ReadElement(bool skip = false);

        
        protected abstract Type ReadKeyPrefix();
        protected abstract void ReadKeySuffix();


        protected virtual bool StartReadObject()
        {
            _serializationDepth++;
            return false;
        }

        protected virtual void EndReadObject() =>
            _serializationDepth--;
        
        
        //private method
        private Type ReadTypeFromHeader<T>()
        {
            var type = ReadKeyPrefix();
            return type;
        }

    }
}