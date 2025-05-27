using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Get the built-in category mappings in Chinese or English
    /// </summary>   
    public class GetCategoryDict
    {
        private readonly static string filePath = "D:\\作业\\2024秋季\\毕业设计\\项目文件\\RevitAgent\\files\\builtInCategory.txt";

        public static Dictionary<string, BuiltInCategory> Execute(bool useChinese)
        {
            try
            {
                var chineseDict = new Dictionary<string, BuiltInCategory>();
                var englishDict = new Dictionary<string, BuiltInCategory>();

                foreach (var line in File.ReadLines(filePath))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 3)
                    {
                        var chineseName = parts[0].Trim();
                        var englishName = parts[1].Trim();
                        var builtInName = parts[2].Trim();

                        if (Enum.TryParse(builtInName, out BuiltInCategory builtInCategory))
                        {
                            chineseDict[chineseName] = builtInCategory;
                            englishDict[englishName] = builtInCategory;
                        }
                        else
                        {
                            TaskDialog.Show("Error", "Failed to parse built-in category: " + builtInName);
                        }
                    }
                    else
                    {
                        TaskDialog.Show("Error", "Invalid line: " + line);
                    }
                }

                return useChinese ? chineseDict : englishDict;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return null;
            }
        }
        public static void DisplayCategoryDict(Dictionary<string, BuiltInCategory> categoryDict)
        {
            if (categoryDict == null)
            {
                TaskDialog.Show("Error", "Category dictionary is null");
                return;
            }
            var message = new System.Text.StringBuilder();
            foreach (var pair in categoryDict)
            {
                message.AppendLine(pair.Key + ": " + pair.Value);
            }
            TaskDialog.Show("Categories", message.ToString());
        }
    }
}
