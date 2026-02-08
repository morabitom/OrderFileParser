using System.Globalization;

namespace OrderFileParser;

public class Order
{
    public struct Header
    {
        public int OrderNumber;
        public int TotalItems;
        public decimal TotalCost;
        public DateTime OrderDate;
        public string CustomerName;
        public string CustomerPhone;
        public string CustomerEmail;
        public bool? Paid;
        public bool? Shipped;
        public bool? Completed;
    }
    public Header HeaderInfo;

    public struct Address
    {
        public string AddressLine1;
        public string AddressLine2;
        public string City;
        public string State;
        public string Zip;
    }
    public Address AddressInfo;

    public struct Detail
    {
        public int LineNumber;
        public int Quantity;
        public decimal CostEach;
        public decimal CostTotal;
        public string? Description;
    }
    public List<Detail> Details = [];

    public List<string>? Errors;
    public bool LoadedSuccessfully => Errors is null or { Count: 0 };

    public Order() { }

    //lazy loading of error list to avoid unnecessary allocations when parsing is successful
    //error string literals have been interned and are passed by reference, no extra allocation will occur.
    public void AddError(string error) => (this.Errors ??= []).Add(error);

    public void ParseHeader(ReadOnlySpan<char> line)
    {
        if (line.Length != 180)
        {
            AddError("Unable to parse header due it invalid line length.");
            return;
        }

        if (!int.TryParse(line.Slice(3, 10), out this.HeaderInfo.OrderNumber))
            AddError("Unable to parse order number.");
        else if (this.HeaderInfo.OrderNumber < 0)
            AddError("Order number cannot be negative.");

        if (!int.TryParse(line.Slice(13, 5), out this.HeaderInfo.TotalItems))
            AddError("Unable to parse total items count.");
        else if (this.HeaderInfo.TotalItems < 0)
            AddError("Total items count cannot be negative.");

        if (!decimal.TryParse(line.Slice(18, 10), NumberStyles.Currency, Constants.CurrentNumberFormatInfo, out this.HeaderInfo.TotalCost))
            AddError("Unable to parse total cost.");
        else if (this.HeaderInfo.TotalCost < 0)
            AddError("Total cost cannot be negative.");

        if (!DateTime.TryParse(line.Slice(28, 19), out this.HeaderInfo.OrderDate))
            AddError("Unable to parse order date.");

        this.HeaderInfo.CustomerName = line.Slice(47, 50).Trim().ToString();
        if (this.HeaderInfo.CustomerName.Length == 0)
            AddError("Customer name cannot be empty.");

        this.HeaderInfo.CustomerPhone = line.Slice(97, 30).Trim().ToString();
        this.HeaderInfo.CustomerEmail = line.Slice(127, 50).Trim().ToString();
        if (this.HeaderInfo.CustomerEmail.Length == 0 && this.HeaderInfo.CustomerPhone.Length == 0)
            AddError("Customer must have at least one phone number or email contact method.");

        this.HeaderInfo.Paid = line[177] switch { '1' => true, '0' => false, _ => null };
        if (this.HeaderInfo.Paid is null)
            AddError("Unable to parse order's paid status");

        this.HeaderInfo.Shipped = line[178] switch { '1' => true, '0' => false, _ => null };
        if (this.HeaderInfo.Shipped is null)
            AddError("Unable to parse order's shipped status");

        this.HeaderInfo.Completed = line[179] switch { '1' => true, '0' => false, _ => null };
        if (this.HeaderInfo.Completed is null)
            AddError("Unable to parse order's completed status");
    }

    public void ParseAddress(ReadOnlySpan<char> line)
    {
        if (line.Length != 165)
        {
            AddError("Unable to parse address due it invalid line length.");
            return;
        }

        if (this.AddressIsLoaded)
        {
            AddError("More than one address specified for order.");
            return;
        }

        this.AddressInfo.AddressLine1 = line.Slice(3, 50).Trim().ToString();
        if (this.AddressInfo.AddressLine1.Length == 0)
            AddError("Address line 1 cannot be empty.");

        this.AddressInfo.AddressLine2 = line.Slice(53, 50).Trim().ToString();

        this.AddressInfo.City = line.Slice(103, 50).Trim().ToString();
        if (string.IsNullOrEmpty(this.AddressInfo.City))
            AddError("City cannot be empty.");

        this.AddressInfo.State = line.Slice(153, 2).Trim().ToString();
        if (this.AddressInfo.State.Length < 2)
            AddError("State must be two characters long.");

        this.AddressInfo.Zip = line.Slice(155, 10).Trim().ToString();
        if (this.AddressInfo.Zip.Length < 5)
            AddError("Zip must be at least five characters long.");
    }

    public void ParseDetail(ReadOnlySpan<char> line, int lineNum)
    {
        if (line.Length != 80)
        {
            AddError($"Unable to parse detail on line {lineNum} due it invalid line length.");
            return;
        }

        Detail detail = new();

        if (!int.TryParse(line.Slice(3, 2), out detail.LineNumber))
            AddError($"Unable to parse detail line number on line {lineNum}.");
        else if (detail.LineNumber < 1)
            AddError($"Detail line number on line {lineNum} cannot be less than 1.");

        if (!int.TryParse(line.Slice(5, 5), out detail.Quantity))
            AddError($"Unable to parse detail #{detail.LineNumber} quantity.");
        else if (detail.Quantity < 0)
            AddError($"Detail #{detail.LineNumber} quantity cannot be less than 0.");

        if (!decimal.TryParse(line.Slice(10, 10), NumberStyles.Currency, Constants.CurrentNumberFormatInfo, out detail.CostEach))
            AddError($"Unable to parse detail #{detail.LineNumber} cost per each.");

        if (!decimal.TryParse(line.Slice(20, 10), NumberStyles.Currency, Constants.CurrentNumberFormatInfo, out detail.CostTotal))
            AddError($"Unable to parse detail #{detail.LineNumber} cost total.");

        detail.Description = line.Slice(30, 50).Trim().ToString();
        if (string.IsNullOrEmpty(detail.Description))
            AddError($"Detail #{detail.LineNumber} description cannot be empty.");

        this.Details.Add(detail);
    }

    private bool AddressIsLoaded => AddressInfo.AddressLine1 != default;
    public void ValidateAddress()
    {
        if (!this.AddressIsLoaded)
            this.AddError("Order is missing an address row.");
    }

    public void ValidateDetails()
    {
        if (this.Details.Count is 0)
            this.AddError("Order is missing at least one detail row.");
    }
}
