using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
	public static class StyleSheetUtils
	{
		public const string ROOT_PATH = "InputModule/StyleSheet/";

		private static readonly string[] COMMON_STYLE_PATHS = { "CommonValue", "CommonColor", };

		public static StyleSheet[] GetStyleSheets(params string[] paths)
		{
			if (BuildPipeline.isBuildingPlayer)
				return Array.Empty<StyleSheet>();

			var styleSheets = new List<StyleSheet>();
			styleSheets.AddRange(GetCommonStyleSheets());
			foreach (var path in paths)
			{
				if (!path.CheckHasValue())
					continue;

				var styleSheet = LoadStyleSheet(path);
				if (styleSheet != null)
					styleSheets.Add(styleSheet);
				else
					Debug.LogError($"Style sheet is not found by path: {path}.");
			}

			return styleSheets.ToArray();
		}

		private static List<StyleSheet> GetCommonStyleSheets()
		{
			var styleSheets = new List<StyleSheet>();
			foreach (var path in COMMON_STYLE_PATHS)
				styleSheets.Add(LoadStyleSheet(path));
			return styleSheets;
		}

		private static StyleSheet LoadStyleSheet(string path) =>
			Resources.Load<StyleSheet>(Path.Combine(ROOT_PATH, path));
	}
}