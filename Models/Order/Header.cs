namespace OrderFileParser.Models.Order;

/*
Order Header Lines - One record per order
------------------------------------
Position	Length	Format				Value	Description
0			3		Fixed				100		Line Type Identifier
3			10		Numeric						Order number
13			5		Numeric						Total Items
18			10		Numeric	(#.00)				Total Cost
28			19		MM/DD/YYYY hh:mm:ss			Order Date
47			50		Text						Customer Name
97			30		Text						Customer Phone
127			50		Text						Customer Email
177 		1		Boolean (0/1)				Paid
178 		1		Boolean (0/1)				Shipped
179 		1		Boolean (0/1)				Completed
*/

public sealed class Header
{
    public int? OrderNumber { get; set; }
    public int? TotalItems { get; set; }
    public decimal? TotalCost { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public bool? Paid { get; set; }
    public bool? Shipped { get; set; }
    public bool? Completed { get; set; }
    
    private Header() { }
    
    public static Header? CreateFromRawData(string line, List<string> errorLog)
    {
        Header header = new()
        {
            OrderNumber = ParseDataField<int>(line.Substring(3,10), nameof(OrderNumber), errorLog),
            TotalItems = ParseDataField<int>(line.Substring(13, 5), nameof(TotalItems), errorLog),
            TotalCost = ParseDataField<decimal>(line.Substring(18, 10), nameof(TotalCost), errorLog),
            OrderDate = ParseDataField<DateTime>(line.Substring(28, 19), nameof(OrderDate), errorLog),
            CustomerName = ParseDataField<string>(line.Substring(47, 50), nameof(CustomerName), errorLog),
            CustomerPhone = ParseDataField<string>(line.Substring(97, 30), nameof(CustomerPhone), errorLog),
            CustomerEmail = ParseDataField<string>(line.Substring(127, 50), nameof(CustomerEmail), errorLog),
            Paid = ParseDataField<bool>(line.Substring(177, 1), nameof(Paid), errorLog),
            Shipped = ParseDataField<bool>(line.Substring(178, 1), nameof(Shipped), errorLog),
            Completed = ParseDataField<bool>(line.Substring(179, 1), nameof(Completed), errorLog)
        };
        return header;
    }
}