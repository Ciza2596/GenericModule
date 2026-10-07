using System;

namespace CizaLocaleModule.Editor
{
	[Flags]
	public enum AnchorKinds
	{
		None = 0,
        
		Left = 1 << 0,
		Right = 1 << 1,
		Top = 1 << 2,
		Bottom = 1 << 3,

		TopLeft = Top | Left,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomRight = Bottom | Right,
	}
}
