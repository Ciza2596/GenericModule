using System;
using System.Collections;
using System.Collections.Generic;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class QueueDataType : CollectionDataType
    {
        private readonly IReflectionHelper _reflectionHelper;

        public QueueDataType(Type type, DataType elementDataType, IReflectionHelper reflectionHelper) : base(type, elementDataType) =>
            _reflectionHelper = reflectionHelper;

        public override void Write(object obj, IWriter writer)
        {
            var list = (ICollection)obj;

            if (ElementDataType == null)
                throw new ArgumentNullException("ES3Type argument cannot be null.");

            int i = 0;
            foreach (object item in list)
            {
                writer.StartWriteCollectionItem(i);
                writer.Write(item, ElementDataType);
                writer.EndWriteCollectionItem(i);
                i++;
            }
        }

        public override object Read<T>(IReader reader)
        {
            return Read(reader);
            /*if(reader.StartReadCollection())
                return null;

            var queue = new Queue<T>();

            // Iterate through each character until we reach the end of the array.
            while(true)
            {
                if(!reader.StartReadCollectionItem())
                    break;
                queue.Enqueue(reader.Read<T>(elementType));
                if(reader.EndReadCollectionItem())
                    break;
            }

            reader.EndReadCollection();
            return queue;*/
        }

        public override void ReadInto<T>(IReader reader, object obj)
        {
            if (reader.StartReadCollection())
                throw new NullReferenceException(
                    "The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

            int itemsLoaded = 0;

            var queue = (Queue<T>)obj;

            // Iterate through each item in the collection and try to load it.
            foreach (var item in queue)
            {
                itemsLoaded++;

                if (!reader.StartReadCollectionItem())
                    break;

                reader.ReadInto<T>(item, ElementDataType);

                // If we find a ']', we reached the end of the array.
                if (reader.EndReadCollectionItem())
                    break;
                // If there's still items to load, but we've reached the end of the collection we're loading into, throw an error.
                if (itemsLoaded == queue.Count)
                    throw new IndexOutOfRangeException(
                        "The collection we are loading is longer than the collection provided as a parameter.");
            }

            // If we loaded fewer items than the parameter collection, throw index out of range exception.
            if (itemsLoaded != queue.Count)
                throw new IndexOutOfRangeException(
                    "The collection we are loading is shorter than the collection provided as a parameter.");

            reader.EndReadCollection();
        }

        public override object Read(IReader reader)
        {
            var genericType = _reflectionHelper.MakeGenericType(typeof(List<>), ElementDataType.Type);
            var instance = (IList)_reflectionHelper.CreateInstance(genericType);

            if (reader.StartReadCollection())
                return null;

            // Iterate through each character until we reach the end of the array.
            while (true)
            {
                if (!reader.StartReadCollectionItem())
                    break;
                instance.Add(reader.Read<object>(ElementDataType));

                if (reader.EndReadCollectionItem())
                    break;
            }

            reader.EndReadCollection();

            return _reflectionHelper.CreateInstance(Type, instance);
        }

        public override void ReadInto(IReader reader, object obj)
        {
            if (reader.StartReadCollection())
                throw new NullReferenceException(
                    "The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

            int itemsLoaded = 0;

            var collection = (ICollection)obj;

            // Iterate through each item in the collection and try to load it.
            foreach (var item in collection)
            {
                itemsLoaded++;

                if (!reader.StartReadCollectionItem())
                    break;

                reader.ReadInto<object>(item, ElementDataType);

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