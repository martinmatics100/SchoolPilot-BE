
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using SchoolPilot.Common.Enums;

namespace SchoolPilot.Common.Extensions
{
    public static class StringExtensions
    {
        private static readonly RandomNumberGenerator RandomNumberGenerator;

        private static readonly char[] AllowedRandomCharSet;

        private static HashSet<char> InvalidCharacterLookup;

        private static readonly List<string> LowercaseWords;

        private static readonly string LowercaseWordsRegex;

        static StringExtensions()
        {
            RandomNumberGenerator = RandomNumberGenerator.Create();
            AllowedRandomCharSet = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            //[*, ^, :, ~, |, >, \, +] are all invalid characters when submitting claims.
            //A hash set is used since it is faster than a regex, but does not have the possible out of bounds limitation of an array if a non-Unicode character were somehow used.
            InvalidCharacterLookup = new HashSet<char>
            {
                '*', '^', ':', '~','|','>','\\','+'
            };

            LowercaseWords = new List<string>
            {
                "A",
                "An",
                "And",
                "Of",
                "Or",
                "The"
            };

            LowercaseWordsRegex = $" (?:{string.Join("|", LowercaseWords)}) ";
        }

        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            var chars = s.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                var hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // if the next character is a space, which is not considered uppercase 
                    // (otherwise we wouldn't be here...)
                    // we want to ensure that the following:
                    // 'FOO bar' is rewritten as 'foo bar', and not as 'foO bar'
                    // The code was written in such a way that the first word in uppercase
                    // ends when if finds an uppercase letter followed by a lowercase letter.
                    // now a ' ' (space, (char)32) is considered not upper
                    // but in that case we still want our current character to become lowercase
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = ToLower(chars[i]);
                    }

                    break;
                }

                chars[i] = ToLower(chars[i]);
            }

            return new string(chars);
        }

        private static char ToLower(char c)
        {
            c = char.ToLowerInvariant(c);
            return c;
        }

        public static string ToUndoCamelCase([NotNull] this string str)
        {
            var chars = str.ToCharArray();
            chars[0] = char.ToUpperInvariant(str[0]);
            return string.Join("", chars);
        }

        public static string ToPascalCase([NotNull] this string strs, bool multiple = false)
        {
            if (multiple)
            {
                if (string.IsNullOrWhiteSpace(strs))
                {
                    return strs;
                }

                var array = strs.Split(' ');

                var result = new List<string>();

                foreach (var str in array)
                {
                    // Select first character, uppercase it and concatenate with the rest of the string
                    var processedPart = str.EndsWith("\"")
                        ? ToTitleCase(str.Substring(0, str.Length - 1)) + "\""
                        : ToTitleCase(str);

                    //Add to the result list
                    result.Add(processedPart);
                }

                return string.Join(" ", result);
            }
            else
            {
                // replace any character that is not Letter or digit or . with _
                var sample = string.Join("", strs.Select(c => char.IsLetterOrDigit(c) || c == '.' ? c.ToString() : "_").ToArray());

                // Split the resulting string by underscore
                // Select first character, uppercase it and concatenate with the rest of the string
                var arr = sample
                    .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => $"{s.Substring(0, 1).ToUpper()}{s.Substring(1)}");

                // Join the resulting collection
                sample = string.Join("", arr);

                return sample;
            }

            string ToTitleCase(string input)
            {
                var parts = input.Split('-').Select(s =>
                {
                    if (!string.IsNullOrEmpty(s) && s.Length > 1)
                    {
                        return char.ToUpper(s[0]) + s.Substring(1);
                    }
                    else if (!string.IsNullOrEmpty(s)) // Single letter part
                    {
                        return char.ToUpper(s[0]).ToString(); // Capitalize single letter
                    }
                    else
                    {
                        return string.Empty; // or handle empty part as needed
                    }
                });

                return string.Join("-", parts);
            }
        }

        /// <summary>
        /// Uses a lookup hashset to determine which special characters should be removed.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string TrimAndRemoveSpecialCharacters(this string str)
        {
            var buffer = new char[str.Length];
            var startIndex = 0;
            var endIndex = -1;
            var currentIndex = 0;
            foreach (var character in str)
            {
                //Increment the starting index past beginning whitespaces. Once a valid, non white-space character is found, start index will no longer increment.
                if (char.IsWhiteSpace(character) && startIndex == currentIndex)
                {
                    startIndex = ++currentIndex;
                }
                else if (!InvalidCharacterLookup.Contains(character))
                {
                    //If a valid, non white-space character is found, set the end index to the current index.
                    if (!char.IsWhiteSpace(character))
                    {
                        endIndex = currentIndex;
                    }

                    buffer[currentIndex++] = character;
                }
            }

            //If the endIndex never moved, then no valid, non white-space character was found.
            //Otherwise, the plus 1 is to be inclusive of the last non-white character, which endIndex marks.
            return endIndex == -1 ? "" : new string(buffer, startIndex, endIndex - startIndex + 1);
        }

        public static string TrimAndRemoveAllSpecialCharacters(this string str)
        {
            InvalidCharacterLookup = new HashSet<char>
            {
                '&', '!', '#', '$', '(', ')', '*', '+', ',', ':', ';', '=', '?', '@', '[', ']', '%',
                '{', '}', '"', '\'', '-', '/', '.', '^', '|', '~', '<', '>'
            };

            return str.TrimAndRemoveSpecialCharacters();
        }

        public static string RemoveDigits(this string target)
        {
            if (target == null)
            {
                return null;
            }

            return Regex.Replace(target, @"\d", string.Empty);
        }

        public static string GenerateRandomString(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "length cannot be less than zero.");
            }

            const int byteSize = 0x100;

            if (byteSize < AllowedRandomCharSet.Length)
            {
                throw new ArgumentException($"allowedChars may contain no more than {byteSize} characters.");
            }

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            var result = new char[length];
            var buffer = new byte[128];
            var index = 0;
            var outOfRangeStart = byteSize - byteSize % AllowedRandomCharSet.Length;
            while (index < length)
            {
                RandomNumberGenerator.GetBytes(buffer);
                for (var i = 0; i < buffer.Length && index < length; ++i)
                {
                    // Divide the byte into allowedCharSet-sized groups. If the
                    // random value falls into the last group and the last group is
                    // too small to choose from the entire allowedCharSet, ignore
                    // the value in order to avoid biasing the result.

                    if (outOfRangeStart <= buffer[i])
                    {
                        continue;
                    }

                    result[index] = AllowedRandomCharSet[buffer[i] % AllowedRandomCharSet.Length];
                    index++;
                }
            }

            return new string(result);
        }

        private static Regex phoneNumbeRegex = new Regex(@"(\d{3})(\d{3})(\d{4})", RegexOptions.Compiled);

        public static string FormatPhoneNumber(this string str, string extension = null)
        {
            if (str.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (str.Length == 10)
            {
                var result = phoneNumbeRegex.Replace(str, "($1) $2-$3");
                if (!extension.IsNullOrEmpty())
                {
                    result += " x" + extension;
                }
                return result;
            }

            return str;
        }

        public static bool IsNullEmptyOrNA(this string str)
        {
            return str.IsNullOrEmpty() || str.Equals("^");
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return str.IsNullOrEmpty() || str.Trim().Length == 0;
        }

        public static string Truncate(this string str, int maxLength, bool returnEmptyAsNull = false)
        {
            if (str == "")
            {
                return returnEmptyAsNull ? null : "";
            }

            if (str != null)
            {
                return str.Length > maxLength ? str.Substring(0, maxLength) : str;
            }

            return null;
        }

        public static string SplitOnCamelCase(this string str)
        {
            return Regex.Replace(str, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static DateTime ParseAsTime(this string str)
        {
            DateTime.TryParseExact(str, new[]
            {
                "h:mm tt", "hh:mm tt"
            }, new DateTimeFormatInfo(), DateTimeStyles.NoCurrentDateDefault, out var result);

            return result;
        }

        /// <summary>
        /// Retrieves a substring of the specified length from the given string.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="length">The desired length of the substring.</param>
        /// <returns>
        /// If the input string is null or empty, an empty string is returned.
        /// If the input string's length is less than the specified length, the entire string is returned.
        /// Otherwise, a substring of the specified length is returned.
        /// </returns>
        public static string GetSubstring(this string str, int length)
        {
            if (str.IsNullOrEmpty())
            {
                return "";
            }

            return str.Length < length ? str : str.Substring(0, length);
        }

        public static Gender? MapGenderString(this string gender)
        {
            switch (gender.ToLower())
            {
                case "male":
                    return Gender.Male;
                case "female":
                    return Gender.Female;
                default:
                    return null;
            }
        }


        public static string Concatenate(this IEnumerable<string> items)
        {
            var itemList = items.ToList();
            switch (itemList.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return itemList[0];
                case 2:
                    return string.Join(" and ", itemList);
                default:
                    return string.Join(", ", itemList.Take(itemList.Count - 1)) + ", and " + itemList.Last();
            }
        }

        public static string ConvertToNormalText(this string text, bool preserveAcronyms = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var newTextBuilder = new StringBuilder(text.Length * 2);

            newTextBuilder.Append(text[0]);

            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1]))
                        || (preserveAcronyms
                            && char.IsUpper(text[i - 1])
                            && i < text.Length - 1
                            && !char.IsUpper(text[i + 1])))
                        newTextBuilder.Append(' ');

                newTextBuilder.Append(text[i]);
            }

            var newText = newTextBuilder.ToString();
            if (Regex.IsMatch(newText, LowercaseWordsRegex))
            {
                foreach (var word in LowercaseWords)
                {
                    newText = Regex.Replace(newText, $" {word} ", $" {word.ToLower()} ");
                }
            }

            return newText;
        }

    }
}
