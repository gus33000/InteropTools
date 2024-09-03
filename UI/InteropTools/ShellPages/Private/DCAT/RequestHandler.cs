using DCATLib.ObjectClasses;
using Flurl.Http;
using System;
using System.Threading.Tasks;

namespace DCATLib
{
    public static class RequestHandler
    {
        public static async Task<CProduct> GetProduct(string ProductId)
        {
            var endpoint = Constants.GetProductEndpoint;
            var cv = Constants.MSCV;

            endpoint = String.Format(endpoint, ProductId);

            var result = await endpoint.WithHeader("MS-CV", cv).GetJsonAsync<CProduct>();
            
            return result;
        }
    }
}
