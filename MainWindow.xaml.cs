using System;
using System.Windows;
using MahApps.Metro.Controls;
using System.Drawing;
using System.Media;
using System.IO;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using System.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;
using StickyWindows;
using StickyWindows.WPF;

namespace TwoFactor
{
    // The extra libraries needed to run this app are included as an embedded resource so the app can run as a single .exe file in a folder without dll's.
    // These are loaded in the App.xaml.cs > Main() with AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>

    // This project also uses the 2 following libraries (https://www.nuget.org/packages/CredentialManagement & https://www.nuget.org/packages/StickyWindows.WPF/0.3.0-unstable0009) but are not installed with the package manager.
    // To include libraries as resource they need to be strongly named but are not when installing from nuget.
    // In order to maintain transparency i did not download and recompile those libraries myself, but included them as code files so everyone can see that no changes were made.


    public partial class MainWindow : MetroWindow
    {
        private StickyWindow StickyWindow;
        private TaskbarIcon TaskbarIcon;
        public static Classes.Setting.Settings Settings;
        public SoundPlayer PlayerError;
        public bool IsFirstLoad;
        public bool PasswordIsCorrect;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                //make windows sticky
                this.Loaded += MakeStickyWindow;

                //initen global variables
                Classes.Globals.InitGlobals();

                //styles and texts
                SetStylesAndTexts();

                //init app specific code
                InitAppStuff();

                //create the taskbar icon
                CreateTaskBarIcon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Localizer.GetLocalized("mainwindow-error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #region init


        /// <summary>
        /// Sets the window styles and localized texts
        /// </summary>
        private void SetStylesAndTexts()
        {
            //title
            this.Title = $"{Classes.Globals.AppDeveloper} {Classes.Globals.AppName}";

            //button tooltips
            tt_Button_desktop_qr.Content = Localizer.GetLocalized("mainwindow-tt-qr-desktop");
            tt_Button_file_qr.Content = Localizer.GetLocalized("mainwindow-tt-qr-file");

            //tooltip text in title bar
            WindowButtons.Minimize = Localizer.GetLocalized("mainwindow-minimize");
            WindowButtons.Maximize = Localizer.GetLocalized("mainwindow-maximize");
            WindowButtons.Restore = Localizer.GetLocalized("mainwindow-restore");
            WindowButtons.Close = Localizer.GetLocalized("mainwindow-close");

            //header buttons
            button_about.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.About, Classes.ResourceController.BrushWhite, null);
            button_pinned.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Pin, Classes.ResourceController.BrushWhite, null);
            button_unpinned.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Unpin, Classes.ResourceController.BrushWhite, null);

            //header button tooltips
            tt_overAppHeader.Content = Localizer.GetLocalized("mainwindow-about");
            tt_Pinned.Content = Localizer.GetLocalized("mainwindow-ontop");
            tt_UnPinned.Content = Localizer.GetLocalized("mainwindow-ontop");
            tt_Button_settings.Content = Localizer.GetLocalized("settings-tooltip");

            //bottom buttons
            button_qr_desktop.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Desktop, Classes.ResourceController.BrushGray, null, 0.5d);
            button_qr_file.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Browse, Classes.ResourceController.BrushGray, null, 0.5d);
            button_settings.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Settings, Classes.ResourceController.BrushGray, null, 0.5d);

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;
            }
        }


        /// <summary>
        /// Start some app specific stuff
        /// </summary>
        public void InitAppStuff()
        {
            IsFirstLoad = true;

            //get the windows password
            string ww = Classes.WindowsPassword.GetPassword();

            //if the password is empty the open the new password window
            if (string.IsNullOrEmpty(ww))
            {
                var passw = new InputPassword();
                passw.ShowDialog();

                //check if the password save was succesful, needed if the close button on the InputPassword window is clicked
                ww = Classes.WindowsPassword.GetPassword();
                if (string.IsNullOrEmpty(ww))
                {
                    CloseApp();
                }
            }

            //load settings
            Settings = Classes.Setting.Load();

            //ask password on open
            if (Settings.ask_password_on_open)
            {
                var askpass = new AskPassword();

                askpass.ShowDialog();

                //needed to capture clicking of the close button in AskPassword window that otherwise bypasses the password check
                if (!PasswordIsCorrect)
                {
                    CloseApp();
                }
            }

            //pin the app if stored in settings
            if (Settings.always_on_top)
            {
                Button_pin_Click(null, null);
            }

            //set the error sound
            PlayerError = new SoundPlayer(Classes.ResourceController.GetStreamFromResource("sound_error.wav"));

            //add the logins to the window
            AddLoginsToPanel();

            IsFirstLoad = false;
        }


        /// <summary>
        /// Creates the taskbar icon with a context menu
        /// </summary>
        public void CreateTaskBarIcon()
        {
            //cerate the taskbar icon
            TaskbarIcon = new TaskbarIcon()
            {
                ToolTipText = this.Title,
                Icon = new Icon(Classes.ResourceController.GetStreamFromResource("favicon_taskbar.ico")),
                ContextMenu = new ContextMenu()
                {
                    HasDropShadow = false
                }
            };

            //add items to the menu
            TaskbarIcon.ContextMenu.Items.Add(Classes.ResourceController.CreateContextMenuItem(Classes.Enums.Icon.Maximize, Localizer.GetLocalized("mainwindow-maximize"), Contextmenu_maximize_Click));
            TaskbarIcon.ContextMenu.Items.Add(Classes.ResourceController.CreateContextMenuItem(Classes.Enums.Icon.About, Localizer.GetLocalized("mainwindow-about"), Button_about_Click));
            TaskbarIcon.ContextMenu.Items.Add(new Separator());
            TaskbarIcon.ContextMenu.Items.Add(Classes.ResourceController.CreateContextMenuItem(Classes.Enums.Icon.Close, Localizer.GetLocalized("mainwindow-close"), Contextmenu_close_Click));

            //insert it at the top of the dockpanel
            dockpanel_main.Children.Insert(0, TaskbarIcon);
        }


        #endregion


        #region various


        /// <summary>
        /// makes the window snap to the edges of the screen
        /// </summary>
        void MakeStickyWindow(object sender, RoutedEventArgs e)
        {
            StickyWindow = this.CreateStickyWindow();
        }


        /// <summary>
        /// Mazimizes the window
        /// </summary>
        private void NormalWindow()
        {
            this.Show();
            this.Activate();
            this.WindowState = WindowState.Normal;
        }


        /// <summary>
        /// Closes the app
        /// </summary>
        public void CloseApp()
        {
            if (TaskbarIcon != null)
            {
                TaskbarIcon.Dispose();
            }

            Application.Current.Shutdown();
            Environment.Exit(0);
        }


        /// <summary>
        /// State changed handler
        /// </summary>
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }

            base.OnStateChanged(e);
        }


        #endregion


        #region menus


        /// <summary>
        /// Mazimizes the window from the context menu
        /// </summary>
        private void Contextmenu_maximize_Click(object Sender, EventArgs e)
        {
            NormalWindow();
        }

        /// <summary>
        /// Closes the app from the context menu
        /// </summary>
        private void Contextmenu_close_Click(object Sender, EventArgs e)
        {
            CloseApp();
        }

        /// <summary>
        /// Opens the about windows from the context menu
        /// </summary>
        private void Button_about_Click(object Sender, EventArgs e)
        {
            NormalWindow();

            var about = new OverApp()
            {
                Owner = Application.Current.MainWindow
            };

            about.ShowDialog();
        }

        /// <summary>
        /// Sets the window to the top or not
        /// </summary>
        private void Button_pin_Click(object Sender, EventArgs e)
        {
            if (this.Topmost)
            {
                this.Topmost = false;
                button_pinned.Visibility = Visibility.Collapsed;
                button_unpinned.Visibility = Visibility.Visible;

                Settings.always_on_top = false;
            }
            else
            {
                this.Topmost = true;
                button_pinned.Visibility = Visibility.Visible;
                button_unpinned.Visibility = Visibility.Collapsed;

                Settings.always_on_top = true;
            }


            //do not save the changes if triggered from MainWindow()
            if (IsFirstLoad)
                return;

            Settings.Save();
        }


        #endregion


        #region buttons


        /// <summary>
        /// Take a screenshot of the desktop button and scan it for a qr code
        /// </summary>
        private void Button_qr_desktop_Click(object sender, RoutedEventArgs e)
        {
            //get the size of the virtual screen (includes all monitors)
            int left = System.Windows.Forms.SystemInformation.VirtualScreen.Left;
            int top = System.Windows.Forms.SystemInformation.VirtualScreen.Top;
            int width = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
            int height = System.Windows.Forms.SystemInformation.VirtualScreen.Height;

            //create a bitmap of the entire desktop
            using (var stream = new MemoryStream())
            using (var bmp = new Bitmap(width, height))
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(left, top, 0, 0, bmp.Size);

                //save to disk or a memorystream if you need the screenshot elsewhere
                //bmp.Save(stream, ImageFormat.Jpeg);
                //bmp.Save(@"c:\temp\test.jpg", ImageFormat.Jpeg);

                //try to find a qr code in the screenshot and display result
                string result = FindQrCodeInImage(bmp);
            }
        }


        /// <summary>
        /// Opens the file browser window to look for a qr code image from the disk
        /// </summary>
        private void Button_qr_file_Click(object sender, EventArgs e)
        {
            //open the file dialog with only images as the filter
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "QR Code | *.jpg; *.jpeg; *.gif; *.bmp; *.png"
            };

            //a file is selected
            if (dialog.ShowDialog() == true)
            {
                //read the selected file as a byte array
                var bin = File.ReadAllBytes(dialog.FileName);

                //try cath if the selected file is not an image or is corrupted
                try
                {
                    //make a bmp from the selected file
                    using (var stream = new MemoryStream(bin))
                    using (var bmp = new Bitmap(stream))
                    {
                        //read the qr code and show the result
                        string result = FindQrCodeInImage(bmp);
                    }
                }
                catch
                {
                    //play the error sound
                    PlayerError.Play();
                }
            }
        }


        /// <summary>
        /// Opens the app settings window
        /// </summary>
        private void Button_settings_Click(object sender, EventArgs e)
        {
            var settings = new Settings()
            {
                Owner = Application.Current.MainWindow
            };

            settings.ShowDialog();
        }


        #endregion


        #region addlogin


        /// <summary>
        /// Searches a bitmap to find a qr code in it
        /// </summary>
        /// <param name="bmp">An image with hopefully a qr code in it</param>
        /// <returns>The string from the qr code. If not found return null</returns>
        private string FindQrCodeInImage(Bitmap bmp)
        {
            //decode the bitmap and try to find a qr code
            var source = new BitmapLuminanceSource(bmp);
            var bitmap = new BinaryBitmap(new HybridBinarizer(source));
            var result = new MultiFormatReader().decode(bitmap);

            //no qr code found in bitmap
            if (result == null || string.IsNullOrEmpty(result.Text.Trim()))
            {
                //play the error sound
                PlayerError.Play();

                return null;
            }

            string token = "";
            string name = Localizer.GetLocalized("mainwindow-new-login");

            //examples
            //formaat fortinet: {SECRET}
            //formaat vdwwd: otpauth://totp/TrainDatabase?secret={SECRET}
            //formaat transip: otpauth://totp/eetcafeh41?secret={SECRET}&issuer=TransIP

            //zoek de juiste data in de string
            if (result.Text.Contains(":"))
            {
                var url = new Uri(result.Text);

                string issuer = System.Web.HttpUtility.ParseQueryString(url.Query).Get("issuer");
                string secret = System.Web.HttpUtility.ParseQueryString(url.Query).Get("secret");
                string account = "";

                try
                {
                    account = result.Text.Split('?')[0].Split('/').Last();
                }
                catch
                {
                }

                if (!string.IsNullOrEmpty(issuer))
                {
                    name = issuer;

                    if (!string.IsNullOrEmpty(account))
                    {
                        name += $" ({account})";
                    }
                }
                else if (!string.IsNullOrEmpty(account))
                {
                    name = account;
                }

                if (!string.IsNullOrEmpty(secret))
                    token = secret;
            }
            else
            {
                token = result.Text;
            }

            //if no token found then quit
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show(Localizer.GetLocalized("mainwindow-qrerror"), Localizer.GetLocalized("mainwindow-qrerror-title"), MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            //check if the token already exists
            if (Settings.logins.Any(x => x.secret == token))
            {
                MessageBox.Show(Localizer.GetLocalized("mainwindow-duplicate"), Localizer.GetLocalized("mainwindow-duplicate-title"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            //create a new login
            var newlogin = new Classes.Setting.Login()
            {
                date_added = DateTime.Now,
                guid = Guid.NewGuid(),
                name = name.Replace("+", " "),
                secret = token
            };

            //save it
            Settings.logins.Add(newlogin);
            Settings.Save();

            //and/or save the new qr code image to disk if needed
            if (Settings.store_barcodes)
            {
                //create a new qr code image
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Height = 300,
                        Width = 300
                    }
                };

                //write the result to the new qr code image
                var qrcode = writer.Write(result.Text);

                //save the barcode to disk
                try
                {
                    qrcode.Save($"qr_code_{newlogin.guid.ToString().Replace("-", "")}.gif", ImageFormat.Gif);
                }
                catch
                {
                    MessageBox.Show(Localizer.GetLocalized("mainwindow-saveqr-warning"), Localizer.GetLocalized("mainwindow-saveqr-title"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            //play the success sound
            var player_ok = new SoundPlayer(Classes.ResourceController.GetStreamFromResource("sound_ok.wav"));
            player_ok.Play();

            //add the new login to the current window
            AddLoginsToPanel();

            //return the found qr code text
            return result.Text;
        }


        /// <summary>
        /// Fills the window with the stored logins
        /// </summary>
        /// <param name="logins">The list of logins</param>
        public void AddLoginsToPanel()
        {
            stackpanel_logins.Children.Clear();

            var d = DateTime.Now;
            var date = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);

            //add each login to the window
            for (int i = 0; i < Settings.logins.Count; i++)
            {
                var loginpanel = new LoginPanel(Settings.logins[i], i == Settings.logins.Count - 1, date);

                stackpanel_logins.Children.Add(loginpanel);
            }
        }


        #endregion
    }
}
