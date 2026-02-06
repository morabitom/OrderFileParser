namespace OrderFileParser.Models.Order;

/*
Order Detail Lines - Multiple records per order
    ------------------------------------
Position	Length	Format				Value	Description
0			3		Fixed				300		Line Type Identifier
3			2		Numeric						Line number
5			5		Numeric						Quantity
10			10		Numeric	(#.00)				Cost each
20			10		Numeric	(#.00)				Total Cost
30			50		Text						Description
*/

public sealed class Detail
{
    public int? LineNumber { get; private set; }
    public int? Quantity { get; private set; }
    public decimal? CostEach { get; private set; }
    public decimal? CostTotal { get; private set; }
    public string? Description { get; private set; }

    public static Detail CreateFromRawData(string line, List<string> errorLog)
    {
        Detail detail = new()
        {
            LineNumber = ParseDataField<int>(line.Substring(3,2), nameof(LineNumber), errorLog),
            Quantity = ParseDataField<int>(line.Substring(5, 5), nameof(Quantity), errorLog),
            CostEach = ParseDataField<decimal>(line.Substring(10, 10), nameof(CostEach), errorLog),
            CostTotal = ParseDataField<decimal>(line.Substring(20, 10), nameof(CostTotal), errorLog),
            Description = ParseDataField<string>(line.Substring(30, 50), nameof(Description), errorLog)
        };
        return detail;
    }
}