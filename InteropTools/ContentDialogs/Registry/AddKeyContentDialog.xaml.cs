using InteropTools.Providers;
using System;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
    public sealed partial class AddKeyContentDialog : ContentDialog
    {
        private readonly IRegistryProvider helper;
        private RegHives hive = RegHives.HKEY_LOCAL_MACHINE;
        private string keylocation = "";
        private string keyname = "";

        public AddKeyContentDialog(RegHives hive, string keylocation, string keyname)
        {
            InitializeComponent();
            helper = App.MainRegistryHelper;
            this.hive = hive;
            this.keylocation = keylocation;
            this.keyname = keyname;
            KeyLocationPathInputBox.Text = keylocation;
            KeyNameInputBox.Text = keyname;

            switch (GetRegistryHiveName(this.hive).ToUpper())
            {
                case "HKEY_CURRENT_CONFIG":
                    {
                        HiveSelector.SelectedIndex = 0;
                        break;
                    }

                case "HKEY_CLASSES_ROOT":
                    {
                        HiveSelector.SelectedIndex = 1;
                        break;
                    }

                case "HKEY_CURRENT_USER":
                    {
                        HiveSelector.SelectedIndex = 2;
                        break;
                    }

                case "HKEY_CURRENT_USER_LOCAL_SETTINGS":
                    {
                        HiveSelector.SelectedIndex = 3;
                        break;
                    }

                case "HKEY_DYN_DATA":
                case "HKEY_DYNAMIC_DATA":
                    {
                        HiveSelector.SelectedIndex = 4;
                        break;
                    }

                case "HKEY_LOCAL_MACHINE":
                    {
                        HiveSelector.SelectedIndex = 5;
                        break;
                    }

                case "HKEY_PERFORMANCE_DATA":
                    {
                        HiveSelector.SelectedIndex = 6;
                        break;
                    }

                case "HKEY_USERS":
                    {
                        HiveSelector.SelectedIndex = 7;
                        break;
                    }
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            keyname = KeyNameInputBox.Text;
            keylocation = KeyLocationPathInputBox.Text;
            hive = GetSelectedHive();
            RunInThreadPool(async () =>
            {
                HelperErrorCodes status = await helper.AddKey(hive, keylocation + "\\" + keyname);

                if (status == HelperErrorCodes.FAILED)
                {
                    RunInUIThread(() => ShowKeyUnableToAddMessageBox());
                }
            });
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private string GetRegistryHiveName(RegHives hive)
        {
            return Enum.GetName(typeof(RegHives), hive);
        }

        private RegHives GetSelectedHive()
        {
            const RegHives hive = RegHives.HKEY_LOCAL_MACHINE;
            int selectedhiveindex = HiveSelector.SelectedIndex;

            switch (selectedhiveindex)
            {
                case 0:
                    {
                        return RegHives.HKEY_CURRENT_CONFIG;
                    }

                case 1:
                    {
                        return RegHives.HKEY_CLASSES_ROOT;
                    }

                case 2:
                    {
                        return RegHives.HKEY_CURRENT_USER;
                    }

                case 3:
                    {
                        return RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS;
                    }

                case 4:
                    {
                        return RegHives.HKEY_DYN_DATA;
                    }

                case 5:
                    {
                        return RegHives.HKEY_LOCAL_MACHINE;
                    }

                case 6:
                    {
                        return RegHives.HKEY_PERFORMANCE_DATA;
                    }

                case 7:
                    {
                        return RegHives.HKEY_USERS;
                    }
            }

            return hive;
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async void RunInUIThread(Action function)
        {
            await
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private async void ShowKeyUnableToAddMessageBox()
        {
            await new Core.MessageDialogContentDialog().ShowMessageDialog(
              ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_add_the_specified_key__no_changes_to_the_phone_registry_were_made_", ResourceContext.GetForCurrentView()).ValueAsString,
              ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }
    }
}