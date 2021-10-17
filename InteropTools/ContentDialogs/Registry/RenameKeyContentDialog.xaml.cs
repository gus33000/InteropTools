// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Linq;
using InteropTools.Providers;
using Windows.ApplicationModel.Resources.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ContentDialogs.Registry
{
    public sealed partial class RenameKeyContentDialog : ContentDialog
    {
        private readonly IRegistryProvider helper;
        private readonly RegHives hive = RegHives.HKEY_LOCAL_MACHINE;
        private readonly string key = "";

        public RenameKeyContentDialog(RegHives hive, string key)
        {
            InitializeComponent();
            helper = App.MainRegistryHelper;
            this.hive = hive;
            this.key = key;
            string currentkey;
            if (key.Contains("\\"))
            {
                currentkey = key.Split('\\').Last();
            }
            else
            {
                currentkey = key;
            }

            NewName.Text = currentkey;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            HelperErrorCodes result = await helper.RenameKey(hive, key, NewName.Text);
            RunInUIThread(() =>
            {
                switch (result)
                {
                    case HelperErrorCodes.AccessDenied:
                        {
                            ShowAccessDeniedMessageBox();
                            break;
                        }

                    case HelperErrorCodes.Failed:
                        {
                            ShowFailedMessageBox();
                            break;
                        }
                }
            });
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void RunInThreadPool(Action function) => await ThreadPool.RunAsync(x => function());

        private async void RunInUIThread(Action function) =>
            await
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => function());

        private async void ShowAccessDeniedMessageBox() =>
            await new Core.MessageDialogContentDialog().ShowMessageDialog(
                ResourceManager.Current.MainResourceMap.GetValue(
                    "Resources/We_couldn_t_rename_the_specified_key_because_its_access_is_denied__no_changes_to_the_phone_registry_were_made",
                    ResourceContext.GetForCurrentView()).ValueAsString,
                ResourceManager.Current.MainResourceMap
                    .GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);

        private async void ShowFailedMessageBox() =>
            await new Core.MessageDialogContentDialog().ShowMessageDialog(
                ResourceManager.Current.MainResourceMap.GetValue(
                    "Resources/We_couldn_t_rename_the_specified_key_due_to_an_unknown_error__no_changes_to_the_phone_registry_were_made",
                    ResourceContext.GetForCurrentView()).ValueAsString,
                ResourceManager.Current.MainResourceMap
                    .GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
    }
}
