using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace CizaTextModule
{
	public static class CsvUtils
	{
		public static readonly string EMPTY = string.Empty;

		public const char COMMA_TAG = ',';
		public const char R_TAG_WITH_SLASH = '\r';
		public const char N_TAG_WITH_SLASH = '\n';

		public const string DOUBLE_QUOTATION_TAG = "\"\"";
		public const char QUOTATION_TAG = '\"';

		public static Dictionary<string, Dictionary<string, string>> CreateTextMapByCategoryByKey(string csvText, string className)
		{
			var filterText = FilterText(csvText);
			var categories = GetCategories(filterText, className);

			var textMapByCategoryByKey = new Dictionary<string, Dictionary<string, string>>();
			var rowTexts = GetRowTexts(filterText, className, "CreateTextMapByCategoryByKey").ToList();
			Assert.IsTrue(rowTexts.Count > 1, $"[{className}::CreateTextMapByCategoryByKey] Only had categories row, hasn't value rows.");
			rowTexts.RemoveAt(0);

			foreach (var rowText in rowTexts)
			{
				// var columns = new List<string>();
				//
				// var columnString = string.Empty;
				// var ignoreCount = 0;
				// var commaAndQuotationCount = 0;
				//
				// for (int i = 0; i < rowText.Length; i++)
				// {
				//     if (ignoreCount > 0)
				//     {
				//         ignoreCount--;
				//         continue;
				//     }
				//
				//     if (commaAndQuotationCount > 0)
				//     {
				//         if (i + 1 < rowText.Length && rowText[i] == QUOTATION_TAG && rowText[i + 1] == COMMA_TAG)
				//         {
				//             columns.Add(columnString);
				//             columnString = string.Empty;
				//             commaAndQuotationCount = 0;
				//             ignoreCount = 1;
				//             continue;
				//         }
				//     }
				//     else
				//     {
				//         if (i + 1 < rowText.Length && rowText[i] == COMMA_TAG && rowText[i + 1] == QUOTATION_TAG)
				//         {
				//             columns.Add(columnString);
				//             columnString = string.Empty;
				//             commaAndQuotationCount = 1;
				//             ignoreCount = 1;
				//             continue;
				//         }
				//
				//         if (rowText[i] == COMMA_TAG)
				//         {
				//             columns.Add(columnString);
				//             columnString = string.Empty;
				//             ignoreCount = 1;
				//             continue;
				//         }
				//     }
				//
				//     columnString += rowText[i];
				//
				//     if (i == rowTexts.Count - 1)
				//         columns.Add(columnString);
				// }


				var columns = new List<string>();
				var splitTexts = rowText.Split(COMMA_TAG).ToArray();
				var hasInComma = false;

				var targetText = string.Empty;

				foreach (var splitText in splitTexts)
				{
					var quotationCount = 0;
					var index = 0;
					while ((index = splitText.IndexOf(QUOTATION_TAG, index)) != -1)
					{
						quotationCount++;
						index += 1;
					}

					if (!hasInComma && CheckIsOdd(quotationCount))
					{
						hasInComma = true;

						var newText = string.Empty;
						var firstQuotation = true;
						foreach (var chr in splitText)
						{
							if (chr == QUOTATION_TAG && firstQuotation)
							{
								firstQuotation = false;
								continue;
							}

							newText += chr;
						}

						targetText += newText;
					}
					else if (hasInComma && CheckIsOdd(quotationCount))
					{
						hasInComma = false;
						var newText = string.Empty;
						var firstQuotation = true;
						foreach (var chr in splitText)
						{
							if (chr == QUOTATION_TAG && firstQuotation)
							{
								firstQuotation = false;
								continue;
							}

							newText += chr;
						}

						targetText += newText;
						columns.Add(targetText);
						targetText = string.Empty;
					}
					else
					{
						if (hasInComma)
						{
							targetText += splitText;
						}
						else
							columns.Add(splitText);
					}
				}

				bool CheckIsOdd(int num) =>
					num % 2 != 0;


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
					var text = i < columns.Count ? columns[i].Trim() : string.Empty;

					if (text.Length > 2 && text[0] == QUOTATION_TAG && text[^1] == QUOTATION_TAG)
						text = text.Substring(1, text.Length - 2);

					textMapByCategory.Add(category, text.Trim());
				}
			}

			return textMapByCategoryByKey;
		}

		public static string[] GetCategories(string csvText, string className)
		{
			var filterText = FilterText(csvText);
			var rowTexts = GetRowTexts(filterText, className, "GetCategories");

			var categories = rowTexts[0].Split(COMMA_TAG).ToList();
			Assert.IsTrue(categories.Count > 1, $"[{className}::GetCategories] Only has key column.");
			categories.RemoveAt(0);

			return categories.ToHashSet().ToArray();
		}

		public static string[] GetRowTexts(string csvText, string className, string methodName)
		{
			var filterText = FilterText(csvText);
			var rowTexts = filterText.Split(N_TAG_WITH_SLASH);
			Assert.IsTrue(rowTexts.Length > 0, $"[{className}::{methodName}] Hasn't categories row.");
			return rowTexts;
		}

		public static string FilterText(string text)
		{
			var newText = text.Replace(DOUBLE_QUOTATION_TAG, QUOTATION_TAG.ToString());
			newText = newText.Replace(R_TAG_WITH_SLASH.ToString(), EMPTY);
			return newText;
		}
	}
}