using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InteropTools.Providers;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Diagnostics;

namespace InteropTools.RemoteClasses.Server
{
	public sealed class RegistryActions
	{
		private readonly IRegistryProvider _helper = App.MainRegistryHelper;

		public async Task<string> RegistryAction(string json, string hostname)
		{
			try
			{
				var data = JsonConvert.DeserializeObject<RootObject>(json);
				var allowed = false;
				var denied = false;

				foreach (var item in App.AllowedRemotes)
					if (string.Equals(item.Hostname, hostname, StringComparison.CurrentCultureIgnoreCase))
					{
						if (string.Equals(item.SessionID, data.SessionID, StringComparison.CurrentCultureIgnoreCase))
						{
							allowed = true;
						}
					}

				foreach (var item in App.DeniedRemotes)
					if (string.Equals(item.Hostname, hostname, StringComparison.CurrentCultureIgnoreCase))
					{
						if (string.Equals(item.SessionID, data.SessionID, StringComparison.CurrentCultureIgnoreCase))
						{
							denied = true;
						}
					}

				if (!allowed)
				{
					if (!denied)
					{
						if (data.Operation == "Authentificate")
						{
							var tsk = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
							{
								var result = await new InteropTools.ContentDialogs.Core.DualMessageDialogContentDialog().ShowDualMessageDialog(App.RemoteLoc, "You can allow it to access it or deny it\n\nIP address: " + hostname,
								             App.RemoteAllowLoc, App.RemoteDenyLoc);

								if (result)
								{
									App.AllowedRemotes.Add(new App.Remote { Hostname = hostname, SessionID = data.SessionID });
									allowed = true;
								}

								else
								{
									App.DeniedRemotes.Add(new App.Remote { Hostname = hostname, SessionID = data.SessionID });
								}
							});

							while (tsk.Status != Windows.Foundation.AsyncStatus.Completed)
							{
							}
						}
					}
				}

				if (allowed)
				{
					switch (data.Operation)
					{
						case "Authentificate":
							{
								try
								{
									var result = new Result {Status = "SUCCESS"};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "GetKeyValue":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									RegTypes type;
									Enum.TryParse(data.ValueType, out type);
									var error = await _helper.GetKeyValue(hive, data.Key, data.ValueName, type);

                                    var valuedata = error.regvalue;

									var result = new Result
									{
										Error = error.returncode.ToString(),
										ValueData = valuedata,
										ValueType = type.ToString()
									};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "SetKeyValue":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									RegTypes type;
									Enum.TryParse(data.ValueType, out type);
									var error = await _helper.SetKeyValue(hive, data.Key, data.ValueName, type, data.ValueData);
									var result = new Result {Error = error.ToString()};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "GetKeyStatus":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									var status = await _helper.GetKeyStatus(hive, data.Key);
									var result = new Result {Status = status.ToString()};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "DeleteValue":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									var error = await _helper.DeleteValue(hive, data.Key, data.ValueName);
									var result = new Result {Error = error.ToString()};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "DeleteKey":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									var error = await _helper.DeleteKey(hive, data.Key, data.Recursive);
									var result = new Result {Error = error.ToString()};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "AddKey":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									var error = await _helper.AddKey(hive, data.Key);
									var result = new Result {Error = error.ToString()};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "RenameKey":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									var error = await _helper.RenameKey(hive, data.Key, data.NewName);
									var result = new Result {Error = error.ToString()};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}
                            
						case "GetRegistryHives2":
							{
								try
								{
									var regitems = await _helper.GetRegistryHives2();
									var result = new Result();
									var items = regitems.Select(item => new Item
									{
										Hive = item.Hive.ToString(),
										Key = item.Key,
										Name = item.Name,
										Type = item.Type.ToString(),
										Value = item.Value,
										ValueType2 = item.ValueType
									}).ToList();
									result.Items = items;
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "GetRegistryItems2":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									var regitems = await _helper.GetRegistryItems2(hive, data.Key);
									var result = new Result();
									var items = regitems.Select(item => new Item
									{
										Hive = item.Hive.ToString(),
										Key = item.Key,
										Name = item.Name,
										Type = item.Type.ToString(),
										Value = item.Value,
										ValueType2 = item.ValueType
									}).ToList();
									result.Items = items;
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "GetKeyValue2":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									uint type = data.ValueType2;
									var error = await _helper.GetKeyValue(hive, data.Key, data.ValueName, type);
                                    var valuedata = error.regvalue;
									var result = new Result
									{
										Error = error.returncode.ToString(),
										ValueData = valuedata,
										ValueType2 = type
									};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "SetKeyValue2":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									uint type = data.ValueType2;
									var error = await _helper.SetKeyValue(hive, data.Key, data.ValueName, type, data.ValueData);
									var result = new Result { Error = error.ToString() };
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "DoesFileExists":
							{
								try
								{
									var result = new Result {Exists = _helper.DoesFileExists(data.Path)};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "GetAppInstallationPath":
							{
								try
								{
									var result = new Result {AppInstallationPath = _helper.GetAppInstallationPath()};
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}

						case "GetKeyLastModifiedTime":
							{
								try
								{
									RegHives hive;
									Enum.TryParse(data.Hive, out hive);
									var error = await _helper.GetKeyLastModifiedTime(hive, data.Key);
									var result = new Result { Error = error.returncode.ToString(), LastModifiedTime = error.LastModified };
									data.Result = result;
									var resultjson = JsonConvert.SerializeObject(data);
									return resultjson;
								}

								catch
								{
									// ignored
								}

								break;
							}
                        case "LoadHive":
                            {
                                try
                                {
                                    var error = await _helper.LoadHive(data.FilePath, data.mountpoint, data.inUser);
                                    var result = new Result { Error = error.ToString() };
                                    data.Result = result;
                                    var resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }

                                catch
                                {
                                    // ignored
                                }

                                break;
                            }
                        case "UnloadHive":
                            {
                                try
                                {
                                    var error = await _helper.UnloadHive(data.mountpoint, data.inUser);
                                    var result = new Result { Error = error.ToString() };
                                    data.Result = result;
                                    var resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }

                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        default:
							{
								break;
							}
					}
				}

				else
				{
					try
					{
						var result = new Result {Status = "FAILED"};
						data.Result = result;
						var resultjson = JsonConvert.SerializeObject(data);
						return resultjson;
					}

					catch
					{
						// ignored
					}
				}
			}
			catch
			{
				// ignored
			}

			return "";
		}

		private class Item
		{
			public string Name { get; set; }
			public string Type { get; set; }
			public string Hive { get; set; }
			public string Key { get; set; }
			public string Value { get; set; }
			public string ValueType { get; set; }
			public uint ValueType2 { get; set; }
		}

		private class Result
		{
			public string Error { get; set; }
			public bool Exists { get; set; }
			public string ValueData { get; set; }
			public string ValueType { get; set; }
			public uint ValueType2 { get; set; }
			public string Status { get; set; }

			public List<Item> Items { get; set; }

			public string AppInstallationPath { get; set; }

			public DateTime LastModifiedTime { get; set; }
		}

		private class RootObject
		{
			public string SessionID { get; set; }
			public string Operation { get; set; }
			public string Path { get; set; }
			public string Hive { get; set; }
			public string Key { get; set; }
			public string ValueName { get; set; }
			public string ValueType { get; set; }
			public uint ValueType2 { get; set; }
			public string ValueData { get; set; }
			public bool Recursive { get; set; }
			public string NewName { get; set; }
			public Result Result { get; set; }
            public string FilePath { get; set; }
            public string mountpoint { get; set; }
            public bool inUser { get; set; }
        }
	}
}