using System;
using System.Collections;
using System.Collections.Generic;


namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public class HashSetDataType : CollectionDataType
    {
        public HashSetDataType(Type type, DataType elementDataType, IDataTypeController dataTypeController,
            IReflectionHelper reflectionHelper) : base(type, elementDataType,
            dataTypeController, reflectionHelper)
        {
        }

        public override void Write(object obj, IWriter writer)
        {
            if (obj == null)
            {
                writer.WriteNull();
                return;
            }

            var list = (IEnumerable)obj;

            if (_elementDataType == null)
                throw new ArgumentNullException("[HashSetDataType::Write] DataType argument cannot be null.");

            var count = 0;
            foreach (var item in list)
                count++;

            var i = 0;
            foreach (object item in list)
            {
                writer.StartWriteCollectionItem(i);
                writer.Write(item, _elementDataType);
                writer.EndWriteCollectionItem(i);
                i++;
            }
        }

        public override object Read<T>(IReader reader)
        {
            var val = Read(reader);
            if (val == null)
                return default(T);
            return (T)val;
        }


        public override object Read(IReader reader)
        {
            var genericArguments = _reflectionHelper.GetGenericArguments(Type);
            var genericArgument = genericArguments[0];
            var listType = _reflectionHelper.MakeGenericType(typeof(List<>), genericArgument);
            var list = (IList)_reflectionHelper.CreateInstance(listType);

            if (!reader.StartReadCollection())
            {
                // Iterate through each character until we reach the end of the array.
                while (true)
                {
                    if (!reader.StartReadCollectionItem())
                        break;
                    var val = reader.Read<object>(_elementDataType);
                    list.Add(val);

                    if (reader.EndReadCollectionItem())
                        break;
                }

                reader.EndReadCollection();
            }

            return _reflectionHelper.CreateInstance(Type, list);
        }

        public override void ReadInto<T>(IReader reader, object obj) =>
            ReadInto(reader, obj);
        

        public override void ReadInto(IReader reader, object obj)
        {
            throw new NotImplementedException(
                "[HashSetDataType::ReadInto] Cannot use LoadInto/ReadInto with HashSet because HashSets do not maintain the order of elements");
        }
    }
}