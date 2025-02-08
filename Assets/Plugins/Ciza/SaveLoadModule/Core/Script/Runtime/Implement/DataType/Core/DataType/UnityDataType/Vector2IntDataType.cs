using UnityEngine;
using UnityEngine.Scripting;

namespace DataType
{
	[Preserve]
	public class Vector2IntDataType : BaseDataType
	{
		private readonly IntDataType _intDataType;

		[Preserve]
		public Vector2IntDataType(IntDataType intDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector2Int), dataTypeController, reflectionHelper) =>
			_intDataType = intDataType;

		public override void Write(object obj, IWriter writer)
		{
			var vector2Int = (Vector2Int)obj;
			writer.WriteProperty(TagUtils.X_TAG, vector2Int.x, _intDataType);
			writer.WriteProperty(TagUtils.Y_TAG, vector2Int.y, _intDataType);
		}

		public override object Read<T>(IReader reader) =>
			new Vector2Int(reader.ReadProperty<int>(_intDataType), reader.ReadProperty<int>(_intDataType));
	}

	[Preserve]
	public class Vector2IntArrayDataType : ArrayDataType
	{
		[Preserve]
		public Vector2IntArrayDataType(Vector2IntDataType vector2IntElementDataType, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(typeof(Vector2Int[]), vector2IntElementDataType, dataTypeController, reflectionHelper) { }
	}
}
