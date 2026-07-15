using Json.Document;

namespace Json.TestProgram;

public class Program
{
    public static void Main()
    {
        const string dataFilepath = "data.json";

        if (!File.Exists(dataFilepath))
        {
            Console.Write("No previous text.\nEnter text: ");
            
            string? input = Console.ReadLine();
            if (input is null)
            {
                Console.WriteLine("No text given, quiting...");
                return;
            }

            string jsonObjectString = JsonDocument.Write(
                new JsonValue.JsonObject([
                    new(
                        new JsonValue.JsonString("modificationCount"),
                        new JsonValue.JsonNumber("1")
                    ),
                    new(
                        new JsonValue.JsonString("previousText"),
                        new JsonValue.JsonString(input)
                    )
                ])
            );

            using StreamWriter sw = File.CreateText(dataFilepath);
            {
                sw.Write(jsonObjectString);
            }
            return;
        } else
        {
            var jsonObjectValue = (JsonValue.JsonObject)JsonDocument.Parse(File.ReadAllText(dataFilepath));

            var previousText = ((JsonValue.JsonString)jsonObjectValue.Get("previousText")).Value;
            var modificationCount = ((JsonValue.JsonNumber)jsonObjectValue.Get("modificationCount")).AsInt64();

            Console.Write($"Previous text: {previousText}\nModification count: {modificationCount}\nEnter text: ");

            string? input = Console.ReadLine();
            if (input is null)
            {
                Console.WriteLine("No text given, quiting...");
                return;
            }

            string jsonObjectString = JsonDocument.Write(
                new JsonValue.JsonObject([
                    new(
                        new JsonValue.JsonString("modificationCount"),
                        new JsonValue.JsonNumber((++modificationCount).ToString())
                    ),
                    new(
                        new JsonValue.JsonString("previousText"),
                        new JsonValue.JsonString(input)
                    )
                ])
            );

            using StreamWriter sw = File.CreateText(dataFilepath);
            sw.Write(jsonObjectString);
            return;
        }
    }
}