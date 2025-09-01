using System.Linq.Expressions;

namespace Web.Entites.IRepositories;
public interface IBackgroundJobsRepository
{
    void Enqueue<T>(Expression<Action<T>> methodCall);
}
