using Models.DTO.Common;

namespace Models.DTO.V1.Responses;

public class V1CreateOrderResponse
{
    public OrderUnit[] Orders { get; set; }
}