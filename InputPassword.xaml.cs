using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace TwoFactor
{
    public partial class InputPassword : MetroWindow
    {
        public InputPassword()
        {
            InitializeComponent();

            //styles and texts
            this.Title = $"{Localizer.GetLocalized("inputpassword-welcome-to")} {Classes.Globals.AppDeveloper} {Classes.Globals.AppName}";

            txt_introtext.Text = Localizer.GetLocalized("inputpassword-create-password");
            txt_label1.Text = Localizer.GetLocalized("inputpassword-password");
            txt_label2.Text = Localizer.GetLocalized("inputpassword-password-repeat");

            textbox_password.Focus();

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                txt_introtext.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label2.Foreground = Classes.ResourceController.BrushDarkModeText;

                textbox_password.Style = Classes.ResourceController.StyleTextBoxDarkMode;
                textbox_password_repeat.Style = Classes.ResourceController.StyleTextBoxDarkMode;

                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("about-ok"));
            }
            else
            {
                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("about-ok"));
            }
        }


        /// <summary>
        /// Saves the newly created password in Windows if the password is the correct length
        /// </summary>
        private void Button_ok_Click(object sender, RoutedEventArgs e)
        {
            string pass1 = textbox_password.Text.Trim();
            string pass2 = textbox_password_repeat.Text.Trim();

            //if textbox is empty or the length is incorrect then make red and quit
            if (string.IsNullOrEmpty(pass1) || pass1.Length < 6 || pass1 != pass2)
            {
                textbox_password.Style = Classes.ResourceController.StyleTextBoxError;
                textbox_password_repeat.Style = Classes.ResourceController.StyleTextBoxError;
                return;
            }

            //store the password in windows
            Classes.WindowsPassword.SetPassword(pass1);

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
        /// Triggers the Button_ok_Click when pressing enter in the textbox
        /// </summary>
        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_ok_Click(null, null);
            }
        }


        /// <summary>
        /// Changes the textbox style when something is types from red to the normal color it the textbox was empty
        /// </summary>
        private void TextBoxResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Classes.Globals.IsDarkMode())
            {
                textbox_password.Style = Classes.ResourceController.StyleTextBoxDarkMode;
                textbox_password_repeat.Style = Classes.ResourceController.StyleTextBoxDarkMode;
            }
            else
            {
                textbox_password.Style = Classes.ResourceController.StyleTextBoxNormal;
                textbox_password_repeat.Style = Classes.ResourceController.StyleTextBoxNormal;
            }
        }
    }
}
