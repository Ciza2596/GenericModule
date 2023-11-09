namespace CizaSaveLoadModule.Implement
{
	public struct DataTypeData
	{
		public DataType.BaseDataType DataType { get; }
		public byte[]            Bytes    { get; }

		public DataTypeData(DataType.BaseDataType dataType, byte[] bytes)
		{
			DataType = dataType;
			Bytes    = bytes;
		}
	}
}
