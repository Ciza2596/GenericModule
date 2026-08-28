using System;


namespace CizaAddressablesModule
{
	public static class AddressablesModuleInstaller
	{
		public static AddressablesByRefCountModule Install() =>
			Install<AddressablesByRefCountModule>();

		public static TAddressablesModule Install<TAddressablesModule>() where TAddressablesModule : AddressablesByRefCountModule =>
			Activator.CreateInstance(typeof(TAddressablesModule)) as TAddressablesModule;
	}
}