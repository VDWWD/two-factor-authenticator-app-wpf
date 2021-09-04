using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Media;

namespace TwoFactor
{
    public partial class Settings : MetroWindow
    {
        public bool PasswordIsCorrect;

        public Settings()
        {
            InitializeComponent();

            //styles and texts
            this.Title = Localizer.GetLocalized("settings-title");

            txt_label1.Text = Localizer.GetLocalized("settings-sortorder");
            txt_label2.Text = Localizer.GetLocalized("settings-saveqr");
            txt_label3.Text = Localizer.GetLocalized("settings-askpass");
            txt_warning.Text = Localizer.GetLocalized("settings-warning");
            toggle_saveqr.OffContent = "";
            toggle_saveqr.OnContent = "";
            toggle_askpass.OffContent = "";
            toggle_askpass.OnContent = "";

            //combobox values
            combobox_sortorder.Items.Add(new Classes.Setting.ComboboxSortorder() { sortorder = Classes.Enums.SortOrder.MostUsed, name = Localizer.GetLocalized("settings-sorting-mostused") });
            combobox_sortorder.Items.Add(new Classes.Setting.ComboboxSortorder() { sortorder = Classes.Enums.SortOrder.Name, name = Localizer.GetLocalized("settings-sorting-name") });
            combobox_sortorder.Items.Add(new Classes.Setting.ComboboxSortorder() { sortorder = Classes.Enums.SortOrder.DateAdded, name = Localizer.GetLocalized("settings-sorting-dateadded") });
            combobox_sortorder.Items.Add(new Classes.Setting.ComboboxSortorder() { sortorder = Classes.Enums.SortOrder.DateLastUsed, name = Localizer.GetLocalized("settings-sorting-datelastused") });

            toggle_saveqr.IsOn = MainWindow.Settings.store_barcodes;
            toggle_askpass.IsOn = MainWindow.Settings.ask_password_on_open;
            combobox_sortorder.SelectedValue = MainWindow.Settings.sortorder;

            //add the toggled handler after setting the value from the settings
            toggle_askpass.Toggled += toggle_askpass_Toggled;

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                toggle_saveqr.Style = (Style)FindResource("ToggleSwitch_Normal_DarkMode");
                toggle_askpass.Style = (Style)FindResource("ToggleSwitch_Normal_DarkMode");
                combobox_sortorder.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#acb6d2");

                txt_label1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label3.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_warning.Foreground = Classes.ResourceController.BrushDarkModeText;

                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("editlogin-save"));
                button_export.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Export, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("settings-export"));
            }
            else
            {
                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("editlogin-save"));
                button_export.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Export, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("settings-export"));
            }

            //if there are no logins then hide export button
            if (MainWindow.Settings.logins == null || MainWindow.Settings.logins.Count == 0)
            {
                button_export.Visibility = Visibility.Hidden;
            }
        }


        /// <summary>
        /// Saves the settings and closes the window
        /// </summary>
        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Settings.store_barcodes = toggle_saveqr.IsOn;
            MainWindow.Settings.ask_password_on_open = toggle_askpass.IsOn;
            MainWindow.Settings.sortorder = ((Classes.Setting.ComboboxSortorder)combobox_sortorder.SelectedItem).sortorder;
            MainWindow.Settings.Save();
            MainWindow.Settings.Sort();
            ((MainWindow)Application.Current.MainWindow).AddLoginsToPanel();

            this.DialogResult = true;
        }


        /// <summary>
        /// Clears the combobox focus to set the border to the default style again
        /// </summary>
        private void ClearComboBoxFocus()
        {
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(combobox_sortorder), null);
            Keyboard.ClearFocus();
        }


        /// <summary>
        /// Handles the keypresses in the window to close it on escape or enter
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            ClearComboBoxFocus();

            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Escape)
            {
                this.DialogResult = true;
            }
        }


        /// <summary>
        /// Exports the settings to a zip file
        /// </summary>
        private void Button_export_Click(object sender, RoutedEventArgs e)
        {
            //create a zip archive
            var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create))
            {
                //add stored qr code images to zip
                foreach (var item in Directory.GetFiles(Classes.Globals.AppPath, "*.gif"))
                {
                    var file = File.ReadAllBytes(item);

                    var zipArchiveEntry = archive.CreateEntry(Path.GetFileName(item), CompressionLevel.Optimal);
                    using (var zipStream = zipArchiveEntry.Open())
                    {
                        zipStream.Write(file, 0, file.Length);
                    }
                }

                //add the settings file to the zip
                var settingsfile = File.ReadAllBytes(Classes.Globals.AppPathSettingsFile);
                var zipArchiveEntry1 = archive.CreateEntry(Path.GetFileName(Classes.Globals.AppPathSettingsFile), CompressionLevel.Optimal);
                using (var zipStream = zipArchiveEntry1.Open())
                {
                    zipStream.Write(settingsfile, 0, settingsfile.Length);
                }

                //add the txt file to the zip
                var textfile = Encoding.ASCII.GetBytes(Localizer.GetLocalized("settings-instructions"));
                var zipArchiveEntry2 = archive.CreateEntry("Instructions.txt", CompressionLevel.Optimal);
                using (var zipStream = zipArchiveEntry2.Open())
                {
                    zipStream.Write(textfile, 0, textfile.Length);
                }
            }

            byte[] bin = ms.ToArray();

            //show save file dialog
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = $"{Classes.Globals.AppName}_backup_{DateTime.Now.ToString("yyyyMMdd")}.zip";
            dialog.DefaultExt = ".zip";

            Nullable<bool> result = dialog.ShowDialog();

            //dialog ok then save
            if (result == true)
            {
                File.WriteAllBytes(dialog.FileName, bin);
            }
        }


        /// <summary>
        /// Window left mouse up event handler
        /// </summary>
        private void MetroWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearComboBoxFocus();
        }


        /// <summary>
        /// Combobox selection changed event handler
        /// </summary>
        private void combobox_sortorder_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ClearComboBoxFocus();
        }


        /// <summary>
        /// Ask for the password when enabling it on startup
        /// </summary>
        private void toggle_askpass_Toggled(object sender, RoutedEventArgs e)
        {
            PasswordIsCorrect = false;

            if (!toggle_askpass.IsOn)
            {
                return;
            }

            var askpass = new AskPassword()
            {
                Owner = this,
                ForSettingsChange = true
            };

            askpass.ShowDialog();

            //needed to capture clicking of the close button in AskPassword window that otherwise bypasses the password check
            if (!PasswordIsCorrect)
            {
                toggle_askpass.IsOn = false;
            }
        }
    }
}
