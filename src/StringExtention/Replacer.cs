using System.Text;

namespace TranslatorHelper.StringHelper
{
    public static class Replacer
    {
        public static string Clean(this string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace("{", "");
            sb.Replace("}", "");

            return sb.ToString();
        }
    }
}
