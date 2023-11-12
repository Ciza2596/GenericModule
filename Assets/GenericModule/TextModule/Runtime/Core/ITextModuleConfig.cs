namespace CizaTextModule
{
	public interface ITextModuleConfig
	{
		bool   IsCustomDefaultCategory { get; }
		string CustomDefaultCategory   { get; }

		string CsvText { get; }

		bool IsShowWarningLog { get; }
	}
}
