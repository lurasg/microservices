using Polly;
namespace eCommerce.OrdersMicroservice.BusinessLogicLayer.Policies;
public interface IUsersMicroServicePolicies
 {
    
    IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();

}

