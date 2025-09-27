using UnityEngine;

namespace CizaLocaleModule
{
	[CreateAssetMenu(fileName = "LocaleModuleConfig", menuName = "Ciza/LocaleModule/LocaleModuleConfig")]
	public class LocaleModuleConfig : ScriptableObject, ILocaleModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		[OverrideDrawer]
		protected string[] _supportLocales;

		[Space]
		[SerializeField]
		protected bool _isIgnoreSourceLocale;

		[SerializeField]
		protected string _sourceLocale;

		[Space]
		[SerializeField]
		protected string _defaultLocale;

		[Space]
		[SerializeField]
		protected char _prefixTag;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string[] SupportLocales => _supportLocales;

		public virtual bool IsIgnoreSourceLocale => _isIgnoreSourceLocale;
		public virtual string SourceLocale => _sourceLocale;

		public virtual string DefaultLocale => _defaultLocale;
		public virtual char PrefixTag => _prefixTag;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_supportLocales = new[] { "en" };

			_isIgnoreSourceLocale = true;
			_sourceLocale = "en";

			_defaultLocale = "en";
			_prefixTag = '/';
		}
	}
}