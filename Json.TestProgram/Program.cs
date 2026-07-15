using Json.Document;

namespace Json.TestProgram;

public class Program
{
    private const string dataFilepath = "data.json";

    public static void Main()
    {
        if (File.Exists(dataFilepath))
            HandlePreviousText();
        else
            HandleNoPreviousText();
    }

    private static string CreateJsonObjectString(string previousText, long modificationCount)
    {
        return JsonDocument.Write(
            new JsonValue.JsonObject([
                new(
                    new JsonValue.JsonString("modificationCount"),
                    new JsonValue.JsonNumber(modificationCount.ToString())
                ),
                new(
                    new JsonValue.JsonString("previousText"),
                    new JsonValue.JsonString(previousText)
                )
            ])
        );
    }

    private static void HandleNoPreviousText()
    {
        Console.Write("No previous text.\nEnter text: ");
            
            string? input = Console.ReadLine();
            if (input is null)
            {
                Console.WriteLine("No text given, quiting...");
                return;
            }

            string jsonObjectString = CreateJsonObjectString(input, 1);

            using StreamWriter sw = File.CreateText(dataFilepath);
            sw.Write(jsonObjectString);
    }

    private static void HandlePreviousText()
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

        string jsonObjectString = CreateJsonObjectString(input, ++modificationCount);

        using StreamWriter sw = File.CreateText(dataFilepath);
        sw.Write(jsonObjectString);
    }
}