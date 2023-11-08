using CizaCore;

namespace CizaOptionModule
{
	public interface IOptionView
	{
		IOptionColumn[] OptionColumns      { get; }
		Option[]        Options            { get; }
		Option[]        OptionsIncludeNull { get; }

		IColumnInfo ColumnInfo { get; }
		IRowInfo    RowInfo    { get; }

		void UnSelectAll();
	}
}
