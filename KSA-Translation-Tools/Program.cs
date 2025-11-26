using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Notice: file must be near KSA-Translation-tools.exe");
        Console.Write("Enter the file name (for example, Gauges.xml): ");
        string file = Console.ReadLine();
        Console.Write("\nEnter the field type (for example, DisplayName): ");
        string fieldType = Console.ReadLine();

        try
        {
            XDocument xmlDoc = XDocument.Load(file);
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (fieldType == "Label")
            {
                ProcessElements(xmlDoc.Descendants("Label"), "Label", result);
                ProcessElements(xmlDoc.Descendants("Button"), "Label", result);
            }
            else
            {
                ProcessElements(xmlDoc.Descendants(fieldType), "Value", result);
            }

            if (result.Any())
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(result, options);
                File.WriteAllText(fieldType + ".json", json);
                Console.WriteLine($"Success {result.Count} fields written. Output file: " + fieldType + ".json\nPress any key...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine($"No data found for field type '{fieldType}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ProcessElements(IEnumerable<XElement> elements, string attributeName, Dictionary<string, string> result)
    {
        foreach (var element in elements)
        {
            string value = element.Attribute(attributeName)?.Value;
            if (!string.IsNullOrEmpty(value) && !result.ContainsKey(value))
            {
                result[value] = "";
            }
        }
    }
}