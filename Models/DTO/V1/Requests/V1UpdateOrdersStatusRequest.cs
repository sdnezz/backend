namespace Models.DTO.V1.Requests;

public class V1UpdateOrdersStatusRequest
{
    public long[] OrderIds { get; set; }

    public string NewStatus { get; set; }
}