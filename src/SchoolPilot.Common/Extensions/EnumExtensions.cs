

using SchoolPilot.Common.Attributes;
using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace SchoolPilot.Common.Extensions
{
    public static class EnumExtensions
    {
        private static readonly Dictionary<Enum, DescriptionAttribute> EnumDescriptions;

        private static readonly HashSet<Enum> ExcludedEnumsFromDescriptions;

        static EnumExtensions()
        {
            var result = EnumExtensionHelper.GetEnumDescriptions();
            EnumDescriptions = result.Descriptions;
            ExcludedEnumsFromDescriptions = result.ExcludedEnums;
        }

        [DebuggerStepThrough]
        public static string GetDescription<TEnum>(this int value) where TEnum : struct, Enum
        {
            return EnumConverter<TEnum>.FromInt32(value).GetDescription();
        }

        [DebuggerStepThrough]
        public static string GetDescription<TEnum>(this long value) where TEnum : struct, Enum
        {
            return EnumConverter<TEnum>.FromInt64(value).GetDescription();
        }

        [DebuggerStepThrough]
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            return value.GetMetadata().Description;
        }

        public static string GetDescriptionOrDefault<TEnum>(this TEnum? value, string defaultDescription = "") where TEnum : struct, Enum
        {
            return value?.GetMetadata().Description ?? defaultDescription;
        }

        [DebuggerStepThrough]
        public static string GetEnumDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            return value?.GetMetadata().Description;
        }

        private static DescriptionAttribute GetMetadata<TEnum>(this TEnum value) where TEnum : Enum
        {
            if (!EnumDescriptions.TryGetValue(value, out var metadata))
            {
                var description = value.ToString();
                metadata = new DescriptionAttribute(description != "0" ? description : "");

                //If the description is empty or None then don't store it in the enum descriptions list. 
                //This is because doing so would bypass the ExcludeEnumValue attribute and cause it to be included in usages of GetValues.
                //There is no reason for None to have not have been in the EnumDescriptions dictionary already if it did not have the ExcludeEnumValue attribute.
                if (description != "" && description != "None" && !ExcludedEnumsFromDescriptions.Contains(value))
                {
                    EnumDescriptions[value] = metadata;
                }
            }

            return metadata;
        }

        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct, Enum
        {
            long lValue = 0;
            foreach (T flag in flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        public static IEnumerable<string> GetDescriptionsAsText<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            return GetFlags(value).Select(GetDescription);
        }

        public static IEnumerable<string> GetDescriptionsAsTextForLongEnum<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            return GetFlagsFromLongEnums(value).Select(GetDescription);
        }

        public static IEnumerable<EnumValueModel> GetValues<TEnum>()
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var enumType = typeof(TEnum);
            var underlyingType = Enum.GetUnderlyingType(enumType);
            var isInt = underlyingType == typeof(int);

            foreach (TEnum value in Enum.GetValues(enumType))
            {
                // Get field info for the enum value
                var fieldInfo = enumType.GetField(value.ToString());
                if (fieldInfo == null)
                    continue;

                // Skip if marked with [ExcludeEnumValue]
                if (fieldInfo.GetCustomAttribute<ExcludeEnumValueAttribute>() != null)
                    continue;

                // Get [Description] attribute if it exists
                var descriptionAttr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                var name = descriptionAttr?.Description ?? value.ToString();

                yield return new EnumValueModel
                {
                    Name = name,
                    Value = isInt ? (object)Convert.ToInt32(value)
                                  : Convert.ToInt64(value).ToString()
                };
            }
        }
        /// Returns an alphabetically ordered values of enums. It takes in IEnumerable of EnumValue models as the
        /// first parameter,the second parameter is a List of 2 elements with the first TEnum inserted at the top and the second TEnum inserted at the bottom of the ordered list
        /// </summary>
        /// <param name="values"></param>
        /// <param name="wrapperEnums"></param>
        /// <returns></returns>
        public static IEnumerable<EnumValueModel> Alphabetize<TEnum>(this IEnumerable<EnumValueModel> values, List<TEnum?> wrapperEnums) where TEnum : struct, Enum
        {
            var alphabetizeList = new List<EnumValueModel>();
            var enumValueModels = values.OrderBy(x => x.Name).ToList();

            EnumValueModel startEnumValue = null;
            if (wrapperEnums != null && wrapperEnums[0].HasValue)
            {
                var startItem = GetDescription((TEnum)wrapperEnums[0]);
                startEnumValue = enumValueModels.FirstOrDefault(x => x.Name == startItem);
            }

            EnumValueModel endEnumValue = null;
            if (wrapperEnums != null && wrapperEnums[1].HasValue)
            {
                var endItem = GetDescription((TEnum)wrapperEnums[1]);
                endEnumValue = enumValueModels.FirstOrDefault(x => x.Name == endItem);
            }

            if (startEnumValue != null)
            {
                alphabetizeList.Add(startEnumValue);
            }

            foreach (var enumValueModel in enumValueModels)
            {
                if (startEnumValue != null && startEnumValue.Name == enumValueModel.Name)
                {
                    continue;
                }

                if (endEnumValue != null && endEnumValue.Name == enumValueModel.Name)
                {
                    continue;
                }

                alphabetizeList.Add(enumValueModel);
            }

            if (endEnumValue != null)
            {
                alphabetizeList.Add(endEnumValue);
            }

            return alphabetizeList;
        }

        public static IEnumerable<TEnum> GetFlags<TEnum>(TEnum input) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return FindFlags(EnumConverter<TEnum>.ToInt32(input)).Cast<TEnum>();
        }

        [DebuggerStepThrough]
        public static IEnumerable<TEnum> GetFlagsFromLongEnums<TEnum>(TEnum input) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return FindFlags(EnumConverter<TEnum>.ToLong(input)).Cast<TEnum>();
        }

        [DebuggerStepThrough]
        public static IEnumerable<int> GetIntFlags<TEnum>(this TEnum input) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return FindFlags(EnumConverter<TEnum>.ToInt32(input));
        }

        [DebuggerStepThrough]
        public static IEnumerable<long> GetLongFlags<TEnum>(this TEnum input) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return FindFlags(EnumConverter<TEnum>.ToLong(input));
        }

        [DebuggerStepThrough]
        public static IEnumerable<string> GetLongFlagsAsString<TEnum>(this TEnum input) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return FindFlags(EnumConverter<TEnum>.ToLong(input)).Select(s => s.ToString()).IsNullOrEmpty() ?
                new[] { "0" } : FindFlags(EnumConverter<TEnum>.ToLong(input)).Select(s => s.ToString());
        }

        public static bool ContainsFlag<TEnum>(this TEnum combined, TEnum flag) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var combinedInt = EnumConverter<TEnum>.ToInt32(combined);
            var flagInt = EnumConverter<TEnum>.ToInt32(flag);
            return (combinedInt & flagInt) == flagInt;
        }

        public static bool ContainsLongFlag<TEnum>(this TEnum combined, TEnum flag) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var combinedLong = EnumConverter<TEnum>.ToLong(combined);
            var flagLong = EnumConverter<TEnum>.ToLong(flag);
            return (combinedLong & flagLong) == flagLong;
        }

        /// <summary>
        /// Tests that a flagged enum is set to only a single value, by testing if it is a power of 2.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAlone<TEnum>(this TEnum input) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var value = EnumConverter<TEnum>.ToInt32(input);
            return IsPowerOfTwo(value);
        }

        private static bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        public static IEnumerable<int> FindFlags(int flags)
        {
            for (int shift = 0, potentialFlag = 1;
                //continues until we are at a number larger than the flags int,
                //or until we have shifted through every bit (flag uses the last bit 30, 31 is the sign)
                potentialFlag <= flags && shift < 31;
                shift += 1, potentialFlag = 1 << shift)
            {
                if ((flags & potentialFlag) == potentialFlag)
                {
                    yield return potentialFlag;
                }
            }
        }

        public static IEnumerable<long> FindFlags(long flags)
        {
            var potentialFlag = 1L;
            for (var shift = 0;
                //continues until we are at a number larger than the flags long,
                //or until we have shifted through every bit (flag uses the last bit 62, 63 is the sign)
                potentialFlag <= flags && shift < 63;
                shift += 1, potentialFlag = 1L << shift)
            {
                if ((flags & potentialFlag) == potentialFlag)
                {
                    yield return potentialFlag;
                }
            }
        }

        public static TEnum Max<TEnum>(TEnum input, TEnum other) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return EnumConverter<TEnum>.FromInt32((Math.Max(EnumConverter<TEnum>.ToInt32(input), EnumConverter<TEnum>.ToInt32(other))));
        }

        /// <summary>
        /// Get the numeric value of the enum as a string.
        /// </summary>
        public static string GetNumericString<TEnum>(this TEnum source) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return EnumConverter<TEnum>.ToInt32(source).ToString();
        }

        /// <summary>
        /// Get the numeric value of the enum as a string, uses optional enum default if null.
        /// </summary>
        public static string GetNumericStringOrDefault<TEnum>(this TEnum? source, TEnum defaultValue) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return EnumConverter<TEnum>.ToInt32(source ?? defaultValue).ToString();
        }

        public static RepeatOnDay ToRepeatEnum(this DayOfWeek target)
        {
            return (RepeatOnDay)Math.Pow(2, (int)target);
        }

        /// <summary>
        /// Converts a non-flagged enum to a flagged enum.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TEnumFlagged"></typeparam>
        /// <param name="target"></param>
        /// <param name="adjustEnum">This parameter should be set to true when both the flagged enum abd target enum count start from 1</param>
        /// 
        /// <returns></returns>
        public static TEnumFlagged ToFlaggedEnum<TEnum, TEnumFlagged>(this TEnum target, bool adjustEnum = false)
            where TEnum : struct, Enum
            where TEnumFlagged : struct, Enum
        {
            var targetIntValue = EnumConverter<TEnum>.ToInt32(target);

            if (targetIntValue == 0 && Enum.IsDefined(typeof(TEnumFlagged), 0))
            {
                return EnumConverter<TEnumFlagged>.FromInt32(0);
            }

            if (adjustEnum)
            {
                targetIntValue--;
            }

            var pow = (int)Math.Pow(2, targetIntValue);
            return EnumConverter<TEnumFlagged>.FromInt32(pow);
        }

        /// <summary>
        /// Allows sorting of an enum in the database by the enum's description
        /// https://stackoverflow.com/a/40203664
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Expression<Func<TSource, int>> DescriptionOrder<TSource, TEnum>(this Expression<Func<TSource, TEnum>> source)
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            var body = ((TEnum[])Enum.GetValues(enumType))
                .OrderBy(value => value.GetDescription())
                .Select((value, ordinal) => new
                {
                    value,
                    ordinal
                })
                .Reverse()
                .Aggregate((Expression)null, (next, item) => next == null
                    ? (Expression)
                    Expression.Constant(item.ordinal)
                    : Expression.Condition(
                        Expression.Equal(source.Body, Expression.Constant(item.value)),
                        Expression.Constant(item.ordinal),
                        next));

            return Expression.Lambda<Func<TSource, int>>(body, "enumOrder", source.Parameters);
        }

        public static Expression<Func<TSource, int>> DescriptionOrder<TSource, TEnum>(this Expression<Func<TSource, TEnum?>> source)
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            var values = ((TEnum[])Enum.GetValues(enumType))
                .OrderBy(value => value.GetDescription())
                .Select((value, ordinal) => new
                {
                    value = (TEnum?)value,
                    ordinal = ordinal + 1 //the null value will be the first
                })
                .ToList();

            values.Reverse();

            //Add null as the first option, which is now the end of the reversed list
            values.Add(new
            {
                value = (TEnum?)null,
                ordinal = 0
            });

            var body = values
                .Aggregate((Expression)null, (next, item) => next == null
                    ? (Expression)
                    Expression.Constant(item.ordinal)
                    : Expression.Condition(
                        Expression.Equal(source.Body, Expression.Constant(item.value, typeof(TEnum?))),
                        Expression.Constant(item.ordinal),
                        next));

            return Expression.Lambda<Func<TSource, int>>(body, "enumOrder", source.Parameters);
        }

        public static Expression<Func<TSource, int>> DescriptionFlaggedOrder<TSource, TEnum>(this Expression<Func<TSource, TEnum>> source)
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            var underlyingEnumType = enumType.GetEnumUnderlyingType();
            var body = GetAllEnums<TEnum>()
                .OrderBy(value => underlyingEnumType == typeof(long) ? value.ConcatenateFlaggedLongEnum() : value.ConcatenateFlaggedEnum())
                .Select((value, ordinal) => new
                {
                    value,
                    ordinal
                })
                .Reverse()
                .Aggregate((Expression)null, (next, item) => next == null
                   ? (Expression)
                   Expression.Constant(item.ordinal)
                   : Expression.Condition(
                       Expression.Equal(source.Body, Expression.Constant(item.value)),
                       Expression.Constant(item.ordinal),
                       next));

            return Expression.Lambda<Func<TSource, int>>(body, source.Parameters[0]);
        }

        public static Expression<Func<TSource, int>> DescriptionFlaggedOrder<TSource, TEnum>(this Expression<Func<TSource, TEnum?>> source)
            where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            var underlyingEnumType = enumType.GetEnumUnderlyingType();
            var values = GetAllEnums<TEnum>()
                .OrderBy(value => underlyingEnumType == typeof(long) ? value.ConcatenateFlaggedLongEnum() : value.ConcatenateFlaggedEnum())
                .Select((value, ordinal) => new
                {
                    value = (TEnum?)value,
                    ordinal = ordinal + 1 //the null value will be the first
                })
                .Reverse()
                .ToList();

            //Add null as the first option, which is now the end of the reversed list
            values.Add(new
            {
                value = (TEnum?)null,
                ordinal = 0
            });

            var body = values.Aggregate((Expression)null, (next, item) => next == null
               ? (Expression)
               Expression.Constant(item.ordinal)
               : Expression.Condition(
                   Expression.Equal(source.Body, Expression.Constant(item.value, typeof(TEnum?))),
                   Expression.Constant(item.ordinal),
                   next));

            return Expression.Lambda<Func<TSource, int>>(body, source.Parameters[0]);
        }

        /// <summary>
        /// Get an expression tree tied to the given expression that provides an integer value for sorting
        /// </summary>
        /// <param name="underlyingType">The enum type to be sorted</param>
        /// <param name="memberExpression">The member expression of the enum typed field to be sorted</param>
        /// <param name="propertyType">The type of the field, given in case of nullable enum</param>
        /// <param name="isFlagged"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static Expression EnumDescriptionOrder<T>(Type propertyType, Type underlyingType, MemberExpression memberExpression, bool isFlagged, bool isNullable)
        {
            if (!underlyingType.IsEnum)
            {
                throw new InvalidOperationException();
            }

            var values = Enum.GetValues(underlyingType).Cast<object>().ToList();
            var expressionType = typeof(T);
            Expression body;

            //This ordering also accounts for the order within the each enum value, i.e. Blue | Red | Green would be considered as Blue | Green | Red.
            //This necessitates that the query using this ordering has each enum sorted within itself, otherwise the result will not look to be ordered
            if (isFlagged)
            {
                var zeroFlag = Enum.Parse(underlyingType, "0");
                var zeroConstant = Expression.Constant(Convert.ChangeType(0, expressionType));

                //OrderBy so that the correct order has an decreasing flag value.
                //e.g. A -> B -> C would get A = 4, B = 2, C = 1
                //Remove the 0 value, as it is inherently accounted for
                var orderedValues = values
                    .Where(w => zeroFlag.ToString() != w.ToString())
                    .OrderByDescending(o => ((Enum)o).GetEnumDescription())
                    .Select((value, ordinal) => new
                    {
                        typedValue = Convert.ChangeType(value, expressionType),
                        value = expressionType == typeof(long) ? Convert.ToInt64(value) : Convert.ToInt32(value),
                        orderedFlagValue = Convert.ChangeType(Math.Pow(2, ordinal), expressionType)
                    })
                    .ToList();

                /*
                 * The below algorithm is designed to calculate a flagged enum permutation's sort value, based on the flags set
                 * There are 4 scenarios defined for each flag, shown below with their order cost
                 * 1. Flag is missing and there are no lower flags => Add no cost
                 * 2. Flag is present and there are no lower flags => Add 1
                 * 3. Flag is present and there are lower flags => Add 1
                 * 4. Flag is missing and there are lower flags => Add flag bit value, so that regardless of lower flags this value can never be better than those with it set
                 * 
                 * Example Proof: The below 4 flagged enum with values A,B,C,D is in order of ascending sort
                 * _,_,_,_  -> 0+0+0+0 = 0
                 * A,_,_,_  -> 1+0+0+0 = 1
                 * A,B,_,_  -> 1+1+0+0 = 2
                 * A,B,C,_  -> 1+1+1+0 = 3
                 * A,B,C,D  -> 1+1+1+1 = 4
                 * A,B,_,D  -> 1+1+2+1 = 5
                 * A,_,C,_  -> 1+4+1+0 = 6
                 * A,_,C,D  -> 1+4+1+1 = 7
                 * A,_,_,D  -> 1+4+2+1 = 8
                 * _,B,_,_  -> 8+1+0+0 = 9
                 * _,B,C,_  -> 8+1+1+0 = 10
                 * _,B,C,D  -> 8+1+1+1 = 11
                 * _,B,_,D  -> 8+1+2+1 = 12
                 * _,_,C,_  -> 8+4+1+0 = 13
                 * _,_,C,D  -> 8+4+1+1 = 14
                 * _,_,_,D  -> 8+4+2+1 = 15
                 */

                Expression aggregateExpression = null;
                for (var j = orderedValues.Count - 1; j >= 0; j--)
                {
                    var currentOrderedFlag = orderedValues[j];
                    dynamic lowerFlagsValue = expressionType == typeof(long) ? 0L : 0;

                    //Get all of the actual values of the flags that are of a lower ordered value
                    for (var k = j - 1; k >= 0; k--)
                    {
                        var lowerFlag = orderedValues[k];
                        lowerFlagsValue += lowerFlag.value;
                    }

                    //Everything is using the underlying number type instead of the enum type, since the And bit expression can not be used with enums.
                    var typedMemberExpression = Expression.Convert(memberExpression, expressionType);

                    //This checks if the current flag value is set in the db value.
                    var currentActualFlagConstant = Expression.Constant(currentOrderedFlag.typedValue, expressionType);
                    var hasCurrentFlagExpression = Expression.And(currentActualFlagConstant, typedMemberExpression);

                    //This checks if any of the lower flags are set in the db value
                    var lowerFlagsConstant = Expression.Constant(Convert.ChangeType(Enum.Parse(underlyingType, lowerFlagsValue.ToString()), expressionType), expressionType);
                    var hasLowerFlagsExpression = Expression.And(lowerFlagsConstant, typedMemberExpression);

                    var orderedExpression =

                        //if (current flag & db value == 0) => if the current flag is NOT set
                        Expression.Condition(Expression.Equal(hasCurrentFlagExpression, zeroConstant),

                            //if (lower flags & db value == 0) => if NO lower flags are set
                            Expression.Condition(Expression.Equal(hasLowerFlagsExpression, zeroConstant),

                                //current flag is not set and no lower flags are set -> add no penalty value
                                zeroConstant,

                                //current flag is not set and there is a lower flag set -> add ordered flag value
                                Expression.Constant(currentOrderedFlag.orderedFlagValue)
                            ),

                            //current flag is set -> add 1
                            Expression.Constant(Convert.ChangeType(1, expressionType))
                        );

                    if (aggregateExpression == null)
                    {
                        aggregateExpression = orderedExpression;
                    }
                    else
                    {
                        aggregateExpression = Expression.Add(aggregateExpression, orderedExpression);
                    }
                }

                if (aggregateExpression == null)
                {
                    return memberExpression;
                }

                //If the member field is nullable, add a null check that returns the best sort value
                if (isNullable)
                {
                    aggregateExpression =
                        Expression.Condition(Expression.Equal(Expression.Constant(null, propertyType), memberExpression),
                            zeroConstant,
                            aggregateExpression);
                }

                body = aggregateExpression;
            }
            else
            {
                var orderedValues = values
                    .OrderBy(value => ((Enum)value).GetEnumDescription())
                    .Select((value, ordinal) => new
                    {
                        value = Convert.ChangeType(value, underlyingType),
                        ordinal = Convert.ChangeType(ordinal + 1, expressionType) //leave room for null, in case it applies
                    })
                    .Reverse()
                    .ToList();

                if (isNullable)
                {
                    //Add null as the first option, which is now the end of the reversed list
                    orderedValues.Add(new
                    {
                        value = (object)null,
                        ordinal = Convert.ChangeType(0, expressionType)
                    });
                }

                body = orderedValues.Aggregate((Expression)null, (next, item) => next == null
                   ? (Expression)
                   Expression.Constant(Convert.ChangeType(item.ordinal, expressionType))
                   : Expression.Condition(
                       Expression.Equal(memberExpression, Expression.Constant(item.value, propertyType)),
                       Expression.Constant(Convert.ChangeType(item.ordinal, expressionType)),
                       next));
            }

            return body;
        }


        public static TAttribute GetAttribute<TEnum, TAttribute>(this TEnum value)
            where TEnum : struct, Enum
            where TAttribute : Attribute
        {
            return typeof(TEnum).GetField(value.ToString()).GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// Concatenates all the flags together using their descriptions with string.Join. This is for int based enums.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConcatenateFlaggedEnum<TEnum>(this TEnum input, string separator = ", ")
            where TEnum : struct, Enum
        {
            var flags = input.GetIntFlags().Select(f => ((TEnum)Enum.Parse(typeof(TEnum), f.ToString())).GetDescription())
                .OrderBy(o => o)
                .ToList();

            return string.Join(separator, flags);
        }

        /// <summary>
        /// Concatenates all the flags together using their descriptions with string.Join. This is for long based enums.
        /// </summary>
        public static string ConcatenateFlaggedLongEnum<TEnum>(this TEnum input, string separator = ", ")
            where TEnum : struct, Enum
        {
            var flags = input.GetLongFlags().Select(f => ((TEnum)Enum.Parse(typeof(TEnum), f.ToString())).GetDescription())
                .OrderBy(o => o)
                .ToList();

            return string.Join(separator, flags);
        }

        /// <summary>
        /// Used for getting all permutations of a flagged enum
        /// Taken from: https://stackoverflow.com/questions/6117011/how-do-i-get-all-possible-combinations-of-an-enum-flags
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static List<T> GetAllEnums<T>()
            where T : Enum
        {
            // The return type of Enum.GetValues is Array but it is effectively int[] per docs
            // This bit converts to int[]
            var values = Enum.GetValues(typeof(T)).Cast<int>().ToArray();

            if (!typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Any())
            {
                // We don't have flags so just return the result of GetValues
                // ReSharper disable once SuspiciousTypeConversion.Global
                // Its an enum of course its both
                return values.Cast<T>().ToList();
            }

            var valuesInverted = values.Select(v => ~v).ToArray();
            var max = 0;
            for (var i = 0; i < values.Length; i++)
            {
                max |= values[i];
            }

            var result = new List<T>();
            for (var i = 0; i <= max; i++)
            {
                var unaccountedBits = i;
                for (var j = 0; j < valuesInverted.Length; j++)
                {
                    // This step removes each flag that is set in one of the Enums thus ensuring that an Enum with missing bits won't be passed an int that has those bits set
                    unaccountedBits &= valuesInverted[j];
                    if (unaccountedBits == 0)
                    {
                        result.Add((T)(object)i);
                        break;
                    }
                }
            }

            //Check for zero
            try
            {
                if (string.IsNullOrEmpty(Enum.GetName(typeof(T), (T)(object)0)))
                {
                    result.Remove((T)(object)0);
                }
            }
            catch
            {
                result.Remove((T)(object)0);
            }

            return result;
        }

        public static TEnum Parse<TEnum>(string target) where TEnum : struct, Enum
        {
            Enum.TryParse(target, out TEnum result);
            return result;
        }

        public static TEnum ParseDescription<TEnum>(string description, bool caseSensitive = true) where TEnum : struct, Enum
        {
            var values = Enum.GetValues(typeof(TEnum));
            if (caseSensitive)
            {
                foreach (var item in values)
                {
                    if (((TEnum)item).GetDescription().Equals(description))
                    {
                        return (TEnum)item;
                    }
                }
            }
            else
            {
                var descriptionLower = description.ToLower();
                foreach (var item in values)
                {
                    if (((TEnum)item).GetDescription().ToLower().Equals(descriptionLower))
                    {
                        return (TEnum)item;
                    }
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Adds or removes a bit from a flagged enum, based on the boolean state.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <param name="state"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static TEnum ChangeFlagState<TEnum>(this TEnum value, bool state, TEnum flag) where TEnum : struct, Enum
        {
            var enumBaseValue = EnumConverter<TEnum>.ToInt32(value);
            var bitToChange = EnumConverter<TEnum>.ToInt32(flag);
            if (state)
            {
                enumBaseValue |= bitToChange;
            }
            else
            {
                enumBaseValue &= ~bitToChange;
            }
            return EnumConverter<TEnum>.FromInt32(enumBaseValue);
        }

        public static string GetFlaggedDescription(this Enum enumValue, Type enumType)
        {
            //Get a list of the set enum flags
            var enumValues = Enum.GetValues(enumType).Cast<Enum>()
                .Where(w => enumValue.HasFlag(w)).ToList();

            //If there is more than one flag, remove the default value which is always chosen
            if (enumValues.Count > 1)
            {
                var enumDefault = (Enum)Activator.CreateInstance(enumType);
                enumValues.Remove(enumDefault);
            }

            return string.Join(", ", enumValues.Select(s => s.GetEnumDescription()));
        }

        public class EnumValueModel
        {
            public string Name { get; set; }

            public object Value { get; set; }
        }

        public static string GetName<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            return Enum.GetName(typeof(TEnum), value);
        }

        public static List<TAttribute> GetMultiAttributes<TEnum, TAttribute>() 
            where TEnum : struct, Enum 
            where TAttribute : Attribute
        {
            var customAttributes = new List<TAttribute>();

            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            foreach (var value in values)
            {
                var field = typeof(TEnum).GetField(value.GetName());

                if (field != null)
                {
                    customAttributes.AddRange(field.GetCustomAttributes<TAttribute>().ToList());
                }
            }

            return customAttributes;
        }


        public static bool TryParse(Type enumType, string stringValue, out object enumValue)
        {
            try
            {
                enumValue = Enum.Parse(enumType, stringValue);
                return true;
            }
            catch (Exception)
            {
                enumValue = null;
                return false;
            }
        }

    }
}

public static class EnumConverter<TEnum> where TEnum : struct, IConvertible
{
    public static readonly Func<int, TEnum> FromInt32 = GenerateMethod<int, TEnum>();
    public static readonly Func<long, TEnum> FromInt64 = GenerateMethod<long, TEnum>();
    public static readonly Func<TEnum, int> ToInt32 = GenerateMethod<TEnum, int>();
    public static readonly Func<TEnum, long> ToLong = GenerateMethod<TEnum, long>();

    private static Func<TFrom, TTo> GenerateMethod<TFrom, TTo>()
    {
        var parameter = Expression.Parameter(typeof(TFrom));
        var convert = Expression.Convert(parameter, typeof(TTo));
        return Expression.Lambda<Func<TFrom, TTo>>(convert, parameter).Compile();
    }
}

