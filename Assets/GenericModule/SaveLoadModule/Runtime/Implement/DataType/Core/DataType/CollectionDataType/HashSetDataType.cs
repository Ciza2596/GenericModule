using System;
using System.Collections;
using System.Collections.Generic;


namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class HashSetDataType : CollectionDataType
	{
		public HashSetDataType(Type type) : base(type){}

        public override void Write(object obj, IWriter writer, ReferenceModes referenceModes)
        {
            if (obj == null) { writer.WriteNull(); return; };

            var list = (IEnumerable)obj;

            if (DataType == null)
                throw new ArgumentNullException("ES3Type argument cannot be null.");

            int count = 0;
            foreach (var item in list)
                count++;

            //writer.StartWriteCollection(count);

            int i = 0;
            foreach (object item in list)
            {
                writer.StartWriteCollectionItem(i);
                writer.Write(item, DataType, referenceModes);
                writer.EndWriteCollectionItem(i);
                i++;
            }

            //writer.EndWriteCollection();
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
            /*var method = typeof(ES3CollectionType).GetMethod("ReadICollection", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(elementType.type);
            if(!(bool)method.Invoke(this, new object[] { reader, list, elementType }))
                return null;*/

            // var genericParam = ES3Reflection.GetGenericArguments(Type)[0];
            // var listType = ES3Reflection.MakeGenericType(typeof(List<>), genericParam);
            // var list = (IList)ES3Reflection.CreateInstance(listType);
            //
            // if (!reader.StartReadCollection())
            // {
            //     // Iterate through each character until we reach the end of the array.
            //     while (true)
            //     {
            //         if (!reader.StartReadCollectionItem())
            //             break;
            //         list.Add(reader.Read<object>(DataType));
            //
            //         if (reader.EndReadCollectionItem())
            //             break;
            //     }
            //
            //     reader.EndReadCollection();
            // }
            //
            // return ES3Reflection.CreateInstance(Type, list);

            return null;
        }

        public override void ReadInto<T>(IReader reader, object obj)
        {
            ReadInto(reader, obj);
        }

        public override void ReadInto(IReader reader, object obj)
		{
            throw new NotImplementedException("Cannot use LoadInto/ReadInto with HashSet because HashSets do not maintain the order of elements");
		}
    }
}