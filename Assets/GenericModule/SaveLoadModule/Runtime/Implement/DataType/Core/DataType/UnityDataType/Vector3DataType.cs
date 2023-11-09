using UnityEngine;

namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public class Vector3DataType : BaseDataType
	{
		private readonly FloatDataType _floatDataType;

		public Vector3DataType(FloatDataType floatDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector3), dataTypeController, reflectionHelper) =>
			_floatDataType = floatDataType;

		public override void Write(object obj, IWriter writer)
		{
			var vector3 = (Vector3)obj;
			writer.WriteProperty(TagUtils.X_TAG, vector3.x, _floatDataType);
			writer.WriteProperty(TagUtils.Y_TAG, vector3.y, _floatDataType);
			writer.WriteProperty(TagUtils.Z_TAG, vector3.z, _floatDataType);
		}

		public override object Read<T>(IReader reader) =>
			new Vector3(reader.ReadProperty<float>(_floatDataType), reader.ReadProperty<float>(_floatDataType), reader.ReadProperty<float>(_floatDataType));
	}

	public class Vector3ArrayDataType : ArrayDataType
	{
		public Vector3ArrayDataType(Vector3DataType vector3DataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector3[]), vector3DataType, dataTypeController, reflectionHelper) { }
	}
}
