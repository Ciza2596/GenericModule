using UnityEngine;
using UnityEngine.Scripting;

namespace DataType
{
	[Preserve]
	public class Vector2DataType : BaseDataType
	{
		private readonly FloatDataType _floatDataType;

		[Preserve]
		public Vector2DataType(FloatDataType floatDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector2), dataTypeController, reflectionHelper) =>
			_floatDataType = floatDataType;

		public override void Write(object obj, IWriter writer)
		{
			var vector2 = (Vector2)obj;
			writer.WriteProperty(TagUtils.X_TAG, vector2.x, _floatDataType);
			writer.WriteProperty(TagUtils.Y_TAG, vector2.y, _floatDataType);
		}

		public override object Read<T>(IReader reader) =>
			new Vector2(reader.ReadProperty<float>(_floatDataType), reader.ReadProperty<float>(_floatDataType));
	}

	[Preserve]
	public class Vector2ArrayDataType : ArrayDataType
	{
		[Preserve]
		public Vector2ArrayDataType(Vector2DataType vector2DataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector2[]), vector2DataType, dataTypeController, reflectionHelper) { }
	}
}
