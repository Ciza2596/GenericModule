using UnityEngine;

namespace CizaLocaleAddressablesModule.Implement
{
	[CreateAssetMenu(fileName = "LocaleAddressablesByRefCountModuleConfig", menuName = "Ciza/LocaleAddressablesByRefCountModule/LocaleAddressablesByRefCountModuleConfig")]
	public class LocaleAddressablesByRefCountModuleConfig : ScriptableObject, ILocaleAddressablesByRefCountModuleConfig
	{
		[SerializeField]
		private string[] _supportLocales = new[] { "en" };

		[Space]
		[SerializeField]
		private string _sourceLocale = "en";

		[SerializeField]
		private string _defaultLocale = "en";

		[Space]
		[SerializeField]
		private char _prefixTag = '/';

		public string[] SupportLocales => _supportLocales;
		public string   SourceLocale   => _sourceLocale;
		public string   DefaultLocale  => _defaultLocale;
		public char     PrefixTag     => _prefixTag;
	}
}
