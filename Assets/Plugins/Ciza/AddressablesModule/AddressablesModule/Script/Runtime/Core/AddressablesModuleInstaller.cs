using System;


namespace CizaAddressablesModule
{
	public static class AddressablesModuleInstaller
	{
		public static AddressablesByRefCountModule Install(string className) =>
			Install<AddressablesByRefCountModule>(className);

		public static TAddressablesModule Install<TAddressablesModule>(string className) where TAddressablesModule : AddressablesByRefCountModule =>
			Activator.CreateInstance(typeof(TAddressablesModule), className) as TAddressablesModule;
	}
}