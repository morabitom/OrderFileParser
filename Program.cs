global using static OrderFileParser.DataFileUtilities;
using System.Text.Encodings.Web;
using System.Text.Json;
using OrderFileParser.Models.Order;

const string fileName = "OrderFile.txt";
var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
Console.WriteLine(File.Exists(filePath) ? $"Found Order File: {fileName}" : "Order File Not Found");

var orders = await Orders.LoadFromFile(filePath);

var jsonOptions = new JsonSerializerOptions
{
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, 
    WriteIndented = true
};

Console.WriteLine(JsonSerializer.Serialize(orders, jsonOptions));
