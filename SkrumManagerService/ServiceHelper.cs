using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkrumManagerService
{
    public class ServiceHelper
    {
        public static string HashPassword(string password)
        {
            // Return null if password is null or empty.
            if (password == null || password == "")
            {
                return null;
            }

            // Hash the password.
            System.Security.Cryptography.SHA512 sha512 = new System.Security.Cryptography.SHA512Managed();
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            byte[] digest = sha512.ComputeHash(encoder.GetBytes(password));
            sha512.Dispose();
            string result = System.BitConverter.ToString(digest);
            result = result.Replace("-", "").ToLower();
            return result;
        }
    }
}
