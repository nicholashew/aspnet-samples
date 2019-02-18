using System;

namespace ConsoleAppBoilerPlate.Helper
{
    public class SafeConvert
    {
        public static bool IsDBNull(object obj)
        {
            return Convert.IsDBNull(obj);
        }

        public static Guid ToGuid(object obj)
        {
            if (obj != null && obj != DBNull.Value)
            {
                try
                {
                    return new Guid(obj.ToString());
                }
                catch
                {
                    return Guid.Empty;
                }
            }

            return Guid.Empty;
        }

        public static TimeSpan ToTimeSpan(object obj)
        {
            return ToTimeSpan(obj, TimeSpan.Zero);
        }

        public static TimeSpan ToTimeSpan(object obj, TimeSpan defaultValue)
        {
            if (obj != null)
                return ToTimeSpan(obj.ToString(), defaultValue);

            return defaultValue;
        }

        public static TimeSpan ToTimeSpan(string s, TimeSpan defaultValue)
        {
            TimeSpan result;
            bool success = TimeSpan.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        public static TimeSpan ToTimeSpan(string s)
        {
            return ToTimeSpan(s, TimeSpan.Zero);
        }

        public static string ToString(string s)
        {
            return ToString(s, string.Empty);
        }

        public static string ToString(string s, string defaultString)
        {
            if (s == null) return defaultString;

            return s.ToString();
        }

        public static string ToString(object s, string defaultString)
        {
            if (s == null) return defaultString;

            return s.ToString();
        }

        public static double ToDouble(string s, double defaultValue)
        {
            double result;
            bool success = double.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        public static double ToDouble(string s)
        {
            return ToDouble(s, 0);
        }

        public static double ToDouble(object obj)
        {
            return ToDouble(obj, 0);
        }

        public static double ToDouble(object obj, double defaultValue)
        {
            if (obj != null)
                return ToDouble(obj.ToString(), defaultValue);

            return defaultValue;
        }

        public static float ToSingle(string s, float defaultValue)
        {
            float result;
            bool success = float.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        public static float ToSingle(string s)
        {
            return ToSingle(s, 0);
        }

        public static float ToSingle(object obj)
        {
            return ToSingle(obj, 0);
        }

        public static float ToSingle(object obj, float defaultValue)
        {
            if (obj != null)
                return ToSingle(obj.ToString(), defaultValue);

            return defaultValue;
        }

        public static decimal ToDecimal(string s, decimal defaultValue)
        {
            decimal result;
            bool success = decimal.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        public static decimal ToDecimal(string s)
        {
            return ToDecimal(s, 0);
        }

        public static decimal ToDecimal(object obj)
        {
            return ToDecimal(obj, 0);
        }

        public static decimal ToDecimal(object obj, decimal defaultValue)
        {
            if (obj != null)
                return ToDecimal(obj.ToString(), defaultValue);

            return defaultValue;
        }

        public static bool ToBoolean(string s, bool defaultValue)
        {
            if (s.TrimStart('0') == "1")
                return true;

            bool result;
            bool success = bool.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        public static bool ToBoolean(string s)
        {
            return ToBoolean(s, false);
        }

        public static bool ToBoolean(object obj)
        {
            return ToBoolean(obj, false);
        }

        public static bool ToBoolean(object obj, bool defaultValue)
        {
            if (obj != null)
                return ToBoolean(obj.ToString(), defaultValue);

            return defaultValue;
        }

        public static DateTime ToDateTime(string s, DateTime defaultValue)
        {
            DateTime result;
            bool success = DateTime.TryParse(s, out result);

            return success ? result : defaultValue;
        }

        public static DateTime ToDateTime(string s)
        {
            return ToDateTime(s, new DateTime(1900, 1, 1));
        }

        public static DateTime ToDateTime(object obj)
        {
            return ToDateTime(obj, new DateTime(1900, 1, 1));
        }

        public static int ToInt32(object obj)
        {
            return ToInt32(obj, 0);
        }

        public static int ToInt32(object obj, int defaultValue)
        {
            if (obj != null)
            {
                int result;
                if (int.TryParse(obj.ToString(), out result))
                {
                    return result;
                };
            }

            return defaultValue;
        }

        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            if (obj != null)
                return ToDateTime(obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// Parse Enum from string
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ToEnum(Type enumType, string text, object defaultValue)
        {
            if (Enum.IsDefined(enumType, text))
            {
                return Enum.Parse(enumType, text, false);
            }

            return defaultValue;
        }

        /// <summary>
        /// Parse Enum from string
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ToEnum(Type enumType, object obj, object defaultValue)
        {
            if (obj != null)
                return ToEnum(enumType, obj.ToString(), defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// Parse Enum from string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ToEnum<T>(object obj, T defaultValue) where T : struct
        {
            return (T)ToEnum(typeof(T), obj, defaultValue);
        }

        /// <summary>
        /// Parse Enum fron index (value)
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object ToEnum(Type enumType, int index)
        {
            return Enum.ToObject(enumType, index);
        }

        /// <summary>
        /// Parse Enum fron int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(int input, bool throwException = true) where T : struct
        {
            return (T)ParseEnum<T>(input, default(T), throwException);
        }

        /// <summary>
        /// Parse Enum fron int
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(int input, T defaultValue, bool throwException = false) where T : struct
        {
            T returnEnum = defaultValue;
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException("Invalid Enum Type. " + typeof(T).ToString() + "  must be an Enum");
            }
            if (Enum.IsDefined(typeof(T), input))
            {
                returnEnum = (T)Enum.ToObject(typeof(T), input);
            }
            else
            {
                if (throwException)
                {
                    throw new InvalidOperationException("Invalid Cast");
                }
            }

            return returnEnum;
        }
    }
}
