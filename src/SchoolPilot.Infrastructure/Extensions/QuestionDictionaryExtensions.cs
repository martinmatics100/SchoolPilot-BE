

using System.Globalization;
using System.Collections;


namespace SchoolPilot.Infrastructure.Extensions
{
    public static class QuestionDictionaryExtensions
    {
        public static DateTime? GetTimeAnswerAsDateTimeOrNull(this Dictionary<string, object> questions, string key)
        {
            if (!questions.TryGetValue(key, out object value))
            {
                return null;
            }
            if (!DateTime.TryParseExact(value?.ToString(), new[] { "h:mm tt", "hh:mm tt" }, new DateTimeFormatInfo(), DateTimeStyles.NoCurrentDateDefault, out DateTime result))
            {
                return null;
            }

            return result;
        }

        public static DateTime? GetDateAnswerAsDateTimeOrNull(this Dictionary<string, object> questions, string key)
        {
            if (!questions.TryGetValue(key, out object value))
            {
                return null;
            }
            if (!DateTime.TryParseExact(value?.ToString(), new[] { "MM/dd/yyyy", "M/d/yyyy", "MM/dd/yy", "M/d/yy" }, new DateTimeFormatInfo(), DateTimeStyles.NoCurrentDateDefault, out DateTime result))
            {
                return null;
            }

            return result;
        }

        public static DateTime GetDateAnswer(this Dictionary<string, object> questions, string key)
        {
            var value = questions[key];
            if (DateTime.TryParseExact(value?.ToString(), new[] { "MM/dd/yyyy", "M/d/yyyy", "MM/dd/yy", "M/d/yy" }, new DateTimeFormatInfo(), DateTimeStyles.NoCurrentDateDefault, out DateTime result))
            {
                return result;
            }
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Used to get the boolean from a checkbox, which would be from the answer have a value or not.
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetCheckboxBoolean(this Dictionary<string, object> questions, string key)
        {
            if (!questions.TryGetValue(key, out object value) || value == null)
            {
                return false;
            }

            if (value is Array checkboxArray)
            {
                return checkboxArray.Length > 0;
            }

            return !string.IsNullOrWhiteSpace(value.ToString());
        }

        public static bool GetOptionsAsFlaggedEnum<TEnum>(this Dictionary<string, object> questions, string key, TEnum defaultValue, out TEnum response)
            where TEnum : struct, Enum
        {
            var result = 0;
            if (!questions.TryGetValue(key, out var value))
            {
                response = defaultValue;
                return false;
            }

            if (value is object[] types)
            {
                var listOfTypes = types.ToList();
                foreach (var type in listOfTypes)
                {
                    if (int.TryParse(type.ToString(), out var convertedValue))
                    {
                        result |= (int)Math.Pow(2, convertedValue - 1);
                    }
                }
            }

            response = EnumConverter<TEnum>.FromInt32(result);
            return true;
        }

        /// <summary>
        /// Gets the string representation of a value from a question dictionary.
        /// If the value in the question is an array then this will return the first value of the array.
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetStringOrDefault(this Dictionary<string, object> questions, string key, string defaultValue = null)
        {
            if (!questions.TryGetValue(key, out object value))
            {
                return defaultValue;
            }

            switch (value)
            {
                //Need to check if it is string before IEnumerable, because a string is also an IEnumerable.
                case string _:
                    return value.ToString();
                case IEnumerable enumerable:
                    {
                        var enumerator = enumerable.GetEnumerator();
                        return enumerator.MoveNext() ? enumerator.Current?.ToString() : null;
                    }
            }
            return value.ToString();
        }

        public static bool TryParseGetGuidAnswers(this Dictionary<string, object> questions, string key, out Guid id)
        {
            id = Guid.Empty;
            if (!questions.TryGetValue(key, out object value))
            {
                return false;
            }

            if (Guid.TryParse(value.ToString(), out var parsedGuid))
            {
                id = parsedGuid;
            }

            return true;
        }
    }
}
