

using JetBrains.Annotations;
using SchoolPilot.Common.Enums;
using System.Linq.Expressions;

namespace SchoolPilot.Common.Interfaces
{
    public interface IMethodExecutionHelper
    {
        string ExecuteOnApi<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall);

        string ExecuteOnApi<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall);

        string ExecuteOnAgent<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall);

        string ExecuteOnAgent<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall);

        string ExecuteOnAnything<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall);

        string ExecuteOnAnything<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall);

        string ExecuteInBackground<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall);

        string ExecuteInBackground<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall);

        string MoveExecutionTo<T>([NotNull, InstantHandle] Expression<Action<T>> methodCall, ExecutionService service);

        string MoveExecutionTo<T>([NotNull, InstantHandle] Expression<Func<T, Task>> methodCall, ExecutionService service);

        string ScheduleInBackground<T>([InstantHandle] Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);

        string ScheduleInBackground<T>([InstantHandle] Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);

        string MoveSchedulingTo<T>([InstantHandle] Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, ExecutionService service);

        string MoveSchedulingTo<T>([InstantHandle] Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt, ExecutionService service);

        //string StartNewBatch(Action<IBatchAction> createAction);

        string ExecuteOnApi<T>(string parentJob, [NotNull, InstantHandle] Expression<Func<T, Task>> methodCall);

        string MoveExecutionTo<T>(string parentJob, [NotNull, InstantHandle] Expression<Func<T, Task>> methodCall, ExecutionService service);
    }
}
