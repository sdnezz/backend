using Models.DTO.Common;

namespace Models.DTO.V1.Responses;

public class V1AuditLogOrderResponse
{
    public AuditLogOrderUnit[] Orders { get; set; }
}