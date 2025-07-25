//using MessagePack;
//using MessagePack.Formatters;
//using MessagePack.Resolvers;
//using Microsoft.Extensions.DependencyInjection;
//using SchoolPilot.Common.MessagePack;

//namespace SchoolPilot.Host.DIContainer.Modules
//{
//    public static class MessagePackModule
//    {
//        private static bool _isRegistered = false;
//        private static readonly object _lock = new object();

//        public static IServiceCollection AddMessagePackSerialization(this IServiceCollection services)
//        {
//            lock (_lock)
//            {
//                if (!_isRegistered)
//                {
//                    // Create and register the composite resolver
//                    var resolver = CompositeResolver.Create(
//                        new IMessagePackFormatter[] { new InsensitiveDictionaryFormatter() },
//                        new IFormatterResolver[] {
//                            NativeDateTimeResolver.Instance,
//                            StandardResolver.Instance
//                        });

//                    // Configure MessagePack options
//                    var options = MessagePackSerializerOptions.Standard
//                        .WithResolver(resolver);

//                    // Register the options for DI
//                    services.AddSingleton(options);

//                    _isRegistered = true;
//                }
//            }

//            return services;
//        }
//    }
//}