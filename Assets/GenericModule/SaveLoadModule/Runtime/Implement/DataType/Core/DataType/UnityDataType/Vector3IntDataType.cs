﻿using UnityEngine;
using UnityEngine.Scripting;

namespace DataType
{
	[Preserve]
	public class Vector3IntDataType : BaseDataType
	{
		private readonly IntDataType _intDataType;

		[Preserve]
		public Vector3IntDataType(IntDataType intDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector3Int), dataTypeController, reflectionHelper) =>
			_intDataType = intDataType;

		public override void Write(object obj, IWriter writer)
		{
			var vector3Int = (Vector3Int)obj;
			writer.WriteProperty(TagUtils.X_TAG, vector3Int.x, _intDataType);
			writer.WriteProperty(TagUtils.Y_TAG, vector3Int.y, _intDataType);
			writer.WriteProperty(TagUtils.Z_TAG, vector3Int.z, _intDataType);
		}

		public override object Read<T>(IReader reader) =>
			new Vector3Int(reader.ReadProperty<int>(_intDataType), reader.ReadProperty<int>(_intDataType), reader.ReadProperty<int>(_intDataType));
	}

	[Preserve]
	public class Vector3IntArrayDataType : ArrayDataType
	{
		[Preserve]
		public Vector3IntArrayDataType(Vector3IntDataType vector3IntElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector3Int[]), vector3IntElementDataType, dataTypeController, reflectionHelper) { }
	}
}
