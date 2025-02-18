using UnityEngine;

namespace CizaLocaleModule
{
	[CreateAssetMenu(fileName = "LocaleModuleConfig", menuName = "Ciza/LocaleModule/LocaleModuleConfig")]
	public class LocaleModuleConfig : ScriptableObject, ILocaleModuleConfig
	{
		[SerializeField]
		[OverrideDrawer]
		protected string[] _supportLocales = new[] { "en" };

		[Space]
		[SerializeField]
		protected bool _isIgnoreSourceLocale = true;

		[SerializeField]
		protected string _sourceLocale = "en";

		[Space]
		[SerializeField]
		protected string _defaultLocale = "en";

		[Space]
		[SerializeField]
		protected char _prefixTag = '/';

		public virtual string[] SupportLocales => _supportLocales;

		public virtual bool IsIgnoreSourceLocale => _isIgnoreSourceLocale;
		public virtual string SourceLocale => _sourceLocale;

		public virtual string DefaultLocale => _defaultLocale;
		public virtual char PrefixTag => _prefixTag;
	}
}