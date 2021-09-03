using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace TwoFactor
{
    public partial class AskPassword : MetroWindow
    {
        public int FaildedAttempts { get; set; }
        public bool ForSettingsChange { get; set; }

        public AskPassword()
        {
            InitializeComponent();

            //styles and texts
            this.Title = Localizer.GetLocalized("askpassword-title");

            textbox_password.Focus();

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                textbox_password.Style = Classes.ResourceController.StyleTextBoxDarkMode;

                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("about-ok"));
            }
            else
            {
                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("about-ok"));
            }
        }


        /// <summary>
        /// Check if the password is correct 3 times. After 3 fails closes the app
        /// </summary>
        private void Button_ok_Click(object sender, RoutedEventArgs e)
        {
            string pass1 = textbox_password.Text.Trim();
            var mainwindow = ((MainWindow)Application.Current.MainWindow);

            //if textbox is empty
            if (string.IsNullOrEmpty(pass1))
            {
                textbox_password.Style = Classes.ResourceController.StyleTextBoxError;
                return;
            }

            //check password, if it hits 3 errors then close on app open. 1 time with settings change
            if (pass1 != Classes.WindowsPassword.GetPassword())
            {
                FaildedAttempts++;
                textbox_password.Text = "";

                if (FaildedAttempts == 1 && ForSettingsChange)
                {
                    this.DialogResult = true;
                }
                else if (FaildedAttempts == 3)
                {
                    MessageBox.Show(Localizer.GetLocalized("askpassword-3times"), "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    mainwindow.CloseApp();
                }

                return;
            }

            //password ok
            if (ForSettingsChange)
            {
                ((Settings)Owner).PasswordIsCorrect = true;
            }
            else
            {
                mainwindow.PasswordIsCorrect = true;
            }

            this.DialogResult = true;
        }


        /// <summary>
        /// Handles the keypresses in the window to close it on escape or enter
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                Button_ok_Click(null, null);
            }
            else if (e.Key == Key.Escape && ForSettingsChange)
            {
                this.DialogResult = true;
            }
            else if (e.Key == Key.Escape)
            {
                ((MainWindow)Application.Current.MainWindow).CloseApp();
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
            }
            else
            {
                textbox_password.Style = Classes.ResourceController.StyleTextBoxNormal;
            }
        }
    }
}
