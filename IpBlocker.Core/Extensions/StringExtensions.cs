using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IpBlocker.Core.Extensions
{
    internal static class StringExtensions
    {
         /// <summary>
        ///     Convert string to int array(',' or ';' separated)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int[] ToIntArray(this string value)
        {
            var split = value.ToArray();

            var array = new int[split.Length];

            for (var i = 0; i < split.Length; i++)
            {
                array[i] = split[i].ToInt();
            }

            return array;
        }

        /// <summary>
        ///     Convert string to double array(',' or ';' separated)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(this string value)
        {
            var split = value.ToArray();

            var array = new double[split.Length];

            for (var i = 0; i < split.Length; i++)
            {
                array[i] = split[i].ToDouble();
            }

            return array;
        }

        /// <summary>
        ///     Convert string to array(',' or ';' separated)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] ToArray(this string value)
        {
            return value.Trim(',', ';', ' ').Split(',', ';');
        }

        /// <summary>
        ///     Convert string to array(',' or ';' separated)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string value)
        {
            if (typeof(T) == typeof(int))
            {
                return value.ToIntArray().Cast<T>().ToList();
            }

            if (typeof(T) == typeof(double))
            {
                return value.ToDoubleArray().Cast<T>().ToList();
            }

            if (typeof(T) == typeof(string))
            {
                return value.ToArray().Cast<T>().ToList();
            }

            throw new NotSupportedException("Invalid Type! Only supported types: int[], double[], string[]");
        }

        /// <summary>
        ///     convert to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            return int.TryParse(value, out var returnVal) ? returnVal : default(int);
        }

        /// <summary>
        ///     convert to double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this string value)
        {
            return double.TryParse(value, NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                                   CultureInfo.InvariantCulture, out var returnVal)
                       ? returnVal
                       : default(double);
        }

        /// <summary>
        ///     Convert to bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBool(this string value)
        {

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value.Trim() == "1")
            {
                return true;
            }

            if (value.Trim() == "0")
            {
                return false;
            }

            bool returnVal;
            return bool.TryParse(value, out returnVal) && returnVal;
        }

        public static T To<T>(this string value)
        {

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    return (T) (object) value.ToBool();
                case TypeCode.Double:
                    return (T) (object) value.ToDouble();
                case TypeCode.Int32:
                    return (T) (object) value.ToInt();
                default:
                    throw new InvalidCastException($"Cannot convert from object to {typeof(T)}");

            }
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool Contains(this List<string> source, string toCheck, StringComparison comp)
        {
            return source.Any(item => item.Contains(toCheck, comp));
        }

        public static object Convert(this string value, string convertTo)
        {
            if (value == null)
            {
                return null;
            }

            switch (convertTo.ToLower())
            {
                case "int":
                    return value.To<int>();
                case "double":
                    return value.To<int>();
                case "boolean":
                case "bool":
                    return value.To<bool>();
                default:
                    return value;
            }
        }
    }
}
