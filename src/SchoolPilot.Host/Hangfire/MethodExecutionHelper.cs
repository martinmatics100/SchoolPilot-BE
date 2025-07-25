

using Hangfire;
using JetBrains.Annotations;
using SchoolPilot.Common.Constants;
using SchoolPilot.Common.Enums;
using SchoolPilot.Common.Interfaces;
using System.Linq.Expressions;
using Hangfire.MetaExtensions;

namespace SchoolPilot.Host.Hangfire
{
    public class MethodExecutionHelper : IMethodExecutionHelper
    {
        private readonly IBackgroundJobClient _client;

        public MethodExecutionHelper(IBackgroundJobClient client)
        {
            _client = client;
        }

        public string ExecuteOnApi<T>([InstantHandle] Expression<Action<T>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Api);
        }

        public string ExecuteOnApi<T>([InstantHandle] Expression<Func<T, Task>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Api);
        }

        public string ExecuteOnAgent<T>([InstantHandle] Expression<Action<T>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Agent);
        }

        public string ExecuteOnAgent<T>([InstantHandle] Expression<Func<T, Task>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Agent);
        }

        public string ExecuteOnAnything<T>([InstantHandle] Expression<Action<T>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Any);
        }

        public string ExecuteOnAnything<T>([InstantHandle] Expression<Func<T, Task>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Any);
        }

        public string ExecuteInBackground<T>([InstantHandle] Expression<Action<T>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Background);
        }

        public string ScheduleInBackground<T>([InstantHandle] Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
        {
            return MoveSchedulingTo(methodCall, enqueueAt, ExecutionService.Background);
        }

        public string ScheduleInBackground<T>([InstantHandle] Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
        {
            return MoveSchedulingTo(methodCall, enqueueAt, ExecutionService.Background);
        }

        public string ExecuteInBackground<T>([InstantHandle] Expression<Func<T, Task>> methodCall)
        {
            return MoveExecutionTo(methodCall, ExecutionService.Background);
        }

        public string MoveExecutionTo<T>([InstantHandle] Expression<Action<T>> methodCall, ExecutionService service)
        {
            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }
            string queue;
            switch (service)
            {
                case ExecutionService.Agent:
                    queue = HangfireConstants.AgentQueue;
                    break;
                case ExecutionService.Api:
                    queue = HangfireConstants.ApiQueue;
                    break;
                case ExecutionService.Any:
                    queue = HangfireConstants.DefaultQueue;
                    break;
                case ExecutionService.Background:
                    queue = HangfireConstants.BackgroundQueue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(service), service, null);
            }
            return _client.UseQueue(queue).Enqueue(methodCall);
        }

        public string MoveExecutionTo<T>([InstantHandle] Expression<Func<T, Task>> methodCall, ExecutionService service)
        {
            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }
            string queue;
            switch (service)
            {
                case ExecutionService.Agent:
                    queue = HangfireConstants.AgentQueue;
                    break;
                case ExecutionService.Api:
                    queue = HangfireConstants.ApiQueue;
                    break;
                case ExecutionService.Any:
                    queue = HangfireConstants.DefaultQueue;
                    break;
                case ExecutionService.Background:
                    queue = HangfireConstants.BackgroundQueue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(service), service, null);
            }
            return _client.UseQueue(queue).Enqueue(methodCall);
        }

        public string MoveSchedulingTo<T>([InstantHandle] Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, ExecutionService service)
        {
            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }
            string queue;
            switch (service)
            {
                case ExecutionService.Agent:
                    queue = HangfireConstants.AgentQueue;
                    break;
                case ExecutionService.Api:
                    queue = HangfireConstants.ApiQueue;
                    break;
                case ExecutionService.Any:
                    queue = HangfireConstants.DefaultQueue;
                    break;
                case ExecutionService.Background:
                    queue = HangfireConstants.BackgroundQueue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(service), service, null);
            }
            return _client.UseQueue(queue).Schedule(methodCall, enqueueAt);
        }

        public string MoveSchedulingTo<T>([InstantHandle] Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt, ExecutionService service)
        {
            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }
            string queue;
            switch (service)
            {
                case ExecutionService.Agent:
                    queue = HangfireConstants.AgentQueue;
                    break;
                case ExecutionService.Api:
                    queue = HangfireConstants.ApiQueue;
                    break;
                case ExecutionService.Any:
                    queue = HangfireConstants.DefaultQueue;
                    break;
                case ExecutionService.Background:
                    queue = HangfireConstants.BackgroundQueue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(service), service, null);
            }
            return _client.UseQueue(queue).Schedule(methodCall, enqueueAt);
        }

        //public string StartNewBatch(Action<IBatchAction> createAction)
        //{
        //    return BatchJob.StartNew(createAction);
        //}

        public string ExecuteOnApi<T>(string parentJob, [InstantHandle, NotNull] Expression<Func<T, Task>> methodCall)
        {
            return MoveExecutionTo(parentJob, methodCall, ExecutionService.Api);
        }

        public string MoveExecutionTo<T>(string parentJob, [InstantHandle, NotNull] Expression<Func<T, Task>> methodCall, ExecutionService service)
        {
            if (methodCall == null)
            {
                throw new ArgumentNullException(nameof(methodCall));
            }
            string queue;
            switch (service)
            {
                case ExecutionService.Agent:
                    queue = HangfireConstants.AgentQueue;
                    break;
                case ExecutionService.Api:
                    queue = HangfireConstants.ApiQueue;
                    break;
                case ExecutionService.Any:
                    queue = HangfireConstants.DefaultQueue;
                    break;
                case ExecutionService.Background:
                    queue = HangfireConstants.BackgroundQueue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(service), service, null);
            }
            return _client.UseQueue(queue).ContinueJobWith(parentJob, methodCall);
        }

    }
}
