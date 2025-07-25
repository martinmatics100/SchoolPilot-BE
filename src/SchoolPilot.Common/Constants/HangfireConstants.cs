

namespace SchoolPilot.Common.Constants
{
    public static class HangfireConstants
    {
        public const string ApiQueue = "api";
        public const string DefaultQueue = "default";
        public const string AgentQueue = "agent";
        public const string BackgroundQueue = "background";
        public const int DataCenterTransitionWindow = 30;

        public static readonly List<string> DefinedQueues = new List<string>
        {
            AgentQueue,
            ApiQueue,
            BackgroundQueue,
            DefaultQueue,
        };

    }
}
