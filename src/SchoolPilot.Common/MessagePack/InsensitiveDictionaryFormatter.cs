//using MessagePack;
//using MessagePack.Formatters;
//using System.Collections.Generic;

//namespace SchoolPilot.Common.MessagePack
//{
//    public class InsensitiveDictionaryFormatter : DictionaryFormatterBase<string, object, Dictionary<string, object>, Dictionary<string, object>.Enumerator, Dictionary<string, object>>
//    {
//        protected override Dictionary<string, object> Create(int count, MessagePackSerializerOptions options)
//        {
//            return new Dictionary<string, object>(count, StringComparer.InvariantCultureIgnoreCase);
//        }

//        protected override void Add(Dictionary<string, object> collection, int index, string key, object value, MessagePackSerializerOptions options)
//        {
//            collection[key] = value;
//        }

//        protected override Dictionary<string, object>.Enumerator GetSourceEnumerator(Dictionary<string, object> source)
//        {
//            return source.GetEnumerator();
//        }

//        protected override Dictionary<string, object> Complete(Dictionary<string, object> intermediateCollection)
//        {
//            // For Dictionary<string, object>, we can just return the collection as-is
//            return intermediateCollection;
//        }
//    }
//}