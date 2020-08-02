using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.CorePages
{
    public sealed partial class UserDisplayControl : UserControl
    {
        public UserDisplayControl()
        {
            this.InitializeComponent();
            Loaded += UserDisplayControl_Loaded;
        }

        private async void UserDisplayControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IReadOnlyList<User> users = await User.FindAllAsync();

                var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                            p.Type == UserType.LocalUser).FirstOrDefault();

                // user may have username
                var data = await current.GetPropertyAsync(KnownUserProperties.AccountName);
                string displayName = (string)data;

                bool okay = false;

                //or may be authenticated using hotmail 
                if (String.IsNullOrEmpty(displayName))
                {
                    okay = true;
                    string a = (string)await current.GetPropertyAsync(KnownUserProperties.FirstName);
                    string b = (string)await current.GetPropertyAsync(KnownUserProperties.LastName);
                    displayName = string.Format("{0} {1}", a, b);
                }

                UserName.Text = displayName;

                if (UserName.Text == "") okay = false;

                // user may have profile pic
                var datapic = await current.GetPictureAsync(UserPictureSize.Size64x64);

                if (datapic != null)
                {
                    okay = true;
                    var pic = new BitmapImage();
                    pic.SetSource(await datapic.OpenReadAsync());

                    var imgbrush = new ImageBrush();
                    imgbrush.ImageSource = pic;

                    Image.Background = imgbrush;
                    PicImage.ProfilePicture = pic;
                }

                if (datapic == null) okay = false;

                if (!okay)
                {
                    Image.Background = new SolidColorBrush(Colors.Gray);
                    UserName.Text = "Unknown User";
                }
            }
            catch
            {

            }
        }
    }
}
