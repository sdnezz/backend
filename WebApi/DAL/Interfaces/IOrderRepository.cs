using WebApi.DAL.Models;

namespace WebApi.DAL.Interfaces;

public interface IOrderRepository
{
    Task<V1OrderDal[]> BulkInsert(V1OrderDal[] model, CancellationToken token);

    Task<V1OrderDal[]> Query(QueryOrdersDalModel model, CancellationToken token);

    Task<List<V1OrderDal>> GetByIdsAsync(List<long> orderIds, CancellationToken token);

    Task UpdateStatusesAsync(List<long> orderIds, string newStatus, CancellationToken token);
}