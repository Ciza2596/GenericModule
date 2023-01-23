using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace DataTypeManager
{
	[UnityEngine.Scripting.Preserve]
	public class StackDataType : CollectionDataType
	{
		public StackDataType(Type type) : base(type){}

		public override void Write(object obj, IWriter writer, ReferenceModes referenceModes)
		{
			var list = (ICollection)obj;

			if(DataType == null)
				throw new ArgumentNullException("ES3Type argument cannot be null.");

			//writer.StartWriteCollection();

			int i = 0;
			foreach(object item in list)
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
            return Read(reader);
			/*if(reader.StartReadCollection())
				return null;

			var stack = new Stack<T>();

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadCollectionItem())
					break;
				stack.Push(reader.Read<T>(elementType));
				if(reader.EndReadCollectionItem())
					break;
			}

			reader.EndReadCollection();
			return stack;*/
		}

		public override void ReadInto<T>(IReader reader, object obj)
		{
			if(reader.StartReadCollection())
				throw new NullReferenceException("The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			int itemsLoaded = 0;

			var stack = (Stack<T>)obj;

			// Iterate through each item in the collection and try to load it.
			foreach(var item in stack)
			{
				itemsLoaded++;

				if(!reader.StartReadCollectionItem())
					break;

				reader.ReadInto<T>(item, DataType);

				// If we find a ']', we reached the end of the array.
				if(reader.EndReadCollectionItem())
					break;
				// If there's still items to load, but we've reached the end of the collection we're loading into, throw an error.
				if(itemsLoaded == stack.Count)
					throw new IndexOutOfRangeException("The collection we are loading is longer than the collection provided as a parameter.");
			}

			// If we loaded fewer items than the parameter collection, throw index out of range exception.
			if(itemsLoaded != stack.Count)
				throw new IndexOutOfRangeException("The collection we are loading is shorter than the collection provided as a parameter.");

			reader.EndReadCollection();
		}

		public override object Read(IReader reader)
		{
			var instance = (IList)ReflectionHelper.CreateInstance(ReflectionHelper.MakeGenericType(typeof(List<>), DataType.Type));

			if(reader.StartReadCollection())
				return null;

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadCollectionItem())
					break;
				instance.Add(reader.Read<object>(DataType));

				if(reader.EndReadCollectionItem())
					break;
			}

			reader.EndReadCollection();

            ReflectionHelper.GetMethods(instance.GetType(), "Reverse").FirstOrDefault(t => !t.IsStatic).Invoke(instance, new object[]{});
            return ReflectionHelper.CreateInstance(Type, instance);
            
		}

		public override void ReadInto(IReader reader, object obj)
		{
			if(reader.StartReadCollection())
				throw new NullReferenceException("The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			int itemsLoaded = 0;

			var collection = (ICollection)obj;

			// Iterate through each item in the collection and try to load it.
			foreach(var item in collection)
			{
				itemsLoaded++;

				if(!reader.StartReadCollectionItem())
					break;

				reader.ReadInto<object>(item, DataType);

				// If we find a ']', we reached the end of the array.
				if(reader.EndReadCollectionItem())
					break;
				// If there's still items to load, but we've reached the end of the collection we're loading into, throw an error.
				if(itemsLoaded == collection.Count)
					throw new IndexOutOfRangeException("The collection we are loading is longer than the collection provided as a parameter.");
			}

			// If we loaded fewer items than the parameter collection, throw index out of range exception.
			if(itemsLoaded != collection.Count)
				throw new IndexOutOfRangeException("The collection we are loading is shorter than the collection provided as a parameter.");

			reader.EndReadCollection();
		}
	}
}