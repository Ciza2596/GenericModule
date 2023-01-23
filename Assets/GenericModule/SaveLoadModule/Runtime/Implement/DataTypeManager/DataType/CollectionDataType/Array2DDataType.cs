﻿using System;
using System.Collections.Generic;

namespace DataTypeManager
{
	public class Array2DDataType : CollectionDataType
	{
		public Array2DDataType(Type type) : base(type){}

		public override void Write(object obj, IWriter writer, ReferenceModes referenceModes)
		{
			var array = (System.Array)obj;

			if(DataType == null)
				throw new ArgumentNullException("ES3Type argument cannot be null.");

			//writer.StartWriteCollection();

			for(int i=0; i < array.GetLength(0); i++)
			{
				writer.StartWriteCollectionItem(i);
				writer.StartWriteCollection();
				for(int j=0; j < array.GetLength(1); j++)
				{
					writer.StartWriteCollectionItem(j);
					writer.Write(array.GetValue(i,j), DataType, referenceModes);
					writer.EndWriteCollectionItem(j);
				}
				writer.EndWriteCollection();
				writer.EndWriteCollectionItem(i);
			}

			//writer.EndWriteCollection();
		}

		public override object Read<T>(IReader reader)
		{
            return Read(reader);
			/*if(reader.StartReadCollection())
				return null;

			// Create a List to store the items as a 1D array, which we can work out the positions of by calculating the lengths of the two dimensions.
			var items = new List<T>();
			int length1 = 0;

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadCollectionItem())
					break;

				ReadICollection<T>(reader, items, elementType);
				length1++;

				if(reader.EndReadCollectionItem())
					break;
			}

			int length2 = items.Count / length1;

			var array = new T[length1,length2];

			for(int i=0; i<length1; i++)
				for(int j=0; j<length2; j++)
					array[i,j] = items[ (i * length2) + j ];

			return array;*/
		}

        public override object Read(IReader reader)
		{
			if(reader.StartReadCollection())
				return null;

			// Create a List to store the items as a 1D array, which we can work out the positions of by calculating the lengths of the two dimensions.
			var items = new List<object>();
			int length1 = 0;

			// Iterate through each character until we reach the end of the array.
			while(true)
			{
				if(!reader.StartReadCollectionItem())
					break;

				ReadICollection<object>(reader, items, DataType);
				length1++;

				if(reader.EndReadCollectionItem())
					break;
			}

			int length2 = items.Count / length1;

			var array = ReflectionHelper.ArrayCreateInstance(DataType.Type, new int[]{length1, length2});

			for(int i=0; i<length1; i++)
				for(int j=0; j<length2; j++)
					array.SetValue(items[ (i * length2) + j ], i, j);

			return array;
		}

        public override void ReadInto<T>(IReader reader, object obj)
        {
            ReadInto(reader, obj);
        }

        public override void ReadInto(IReader reader, object obj)
		{
            var array = (Array)obj;

			if(reader.StartReadCollection())
				throw new NullReferenceException("The Collection we are trying to load is stored as null, which is not allowed when using ReadInto methods.");

			bool iHasBeenRead = false;

			for(int i=0; i < array.GetLength(0); i++)
			{
				bool jHasBeenRead = false;

				if(!reader.StartReadCollectionItem())
					throw new IndexOutOfRangeException("The collection we are loading is smaller than the collection provided as a parameter.");

				reader.StartReadCollection();
				for(int j=0; j < array.GetLength(1); j++)
				{
					if(!reader.StartReadCollectionItem())
						throw new IndexOutOfRangeException("The collection we are loading is smaller than the collection provided as a parameter.");
					reader.ReadInto<object>(array.GetValue(i,j), DataType);
					jHasBeenRead = reader.EndReadCollectionItem();
				}

				if(!jHasBeenRead)
					throw new IndexOutOfRangeException("The collection we are loading is larger than the collection provided as a parameter.");

				reader.EndReadCollection();

				iHasBeenRead = reader.EndReadCollectionItem();
			}

			if(!iHasBeenRead)
					throw new IndexOutOfRangeException("The collection we are loading is larger than the collection provided as a parameter.");

			reader.EndReadCollection();
		}
	}
}