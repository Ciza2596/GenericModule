using System;
using System.Collections;
using System.Collections.Generic;

namespace DataType
{
	public abstract class CollectionDataType : DataType
	{
		protected readonly DataType _elementDataType;
		
		protected CollectionDataType(Type type, DataType elementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(type, dataTypeController, reflectionHelper)
		{
			_elementDataType = elementDataType;
			IsCollection = true;
 
			// If the element type is null (i.e. unsupported), make this ES3Type null.
			if(_elementDataType == null)
				IsUnsupported = true;
		}
		
		public abstract object Read(IReader reader);
        public abstract void ReadInto(IReader reader, object obj);
        public abstract override void Write(object obj, IWriter writer);

        protected virtual bool ReadICollection<T>(IReader reader, ICollection<T> collection, DataType elementType)
		{
			if(reader.StartReadCollection())
				return false;

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadCollectionItem())
					break;
				collection.Add(reader.Read<T>(elementType));

				if(reader.EndReadCollectionItem())
					break;
			}

			reader.EndReadCollection();

			return true;
		}

        protected virtual void ReadICollectionInto(IReader reader, ICollection collection, DataType dataType)
		{
			if(reader.StartReadCollection())
				throw new NullReferenceException("The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			int itemsLoaded = 0;

			// Iterate through each item in the collection and try to load it.
			foreach(var item in collection)
			{
				itemsLoaded++;

				if(!reader.StartReadCollectionItem())
					break;

				reader.ReadInto<object>(item, dataType);

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