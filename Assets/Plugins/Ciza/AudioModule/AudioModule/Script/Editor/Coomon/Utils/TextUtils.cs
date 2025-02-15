using System.Globalization;

namespace CizaAudioModule.Editor
{
	public static class TextUtils
	{
		private static readonly TextInfo TXT = CultureInfo.InvariantCulture.TextInfo;

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public static string Humanize(string source)
		{
#if UNITY_EDITOR
			source = UnityEditor.ObjectNames.NicifyVariableName(source);
#endif

			char[] characters = source.ToCharArray();
			for (int i = 0; i < characters.Length; ++i)
			{
				if (characters[i] == '-') characters[i] = ' ';
				if (characters[i] == '_') characters[i] = ' ';
			}

			source = new string(characters);
			source = TXT.ToTitleCase(source);

			return source;
		}
	}
}