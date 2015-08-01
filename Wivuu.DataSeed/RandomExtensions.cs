using System;
using System.Text;

namespace Wivuu.DataSeed
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Gets the next random string.
        /// </summary>
        /// <param name="random">The random object to use to generate the string.</param>
        /// <param name="minLength">The minimum length of the string.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <returns>A random string of the specified length.</returns>
        public static string NextString(this Random random, int minLength, int maxLength)
        {
            var length = random.Next(minLength, maxLength);
            var sb     = new StringBuilder(length);

            for (int i = 0; i < length; ++i)
            {
                var c = (char)('a' + (char)random.Next(0, 25));

                if (random.Next(0, 1) == 0)
                    sb.Append(Char.ToUpper(c));
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Nexts the next random unique identifier.
        /// </summary>
        /// <param name="random">The random object to use to generate the guid.</param>
        /// <returns>The next random guid.</returns>
        public static Guid NextGuid(this Random random) => new Guid(
            a: random.Next(),
            b: (short)random.Next(short.MinValue, short.MaxValue),
            c: (byte)random.Next(0, 255),
            d: (byte)random.Next(0, 255),
            e: (byte)random.Next(0, 255),
            f: (byte)random.Next(0, 255),
            g: (byte)random.Next(0, 255),
            h: (byte)random.Next(0, 255),
            i: (byte)random.Next(0, 255),
            j: (byte)random.Next(0, 255),
            k: (byte)random.Next(0, 255)
        );

        /// <summary>
        /// Gets the next random <see cref="DateTime"/> from 1990 to 2020.
        /// </summary>
        /// <param name="random">The random object used to generate the datetime.</param>
        /// <returns>A random <see cref="DateTime"/>.</returns>
        public static DateTime NextDateTime(this Random random) => new DateTime(
            random.Next(1990, 2020),
            random.Next(1, 12),
            random.Next(1, 28)
        );

        /// <summary>
        /// Gets the next random <see cref="DateTime?"/>.
        /// </summary>
        /// <param name="random">The random object used to generate the datetime.</param>
        /// <param name="percentNull">The percent of time that this should return null.</param>
        /// <returns>A <see cref="DateTime?"/>.</returns>
        public static DateTime? NextNullableDateTime(this Random random, float percentNull = .5f)
        {
            var isNull = random.NextDouble() <= percentNull;

            if (isNull)
                return default(DateTime?);

            return random.NextDateTime();
        }
    }
}
