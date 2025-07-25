

using SchoolPilot.Common.Extensions;
using System.Diagnostics;

namespace SchoolPilot.Common.Helpers
{
    public class EnumEngine : IEnumEngine
    {
        private readonly Dictionary<string, List<object>> _enumList;

        public EnumEngine()
        {
            var method = typeof(EnumExtensions).GetMethod("GetValues");
            Debug.Assert(method != null, nameof(method) + " != null");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _enumList = assemblies
                .Where(assembly => assembly.FullName != null && assembly.FullName.StartsWith("SchoolPilot"))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsEnum)
                .ToDictionary(type => type.Name, k =>
                {
                    var methodInfo = method.MakeGenericMethod(k);
                    var getValues = (Func<IEnumerable<object>>)Delegate.CreateDelegate(typeof(Func<IEnumerable<object>>), methodInfo);
                    return getValues().ToList();
                }, StringComparer.InvariantCultureIgnoreCase);
        }

        public List<object> GetValues(string enumName)
        {
            return _enumList[enumName];
        }

        public Dictionary<string, List<object>> GetValueDictionary()
        {
            return _enumList;
        }

        public List<string> GetNames()
        {
            return _enumList.Keys.OrderBy(o => o).ToList();
        }
    }
}
