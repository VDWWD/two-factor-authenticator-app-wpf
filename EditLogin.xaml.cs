using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;

namespace TwoFactor
{
    public partial class EditLogin : MetroWindow
    {
        public Classes.Setting.Login Login { get; set; }
        public string QrPath { get; set; }
        public byte[] QrImage { get; set; }


        public EditLogin(Classes.Setting.Login login)
        {
            InitializeComponent();
            Login = login;

            //styles and texts
            this.Title = Localizer.GetLocalized("editlogin-title") + Login.name;

            txt_dateadded1.Text = Localizer.GetLocalized("editlogin-date-added");
            txt_dateused1.Text = Localizer.GetLocalized("editlogin-date-used");
            txt_usages1.Text = Localizer.GetLocalized("editlogin-usage");

            txt_dateadded2.Text = $"{getDateWithoutWeekday(Login.date_added)}, {Login.date_added.ToShortTimeString()}";
            txt_dateused2.Text = $"{getDateWithoutWeekday(Login.date_used)}, {Login.date_used.ToShortTimeString()}";
            txt_usages2.Text = Login.used.ToString();

            textbox_login.Text = Login.name;
            textbox_login.Focus();
            textbox_login.CaretIndex = Login.name.Length;

            //if the login has never been used
            if (Login.date_used.Year < 2000)
            {
                txt_dateused1.Text = txt_dateadded1.Text;
                txt_dateused2.Text = txt_dateadded2.Text;
                txt_dateadded1.Text = "";
                txt_dateadded2.Text = "";
            }

            QrPath = Classes.Globals.AppPath + $"qr_code_{Login.guid.ToString().Replace("-", "")}.gif";

            //check if there is a stored qr image of the login, if found show it
            if (File.Exists(QrPath))
            {
                QrImage = File.ReadAllBytes(QrPath);
                Image1.Source = BitmapFrame.Create(new MemoryStream(QrImage));
            }
            else
            {
                button_download.Visibility = Visibility.Collapsed;
                this.Height = 300;
            }

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                txt_dateadded1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_dateused1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_usages1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_dateadded2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_dateused2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_usages2.Foreground = Classes.ResourceController.BrushDarkModeText;

                button_delete.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Delete, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("editlogin-delete"));
                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("editlogin-save"));

                textbox_login.Style = Classes.ResourceController.StyleTextBoxDarkMode;
            }
            else
            {
                button_delete.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Delete, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("editlogin-delete"));
                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("editlogin-save"));
            }
        }


        /// <summary>
        /// Saves the new login name if the name is the correct length
        /// </summary>
        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            string value = textbox_login.Text.Trim();

            //if textbox is empty or the length is incorrect then make red and quit
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                textbox_login.Style = Classes.ResourceController.StyleTextBoxError;
                return;
            }

            Login.name = value;

            MainWindow.Settings.Save();

            this.DialogResult = true;
        }


        /// <summary>
        /// Handles the keypresses in the window to close it on escape or triggers Button_save_Click on enter
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = true;
            }
            else if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                Button_save_Click(null, null);
            }
        }


        /// <summary>
        /// Triggers the Button_save_Click when pressing enter in the textbox
        /// </summary>
        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_save_Click(null, null);
            }
        }


        /// <summary>
        /// Changes the textbox style when something is types from red to the normal color it the textbox was empty
        /// </summary>     
        private void TextBoxResult_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Classes.Globals.IsDarkMode())
            {
                textbox_login.Style = Classes.ResourceController.StyleTextBoxDarkMode;
            }
            else
            {
                textbox_login.Style = Classes.ResourceController.StyleTextBoxNormal;
            }
        }


        /// <summary>
        /// Show login delete confirmation. When yes removes it and saves
        /// </summary>
        private void Button_delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(string.Format(Localizer.GetLocalized("editlogin-delete-confirm"), Login.name), Localizer.GetLocalized("editlogin-delete-title"), MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow.Settings.logins.RemoveAll(x => x.guid == Login.guid);
                MainWindow.Settings.Save();
                this.DialogResult = true;
            }
        }


        /// <summary>
        /// If there is a qr code, it opens the save dialog and writes the qr code image to disk
        /// </summary>
        private void Button_download_qr_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = $"qr_code_{Login.name.Replace(" ", "-")}.gif";
            dialog.DefaultExt = ".gif";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                File.WriteAllBytes(dialog.FileName, QrImage);
            }
        }


        /// <summary>
        /// Gets the date without weekday in the correct localization: May 27, 2021
        /// </summary>
        /// <param name="value">The DateTime date</param>
        /// <returns>String with the date without weekday</returns>
        public string getDateWithoutWeekday(DateTime date)
        {
            return date.ToLongDateString().Replace(DateTimeFormatInfo.CurrentInfo.GetDayName(Convert.ToDateTime(date).DayOfWeek), "").TrimStart(", ".ToCharArray());
        }
    }
}
