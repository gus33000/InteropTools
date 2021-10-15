using InteropTools.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
    public sealed partial class ImportRegContentDialog : ContentDialog
    {
        private readonly ObservableCollection<DisplayItem> displaylist = new();

        private IReadOnlyList<RegFileItem> regoperations;

        public ImportRegContentDialog(StorageFile file)
        {
            InitializeComponent();
            ImportDetailsListView.ItemsSource = displaylist;
            OpenFile(file);
        }

        public enum RegFileOperation
        {
            REG_DELETE,
            REG_ADD
        }

        public enum RegItemType
        {
            KEY,
            VALUE
        }

        public static bool ParseReg(string[] regfiletext, out IReadOnlyList<RegFileItem> reglist)
        {
            List<RegFileItem> list = new();
            reglist = list;
            bool expects = true;
            bool isinkey = false;
            bool isStillParsingValue = false;
            RegHives curhive = RegHives.HKEY_LOCAL_MACHINE;
            string curkey = "";
            string curvalue = "";
            string curdatasecondpart = "";

            try
            {
                foreach (string str in regfiletext)
                {
                    string curstr = str.TrimStart().TrimEnd();

                    if (expects)
                    {
                        if (curstr != "")
                        {
                            if (curstr != "Windows Registry Editor Version 5.00")
                            {
                                return false;
                            }
                            else
                            {
                                expects = false;
                            }
                        }
                    }
                    else
                    {
                        if (curstr != "")
                        {
                            if (curstr[0] != ';')
                            {
                                if (curstr[0] == '[')
                                {
                                    if (isStillParsingValue)
                                    {
                                        return false;
                                    }

                                    if (curstr[1] != '-')
                                    {
                                        isinkey = true;
                                        RegHives hive =
                                      (RegHives)
                                      Enum.Parse(typeof(RegHives),
                                                 curstr.Replace("[", "").Replace("]", "").Split('\\')[0]);
                                        string key = string.Join("\\",
                                                          curstr.Replace("[", "").Replace("]", "").Split('\\').Skip(1));
                                        RegItemType type = RegItemType.KEY;
                                        RegFileOperation operation = RegFileOperation.REG_ADD;
                                        curhive = hive;
                                        curkey = key;
                                        list.Add(new RegFileItem
                                        {
                                            hive = hive,
                                            key = key,
                                            operation = operation,
                                            type = type
                                        });
                                    }
                                    else
                                    {
                                        isinkey = false;
                                        RegHives hive =
                                      (RegHives)
                                      Enum.Parse(typeof(RegHives),
                                                 curstr.Replace("[", "").Replace("]", "").Split('\\')[0]);
                                        string key = string.Join("\\",
                                                          curstr.Replace("[", "").Replace("]", "").Split('\\').Skip(1));
                                        RegItemType type = RegItemType.KEY;
                                        RegFileOperation operation = RegFileOperation.REG_DELETE;
                                        list.Add(new RegFileItem
                                        {
                                            hive = hive,
                                            key = key,
                                            operation = operation,
                                            type = type
                                        });
                                    }
                                }
                                else
                                    if (isinkey)
                                {
                                    //Now we need to parse values
                                    if (!isStillParsingValue)
                                    {
                                        curdatasecondpart = "";

                                        if (curstr.Split('=').Length == 2)
                                        {
                                            if ((curstr.Split('=')[0][0] == '@') && (curstr.Split('=')[0].Length == 1))
                                            {
                                                string valuename = "";
                                                curvalue = valuename;
                                                string secondpart = curstr.Split('=')[1];

                                                if (secondpart.Last() == '\\')
                                                {
                                                    isStillParsingValue = true;
                                                    curdatasecondpart = string.Join("",
                                                                                    secondpart.Reverse().Skip(1).Reverse());

                                                    if (secondpart.StartsWith("-"))
                                                    {
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((secondpart[0] == '"') && (secondpart.Last() == '"'))
                                                    {
                                                        //REG_SZ
                                                        RegTypes type = RegTypes.REG_SZ;
                                                        string valuedata = string.Join("",
                                                                                    secondpart.Skip(1).Reverse().Skip(1).Reverse());
                                                        valuedata = Regex.Unescape(valuedata);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                        if (secondpart.StartsWith("hex(0):"))
                                                    {
                                                        // REG_NONE
                                                        RegTypes type = RegTypes.REG_NONE;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                            if (secondpart.StartsWith("hex(1):"))
                                                    {
                                                        // REG_SZ
                                                        RegTypes type = RegTypes.REG_SZ;
                                                        string[] tempAry = secondpart.Split(':')[1].Split(',');
                                                        byte[] decBytes2 = new byte[tempAry.Length];

                                                        for (int i = 0; i < tempAry.Length; i++)
                                                        {
                                                            decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                        }

                                                        string valuedata = Encoding.Unicode.GetString(decBytes2);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                if (secondpart.StartsWith("hex(2):"))
                                                    {
                                                        // REG_EXPAND_SZ
                                                        RegTypes type = RegTypes.REG_EXPAND_SZ;
                                                        string[] tempAry = secondpart.Split(':')[1].Split(',');
                                                        byte[] decBytes2 = new byte[tempAry.Length];

                                                        for (int i = 0; i < tempAry.Length; i++)
                                                        {
                                                            decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                        }

                                                        string valuedata = Encoding.Unicode.GetString(decBytes2);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                    if (secondpart.StartsWith("hex(3):"))
                                                    {
                                                        // REG_BINARY
                                                        RegTypes type = RegTypes.REG_BINARY;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                        if (secondpart.StartsWith("hex:"))
                                                    {
                                                        // REG_BINARY
                                                        RegTypes type = RegTypes.REG_BINARY;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                            if (secondpart.StartsWith("hex(4):"))
                                                    {
                                                        // REG_DWORD
                                                        RegTypes type = RegTypes.REG_DWORD;
                                                        string valuedata =
                                                                                  int.Parse(
                                                                                    string.Concat(secondpart.Split(':')[1].Split(',').Reverse()),
                                                                                    NumberStyles.HexNumber).ToString();
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                if (secondpart.StartsWith("dword:"))
                                                    {
                                                        // REG_DWORD
                                                        RegTypes type = RegTypes.REG_DWORD;
                                                        string valuedata =
                                                                                      int.Parse(secondpart.Split(':')[1], NumberStyles.HexNumber)
                                                                                      .ToString();
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                    if (secondpart.StartsWith("hex(5):"))
                                                    {
                                                        // REG_DWORD_BIG_ENDIAN
                                                        RegTypes type = RegTypes.REG_DWORD_BIG_ENDIAN;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                        if (secondpart.StartsWith("hex(6):"))
                                                    {
                                                        // REG_LINK
                                                        RegTypes type = RegTypes.REG_LINK;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                            if (secondpart.StartsWith("hex(7):"))
                                                    {
                                                        // REG_MULTI_SZ
                                                        RegTypes type = RegTypes.REG_MULTI_SZ;
                                                        string tmpbufferstr = secondpart.Split(':')[1];
                                                        const string trim = ",00,00";
                                                        tmpbufferstr = tmpbufferstr.TrimEnd(trim.ToCharArray());
                                                        string[] data =
                                                                                                  new string[
                                                                                                  tmpbufferstr.Split(new[] { ",00,00,00," },
                                                                                                                     StringSplitOptions.None).Length];
                                                        int counter = 0;

                                                        foreach (
                                                          string part in
                                                          tmpbufferstr.Split(new[] { ",00,00,00," }, StringSplitOptions.None)
                                                        )
                                                        {
                                                            string[] tempAry = (part + ",00").Split(',');
                                                            byte[] decBytes2 = new byte[tempAry.Length];

                                                            for (int i = 0; i < tempAry.Length; i++)
                                                            {
                                                                decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                            }

                                                            data[counter] = Encoding.Unicode.GetString(decBytes2);
                                                            counter++;
                                                        }
                                                        string valuedata = string.Join("\n", data);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                if (secondpart.StartsWith("hex(8):"))
                                                    {
                                                        // REG_RESOURCE_LIST
                                                        RegTypes type = RegTypes.REG_RESOURCE_LIST;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                    if (secondpart.StartsWith("hex(9):"))
                                                    {
                                                        // REG_FULL_RESOURCE_DESCRIPTOR
                                                        RegTypes type = RegTypes.REG_FULL_RESOURCE_DESCRIPTOR;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                        if (secondpart.StartsWith("hex(a):"))
                                                    {
                                                        // REG_RESOURCE_REQUIREMENTS_LIST
                                                        RegTypes type = RegTypes.REG_RESOURCE_REQUIREMENTS_LIST;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                            if (secondpart.StartsWith("hex(b):"))
                                                    {
                                                        // REG_QWORD
                                                        RegTypes type = RegTypes.REG_QWORD;
                                                        string[] tempAry = secondpart.Split(':')[1].Split(',');
                                                        byte[] decBytes2 = new byte[tempAry.Length];

                                                        for (int i = 0; i < tempAry.Length; i++)
                                                        {
                                                            decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                        }

                                                        string valuedata = BitConverter.ToInt64(decBytes2, 0).ToString();
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                                if (secondpart.StartsWith("hex("))
                                                    {
                                                        uint type = uint.Parse(secondpart.Split(':')[0].Replace("hex(", "").Replace(")", ""), NumberStyles.HexNumber);
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype2 = type
                                                        });
                                                    }
                                                    else
                                                                                                                    if (secondpart.StartsWith("-"))
                                                    {
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_DELETE,
                                                            type = RegItemType.VALUE,
                                                            valuename = valuename
                                                        });
                                                    }
                                                    else
                                                    {
                                                        return false;
                                                    }
                                                }
                                            }
                                            else
                                                if ((curstr.Split('=')[0][0] == '"') && (curstr.Split('=')[0].Last() == '"'))
                                            {
                                                string valuename = string.Join("",
                                                                                curstr.Split('=')[0].Skip(1).Reverse().Skip(1).Reverse());
                                                valuename = Regex.Unescape(valuename);
                                                curvalue = valuename;
                                                string secondpart = curstr.Split('=')[1];

                                                if (secondpart.Last() == '\\')
                                                {
                                                    isStillParsingValue = true;
                                                    curdatasecondpart = string.Join("",
                                                                                    secondpart.Reverse().Skip(1).Reverse());

                                                    if (secondpart.StartsWith("-"))
                                                    {
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((secondpart[0] == '"') && (secondpart.Last() == '"'))
                                                    {
                                                        //REG_SZ
                                                        RegTypes type = RegTypes.REG_SZ;
                                                        string valuedata = string.Join("",
                                                                                        secondpart.Skip(1).Reverse().Skip(1).Reverse());
                                                        valuedata = Regex.Unescape(valuedata);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                        if (secondpart.StartsWith("hex(0):"))
                                                    {
                                                        // REG_NONE
                                                        RegTypes type = RegTypes.REG_NONE;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                            if (secondpart.StartsWith("hex(1):"))
                                                    {
                                                        // REG_SZ
                                                        RegTypes type = RegTypes.REG_SZ;
                                                        string[] tempAry = secondpart.Split(':')[1].Split(',');
                                                        byte[] decBytes2 = new byte[tempAry.Length];

                                                        for (int i = 0; i < tempAry.Length; i++)
                                                        {
                                                            decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                        }

                                                        string valuedata = Encoding.Unicode.GetString(decBytes2);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                if (secondpart.StartsWith("hex(2):"))
                                                    {
                                                        // REG_EXPAND_SZ
                                                        RegTypes type = RegTypes.REG_EXPAND_SZ;
                                                        string[] tempAry = secondpart.Split(':')[1].Split(',');
                                                        byte[] decBytes2 = new byte[tempAry.Length];

                                                        for (int i = 0; i < tempAry.Length; i++)
                                                        {
                                                            decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                        }

                                                        string valuedata = Encoding.Unicode.GetString(decBytes2);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                    if (secondpart.StartsWith("hex(3):"))
                                                    {
                                                        // REG_BINARY
                                                        RegTypes type = RegTypes.REG_BINARY;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                        if (secondpart.StartsWith("hex:"))
                                                    {
                                                        // REG_BINARY
                                                        RegTypes type = RegTypes.REG_BINARY;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                            if (secondpart.StartsWith("hex(4):"))
                                                    {
                                                        // REG_DWORD
                                                        RegTypes type = RegTypes.REG_DWORD;
                                                        string valuedata =
                                                                                      int.Parse(
                                                                                        string.Concat(secondpart.Split(':')[1].Split(',').Reverse()),
                                                                                        NumberStyles.HexNumber).ToString();
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                if (secondpart.StartsWith("dword:"))
                                                    {
                                                        // REG_DWORD
                                                        RegTypes type = RegTypes.REG_DWORD;
                                                        string valuedata =
                                                                                          int.Parse(secondpart.Split(':')[1], NumberStyles.HexNumber)
                                                                                          .ToString();
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                    if (secondpart.StartsWith("hex(5):"))
                                                    {
                                                        // REG_DWORD_BIG_ENDIAN
                                                        RegTypes type = RegTypes.REG_DWORD_BIG_ENDIAN;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                        if (secondpart.StartsWith("hex(6):"))
                                                    {
                                                        // REG_LINK
                                                        RegTypes type = RegTypes.REG_LINK;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                            if (secondpart.StartsWith("hex(7):"))
                                                    {
                                                        // REG_MULTI_SZ
                                                        RegTypes type = RegTypes.REG_MULTI_SZ;
                                                        string tmpbufferstr = secondpart.Split(':')[1];
                                                        const string trim = ",00,00";
                                                        tmpbufferstr = tmpbufferstr.TrimEnd(trim.ToCharArray());
                                                        string[] data =
                                                                                                      new string[
                                                                                                      tmpbufferstr.Split(new[] { ",00,00,00," },
                                                                                                                         StringSplitOptions.None).Length];
                                                        int counter = 0;

                                                        foreach (
                                                          string part in
                                                          tmpbufferstr.Split(new[] { ",00,00,00," }, StringSplitOptions.None)
                                                        )
                                                        {
                                                            string[] tempAry = (part + ",00").Split(',');
                                                            byte[] decBytes2 = new byte[tempAry.Length];

                                                            for (int i = 0; i < tempAry.Length; i++)
                                                            {
                                                                decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                            }

                                                            data[counter] = Encoding.Unicode.GetString(decBytes2);
                                                            counter++;
                                                        }
                                                        string valuedata = string.Join("\n", data);
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                if (secondpart.StartsWith("hex(8):"))
                                                    {
                                                        // REG_RESOURCE_LIST
                                                        RegTypes type = RegTypes.REG_RESOURCE_LIST;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                    if (secondpart.StartsWith("hex(9):"))
                                                    {
                                                        // REG_FULL_RESOURCE_DESCRIPTOR
                                                        RegTypes type = RegTypes.REG_FULL_RESOURCE_DESCRIPTOR;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                        if (secondpart.StartsWith("hex(a):"))
                                                    {
                                                        // REG_RESOURCE_REQUIREMENTS_LIST
                                                        RegTypes type = RegTypes.REG_RESOURCE_REQUIREMENTS_LIST;
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                            if (secondpart.StartsWith("hex(b):"))
                                                    {
                                                        // REG_QWORD
                                                        RegTypes type = RegTypes.REG_QWORD;
                                                        string[] tempAry = secondpart.Split(':')[1].Split(',');
                                                        byte[] decBytes2 = new byte[tempAry.Length];

                                                        for (int i = 0; i < tempAry.Length; i++)
                                                        {
                                                            decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                        }

                                                        string valuedata = BitConverter.ToInt64(decBytes2, 0).ToString();
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype = type
                                                        });
                                                    }
                                                    else
                                                                                                                if (secondpart.StartsWith("hex("))
                                                    {
                                                        uint type = uint.Parse(secondpart.Split(':')[0].Replace("hex(", "").Replace(")", ""), NumberStyles.HexNumber);
                                                        string valuedata = secondpart.Split(':')[1].Replace(",", "");
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_ADD,
                                                            type = RegItemType.VALUE,
                                                            valuedata = valuedata,
                                                            valuename = valuename,
                                                            valuetype2 = type
                                                        });
                                                    }
                                                    else
                                                                                                                    if (secondpart.StartsWith("-"))
                                                    {
                                                        list.Add(new RegFileItem
                                                        {
                                                            hive = curhive,
                                                            key = curkey,
                                                            operation = RegFileOperation.REG_DELETE,
                                                            type = RegItemType.VALUE,
                                                            valuename = valuename
                                                        });
                                                    }
                                                    else
                                                    {
                                                        return false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        // Continue parsing
                                        if (curstr.Last() == '\\')
                                        {
                                            isStillParsingValue = true;
                                            curdatasecondpart += string.Join("", curstr.Reverse().Skip(1).Reverse());

                                            if (curstr.StartsWith("-"))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            isStillParsingValue = false;
                                            curdatasecondpart += curstr;

                                            if (curstr.StartsWith("-"))
                                            {
                                                return false;
                                            }

                                            if ((curdatasecondpart[0] == '"') && (curdatasecondpart.Last() == '"'))
                                            {
                                                //REG_SZ
                                                RegTypes type = RegTypes.REG_SZ;
                                                string valuedata = string.Join("",
                                                                            curdatasecondpart.Skip(1).Reverse().Skip(1).Reverse());
                                                valuedata = Regex.Unescape(valuedata);
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                if (curdatasecondpart.StartsWith("hex(0):"))
                                            {
                                                // REG_NONE
                                                RegTypes type = RegTypes.REG_NONE;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                    if (curdatasecondpart.StartsWith("hex(1):"))
                                            {
                                                // REG_SZ
                                                RegTypes type = RegTypes.REG_SZ;
                                                string[] tempAry = curdatasecondpart.Split(':')[1].Split(',');
                                                byte[] decBytes2 = new byte[tempAry.Length];

                                                for (int i = 0; i < tempAry.Length; i++)
                                                {
                                                    decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                }

                                                string valuedata = Encoding.Unicode.GetString(decBytes2);
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                        if (curdatasecondpart.StartsWith("hex(2):"))
                                            {
                                                // REG_EXPAND_SZ
                                                RegTypes type = RegTypes.REG_EXPAND_SZ;
                                                string[] tempAry = curdatasecondpart.Split(':')[1].Split(',');
                                                byte[] decBytes2 = new byte[tempAry.Length];

                                                for (int i = 0; i < tempAry.Length; i++)
                                                {
                                                    decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                }

                                                string valuedata = Encoding.Unicode.GetString(decBytes2);
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                            if (curdatasecondpart.StartsWith("hex(3):"))
                                            {
                                                // REG_BINARY
                                                RegTypes type = RegTypes.REG_BINARY;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                if (curdatasecondpart.StartsWith("hex:"))
                                            {
                                                // REG_BINARY
                                                RegTypes type = RegTypes.REG_BINARY;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                    if (curdatasecondpart.StartsWith("hex(4):"))
                                            {
                                                // REG_DWORD
                                                RegTypes type = RegTypes.REG_DWORD;
                                                string valuedata =
                                                                          int.Parse(
                                                                            string.Concat(curdatasecondpart.Split(':')[1].Split(',').Reverse()),
                                                                            NumberStyles.HexNumber).ToString();
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                        if (curdatasecondpart.StartsWith("dword:"))
                                            {
                                                // REG_DWORD
                                                RegTypes type = RegTypes.REG_DWORD;
                                                string valuedata =
                                                                              int.Parse(curdatasecondpart.Split(':')[1], NumberStyles.HexNumber)
                                                                              .ToString();
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                            if (curdatasecondpart.StartsWith("hex(5):"))
                                            {
                                                // REG_DWORD_BIG_ENDIAN
                                                RegTypes type = RegTypes.REG_DWORD_BIG_ENDIAN;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                                if (curdatasecondpart.StartsWith("hex(6):"))
                                            {
                                                // REG_LINK
                                                RegTypes type = RegTypes.REG_LINK;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                                    if (curdatasecondpart.StartsWith("hex(7):"))
                                            {
                                                // REG_MULTI_SZ
                                                RegTypes type = RegTypes.REG_MULTI_SZ;
                                                string tmpbufferstr = curdatasecondpart.Split(':')[1];
                                                const string trim = ",00,00";
                                                tmpbufferstr = tmpbufferstr.TrimEnd(trim.ToCharArray());
                                                string[] data =
                                                                                          new string[
                                                                                          tmpbufferstr.Split(new[] { ",00,00,00," }, StringSplitOptions.None)
                                                                                          .Length];
                                                int counter = 0;

                                                foreach (
                                                  string part in
                                                  tmpbufferstr.Split(new[] { ",00,00,00," }, StringSplitOptions.None))
                                                {
                                                    string[] tempAry = (part + ",00").Split(',');
                                                    byte[] decBytes2 = new byte[tempAry.Length];

                                                    for (int i = 0; i < tempAry.Length; i++)
                                                    {
                                                        decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                    }

                                                    data[counter] = Encoding.Unicode.GetString(decBytes2);
                                                    counter++;
                                                }
                                                string valuedata = string.Join("\n", data);
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                                        if (curdatasecondpart.StartsWith("hex(8):"))
                                            {
                                                // REG_RESOURCE_LIST
                                                RegTypes type = RegTypes.REG_RESOURCE_LIST;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                                            if (curdatasecondpart.StartsWith("hex(9):"))
                                            {
                                                // REG_FULL_RESOURCE_DESCRIPTOR
                                                RegTypes type = RegTypes.REG_FULL_RESOURCE_DESCRIPTOR;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                                                if (curdatasecondpart.StartsWith("hex(a):"))
                                            {
                                                // REG_RESOURCE_REQUIREMENTS_LIST
                                                RegTypes type = RegTypes.REG_RESOURCE_REQUIREMENTS_LIST;
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                                                    if (curdatasecondpart.StartsWith("hex(b):"))
                                            {
                                                // REG_QWORD
                                                RegTypes type = RegTypes.REG_QWORD;
                                                string[] tempAry = curdatasecondpart.Split(':')[1].Split(',');
                                                byte[] decBytes2 = new byte[tempAry.Length];

                                                for (int i = 0; i < tempAry.Length; i++)
                                                {
                                                    decBytes2[i] = Convert.ToByte(tempAry[i], 16);
                                                }

                                                string valuedata = BitConverter.ToInt64(decBytes2, 0).ToString();
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype = type
                                                });
                                            }
                                            else
                                                                                                        if (curdatasecondpart.StartsWith("hex("))
                                            {
                                                uint type = uint.Parse(curdatasecondpart.Split(':')[0].Replace("hex(", "").Replace(")", ""), NumberStyles.HexNumber);
                                                string valuedata = curdatasecondpart.Split(':')[1].Replace(",", "");
                                                list.Add(new RegFileItem
                                                {
                                                    hive = curhive,
                                                    key = curkey,
                                                    operation = RegFileOperation.REG_ADD,
                                                    type = RegItemType.VALUE,
                                                    valuedata = valuedata,
                                                    valuename = curvalue,
                                                    valuetype2 = type
                                                });
                                            }
                                            else
                                                                                                            if (curdatasecondpart.StartsWith("-"))
                                            {
                                                return false;
                                            }
                                            else
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            reglist = list;
            return true;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            IRegistryProvider helper = App.MainRegistryHelper;

            foreach (RegFileItem regoperation in regoperations)
            {
                if (regoperation.operation == RegFileOperation.REG_ADD)
                {
                    if (regoperation.type == RegItemType.KEY)
                    {
                        KeyStatus status = await helper.GetKeyStatus(regoperation.hive, regoperation.key);

                        if ((status == KeyStatus.NotFound) || (status == KeyStatus.Unknown))
                        {
                            await helper.AddKey(regoperation.hive, regoperation.key);
                        }
                    }
                    else
                    {
                        if (regoperation.valuetype2.HasValue)
                        {
                            await helper.SetKeyValue(regoperation.hive, regoperation.key, regoperation.valuename,
                                               regoperation.valuetype2.Value, regoperation.valuedata);
                        }
                        else
                        {
                            await helper.SetKeyValue(regoperation.hive, regoperation.key, regoperation.valuename,
                                               regoperation.valuetype, regoperation.valuedata);
                        }
                    }
                }
                else
                {
                    if (regoperation.type == RegItemType.KEY)
                    {
                        await helper.DeleteKey(regoperation.hive, regoperation.key, true);
                    }
                    else
                    {
                        await helper.DeleteValue(regoperation.hive, regoperation.key, regoperation.valuename);
                    }
                }
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private string GetValueTypeName(RegTypes type)
        {
            switch (type)
            {
                case RegTypes.REG_BINARY:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Binary", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_FULL_RESOURCE_DESCRIPTOR:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Hardware_Resource_List", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_DWORD:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Integer", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_DWORD_BIG_ENDIAN:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Integer_Big_Endian", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_QWORD:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Long", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_MULTI_SZ:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Multi_String", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_NONE:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/None", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_RESOURCE_LIST:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Resource_List", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_RESOURCE_REQUIREMENTS_LIST:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Resource_Requirement", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_SZ:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/String", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_LINK:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Symbolic_Link", ResourceContext.GetForCurrentView()).ValueAsString;
                    }

                case RegTypes.REG_EXPAND_SZ:
                    {
                        return ResourceManager.Current.MainResourceMap.GetValue("Resources/Variable_String", ResourceContext.GetForCurrentView()).ValueAsString;
                    }
            }

            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        private async void OpenFile(StorageFile file)
        {
            if (file != null)
            {
                List<string> lines = new();

                using (Windows.Storage.Streams.IRandomAccessStreamWithContentType inputStream = await file.OpenReadAsync())
                {
                    using Stream classicStream = inputStream.AsStreamForRead();
                    using StreamReader streamReader = new(classicStream);
                    while (streamReader.Peek() >= 0)
                    {
                        lines.Add(streamReader.ReadLine());
                    }
                }

                bool result = ParseReg(lines.ToArray(), out regoperations);

                if (!result)
                {
                    Hide();
                    await new Core.MessageDialogContentDialog().ShowMessageDialog("We couldn't parse the provided REG file, please provide a valid REG 5.0 compliant REG file.", "Invalid REG file");
                }

                foreach (RegFileItem regoperation in regoperations)
                {
                    if (regoperation.operation == RegFileOperation.REG_ADD)
                    {
                        if (regoperation.type == RegItemType.KEY)
                        {
                            displaylist.Add(new DisplayItem());
                            displaylist.Add(new DisplayItem
                            {
                                Symbol = "",
                                DisplayName = "+ " + regoperation.hive + "\\" + regoperation.key,
                                Description = ResourceManager.Current.MainResourceMap.GetValue("Resources/Key", ResourceContext.GetForCurrentView()).ValueAsString
                            });
                        }
                        else
                        {
                            displaylist.Add(new DisplayItem
                            {
                                Symbol = "",
                                DisplayName = "+ " + regoperation.valuename,
                                Description =
                                regoperation.valuedata + "\n(" + GetValueTypeName(regoperation.valuetype) + ")"
                            });
                        }
                    }
                    else
                    {
                        if (regoperation.type == RegItemType.KEY)
                        {
                            displaylist.Add(new DisplayItem());
                            displaylist.Add(new DisplayItem
                            {
                                Symbol = "",
                                DisplayName = "- " + regoperation.hive + "\\" + regoperation.key,
                                Description = ResourceManager.Current.MainResourceMap.GetValue("Resources/Key", ResourceContext.GetForCurrentView()).ValueAsString
                            });
                        }
                        else
                        {
                            displaylist.Add(new DisplayItem
                            {
                                Symbol = "",
                                DisplayName = "- " + regoperation.valuename,
                                Description = ResourceManager.Current.MainResourceMap.GetValue("Resources/Value", ResourceContext.GetForCurrentView()).ValueAsString
                            });
                        }
                    }
                }
            }
        }

        public class DisplayItem
        {
            public string Description { get; set; }
            public string DisplayName { get; set; }
            public string Symbol { get; set; }
        }

        public class RegFileItem
        {
            public RegHives hive { get; set; }
            public string key { get; set; }
            public RegFileOperation operation { get; set; }
            public RegItemType type { get; set; }
            public string valuedata { get; set; }
            public string valuename { get; set; }
            public RegTypes valuetype { get; set; }
            public uint? valuetype2 { get; set; }
        }
    }
}