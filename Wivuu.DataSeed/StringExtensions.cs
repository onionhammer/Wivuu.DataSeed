using System;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace Wivuu.DataSeed
{
    public static class StringExtensions
    {
        /// <summary>
        /// Generates guid from input string
        /// WARNING: This should only be used to seed local test data
        /// </summary>
        public static Guid ToGuid(this string source)
        {
            Contract.Assert(source != null);

            using (var crypto = new SHA1CryptoServiceProvider())
            {
                var bytes  = Encoding.UTF8.GetBytes(source);
                var hashed = crypto.ComputeHash(bytes);

                Array.Resize(ref hashed, 16);
                return new Guid(hashed);
            }
        }
    }
}
