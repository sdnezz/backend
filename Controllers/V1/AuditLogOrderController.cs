using Microsoft.AspNetCore.Mvc;
using Models.DTO.Common;
using Models.DTO.V1.Requests;
using Models.Dto.V1.Responses;
using Models.DTO.V1.Responses;
using SolutionLab1.BLL.Services;
using SolutionLab1.Validators;


namespace SolutionLab1.Controllers.V1;


[Route("api/v1/audit/log-order")]
public class AuditLogOrderController(AuditLogService auditLogService, ValidatorFactory validatorFactory) : ControllerBase
{
    [HttpPost("batch-create")]
    public async Task<ActionResult<V1AuditLogOrderResponse>> V1BatchCreate([FromBody] V1AuditLogOrderRequest request, CancellationToken token)
    {
        var validationResult = await validatorFactory.GetValidator<V1AuditLogOrderRequest>().ValidateAsync(request, token);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }
        
        var auditLogOrderUnits = request.Orders.Select(x => new AuditLogOrderUnit
        {
            OrderId = x.OrderId,
            OrderItemId = x.OrderItemId,
            CustomerId = x.CustomerId,
            OrderStatus = x.OrderStatus
        }).ToArray();
        
        var res = await auditLogService.BatchInsert(auditLogOrderUnits, token);

        return Ok(new V1AuditLogOrderResponse
        {
            Orders = Map(res)
        });
    }
    
    private AuditLogOrderUnit[] Map(AuditLogOrderUnit[] logs)
    {
        return logs.Select(x => new AuditLogOrderUnit
        {
            OrderId = x.OrderId,
            OrderItemId = x.OrderItemId,
            CustomerId = x.CustomerId,
            OrderStatus = x.OrderStatus,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
        }).ToArray();
    }
}