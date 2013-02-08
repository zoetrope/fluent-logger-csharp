/*--------------------------------------------------------------------------
 * ImplicitQueryString
 * ver 1.0.0.0 (Feb. 18th, 2012)
 *
 * created and maintained by neuecc <ils@neue.cc - @neuecc on Twitter>
 * licensed under Microsoft Public License(Ms-PL)
 * http://implicitquerystring.codeplex.com/
 *--------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Codeplex.Web
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// <para>Parse value to impilicit convert to left basic type(int, long, double, string, datetime, etc...).</para>
        /// <para>If key is not found, left is nullable then return null, else KeyNotFoundException.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <returns>Implicit convertable to basic types(int, long, double, string, datetime, etc...) value.</returns>
        public static ConvertableString ParseValue(this NameValueCollection source, string key)
        {
            return ParseValue(source, key, null);
        }

        /// <summary>
        /// <para>Parse value to impilicit convert to left basic type(int, long, double, string, datetime, etc...).</para>
        /// <para>If key is not found, left is nullable then return null, else KeyNotFoundException.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Implicit convertable to basic types(int, long, double, string, datetime, etc...) value.</returns>
        public static ConvertableString ParseValue(this NameValueCollection source, string key, Func<string, string> converter)
        {
            var values = source.GetValues(key);
            if (values == null) return new ConvertableString(null);

            var value = values[0];
            return new ConvertableString(converter == null ? value : converter(value));
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum. Enum parsing ignoreCase is false.</para>
        /// <para>If key is not found then throw KeyNotFoundException.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnum<T>(this NameValueCollection source, string key)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ParseEnum<T>(source, key, null, false);
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum. Enum parsing ignoreCase is false.</para>
        /// <para>If key is not found then throw KeyNotFoundException.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnum<T>(this NameValueCollection source, string key, Func<string, string> converter)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ParseEnum<T>(source, key, converter, false);
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum.</para>
        /// <para>If key is not found then throw KeyNotFoundException.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key. If key is not found then throw KeyNotFoundException.</param>
        /// <param name="ignoreCase">Is ignore allow parse capital or lower case.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnum<T>(this NameValueCollection source, string key, bool ignoreCase)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ParseEnum<T>(source, key, null, ignoreCase);
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum.</para>
        /// <para>If key is not found then throw KeyNotFoundException.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <param name="ignoreCase">Is ignore allow parse capital or lower case.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnum<T>(this NameValueCollection source, string key, Func<string, string> converter, bool ignoreCase)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var values = source.GetValues(key);
            if (values == null) throw new KeyNotFoundException();

            var value = values[0];
            return (T)Enum.Parse(typeof(T), converter == null ? value : converter(value), ignoreCase);
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Boolean ParseValueOrDefault(this NameValueCollection source, string key, Boolean defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Boolean ParseValueOrDefault(this NameValueCollection source, string key, Boolean defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Boolean result;
                if (value != null && Boolean.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Char ParseValueOrDefault(this NameValueCollection source, string key, Char defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Char ParseValueOrDefault(this NameValueCollection source, string key, Char defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Char result;
                if (value != null && Char.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static SByte ParseValueOrDefault(this NameValueCollection source, string key, SByte defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static SByte ParseValueOrDefault(this NameValueCollection source, string key, SByte defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                SByte result;
                if (value != null && SByte.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Byte ParseValueOrDefault(this NameValueCollection source, string key, Byte defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Byte ParseValueOrDefault(this NameValueCollection source, string key, Byte defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Byte result;
                if (value != null && Byte.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Int16 ParseValueOrDefault(this NameValueCollection source, string key, Int16 defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Int16 ParseValueOrDefault(this NameValueCollection source, string key, Int16 defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Int16 result;
                if (value != null && Int16.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static UInt16 ParseValueOrDefault(this NameValueCollection source, string key, UInt16 defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static UInt16 ParseValueOrDefault(this NameValueCollection source, string key, UInt16 defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                UInt16 result;
                if (value != null && UInt16.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Int32 ParseValueOrDefault(this NameValueCollection source, string key, Int32 defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Int32 ParseValueOrDefault(this NameValueCollection source, string key, Int32 defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Int32 result;
                if (value != null && Int32.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static UInt32 ParseValueOrDefault(this NameValueCollection source, string key, UInt32 defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static UInt32 ParseValueOrDefault(this NameValueCollection source, string key, UInt32 defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                UInt32 result;
                if (value != null && UInt32.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Int64 ParseValueOrDefault(this NameValueCollection source, string key, Int64 defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Int64 ParseValueOrDefault(this NameValueCollection source, string key, Int64 defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Int64 result;
                if (value != null && Int64.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static UInt64 ParseValueOrDefault(this NameValueCollection source, string key, UInt64 defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static UInt64 ParseValueOrDefault(this NameValueCollection source, string key, UInt64 defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                UInt64 result;
                if (value != null && UInt64.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Single ParseValueOrDefault(this NameValueCollection source, string key, Single defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Single ParseValueOrDefault(this NameValueCollection source, string key, Single defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Single result;
                if (value != null && Single.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Double ParseValueOrDefault(this NameValueCollection source, string key, Double defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Double ParseValueOrDefault(this NameValueCollection source, string key, Double defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Double result;
                if (value != null && Double.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Decimal ParseValueOrDefault(this NameValueCollection source, string key, Decimal defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static Decimal ParseValueOrDefault(this NameValueCollection source, string key, Decimal defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                Decimal result;
                if (value != null && Decimal.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Parse value.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed value or default value.</returns>
        public static DateTime ParseValueOrDefault(this NameValueCollection source, string key, DateTime defaultValue)
        {
            return ParseValueOrDefault(source, key, defaultValue, null);
        }

        /// <summary>
        /// <para>Parse value. If fail parsing or key not found then return defaultValue.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed value or default value.</returns>
        public static DateTime ParseValueOrDefault(this NameValueCollection source, string key, DateTime defaultValue, Func<string, string> converter)
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                DateTime result;
                if (value != null && DateTime.TryParse(converter == null ? value : converter(value), out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum. Enum parsing ignoreCase is false.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key. If key is not found then throw KeyNotFoundException.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnumOrDefault<T>(this NameValueCollection source, string key)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ParseEnumOrDefault(source, key, default(T), null, false);
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum. Enum parsing ignoreCase is false.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key. If key is not found then throw KeyNotFoundException.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnumOrDefault<T>(this NameValueCollection source, string key, T defaultValue)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ParseEnumOrDefault(source, key, defaultValue, null, false);
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum. Enum parsing ignoreCase is false.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key. If key is not found then throw KeyNotFoundException.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnumOrDefault<T>(this NameValueCollection source, string key, T defaultValue, Func<string, string> converter)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ParseEnumOrDefault(source, key, defaultValue, converter, false);
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key. If key is not found then throw KeyNotFoundException.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="ignoreCase">Is ignore allow parse capital or lower case.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnumOrDefault<T>(this NameValueCollection source, string key, T defaultValue, bool ignoreCase)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return ParseEnumOrDefault(source, key, defaultValue, null, ignoreCase);
        }

        /// <summary>
        /// <para>Convert to enum. T must be enum.</para>
        /// <para>If fail parsing or key not found then return defaultValue.</para>
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="source">Collection holder.</param>
        /// <param name="key">Value's key. If key is not found then throw KeyNotFoundException.</param>
        /// <param name="defaultValue">Returns default value if parsing failed.</param>
        /// <param name="converter">Convert string to string before parse.</param>
        /// <param name="ignoreCase">Is ignore allow parse capital or lower case.</param>
        /// <returns>Parsed enum.</returns>
        public static T ParseEnumOrDefault<T>(this NameValueCollection source, string key, T defaultValue, Func<string, string> converter, bool ignoreCase)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var values = source.GetValues(key) ?? new string[0];
            if (values.Length != 0)
            {
                var value = values[0];
                T result;
                if (value != null && TryParsePrivate(converter == null ? value : converter(value), ignoreCase, out result))
                {
                    return result;
                }
                else
                {
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Is NameValueCollection contains key.
        /// </summary>
        /// <param name="source">Collection.</param>
        /// <param name="key">Key.</param>
        /// <returns>IsContainsKey</returns>
        public static bool ContainsKey(this NameValueCollection source, string key)
        {
            return source.GetValues(key) != null;
        }

        // for .NET 3.5 compatibility
        static bool TryParsePrivate<T>(string value, bool ignoreCase, out T result)
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }
    }

    /// <summary>
    /// <para>Implicit convertable string. This class is specialized for NameValueCollectionExtensions.</para>
    /// <para>If value is null and convert to struct then throw KeyNotFoundException.</para>
    /// </summary>
    public struct ConvertableString
    {
        readonly string Value;

        public ConvertableString(string value)
        {
            this.Value = value;
        }

        public static implicit operator Boolean(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Boolean.Parse(self.Value);
        }

        public static implicit operator Boolean?(ConvertableString self)
        {
            Boolean value;
            return (self.Value != null && Boolean.TryParse(self.Value, out value))
                ? new Nullable<Boolean>(value)
                : null;
        }

        public static implicit operator Char(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Char.Parse(self.Value);
        }

        public static implicit operator Char?(ConvertableString self)
        {
            Char value;
            return (self.Value != null && Char.TryParse(self.Value, out value))
                ? new Nullable<Char>(value)
                : null;
        }

        public static implicit operator SByte(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return SByte.Parse(self.Value);
        }

        public static implicit operator SByte?(ConvertableString self)
        {
            SByte value;
            return (self.Value != null && SByte.TryParse(self.Value, out value))
                ? new Nullable<SByte>(value)
                : null;
        }

        public static implicit operator Byte(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Byte.Parse(self.Value);
        }

        public static implicit operator Byte?(ConvertableString self)
        {
            Byte value;
            return (self.Value != null && Byte.TryParse(self.Value, out value))
                ? new Nullable<Byte>(value)
                : null;
        }

        public static implicit operator Int16(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Int16.Parse(self.Value);
        }

        public static implicit operator Int16?(ConvertableString self)
        {
            Int16 value;
            return (self.Value != null && Int16.TryParse(self.Value, out value))
                ? new Nullable<Int16>(value)
                : null;
        }

        public static implicit operator UInt16(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return UInt16.Parse(self.Value);
        }

        public static implicit operator UInt16?(ConvertableString self)
        {
            UInt16 value;
            return (self.Value != null && UInt16.TryParse(self.Value, out value))
                ? new Nullable<UInt16>(value)
                : null;
        }

        public static implicit operator Int32(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Int32.Parse(self.Value);
        }

        public static implicit operator Int32?(ConvertableString self)
        {
            Int32 value;
            return (self.Value != null && Int32.TryParse(self.Value, out value))
                ? new Nullable<Int32>(value)
                : null;
        }

        public static implicit operator UInt32(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return UInt32.Parse(self.Value);
        }

        public static implicit operator UInt32?(ConvertableString self)
        {
            UInt32 value;
            return (self.Value != null && UInt32.TryParse(self.Value, out value))
                ? new Nullable<UInt32>(value)
                : null;
        }

        public static implicit operator Int64(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Int64.Parse(self.Value);
        }

        public static implicit operator Int64?(ConvertableString self)
        {
            Int64 value;
            return (self.Value != null && Int64.TryParse(self.Value, out value))
                ? new Nullable<Int64>(value)
                : null;
        }

        public static implicit operator UInt64(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return UInt64.Parse(self.Value);
        }

        public static implicit operator UInt64?(ConvertableString self)
        {
            UInt64 value;
            return (self.Value != null && UInt64.TryParse(self.Value, out value))
                ? new Nullable<UInt64>(value)
                : null;
        }

        public static implicit operator Single(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Single.Parse(self.Value);
        }

        public static implicit operator Single?(ConvertableString self)
        {
            Single value;
            return (self.Value != null && Single.TryParse(self.Value, out value))
                ? new Nullable<Single>(value)
                : null;
        }

        public static implicit operator Double(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Double.Parse(self.Value);
        }

        public static implicit operator Double?(ConvertableString self)
        {
            Double value;
            return (self.Value != null && Double.TryParse(self.Value, out value))
                ? new Nullable<Double>(value)
                : null;
        }

        public static implicit operator Decimal(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return Decimal.Parse(self.Value);
        }

        public static implicit operator Decimal?(ConvertableString self)
        {
            Decimal value;
            return (self.Value != null && Decimal.TryParse(self.Value, out value))
                ? new Nullable<Decimal>(value)
                : null;
        }

        public static implicit operator DateTime(ConvertableString self)
        {
            if (self.Value == null) throw new KeyNotFoundException();
            return DateTime.Parse(self.Value);
        }

        public static implicit operator DateTime?(ConvertableString self)
        {
            DateTime value;
            return (self.Value != null && DateTime.TryParse(self.Value, out value))
                ? new Nullable<DateTime>(value)
                : null;
        }

        public static implicit operator String(ConvertableString self)
        {
            return self.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}