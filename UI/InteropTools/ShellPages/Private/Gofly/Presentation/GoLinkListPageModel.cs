using MyToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace StoreProjectApp.Presentation
{
    public class GoLinkListPageModel : ViewModelBase
    {
        private string _filter;
        private Update[] _golinks;

        private List<Update> list = new List<Update>();

        public int globalprogress = 0;

        public bool pause = false;

        public GoLinkListPageModel()
        {
            //ReadGoLink("787036", "900000");
            //DoIt(787036, 900000);
        }

        public async void DoIt(int startid, int endid)
        {
            globalprogress = startid;
            for (int i = startid; i <= endid; i++)
            {
                startid = i;
                string productid = i.ToString();
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                httpClientHandler.AllowAutoRedirect = false;

                try
                {
                    // passed in here
                    using (HttpClient client = new HttpClient(httpClientHandler))
                    {
                        HttpResponseMessage response = await client.GetAsync(new Uri("http://go.microsoft.com/fwlink/?LinkId=" + productid));
                        if (response.Headers.Location.ToString() != "http://www.microsoft.com/")
                        {
                            list.Insert(0, new Update { ID = productid, Link = response.Headers.Location.ToString() });
                            list.Sort((x, y) => y.ID.CompareTo(x.ID));
                            GoLink = list.ToArray();
                        }
                    }
                }
                catch
                {

                }
                if (pause)
                {
                    break;
                }
            }
        }



        /// <summary>Gets or sets the filter. </summary>
        public string Filter
        {
            get { return _filter; }
            set { Set(ref _filter, value); }
        }

        /// <summary>Gets or sets the people. </summary>
        public Update[] GoLink
        {
            get { return _golinks; }
            set { Set(ref _golinks, value); }
        }
    }

    public class Update
    {
        public string ID { get; set; }

        public string Link { get; set; }

    }

    /*public async void ReadGoLink(string productid)
    {
        HttpClientHandler httpClientHandler = new HttpClientHandler();
        httpClientHandler.AllowAutoRedirect = false;

        try
        {
            // passed in here
            using (HttpClient client = new HttpClient(httpClientHandler))
            {
                HttpResponseMessage response = await client.GetAsync(new Uri("http://go.microsoft.com/fwlink/?LinkId=" + productid));
                list.Insert(0, new GoLink { ID = productid, Link = response.Headers.Location.ToString() });
                list.Sort((x, y) => y.ID.CompareTo(x.ID));
                GoLink = list.ToArray();
            }
        }
        catch
        {

        }
    }
            /*public async void DoIt(int startid, int endid)
        {
            int current = 0;
            globalprogress = startid;
            for (int i = startid; i <= endid; i++)
            {
                current++;
                startid = i;
                if (current == 1000)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    current = 0;
                }
                ReadGoLink(i.ToString());
                if (pause)
                {
                    break;
                }
            }
        }*/
}
