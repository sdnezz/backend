using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Models.DTO.V1.Requests;
using Models.Dto.V1.Responses;
using Models.DTO.V1.Responses;

namespace Consumer.Clients;

public class OmsClient(HttpClient client)
{
    public async Task<V1AuditLogOrderResponse> LogOrder(V1AuditLogOrderRequest request, CancellationToken token)
    {
        var msg = await client.PostAsync("api/v1/audit/log-order/batch-create", new StringContent(request.ToJson(), Encoding.UTF8, "application/json"), token);
        if (msg.IsSuccessStatusCode)
        {
            var content = await msg.Content.ReadAsStringAsync(cancellationToken: token);
            return content.FromJson<V1AuditLogOrderResponse>();
        }

        throw new HttpRequestException();
    }
}