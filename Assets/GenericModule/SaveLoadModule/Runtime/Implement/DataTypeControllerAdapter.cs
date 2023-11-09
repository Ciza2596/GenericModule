using System;
using DataType;
using DataType.Implement;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	public class DataTypeControllerAdapter : IDataTypeController
	{
		private readonly DataTypeController _dataTypeController;

		[Preserve]
		public DataTypeControllerAdapter(IReflectionHelper reflectionHelper) =>
			_dataTypeController = new DataTypeController(this, reflectionHelper, new IDataTypeControllerInstaller[] { new PrimitiveDataTypeControllerInstaller(), new UnityDataTypeControllerInstaller() });

		public DataType.DataType GetOrCreateDataType(Type key) => _dataTypeController.GetOrCreateDataType(key);
	}
}
