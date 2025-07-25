
namespace SchoolPilot.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static TV GetAndRemoveOrDefault<TK, TV>(this IDictionary<TK, TV> instance, TK key, TV defaultValue)
        {
            if (!instance.TryGetValue(key, out var value) || value == null)
            {
                return defaultValue;
            }
            instance.Remove(key);
            return value;
        }

        public static TV GetAndRemove<TK, TV>(this IDictionary<TK, TV> instance, TK key)
        {
            var value = instance[key];
            instance.Remove(key);
            return value;
        }

        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> instance, TK key)
        {
            instance.TryGetValue(key, out var value);
            if (EqualityComparer<TV>.Default.Equals(value, default(TV)))
            {
                return default(TV);
            }
            return value;
        }

        public static void AddToList<TKey, TValue, TCollectionItem>(this IDictionary<TKey, TValue> dictionary, TKey key, TCollectionItem item)
            where TValue : class, ICollection<TCollectionItem>, new()
        {
            var collection = dictionary.GetOrDefault(key) ?? new TValue();
            collection.Add(item);
            dictionary[key] = collection;
        }

        public static void AddManyToList<TKey, TValue, TCollectionItem>(this IDictionary<TKey, TValue> dictionary, TKey key, ICollection<TCollectionItem> items)
            where TValue : class, ICollection<TCollectionItem>, new()
        {
            var collection = dictionary.GetOrDefault(key) ?? new TValue();
            foreach (var item in items)
            {
                collection.Add(item);
            }
            dictionary[key] = collection;
        }

        /// <summary>
        /// This method retrieves the value for the given key from the dictionary
        /// </summary>
        /// <typeparam name="TK">Generic Type for the Dictionary's Key</typeparam>
        /// <typeparam name="TV">Generic Type for the Dictionary's Value</typeparam>
        /// <param name="instance">The dictionary in which we need to search for</param>
        /// <param name="key">The key of the value which needs to be retrieved from the dictionary</param>
        /// <param name="defaultValue">The default value to return if the value of the given key doesn't exist or if the value is null</param>
        /// <returns>Value or the Default Type of the value stored in the dictionary</returns>
        public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> instance, TK key, TV defaultValue)
        {
            if (!instance.TryGetValue(key, out var value))
            {
                return defaultValue;
            }
            var type = typeof(TV);
            //Only non value types will be checked against their default.
            //This is because we don't want to end up ignoring a valid value in an enum because the valid value is equal to the default.
            if (!type.IsValueType && EqualityComparer<TV>.Default.Equals(value, default(TV)))
            {
                return defaultValue;
            }
            return value;
        }

        public static TV Get<TK, TV>(this IDictionary<TK, TV> instance, TK key)
        {
            instance.TryGetValue(key, out var value);
            return value;
        }

        public static bool IsNotNullOrEmpty<TK, TV>(this Dictionary<TK, TV> dictionary)
        {
            return dictionary != null && dictionary.Count > 0;
        }

        public static bool IsTrue(this Dictionary<string, object> dictionary, string key)
        {
            return dictionary.GetOrDefault(key)?.ToString().ToLower() == "true";
        }

        public static void AddOrSet<TK>(this Dictionary<TK, int> instance, TK key, int additionValue)
        {
            if (instance.TryGetValue(key, out int value))
            {
                instance[key] = value + additionValue;
            }
            else
            {
                instance[key] = additionValue;
            }
        }

        /// <summary>
        /// Add the given dictionary key value pairs to the source dictionary. Overlapping keys will override the source.
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="source"></param>
        /// <param name="dictionaryToAdd"></param>
        public static void AddFromDictionary<TK, TV>(this Dictionary<TK, TV> source, Dictionary<TK, TV> dictionaryToAdd)
        {
            foreach (var keyValue in dictionaryToAdd)
            {
                source[keyValue.Key] = keyValue.Value;
            }
        }
    }
}
