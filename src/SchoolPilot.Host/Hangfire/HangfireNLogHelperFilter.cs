

using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Newtonsoft.Json;
using NLog;
using SchoolPilot.Common.Constants;
using SchoolPilot.Common.Exceptions;

namespace SchoolPilot.Host.Hangfire
{
    public class HangfireNLogHelperFilter : IServerFilter, IClientFilter, IElectStateFilter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnPerforming(PerformingContext filterContext)
        {
            MappedDiagnosticsLogicalContext.Set("HangfireBackgroundJobId", filterContext.BackgroundJob.Id);

            var actionId = Guid.NewGuid().ToString("N");
            MappedDiagnosticsLogicalContext.Set(LoggingConstants.ActionId, actionId);

            var currentValue = filterContext.GetJobParameter<string>(LoggingConstants.TraceGlob) ?? "0:";
            var nextValue = currentValue + actionId + ":";
            MappedDiagnosticsLogicalContext.Set(LoggingConstants.TraceGlob, nextValue);
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }

        public void OnCreating(CreatingContext filterContext)
        {
            var currentGlob = MappedDiagnosticsLogicalContext.Get(LoggingConstants.TraceGlob) ?? "0:";
            filterContext.SetJobParameter(LoggingConstants.TraceGlob, currentGlob);
        }

        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState != null)
            {
                if (failedState.Exception is InvalidDataCenterException)
                {
                    _logger.Info($"Delaying Job: {context.BackgroundJob.Id} by {HangfireConstants.DataCenterTransitionWindow} minutes");

                    var delayTime = TimeSpan.FromMinutes(HangfireConstants.DataCenterTransitionWindow);
                    var delayedScheduleState = new ScheduledState(delayTime)
                    {
                        Reason = "DataCenter Fail-over"
                    };

                    context.CandidateState = delayedScheduleState;
                }
                else
                {
                    var exception = JsonConvert.SerializeObject(failedState.SerializeData());

                    _logger.Error(
                        "Job `{0}` has been failed due to an exception. `{1}`",
                        context.BackgroundJob.Id,
                        exception);
                }
            }
        }
    }
}
