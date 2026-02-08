using System.Text.Encodings.Web;
using System.Text.Json;

namespace OrderFileParser;

class Program
{
    static async Task Main(string[] args)
    {
        const string fileName = "OrderFile.txt";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        var (orders, error) = await Orders.TryLoadFromFileAsync(filePath);
        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Error loading orders: {error}");
            return;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, 
            IncludeFields = true,
            WriteIndented = true
        };

        Console.WriteLine(JsonSerializer.Serialize(orders, jsonOptions));
    }
}