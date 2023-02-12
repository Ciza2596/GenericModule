using System;
using System.Collections;


namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class ListDataType : CollectionDataType
    {

        public ListDataType(Type type, DataType elementDataType, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper) : base(type, elementDataType, dataTypeController, reflectionHelper)
        {
        }

        public override void Write(object obj, IWriter writer)
        {
            if (obj == null)
            {
                writer.WriteNull();
                return;
            }

            var list = (IList)obj;

            if (_elementDataType == null)
                throw new ArgumentNullException("[ListDataType::Write] DataType argument cannot be null.");

            int i = 0;
            foreach (object item in list)
            {
                writer.StartWriteCollectionItem(i);
                writer.Write(item, _elementDataType);
                writer.EndWriteCollectionItem(i);
                i++;
            }
        }

        public override object Read<T>(IReader reader) =>
            Read(reader);
        

        public override void ReadInto<T>(IReader reader, object obj)
        {
            ReadICollectionInto(reader, (ICollection)obj, _elementDataType);
        }

        public override object Read(IReader reader)
        {
            var instance = (IList)_reflectionHelper.CreateInstance(Type);

            if (reader.StartReadCollection())
                return null;

            // Iterate through each character until we reach the end of the array.
            while (true)
            {
                if (!reader.StartReadCollectionItem())
                    break;
                instance.Add(reader.Read<object>(_elementDataType));

                if (reader.EndReadCollectionItem())
                    break;
            }

            reader.EndReadCollection();

            return instance;
        }

        public override void ReadInto(IReader reader, object obj)
        {
            var collection = (IList)obj;

            if (reader.StartReadCollection())
                throw new NullReferenceException(
                    "[ListDataType::ReadInto] The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

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
                        "[ListDataType::ReadInto] The collection we are loading is longer than the collection provided as a parameter.");
            }

            // If we loaded fewer items than the parameter collection, throw index out of range exception.
            if (itemsLoaded != collection.Count)
                throw new IndexOutOfRangeException(
                    "[ListDataType::ReadInto] The collection we are loading is shorter than the collection provided as a parameter.");

            reader.EndReadCollection();
        }
    }
}