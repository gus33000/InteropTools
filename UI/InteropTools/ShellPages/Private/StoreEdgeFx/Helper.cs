using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Marketplace.Storefront;
using System.Diagnostics;
using Windows.Web.Http;
using System;
using Newtonsoft.Json;

namespace OneStore.StoreEdgeFx
{
    public class Helper
    {
        public async static Task<Product> GetProduct(string ProductId)
        {
            string url = "https://storeedgefd.dsx.mp.microsoft.com/v7.0/products/" + ProductId + "?market=US&locale=en-US";
            using (var httpClient = new HttpClient())
            {
                var op = httpClient.GetStringAsync(new Uri(url));
                op.Progress = new Windows.Foundation.AsyncOperationProgressHandler<string, HttpProgress>((result, progress) => { Debug.WriteLine(progress.Stage); });
                var json = await op;
                return JsonConvert.DeserializeObject<Product>(json);
            }
        }
    }
}
