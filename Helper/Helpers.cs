using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace OrderApp.Helper
{
    public class Helpers
    {
        public string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        //public static string Translate(string key)
        //{
        //    //return Resources.Strings.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);
        //}


    }
}
