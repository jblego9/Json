using Json.Serialization;

namespace Json.TestProgram;

public class Program
{
    private const string DATA_FILEPATH = "Json.TestProgram.data.json";

    private class Data
    {
        public Data()
        {
            PreviousText = "";
            ModificationCount = 1;
        }

        public string PreviousText;
        public int ModificationCount;
    }

    public static void Main()
    {
        if (File.Exists(DATA_FILEPATH))
            HandlePreviousText();
        else
            HandleNoPreviousText();
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
            jsonObjectString = JsonSerializer.Serialize(new { ModificationCount = 0, PreviousText = input });
        } catch (FormatException err) {
            Console.WriteLine($"{err.Message}, quitting...");
            return;
        }

        using StreamWriter sw = File.CreateText(DATA_FILEPATH);
        sw.Write(jsonObjectString);
    }

    private static void HandlePreviousText()
    {
        var data = JsonDeserializer.Deserialize<Data>(File.ReadAllText(DATA_FILEPATH));
        Console.Write($"Previous text: {data.PreviousText}\nModification count: {data.ModificationCount}\nEnter text: ");

        string? input = Console.ReadLine();
        if (input is null)
        {
            Console.WriteLine("No text given, quiting...");
            return;
        }

        data.ModificationCount++;
        string jsonObjectString;

        try {
            jsonObjectString = JsonSerializer.Serialize(new { data.ModificationCount, PreviousText = input });
        } catch (FormatException err) {
            Console.WriteLine($"{err.Message}, quitting...");
            return;
        }

        using StreamWriter sw = File.CreateText(DATA_FILEPATH);
        sw.Write(jsonObjectString);
    }
}