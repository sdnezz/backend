using Models.DTO.Common;

namespace Models.DTO.V1.Responses;

public class V1QueryOrdersResponse
{
    public OrderUnit[] Orders { get; set; }
}