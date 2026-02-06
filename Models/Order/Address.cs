namespace OrderFileParser.Models.Order;

/*
Address Lines - One record per order
------------------------------------
Position	Length	Format				Value	Description
0			3		Fixed				200		Line Type Identifier
3			50		Text						Address line 1
53			50		Text						Address line 2
103			50		Text						City
153			2		Text						State
155			10		Text						Zip
*/
public sealed class Address
{
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Zip { get; private set; }

    private Address() { }
    public static Address? CreateFromRawData(string line, List<string> errorLog)
    {
        Address address = new()
        {
            AddressLine1 = ParseDataField<string>(line.Substring(3,50), nameof(AddressLine1), errorLog),
            AddressLine2 = ParseDataField<string>(line.Substring(53, 50), nameof(AddressLine2), errorLog),
            City = ParseDataField<string>(line.Substring(103, 50), nameof(City), errorLog),
            State = ParseDataField<string>(line.Substring(153, 2), nameof(State), errorLog),
            Zip = ParseDataField<string>(line.Substring(155, 10), nameof(Zip), errorLog)
        };
        return address;
    }
}