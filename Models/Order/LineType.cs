namespace OrderFileParser.Models.Order;

public enum LineType
{
    Header = 100,
    Address = 200,
    Detail = 300,
    Unknown
}

public static class LineTypeExtensions
{
    public static LineType GetLineType(string line) =>
        Enum.Parse<LineType>(line[..3], true);
}
