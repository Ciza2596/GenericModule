namespace CizaSaveLoadModule.Implement
{
	public struct DataTypeData
	{
		public DataType.DataType DataType { get; }
		public byte[]            Bytes    { get; }

		public DataTypeData(DataType.DataType dataType, byte[] bytes)
		{
			DataType = dataType;
			Bytes    = bytes;
		}
	}
}
