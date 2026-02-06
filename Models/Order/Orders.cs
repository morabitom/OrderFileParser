namespace OrderFileParser.Models.Order;

public sealed class Orders : List<Order>
{
    private Orders() { }
    public static async Task<Orders> LoadFromFile(string filePath)
    {
        Orders orders = [];
        var lines = await File.ReadAllLinesAsync(filePath);
        var rawOrdersData = GetRawOrdersData(lines);

        foreach (var data in rawOrdersData)
            orders.Add(Order.CreateFromRawFileData(data));
        
        return orders;
    }

}