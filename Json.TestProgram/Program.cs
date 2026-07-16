using Json.Document;
using Json.Serialization;

namespace Json.TestProgram;

public class Program
{
    private const string DATA_FILEPATH = "Json.TestProgram.data.json";

    public static void Main()
    {
        if (File.Exists(DATA_FILEPATH))
            HandlePreviousText();
        else
            HandleNoPreviousText();
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> on serialize failure.
    /// <para>Throws <see cref="FormatException"/> on write failure.</para>
    /// </summary>
    private static string CreateJsonObjectString(string previousText, long modificationCount)
    {
        return JsonSerializer.Serialize(new { ModificationCount = modificationCount, PreviousText = previousText });
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

        string jsonObjectString;
        try {
            jsonObjectString = CreateJsonObjectString(input, 1);
        } catch (FormatException err) {
            Console.WriteLine($"{err.Message}, quitting...");
            return;
        }

        using StreamWriter sw = File.CreateText(DATA_FILEPATH);
        sw.Write(jsonObjectString);
    }

    private static void HandlePreviousText()
    {
        var jsonObjectValue = (JsonValue.JsonObject)JsonDocument.Parse(File.ReadAllText(DATA_FILEPATH));

        var previousText = ((JsonValue.JsonString)jsonObjectValue.Get("PreviousText")).Value;
        var modificationCount = ((JsonValue.JsonNumber)jsonObjectValue.Get("ModificationCount")).AsInt64();

        Console.Write($"Previous text: {previousText}\nModification count: {modificationCount}\nEnter text: ");

        string? input = Console.ReadLine();
        if (input is null)
        {
            Console.WriteLine("No text given, quiting...");
            return;
        }

        string jsonObjectString;
        try {
            jsonObjectString = CreateJsonObjectString(input, ++modificationCount);
        } catch (FormatException err) {
            Console.WriteLine($"{err.Message}, quitting...");
            return;
        }

        using StreamWriter sw = File.CreateText(DATA_FILEPATH);
        sw.Write(jsonObjectString);
    }
}