using System.Globalization;
using OrderFileParser.Models.Order;
using static OrderFileParser.Models.Order.LineTypeExtensions;

namespace OrderFileParser;

public static class DataFileUtilities
{
    public static IEnumerable<List<string>> GetRawOrdersData(IEnumerable<string> lines)
    {
        List<string>? current = null;
        foreach (var line in lines)
        {
            if (GetLineType(line) is LineType.Header)
            {
                if (current is not null)
                    yield return current;
                current = [];
            }

            current?.Add(line);
        }

        if (current is not null)
            yield return current;
    }
    public static T? ParseDataField<T>(string data, string fieldName, List<string> errorLog) where T : IParsable<T>
    {
        data = data.Trim();
        if (typeof(T) == typeof(bool) && data.Length == 1 && (data[0] == '1' || data[0] == '0'))
            data = data[0] == '1' ? "True" : "False";
        
        if (T.TryParse(data, CultureInfo.CurrentCulture, out var result))
            return result;
        
        errorLog.Add($"Unable to parse field: {fieldName} from data: {data}");
        return default(T?);
    }
}