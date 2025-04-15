using System.Globalization;
using UnityEngine;

namespace CizaAudioModule
{
	public static class ColorUtils
	{
		private static readonly CultureInfo CULTURE = CultureInfo.InvariantCulture;
		private const NumberStyles HEX = NumberStyles.HexNumber;

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public static Color Parse(string input)
		{
			if (input.Length > 9)
				return default;

			if (input[0] == '#')
				input = input[1..];

			var inputLength = input.Length;

			if (inputLength != 6 && inputLength != 8)
				return default;
			var r = int.Parse($"{input[0]}{input[1]}", HEX, CULTURE);
			var g = int.Parse($"{input[2]}{input[3]}", HEX, CULTURE);
			var b = int.Parse($"{input[4]}{input[5]}", HEX, CULTURE);

			var a = inputLength == 8 ? int.Parse($"{input[6]}{input[7]}", HEX, CULTURE) : 255;
			return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		}
	}
}