using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace CizaTextModule
{
	public class CsvUtils
	{
		public const char COMMA_TAG        = ',';
		public const char N_TAG_WITH_SLASH = '\n';

		public static Dictionary<string, Dictionary<string, string>> CreateTextMapByCategoryByKey(string csvText, string className)
		{
			var categories = GetCategories(csvText, className);

			var textMapByCategoryByKey = new Dictionary<string, Dictionary<string, string>>();
			var rowTexts               = GetRowTexts(csvText, className, "CreateTextMapByCategoryByKey").ToList();
			Assert.IsTrue(rowTexts.Count > 1, $"[{className}::CreateTextMapByCategoryByKey] Only had categories row, hasn't value rows.");
			rowTexts.RemoveAt(0);

			foreach (var rowText in rowTexts)
			{
				var columns = rowText.Split(COMMA_TAG).ToList();
				if (columns.Count != (categories.Length + 1))
					continue;

				var key = columns[0].Trim();
				if (string.IsNullOrWhiteSpace(key) || string.IsNullOrEmpty(key))
					continue;

				var textMapByCategory = new Dictionary<string, string>();
				textMapByCategoryByKey.Add(key, textMapByCategory);

				columns.RemoveAt(0);

				for (var i = 0; i < categories.Length; i++)
				{
					var category = categories[i].Trim();
					var text     = i < columns.Count ? columns[i].Trim() : string.Empty;
					textMapByCategory.Add(category, text);
				}
			}

			return textMapByCategoryByKey;
		}

		public static string[] GetCategories(string csvText, string className)
		{
			var rowTexts = GetRowTexts(csvText, className, "GetCategories");

			var categories = rowTexts[0].Split(COMMA_TAG).ToList();
			Assert.IsTrue(categories.Count > 1, $"[{className}::GetCategories] Only has key column.");
			categories.RemoveAt(0);

			return categories.ToHashSet().ToArray();
		}

		public static string[] GetRowTexts(string csvText, string className, string methodName)
		{
			var rowTexts = csvText.Split(N_TAG_WITH_SLASH);
			Assert.IsTrue(rowTexts.Length > 0, $"[{className}::{methodName}] Hasn't categories row.");
			return rowTexts;
		}
	}
}
