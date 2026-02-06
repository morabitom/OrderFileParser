using static OrderFileParser.Models.Order.LineTypeExtensions;
namespace OrderFileParser.Models.Order;

public sealed class Order
{
    public readonly List<string> Errors = [];
    public bool LoadedSuccessfully => Errors.Count == 0;
    public Header? Header { get; private set; }
    public Address? Address { get; private set; }
    public IReadOnlyList<Detail> Details => _details;
    private readonly List<Detail> _details = [];

    private Order() {}
    
    public static Order CreateFromRawFileData(List<string> lines)
    {
        Order order = new();
        foreach (var line in lines)
        {
            switch (GetLineType(line))
            {
                case LineType.Header:
                    if (order.Header is not null)
                        throw new FormatException(
                            $"More than one header specified for order {order.Header?.OrderNumber}");
                    order.Header = Header.CreateFromRawData(line, order.Errors);
                    break;
                case LineType.Address:
                    if (order.Address is not null)
                        throw new FormatException(
                            $"More than one address specified for order {order.Header?.OrderNumber}");
                    order.Address = Address.CreateFromRawData(line, order.Errors);
                    break;
                case LineType.Detail:
                    order._details.Add(Detail.CreateFromRawData(line, order.Errors));
                    break;
                case LineType.Unknown:
                default:
                    throw new FormatException($"Unknown line type {line}");
            }
        }
            
        if (order.Header is null)
            throw new FormatException("No header specified for order");
        if (order.Address is null)
            throw new FormatException("No address specified for order");

        return order;
    }
}