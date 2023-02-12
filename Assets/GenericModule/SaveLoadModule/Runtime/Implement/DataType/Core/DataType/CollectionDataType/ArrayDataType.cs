using System;
using System.Collections;
using System.Collections.Generic;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class ArrayDataType : CollectionDataType
    {

        public ArrayDataType(Type type, DataType elementDataType, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper) : base(type, elementDataType, dataTypeController, reflectionHelper)
        {
        }
        
        
        public override void Write(object obj, IWriter writer)
        {
            var array = (Array)obj;

            if (_elementDataType == null)
                throw new ArgumentNullException("ES3Type argument cannot be null.");

            for (int i = 0; i < array.Length; i++)
            {
                writer.StartWriteCollectionItem(i);
                writer.Write(array.GetValue(i), _elementDataType);
                writer.EndWriteCollectionItem(i);
            }
        }

        public override object Read(IReader reader)
        {
            var list = new List<object>();
            if (!ReadICollection(reader, list, _elementDataType))
                return null;

            var array = _reflectionHelper.CreateArrayInstance(_elementDataType.Type, list.Count);
            int i = 0;
            foreach (var item in list)
            {
                array.SetValue(item, i);
                i++;
            }

            return array;
        }

        public override object Read<T>(IReader reader)
        {
            return Read(reader);
        }

        public override void ReadInto<T>(IReader reader, object obj)
        {
            ReadICollectionInto(reader, (ICollection)obj, _elementDataType);
        }

        public override void ReadInto(IReader reader, object obj)
        {
            var collection = (IList)obj;

            // if (collection.Count == 0)
            //     ES3Debug.LogWarning("LoadInto/ReadInto expects a collection containing instances to load data in to, but the collection is empty.");

            if (reader.StartReadCollection())
                throw new NullReferenceException(
                    "The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

            int itemsLoaded = 0;

            // Iterate through each item in the collection and try to load it.
            foreach (var item in collection)
            {
                itemsLoaded++;

                if (!reader.StartReadCollectionItem())
                    break;

                reader.ReadInto<object>(item, _elementDataType);

                // If we find a ']', we reached the end of the array.
                if (reader.EndReadCollectionItem())
                    break;

                // If there's still items to load, but we've reached the end of the collection we're loading into, throw an error.
                if (itemsLoaded == collection.Count)
                    throw new IndexOutOfRangeException(
                        "The collection we are loading is longer than the collection provided as a parameter.");
            }

            // If we loaded fewer items than the parameter collection, throw index out of range exception.
            if (itemsLoaded != collection.Count)
                throw new IndexOutOfRangeException(
                    "The collection we are loading is shorter than the collection provided as a parameter.");

            reader.EndReadCollection();
        }
    }
}