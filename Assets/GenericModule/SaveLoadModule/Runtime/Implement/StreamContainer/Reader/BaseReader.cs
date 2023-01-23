using System;
using System.IO;
using UnityEngine.Assertions;

namespace SaveLoadModule.Implement
{
    public abstract class BaseReader : IReader
    {
        //public method
        public BaseReader(Stream stream, IDataTypeController dataTypeController)
        {
        }

        public T Read<T>(string key)
        {
            Assert.IsTrue(TryGoTo(key), $"[BaseReader::Read] Cant find key: {key}");

            var type = ReadType<T>();
            //T obj = Re

            return default;
        }




        public abstract string ReadPropertyName();
        public void Skip() => ReadElement(true);


        //protected method
        protected bool TryGoTo(string key)
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(key), "[BaseReader::TryGoTo] Key cannot be NULL when loading data.");

            var currentKey = string.Empty;
            while ((currentKey = ReadPropertyName()) != key)
            {
                if (currentKey is null)
                    return false;
                Skip();
            }

            return true;
        }

        protected abstract Type ReadType<T>();

        protected abstract byte[] ReadElement(bool skip = false);

    }
}