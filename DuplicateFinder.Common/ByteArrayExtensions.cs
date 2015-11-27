using System.Text;

namespace DuplicateFinder.Common
{
    public static class ByteArrayExtensions
    {
        public static string ToHex(this byte[] array)
        {
            var hexHashBuilder = new StringBuilder();
            foreach (var i in array)
            {
                hexHashBuilder.Append(i.ToString("x2"));
            }

            return hexHashBuilder.ToString();
        }
    }
}
