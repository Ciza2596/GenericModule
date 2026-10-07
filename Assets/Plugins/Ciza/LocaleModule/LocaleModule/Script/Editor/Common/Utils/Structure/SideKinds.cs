using System;

namespace CizaLocaleModule.Editor
{
	[Flags]
	public enum SideKinds
	{
		Left = 1 << 0,
		Right = 1 << 1,
		Top = 1 << 2,
		Bottom = 1 << 3,

		All = Left | Right | Top | Bottom,
		Horizontal = Left | Right,
		Vertical = Top | Bottom,

		TopLeft = Top | Left,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right,

		NoLeft = All ^ Left,
		NoRight = All ^ Right,
		NoTop = All ^ Top,
		NoBottom = All ^ Bottom
	}
}
