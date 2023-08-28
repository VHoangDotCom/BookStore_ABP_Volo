using System.Text.RegularExpressions;

namespace CloudinaryTest.Common
{
    public class CommonUtil
    {
        // Function to get the natural sort key of a string
        public static string GetNaturalSortKey(string value)
        {
            return Regex.Replace(value, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }
    }
}
