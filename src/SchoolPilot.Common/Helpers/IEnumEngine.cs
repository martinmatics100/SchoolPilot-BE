

using static SchoolPilot.Common.Extensions.EnumExtensions;

namespace SchoolPilot.Common.Helpers
{
    public interface IEnumEngine
    {
        List<object> GetValues(string enumName);

        List<string> GetNames();

        Dictionary<string, List<object>> GetValueDictionary();
    }
}
