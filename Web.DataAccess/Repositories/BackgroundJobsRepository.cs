using Hangfire;
using System.Linq.Expressions;

namespace Web.DataAccess.Repositories;
public class BackgroundJobsRepository: IBackgroundJobsRepository
{
    public void Enqueue<T>(Expression<Action<T>> methodCall)
    {
        BackgroundJob.Enqueue(methodCall);
    }
}
