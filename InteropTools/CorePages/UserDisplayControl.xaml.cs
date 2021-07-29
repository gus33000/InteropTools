using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.CorePages
{
    public sealed partial class UserDisplayControl : UserControl
    {
        public UserDisplayControl()
        {
            InitializeComponent();
            Loaded += UserDisplayControl_Loaded;
        }

        private async void UserDisplayControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                IReadOnlyList<User> users = await User.FindAllAsync();

                User current = users.FirstOrDefault(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                            p.Type == UserType.LocalUser);

                // user may have username
                object data = await current.GetPropertyAsync(KnownUserProperties.AccountName);
                string displayName = (string)data;

                bool okay = false;

                //or may be authenticated using hotmail
                if (string.IsNullOrEmpty(displayName))
                {
                    okay = true;
                    string a = (string)await current.GetPropertyAsync(KnownUserProperties.FirstName);
                    string b = (string)await current.GetPropertyAsync(KnownUserProperties.LastName);
                    displayName = string.Format("{0} {1}", a, b);
                }

                UserName.Text = displayName;

                if (UserName.Text?.Length == 0)
                {
                    okay = false;
                }

                // user may have profile pic
                Windows.Storage.Streams.IRandomAccessStreamReference datapic = await current.GetPictureAsync(UserPictureSize.Size64x64);

                if (datapic != null)
                {
                    okay = true;
                    BitmapImage pic = new();
                    pic.SetSource(await datapic.OpenReadAsync());

                    ImageBrush imgbrush = new()
                    {
                        ImageSource = pic
                    };

                    Image.Background = imgbrush;
                    PicImage.ProfilePicture = pic;
                }

                if (datapic == null)
                {
                    okay = false;
                }

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