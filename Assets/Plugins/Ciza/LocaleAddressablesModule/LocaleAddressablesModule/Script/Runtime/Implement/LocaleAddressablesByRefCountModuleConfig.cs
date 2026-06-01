using CizaLocaleModule;
using UnityEngine;

namespace CizaLocaleAddressablesModule.Implement
{
	[CreateAssetMenu(fileName = "LocAddr.Config.asset", menuName = "Ciza/LocaleAddressablesByRefCountModule/LocaleAddressablesByRefCountModuleConfig")]
	public class LocaleAddressablesByRefCountModuleConfig : ScriptableObject, ILocaleAddressablesByRefCountModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		[OverrideDrawer]
		protected string[] _supportLocales;

		[Space]
		[SerializeField]
		protected string _sourceLocale;
		
		[SerializeField]
		protected bool _isIgnoreSourceLocalePrefix;

		[Space]
		[SerializeField]
		protected string _defaultLocale;

		[Space]
		[SerializeField]
		protected char _prefixTag;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string[] SupportLocales => _supportLocales;
		
		public virtual string SourceLocale => _sourceLocale;
		public virtual bool IsIgnoreSourceLocalePrefix => _isIgnoreSourceLocalePrefix;

		public virtual string DefaultLocale => _defaultLocale;
		public virtual char PrefixTag => _prefixTag;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_supportLocales = new[] { "en" };
			
			_sourceLocale = "en";
			_isIgnoreSourceLocalePrefix = true;

			_defaultLocale = "en";
			_prefixTag = '/';
		}
	}
}