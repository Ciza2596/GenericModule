using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	[Preserve]
	public struct DataTypeData
	{
		public DataType.BaseDataType DataType { get; }
		public byte[]            Bytes    { get; }

		[Preserve]
		public DataTypeData(DataType.BaseDataType dataType, byte[] bytes)
		{
			DataType = dataType;
			Bytes    = bytes;
		}
	}
}
