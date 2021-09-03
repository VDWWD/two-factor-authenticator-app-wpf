using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace TwoFactor
{
    public partial class OverApp : MetroWindow
    {
        public OverApp()
        {
            InitializeComponent();

            //styles and texts
            this.Title = Localizer.GetLocalized("mainwindow-about");
            txt_copyright2.Text = string.Format("{0}, {1}", DateTime.Now.Year.ToString(), Classes.Globals.AppCopyright);
            txt_website.Text = Classes.Globals.AppUrl.Replace("https://", "");
            txt_email.Text = Classes.Globals.AppEmail;
            txt_versie.Text = string.Format("{0} {1}", Localizer.GetLocalized("about-version"), Classes.Globals.AppVersion.ToString().Replace(",", "."));

            vdwwdlogo.Source = Classes.ResourceController.GetImageFromResource("vdwwd.png");

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                txt_copyright1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_copyright2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_website.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_email.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_versie.Foreground = Classes.ResourceController.BrushDarkModeText;

                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("about-ok"));
            }
            else
            {
                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("about-ok"));
            }
        }


        /// <summary>
        /// Closes the window
        /// </summary>
        private void Button_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        /// <summary>
        /// Handles the keypresses in the window to close it on escape or enter
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Escape)
            {
                Button_ok_Click(null, null);
            }
        }


        /// <summary>
        /// Opens the url in the default browser on the computer
        /// </summary>
        private void Hyperlink1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Classes.Globals.AppUrl);
        }


        /// <summary>
        /// Trigger the mailto to open the mail client on the computer
        /// </summary>
        private void Hyperlink2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto:" + Classes.Globals.AppEmail);
        }
    }
}
