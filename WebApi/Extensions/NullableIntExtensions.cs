namespace WebApiTemplate.Extensions
{
    public class NullableIntExtensions
    {
        public static int? TryGetInt(string value)
        {
            int i;
            bool success = int.TryParse(value, out i);
            return success ? (int?)i : (int?)null;
        }
    }
}
