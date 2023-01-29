using System;
using System.Collections.Generic;
using DataType;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public abstract class BaseWriter : IWriter, DataType.IWriter, IDisposable
    {
        private const string VALUE_TAG = "value";


        private readonly DataType.ReferenceModes _referenceMode;
        private readonly IDataTypeController _dataTypeController;
        private readonly IReflectionHelper _reflectionHelper;

        private readonly HashSet<string> _keysToDelete = new HashSet<string>();
        protected int _serializationDepth;


        //public method
        public BaseWriter(ReferenceModes referenceMode, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper)
        {
            _referenceMode = ((DataType.ReferenceModes)(int)referenceMode);

            _dataTypeController = dataTypeController;
            _reflectionHelper = reflectionHelper;
        }


        //SaveLoadModule IWriter callback
        public void Write<T>(string key, object value)
        {
            var type = typeof(T);
            var currentType = type == typeof(object) ? value.GetType() : type;

            Write(currentType, key, value);
        }

        public void Save(IReader reader)
        {
            Merge(reader);
            EndWriteFile();
            
        }


        //DataType IWriter callback
        public bool IsSafeReflection => true;

        public void WriteType(Type type)
        {
            var typeString = _reflectionHelper.GetTypeString(type);
            WriteProperty(DataType.DataType.TYPE_FIELD_NAME, typeString);
        }

        public void WriteProperty(string name, object value, DataType.DataType dataType)
        {
            WriteProperty(name, value, dataType, _referenceMode);
        }

        public abstract void WritePrimitive(string value);

        public abstract void WritePrimitive(int value);

        public abstract void WritePrimitive(bool value);

        public abstract void WritePrimitive(byte value);


        public abstract void WritePrimitive(char value);

        public abstract void WritePrimitive(decimal value);

        public abstract void WritePrimitive(double value);

        public abstract void WritePrimitive(float value);

        public abstract void WritePrimitive(long value);

        public abstract void WritePrimitive(sbyte value);

        public abstract void WritePrimitive(short value);

        public abstract void WritePrimitive(uint value);

        public abstract void WritePrimitive(ulong value);

        public abstract void WritePrimitive(ushort value);

        public virtual void StartWriteCollection() =>
            _serializationDepth++;

        public virtual void EndWriteCollection() =>
            _serializationDepth--;
        
        public abstract void StartWriteCollectionItem(int index);

        public abstract void EndWriteCollectionItem(int index);


        public void Write(object value, DataType.DataType dataType)
        {
            Write(value, dataType, _referenceMode);
        }

        public abstract void StartWriteDictionaryKey(int index);

        public abstract void EndWriteDictionaryKey(int index);

        public abstract void StartWriteDictionaryValue(int index);

        public abstract void EndWriteDictionaryValue(int index);


        public abstract void WriteNull();

        public abstract void Dispose();

        //For child class method
        protected virtual void StartWriteProperty(string name) =>
            Assert.IsTrue(!string.IsNullOrWhiteSpace(name), "[BaseWriter::StartWriteProperty] Name is null.");

        protected abstract void EndWriteProperty(string name);

        protected virtual void StartWriteFile() =>
            _serializationDepth++;
        

        protected virtual void EndWriteFile() =>
            _serializationDepth--;
        

        protected virtual void StartWriteObject(string name) =>
            _serializationDepth++;

        protected virtual void EndWriteObject(string name) =>
            _serializationDepth--;


        protected abstract void StartWriteDictionary();
        protected abstract void EndWriteDictionary();

        
        protected abstract void WriteRawProperty(string name, byte[] value);


        //protected method
        protected void Write(object value,
            DataType.ReferenceModes memberReferenceMode = DataType.ReferenceModes.ByRef)
        {
            var type = _dataTypeController.GetOrCreateDataType(value.GetType());
            Write(value, type, memberReferenceMode);
        }

        //private method
        private void Write(Type type, string key, object value)
        {
            StartWriteProperty(key);
            StartWriteObject(key);
            WriteType(type);
            var dataType = _dataTypeController.GetOrCreateDataType(type);
            WriteProperty(VALUE_TAG, value, dataType, _referenceMode);
            EndWriteObject(key);
            EndWriteProperty(key);
            MarkKeyForDeletion(key);
        }

        private void Write(string key, Type type, byte[] value)
        {
            StartWriteProperty(key);
            StartWriteObject(key);
            WriteType(type);
            WriteRawProperty(VALUE_TAG, value);
            EndWriteObject(key);
            EndWriteProperty(key);
            MarkKeyForDeletion(key);
        }

        private void Write(object value, DataType.DataType dataType, DataType.ReferenceModes referenceMode)
        {
            // Note that we have to check UnityEngine.Object types for null by casting it first, otherwise
            // it will always return false.
            if (value == null)
            {
                WriteNull();
                return;
            }

            // Deal with System.Objects
            if (dataType == null || dataType.Type == typeof(object))
            {
                var valueType = value.GetType();
                dataType = _dataTypeController.GetOrCreateDataType(valueType);


                Assert.IsNotNull(dataType, $"[BaseWriter::Write] Types of {valueType} are not supported.");

                if (!dataType.IsCollection && !dataType.IsDictionary)
                {
                    StartWriteObject(null);
                    WriteType(valueType);

                    dataType.Write(value, this);

                    EndWriteObject(null);
                    return;
                }
            }

            Assert.IsNotNull(dataType, $"[BaseWriter::Write] DataType argument cannot be null");


            if (dataType.IsCollection)
            {
                StartWriteCollection();
                ((CollectionDataType)dataType).Write(value, this);
                EndWriteCollection();
                return;
            }

            if (dataType.IsDictionary)
            {
                StartWriteDictionary();
                ((DictionaryDataType)dataType).Write(value, this);
                EndWriteDictionary();
                return;
            }

            dataType.Write(value, this);
        }

        private void MarkKeyForDeletion(string key) =>
            _keysToDelete.Add(key);


        private void WriteProperty(string name, object value, DataType.DataType dataType,
            DataType.ReferenceModes referenceMode)
        {
            StartWriteProperty(name);
            Write(value, dataType, referenceMode);
            EndWriteProperty(name);
        }

        private void WriteProperty(string name, object value) =>
            WriteProperty(name, value, _referenceMode);


        private void WriteProperty(string name, object value, DataType.ReferenceModes referenceMode)
        {
            StartWriteProperty(name);
            Write(value, referenceMode);
            EndWriteProperty(name);
        }

        private void Merge(IReader reader)
        {
            var raws = reader.Raws;

            foreach (KeyValuePair<string, DataTypeData> raw in raws)
            {
                var key = raw.Key;
                var value = raw.Value;
                if (!_keysToDelete.Contains(key) && value.DataType != null)
                    Write(key, value.DataType.Type, value.Bytes);
            }
        }
        
    }
}