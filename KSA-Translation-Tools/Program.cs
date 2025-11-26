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
        
        Console.Write("Enter 1 for parse mode or 2 for instal mode: ");
        string mode = Console.ReadLine();

        if(mode == "1") { 
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
                    Console.WriteLine($"Success {result.Count} fields written. Output file: " + fieldType + ".json");
                
                }
                else
                {
                    Console.WriteLine($"No data found for field type '{fieldType}'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }

        }

        if (mode == "2")
        {
            Console.WriteLine("\nNotice: file must be near KSA-Translation-tools.exe");
            Console.Write("Enter the file name (for example, Gauges.xml): ");
            string xmlFile = Console.ReadLine();

            Console.WriteLine("\nNotice: file, for explame DisplayName must exists DisplayName.json");
            Console.Write("\nEnter the field type (for example, DisplayName): ");
            string fieldType = Console.ReadLine();

            try
            {
                string jsonContent = File.ReadAllText(fieldType + ".json");
                var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                XDocument xmlDoc = XDocument.Load(xmlFile);
                string attributeName = (fieldType == "Label") ? "Label" : "Value";

                int updateCount = 0;
                var elements = xmlDoc.Descendants(fieldType);

                foreach (var element in elements)
                {
                    var attribute = element.Attribute(attributeName);
                    if (attribute != null && translations.ContainsKey(attribute.Value))
                    {
                        string oldValue = attribute.Value;
                        string newValue = translations[oldValue];

                        if (!string.IsNullOrEmpty(newValue) && newValue!="")
                        {
                            attribute.Value = newValue;
                            updateCount++;
                            Console.WriteLine($"Updated: '{oldValue}' -> '{newValue}'");
                        }
                    }
                }
                xmlDoc.Save(xmlFile);

                Console.WriteLine($"Done! Updated {updateCount} elements.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
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