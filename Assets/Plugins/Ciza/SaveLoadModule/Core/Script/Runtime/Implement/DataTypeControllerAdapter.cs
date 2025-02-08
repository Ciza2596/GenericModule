using System;
using DataType;
using DataType.Implement;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	[Preserve]
	public class DataTypeControllerAdapter : IDataTypeController
	{
		private readonly DataTypeController _dataTypeController;

		[Preserve]
		public DataTypeControllerAdapter(IReflectionHelper reflectionHelper) =>
			_dataTypeController = new DataTypeController(this, reflectionHelper, new IDataTypeControllerInstaller[] { new PrimitiveDataTypeControllerInstaller(), new UnityDataTypeControllerInstaller() });

		public BaseDataType GetOrCreateDataType(Type key) => _dataTypeController.GetOrCreateDataType(key);
	}
}
