using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CizaTextModule
{
	public static class StringExtension
	{
		public const string CONTROLLER_TEXT_KEY_PATTERN = @"<ControllerTextKey=""([^""]*)""[^>]*>";
		public const string LOCALE_TEXT_KEY_PATTERN = @"<LocaleTextKey=""([^""]*)""[^>]*>";


		public static bool CheckHasValue(this string str) =>
			!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);

		public static string[] GetControllerTextKeys(this string str) =>
			str.GetKeys(CONTROLLER_TEXT_KEY_PATTERN);

		public static string[] GetLocaleTextKeys(this string str) =>
			str.GetKeys(LOCALE_TEXT_KEY_PATTERN);

		public static string[] GetKeys(this string str, string keyPattern)
		{
			if (!str.CheckHasValue())
				return Array.Empty<string>();

			var matches = Regex.Matches(str, keyPattern);

			var keys = new HashSet<string>();
			for (var i = 0; i < matches.Count; i++)
				keys.Add(matches[i].Groups[1].Value);

			return keys.ToArray();
		}

		public static string ReplaceByControllerTextKey(this string str, Dictionary<string, string> textMapByKey, string className, string methodName) =>
			str.Replace(CONTROLLER_TEXT_KEY_PATTERN, textMapByKey, className, methodName);

		public static string ReplaceByLocaleTextKey(this string str, Dictionary<string, string> textMapByKey, string className, string methodName) =>
			str.Replace(LOCALE_TEXT_KEY_PATTERN, textMapByKey, className, methodName);

		public static string Replace(this string str, string keyPattern, Dictionary<string, string> textMapByKey, string className, string methodName)
		{
			if (!str.CheckHasValue())
				return string.Empty;

			var matches = Regex.Matches(str, keyPattern);

			foreach (Match match in matches)
			{
				if (!textMapByKey.TryGetValue(match.Groups[1].Value, out var text))
				{
					Debug.LogWarning($"[{className}::{methodName}] Text not be found by keyWithPattern: {match.Value}.");
					continue;
				}

				str = str.Replace(match.Value, text);
			}

			return str;
		}
	}
}