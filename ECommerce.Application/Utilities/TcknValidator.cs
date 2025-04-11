namespace ECommerce.Application.Utilities
{
    public static class TcknValidator
    {
        public static bool Validate(string tckn)
        {
            if (string.IsNullOrEmpty(tckn) || tckn.Length != 11 || !tckn.All(char.IsDigit))
            {
                return false;
            }

            if (tckn[0] == '0' || tckn[10] == '0')
            {
                return false;
            }

            return true;
        }
    }
}