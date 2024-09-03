using System;
using System.Collections.Generic;
using MyToolkit.Mvvm;
using Windows.Web.Http;
using Windows.Data.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Web.Http.Filters;
using System.Net;
using System.IO;
using Flurl.Http;
using DCATLib;
using System.Linq;
using OneStore.StoreEdgeFx;
using Newtonsoft.Json;
using Microsoft.Marketplace.Storefront;
using DCATLib.ObjectClasses;

namespace StoreProjectApp.Presentation
{
    public sealed class ProductListPageModel : ViewModelBase
    {
        private string _filter;
        private Product[] _products;

        private int iterationcount = 0;

        private List<Product> list = new List<Product>();

        public ProductListPageModel()
        {
            AddPackagesOnline();
        }

        /// <summary>Gets or sets the filter. </summary>
        public string Filter
        {
            get { return _filter; }
            set { Set(ref _filter, value); }
        }

        public async void AddPackagesOnline()
        {
            //await Helper.GetProduct("9wzdncrfj364");


            var RootFilter = new HttpBaseProtocolFilter();
            RootFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.NoCache;
            RootFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;

            RootFilter.UseProxy = false;

            var http = new HttpClient(RootFilter);
            
            var url = String.Format("http://pastebin.com/raw/m5LP716y");
            var response = await http.GetAsync(new Uri(url));
            var result = await response.Content.ReadAsStringAsync();

            foreach (string productid in result.Split(char.Parse("\n")))
            {
                AddProduct(productid.Replace("\r", ""), (result.Split(char.Parse("\n")).Length));
                //AddProduct("9WZDNCRFJ364", (result.Split(char.Parse("\n")).Length));
            }

            //AddProduct("9WZDNCRFJ364", 1);
        }

        public async void AddProduct(string productid, int totalcount)
        {
            try
            {
                /*Uri uri = new Uri(String.Format("http://displaycatalog.mp.microsoft.com/v7.0/products/{0}/?languages=en-US,en,neutral&oemId=MICROSOFT&market=US&fieldsTemplate=details&scmId=Public&moId=Public", productid));
                HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                webrequest.Headers["MS-CV"] = "xK2sLQvOQEO68DsH";
                webrequest.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                //webrequest.Headers["Pragma"] = "no-cache";
                webrequest.Headers["Expires"] = "0";
                webrequest.Method = "GET";

                var response = await webrequest.GetResponseAsync();
                StreamReader streamReader1 = new StreamReader(response.GetResponseStream());
                var json = streamReader1.ReadToEnd();*/

                string url = String.Format("http://displaycatalog.mp.microsoft.com/v7.0/products/{0}/?languages=en-US,en,neutral&oemId=MICROSOFT&market=US&fieldsTemplate=details&scmId=Public&moId=Public", productid);

                //var json = await url.WithHeader("MS-CV", "uPvMWf/GQEeAWT8M").GetStringAsync();

                //Debug.WriteLine(json);

                //var client = new Flurl.Url(url);


                var RootFilter = new HttpBaseProtocolFilter();
                RootFilter.CacheControl.ReadBehavior = Windows.Web.Http.Filters.HttpCacheReadBehavior.NoCache;
                RootFilter.CacheControl.WriteBehavior = Windows.Web.Http.Filters.HttpCacheWriteBehavior.NoCache;

                RootFilter.UseProxy = false;

                var HttpClient = new HttpClient(RootFilter);
                
                HttpClient.DefaultRequestHeaders.Add("MS-CV", "xK2sLQvOQEO68DsH");

                HttpResponseMessage response = await HttpClient.GetAsync(new Uri(url));
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                //var result = await RequestHandler.GetProduct(productid);

                var result = JsonConvert.DeserializeObject<CProduct>(json);

                if (result != null)
                {
                    var Title = result.Product.LocalizedProperties[0].ProductTitle;//result["Product"].GetObject()["LocalizedProperties"].GetArray()[0].GetObject()["ProductTitle"].GetString();
                    var Description = result.Product.LocalizedProperties[0].ProductDescription;//result["Product"].GetObject()["LocalizedProperties"].GetArray()[0].GetObject()["ProductDescription"].GetString();
                    var LastUpdated = result.Product.LastModifiedDate;//result["Product"].GetObject()["LastModifiedDate"].GetString();
                    var Date = String.Format("{0:yyyy/MM/dd HH:mm:ss zzz}", result.Product.DisplaySkuAvailabilities[0].Sku.LastModifiedDate.Value);
                    try
                    {
                        Date = String.Format("{0:yyyy/MM/dd HH:mm:ss zzz}", DateTime.Parse((string)LastUpdated));
                    }
                    catch
                    {

                    }
                    var WUCatID = result.Product.DisplaySkuAvailabilities[0].Sku.Properties.FulfillmentData.WuCategoryId;//result["Product"].GetObject()["DisplaySkuAvailabilities"].GetArray()[0].GetObject()["Sku"].GetObject()["Properties"].GetObject()["FulfillmentData"].GetObject()["WuCategoryId"].GetString();
                    var ImageArrays = result.Product.LocalizedProperties[0].Images;//result["Product"].GetObject()["LocalizedProperties"].GetArray()[0].GetObject()["Images"].GetArray();
                    var PublisherName = "";
                    try
                    {
                        PublisherName = result.Product.LocalizedProperties[0].PublisherName;//result["Product"].GetObject()["LocalizedProperties"].GetArray()[0].GetObject()["PublisherName"].GetString();
                    }
                    catch
                    {
                    }

                    string iconuri = "";
                    string backgroundcolor = "";
                    foreach (var image in ImageArrays)
                    {
                        if (image.ImagePurpose.ToLower() == "logo")
                        {
                            iconuri = image.Uri;
                            if (iconuri != "" && !iconuri.Contains("http"))
                            {
                                iconuri = "http:" + iconuri;
                            }
                            backgroundcolor = image.BackgroundColor;
                        }
                    }
                    var UsageData = result.Product.MarketProperties[0].UsageData;//result["Product"].GetObject()["MarketProperties"].GetArray()[0].GetObject()["UsageData"].GetArray();
                    long PurchaseCount = 0;
                    long RatingCount = 0;
                    long AverageRating = 0;
                    foreach (var item in UsageData)
                    {
                        if (item.AggregateTimeSpan == "AllTime")
                        {
                            PurchaseCount = (long)item.PurchaseCount;
                            RatingCount = (long)item.RatingCount;
                            AverageRating = (long)item.AverageRating;
                        }
                    }
                    ObservableCollection<PackageItem> ItemsList = new ObservableCollection<PackageItem>();
                    try
                    {
                        var packagenoderoot = result.Product.DisplaySkuAvailabilities[0].Sku.Properties.Delivery.Packages;//result["Product"].GetObject()["DisplaySkuAvailabilities"].GetArray()[0].GetObject()["Sku"].GetObject()["Properties"].GetObject()["Delivery"].GetObject()["Packages"].GetArray();


                        foreach (var package in packagenoderoot)
                        {
                            List<string> FrameworkDependencies = new List<string>();
                            try
                            {
                                if (package.FrameworkDependencies != null)
                                {
                                    foreach (var dep in package.FrameworkDependencies)
                                    {
                                        FrameworkDependencies.Add(dep.PackageIdentity.Replace("\"", ""));
                                    }
                                }
                            }
                            catch
                            {

                            }
                            List<string> PlatformDependencies = new List<string>();
                            try
                            {
                                if (package.PlatformDependencies != null)
                                {
                                    foreach (var dep in package.PlatformDependencies)
                                    {
                                        PlatformDependencies.Add(dep.PlatformName.Replace("\"", ""));
                                    }
                                }
                            }
                            catch
                            {

                            }
                            List<string> Architectures = new List<string>();
                            try
                            {
                                if (package.Architectures != null)
                                {
                                    foreach (var dep in package.Architectures)
                                    {
                                        Architectures.Add(dep.Replace("\"", ""));
                                    }
                                }
                            }
                            catch
                            {

                            }
                            ItemsList.Add(new PackageItem
                            {
                                PackageFullName = package.PackageFullName.Replace("\"", ""),
                                PackageFormat = package.PackageFormat.Replace("\"", ""),
                                Architectures = Architectures,
                                PlatformDependencies = PlatformDependencies,
                                FrameworkDependencies = FrameworkDependencies,
                                PackageUri = package.PackageUri.Replace("\"", "")
                            });
                            list.Sort((x, y) => y.LastUpdated.CompareTo(x.LastUpdated));
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        var packagenoderoot = result.Product.DisplaySkuAvailabilities[0].Sku.Properties.Packages;//result["Product"].GetObject()["DisplaySkuAvailabilities"].GetArray()[0].GetObject()["Sku"].GetObject()["Properties"].GetObject()["Packages"].GetArray();

                        foreach (var package in packagenoderoot)
                        {
                            List<string> FrameworkDependencies = new List<string>();
                            try
                            {
                                if (package.FrameworkDependencies != null)
                                {
                                    foreach (var dep in package.FrameworkDependencies)
                                    {
                                        FrameworkDependencies.Add(dep.PackageIdentity.Replace("\"", ""));
                                    }
                                }
                            }
                            catch
                            {

                            }
                            List<string> PlatformDependencies = new List<string>();
                            try
                            {
                                if (package.PlatformDependencies != null)
                                {
                                    foreach (var dep in package.PlatformDependencies)
                                    {
                                        PlatformDependencies.Add(dep.PlatformName.Replace("\"", ""));
                                    }
                                }
                            }
                            catch
                            {

                            }
                            List<string> Architectures = new List<string>();
                            try
                            {
                                if (package.Architectures != null)
                                {
                                    foreach (var dep in package.Architectures)
                                    {
                                        Architectures.Add(dep.Replace("\"", ""));
                                    }
                                }
                            }
                            catch
                            {

                            }
                            ItemsList.Add(new PackageItem
                            {
                                PackageFullName = package.PackageFullName.Replace("\"", ""),
                                PackageFormat = package.PackageFormat.Replace("\"", ""),
                                Architectures = Architectures,
                                PlatformDependencies = PlatformDependencies,
                                FrameworkDependencies = FrameworkDependencies,
                                PackageUri = package.PackageUri.Replace("\"", "")
                            });
                            list.Sort((x, y) => y.LastUpdated.CompareTo(x.LastUpdated));
                        }
                    }
                    catch
                    {

                    }

                    //list.Sort((x, y) => y.LastUpdated.CompareTo(x.LastUpdated));

                    list.Add(
                        new Product
                        {
                            ProductID = productid,
                            Name = Title + (ItemsList.Any(x => x.PackageUri != "https://productingestionbin1.blob.core.windows.net" && !string.IsNullOrEmpty(x.PackageUri)) ? " ⓘ" : ""),
                            Description = Description,
                            LastUpdated = String.Format("{0:yyyy/MM/dd HH:mm:ss zzz}", Date),
                            Icon = iconuri,
                            Publisher = PublisherName,
                            BackgroundColor = backgroundcolor,
                            AverageRating = AverageRating,
                            PurchaseCount = PurchaseCount,
                            RatingCount = RatingCount,
                            ItemsList = ItemsList,
                            WUCatID = WUCatID
                        }
                    );
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + e.StackTrace);
            }
            iterationcount++;
            if (totalcount == iterationcount)
            {
                Product = list.ToArray();
            }
        }

        /// <summary>Gets or sets the people. </summary>
        public Product[] Product
        {
            get { return _products; }
            set { Set(ref _products, value); }
        }
    }

    public class PackageItem
    {
        public string PackageFullName { get; set; }
        public List<string> Architectures { get; set; }
        public string PackageFormat { get; set; }
        public List<string> PlatformDependencies { get; set; }
        public List<string> FrameworkDependencies { get; set; }
        public string PackageUri { get; internal set; }
    }

    public class Product
    {
        public string ProductID { get; set; }

        public string Name { get; set; }

        public string LastUpdated { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string Publisher { get; set; }

        public string BackgroundColor { get; set; }

        public long PurchaseCount { get; set; }

        public long RatingCount { get; set; }

        public long AverageRating { get; set; }

        public ObservableCollection<PackageItem> ItemsList { get; set; }

        public string WUCatID { get; set; }

    }
}
