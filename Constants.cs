using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OrderFileParser
{
    public static class Constants
    {
        public static class LineType
        {
            public const string Header = "100";
            public const string Address = "200";
            public const string Detail = "300";
        }

        public static readonly NumberFormatInfo CurrentNumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
    }
}
