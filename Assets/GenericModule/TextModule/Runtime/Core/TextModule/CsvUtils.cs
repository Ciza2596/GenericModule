using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace CizaTextModule
{
    public class CsvUtils
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
                var columns = new List<string>();

                var columnString = string.Empty;
                var nextCount = 0;

                for (int i = 0; i < rowText.Length; i++)
                {
                    if (nextCount > 0)
                    {
                        nextCount--;
                        continue;
                    }

                    if (i + 1 < rowText.Length && rowText[i] == COMMA_TAG && rowText[i + 1] == QUOTATION_TAG)
                    {
                        columns.Add(columnString);
                        columnString = string.Empty;
                        nextCount = 1;
                        continue;
                    }

                    columnString += rowText[i];

                    if (i == rowTexts.Count - 1)
                        columns.Add(columnString);
                }


                // var columns = rowText.Split(COMMA_TAG).ToList();
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