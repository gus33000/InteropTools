using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Windows.Storage;

namespace PhotoshoppedUUPCLI.UUP.FE3.Requests
{
    public class RequestHandler
    {
        public static string GetCookieRequest()
        {
            var env = new GetCookieRequestClasses.Envelope
            {
                Header = new GetCookieRequestClasses.Header
                {
                    Action = new GetCookieRequestClasses.Action
                    {
                        MustUnderstand = "1",
                        Text = "http://www.microsoft.com/SoftwareDistribution/Server/ClientWebService/GetCookie"
                    },
                    MessageID = "urn:uuid:b9b43757-2247-4d7b-ae8f-a71ba8a22385",
                    To = new GetCookieRequestClasses.To
                    {
                        MustUnderstand = "1",
                        Text = "https://fe3.delivery.mp.microsoft.com/ClientWebService/client.asmx"
                    },
                    Security = new GetCookieRequestClasses.Security
                    {
                        MustUnderstand = "1",
                        Timestamp = new GetCookieRequestClasses.Timestamp
                        {
                            Created = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                            Expires = "2017-09-29T06:25:43.943Z"
                        },
                        WindowsUpdateTicketsToken = new GetCookieRequestClasses.WindowsUpdateTicketsToken
                        {
                            Id = "ClientMSA",
                            TicketType = new GetCookieRequestClasses.TicketType()
                            {
                                Name = "MSA",
                                Version = "1.0",
                                Policy = "MBI_SSL",
                                User = ""
                            }
                        }
                    }
                },
                Body = new GetCookieRequestClasses.Body
                {
                    GetCookie = new GetCookieRequestClasses.GetCookie
                    {
                        OldCookie = new GetCookieRequestClasses.OldCookie
                        {
                            Expiration = "2045-01-27T14:47:10Z"
                        },
                        LastChange = "2015-10-21T17:01:07.1472913Z",
                        CurrentTime = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                        ProtocolVersion = "1.40"
                    }
                }
            };

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(env.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                x.Serialize(textWriter, env);
                return textWriter.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "");
            }
        }

        public static async System.Threading.Tasks.Task<string> GetExtendedUpdateInfo2RequestAsync(string updateid, string revisionnumber, AttributeData attribdata, string token)
        {
            var GetExtendedUpdateInfo2 = new GetExtendedUpdateInfo2RequestClasses.GetExtendedUpdateInfo2
            {
                UpdateIDs = new GetExtendedUpdateInfo2RequestClasses.UpdateIDs
                {
                    UpdateIdentity = new GetExtendedUpdateInfo2RequestClasses.UpdateIdentity
                    {
                        UpdateID = updateid,
                        RevisionNumber = revisionnumber
                    }
                },
                InfoTypes = new GetExtendedUpdateInfo2RequestClasses.InfoTypes
                {
                    XmlUpdateFragmentType = new List<string>
                    {
                        "FileUrl",
                        "FileDecryption"
                    }
                },
                DeviceAttributes = attribdata.DeviceAttributes,
                CallerAttributes = "IsSeeker=1;",
                Products = attribdata.Products
            };

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(GetExtendedUpdateInfo2.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                x.Serialize(textWriter, GetExtendedUpdateInfo2);
                string strsyncupdates = textWriter.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "").Replace("&amp;amp;", "&amp;");

                string result;
                
                var xmlfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ShellPages//Private//GoFly//UUP//FE3//Requests//WorkaroundGetExtendedUpdateInfo2.xml", UriKind.Absolute));

                List<string> lines = new List<string>();
                using (var inputStream = await xmlfile.OpenReadAsync())
                using (var classicStream = inputStream.AsStreamForRead())
                using (var streamReader = new StreamReader(classicStream))
                {
                    while (streamReader.Peek() >= 0)
                    {
                        lines.Add(streamReader.ReadLine());
                    }
                }

                string xmlcontent = string.Join("\n", lines);

                result = String.Format(xmlcontent, strsyncupdates, token);

                return result;
            }
        }

        public static async System.Threading.Tasks.Task<string> GetSyncUpdatesRequestAsync(CookieData cookie, AttributeData attributedata, string cachedids, string token)
        {
            var SyncUpdates = new SyncUpdatesRequestClasses.SyncUpdates
            {
                Cookie = new SyncUpdatesRequestClasses.Cookie
                {
                    Expiration = cookie.Expiration,
                    EncryptedData = cookie.EncryptedData
                },
                Parameters = new SyncUpdatesRequestClasses.Parameters
                {
                    ExpressQuery = "false",
                    InstalledNonLeafUpdateIDs = "",
                    OtherCachedUpdateIDs = cachedids,
                    SkipSoftwareSync = "false",
                    NeedTwoGroupOutOfScopeUpdates = "false",
                    AlsoPerformRegularSync = "true",
                    ComputerSpec = "",
                    ExtendedUpdateInfoParameters = new SyncUpdatesRequestClasses.ExtendedUpdateInfoParameters
                    {
                        XmlUpdateFragmentTypes = new SyncUpdatesRequestClasses.XmlUpdateFragmentTypes
                        {
                            XmlUpdateFragmentType = new List<string>
                                    {
                                        "Extended",
                                        "LocalizedProperties",
                                        "Eula"
                                    }
                        },
                        Locales = new SyncUpdatesRequestClasses.Locales
                        {
                            String = new System.Collections.Generic.List<string>
                                    {
                                        "en-US",
                                        "en"
                                    }
                        }
                    },
                    ClientPreferredLanguages = new SyncUpdatesRequestClasses.ClientPreferredLanguages
                    {
                        String = "en-US"
                    },
                    ProductsParameters = new SyncUpdatesRequestClasses.ProductsParameters
                    {
                        SyncCurrentVersionOnly = "false",
                        DeviceAttributes = attributedata.DeviceAttributes,
                        CallerAttributes = "IsSeeker=1;",
                        Products = attributedata.Products
                    }
                }
            };

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(SyncUpdates.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                x.Serialize(textWriter, SyncUpdates);
                string strsyncupdates = textWriter.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "").Replace("&amp;amp;", "&amp;");

                string result;
                
                var xmlfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ShellPages//Private//GoFly//UUP//FE3//Requests//WorkaroundSyncUpdates.xml", UriKind.Absolute));

                List<string> lines = new List<string>();
                using (var inputStream = await xmlfile.OpenReadAsync())
                using (var classicStream = inputStream.AsStreamForRead())
                using (var streamReader = new StreamReader(classicStream))
                {
                    while (streamReader.Peek() >= 0)
                    {
                        lines.Add(streamReader.ReadLine());
                    }
                }

                string xmlcontent = string.Join("\n", lines);
                
                result = String.Format(xmlcontent, strsyncupdates, token);

                return result;
            }
        }
    }
}
