using System;
using System.Collections.Generic;
using System.IO;
using DataType;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public abstract class BaseWriter : IWriter, DataType.IWriter
    {
        private const string VALUE_TAG = "value";


        private readonly DataType.ReferenceModes _referenceMode;
        private readonly IDataTypeController _dataTypeController;
        private readonly IReflectionHelper _reflectionHelper;
        
        protected HashSet<string> _keysToDelete = new HashSet<string>();
        protected int _serializationDepth;


        //public method
        public BaseWriter(ReferenceModes referenceMode, Stream stream, IDataTypeController dataTypeController,
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

        public void Save()
        {
        }


        //DataType IWriter callback
        public void WriteType(Type type)
        {
            var typeString = _reflectionHelper.GetTypeString(type);
            WriteProperty(DataType.DataType.TYPE_FIELD_NAME, typeString);
        }

        public void WriteProperty(string name, object value, DataType.DataType dataType)
        {
            throw new NotImplementedException();
        }

        public void WriteProperty(string name, int value, DataType.DataType dataType)
        {
            throw new NotImplementedException();
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

        public void StartWriteCollectionItem(int index)
        {
            throw new NotImplementedException();
        }

        public void StartWriteCollection()
        {
            throw new NotImplementedException();
        }

        public void EndWriteCollectionItem(int index)
        {
            throw new NotImplementedException();
        }

        public void EndWriteCollection()
        {
            throw new NotImplementedException();
        }

        public void Write(object value, DataType.DataType dataType)
        {
            Write(value, dataType, _referenceMode);
        }

        public void StartWriteDictionaryKey(int i)
        {
            throw new NotImplementedException();
        }

        public void EndWriteDictionaryKey(int i)
        {
            throw new NotImplementedException();
        }

        public void StartWriteDictionaryValue(int i)
        {
            throw new NotImplementedException();
        }

        public void EndWriteDictionaryValue(int i)
        {
            throw new NotImplementedException();
        }

        public abstract void WriteNull();


        //For child class method
        protected virtual void StartWriteProperty(string name) =>
            Assert.IsTrue(!string.IsNullOrWhiteSpace(name), "[BaseWriter::StartWriteProperty] Name is null.");

        protected abstract void EndWriteProperty(string name);


        protected virtual void StartWriteObject(string name) =>
            _serializationDepth++;

        protected virtual void EndWriteObject(string name) =>
            _serializationDepth--;


        protected abstract void StartWriteDictionary();
        protected abstract void EndWriteDictionary();

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

        public void WriteProperty(string name, object value) =>
            WriteProperty(name, value, _referenceMode);


        private void WriteProperty(string name, object value, DataType.ReferenceModes referenceMode)
        {
            StartWriteProperty(name);
            Write(value, referenceMode);
            EndWriteProperty(name);
        }
    }
}