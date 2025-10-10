using SolutionLab1.DAL.Models;

namespace SolutionLab1.DAL.Interfaces;

public interface IOrderItemRepository
{
    Task<V1OrderItemDal[]> BulkInsert(V1OrderItemDal[] model, CancellationToken token);
    
    Task<V1OrderItemDal[]> Query(QueryOrderItemsDalModel model, CancellationToken token);
}