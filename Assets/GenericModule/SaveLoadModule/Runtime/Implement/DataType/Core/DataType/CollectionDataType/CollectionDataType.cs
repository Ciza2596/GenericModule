﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public abstract class CollectionDataType : DataType
	{
		public DataType DataType { get; private set; }

		/*protected ES3Reflection.ES3ReflectedMethod readMethod = null;
		protected ES3Reflection.ES3ReflectedMethod readIntoMethod = null;*/

        public abstract object Read(IReader reader);
        public abstract void ReadInto(IReader reader, object obj);
        public abstract void Write(object obj, IWriter writer, ReferenceModes referenceModes);

        public CollectionDataType(Type type) : base(type)
		{
			// DataType = DataTypeManager.GetOrCreateDataType(ES3Reflection.GetElementTypes(type)[0], false);
			IsCollection = true;

			// If the element type is null (i.e. unsupported), make this ES3Type null.
			// if(elementType == null)
			// 	isUnsupported = true;
		}

        public CollectionDataType(Type type, DataType dataType) : base(type)
		{
			DataType = dataType;
			IsCollection = true;
		}

        [UnityEngine.Scripting.Preserve]
        public override void Write(object obj, IWriter writer)
		{
			Write(obj, writer, ReferenceModes.ByRefAndValue);
		}

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

        protected virtual void ReadICollectionInto<T>(IReader reader, ICollection<T> collection, DataType dataType)
        {
            ReadICollectionInto(reader, collection, dataType);
        }

        [UnityEngine.Scripting.Preserve]
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

		/*
		 * 	Calls the Read method using reflection so we don't need to provide a generic parameter.
		 */
		/*public virtual object Read(ES3Reader reader)
		{
			if(readMethod == null)
				readMethod = ES3Reflection.GetMethod(this.GetType(), "Read", new Type[]{elementType.type}, new Type[]{typeof(ES3Reader)});
			return readMethod.Invoke(this, new object[]{reader});
		}

		public virtual void ReadInto(ES3Reader reader, object obj)
		{
			if(readIntoMethod == null)
				readIntoMethod = ES3Reflection.GetMethod(this.GetType(), "ReadInto", new Type[]{elementType.type}, new Type[]{typeof(ES3Reader), typeof(object)});
			readIntoMethod.Invoke(this, new object[]{reader, obj});
		}*/
	}
}