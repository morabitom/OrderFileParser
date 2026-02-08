using static OrderFileParser.Constants;

namespace OrderFileParser;

public sealed class Orders : List<Order>
{
    private Orders() { }

    public static async Task<(Orders, string)> TryLoadFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return ([], $"Order file not found in: ${filePath}");
        
        try
        {
            return (await LoadFromFileAsync(filePath), string.Empty);
        }
        catch(FileNotFoundException)
        {
            return ([], $"Order file not found in: ${filePath}");
        }
        catch (IOException)
        {
            return ([], "IO Error encountered accessing order file.");
        }
        catch (Exception ex)
        {
            return ([], $"Unexpected error encountered loading order file: {ex.Message}");
        }
    }
    private static async Task<Orders> LoadFromFileAsync(string filePath)
    {
        Orders orders = [];
        Order currentOrder = new();
        int idx = 1;
        string? lineString;

        using StreamReader reader = new(filePath);
        while ((lineString = await reader.ReadLineAsync()) is not null)
        {
            var line = lineString.AsSpan();
            if (line.Length < 80)
            {
                Console.WriteLine($"Line number {idx} is too short to be parsed.");
                continue;
            }
                
            switch (line.Slice(0,3))
            {
                case LineType.Header:
                    if (currentOrder.HeaderInfo.OrderNumber != default)
                    {
                        currentOrder.ValidateAddress();
                        currentOrder.ValidateDetails();
                        orders.Add(currentOrder);
                        currentOrder = new();
                    }
                    currentOrder.ParseHeader(line);
                    break;
                case LineType.Address:
                    currentOrder.ParseAddress(line);
                    break;
                case LineType.Detail:
                    currentOrder.ParseDetail(line, idx);
                    break;
                default:
                    currentOrder.AddError($"Unknown row type encountered on line number {idx}");
                    break;
            }
            idx++;
        }

        currentOrder.ValidateAddress();
        currentOrder.ValidateDetails();
        orders.Add(currentOrder);

        return orders;
    }
}
