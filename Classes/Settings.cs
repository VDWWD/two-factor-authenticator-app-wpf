using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Linq;

namespace TwoFactor.Classes
{
    public class Setting
    {
        /// <summary>
        /// Loads the settings from the file on the disk
        /// </summary>
        /// <param name="new_settings_load"></param>
        /// <returns>The settings class</returns>
        public static Settings Load(bool new_settings_load)
        {
            var settings = new Settings();

            //check if there is a settings file
            if (File.Exists(Globals.AppPathSettingsFile))
            {
                try
                {
                    using (var stream = File.OpenRead(Globals.AppPathSettingsFile))
                    using (var sr = new StreamReader(stream))
                    {
                        string xml = Encryption.Decrypt(sr.ReadToEnd());
                        var serializer = new XmlSerializer(typeof(Settings));

                        var rdr = new StringReader(xml);
                        settings = (Settings)serializer.Deserialize(rdr);
                    }
                }
                catch
                {
                    //if incorrect xml then save again
                    settings.Save();

                    MessageBox.Show(Localizer.GetLocalized("app-read-error"), Localizer.GetLocalized("app-error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            ////add some logins for testing
            //if (settings.logins == null || settings.logins.Count == 0)
            //{
            //    settings.logins = new List<Login>()
            //    {
            //        new Login()
            //        {
            //            guid = Guid.NewGuid(),
            //            name = "Login 1",
            //            secret = "ABCABC",
            //            date_added = DateTime.Now.AddYears(-1),
            //            date_used = DateTime.Now.AddDays(-10),
            //            used = 400
            //        },
            //        new Login()
            //        {
            //            guid = Guid.NewGuid(),
            //            name = "Login 2",
            //            secret = "DEFDEF",
            //            date_added = DateTime.Now.AddYears(-2),
            //            date_used = DateTime.Now.AddDays(-20),
            //            used = 500
            //        },
            //        new Login()
            //        {
            //            guid = Guid.NewGuid(),
            //            name = "Login 3",
            //            secret = "GHIGHI",
            //            date_added = DateTime.Now.AddYears(-3),
            //            date_used = DateTime.Now.AddDays(-30),
            //            used = 300
            //        }
            //    };
            //}

            //sort the logins
            settings.Sort();

            return settings;
        }


        [XmlRoot(ElementName = "logins")]
        public class Settings
        {
            public List<Login> logins { get; set; }
            public bool store_barcodes { get; set; }
            public bool always_on_top { get; set; }
            public bool ask_password_on_open { get; set; }
            public Enums.SortOrder sortorder { get; set; }

            public Settings()
            {
                logins = new List<Login>();
            }


            public void Save()
            {
                try
                {
                    using (var writer = new StreamWriter(Globals.AppPathSettingsFile))
                    using (var sw = new StringWriter())
                    {
                        var serializer = new XmlSerializer(this.GetType());
                        serializer.Serialize(sw, this);

                        writer.Write(Encryption.Encrypt(sw.ToString()));
                        writer.Flush();
                    }
                }
                catch
                {
                    MessageBox.Show(Localizer.GetLocalized("app-write-error"), Localizer.GetLocalized("app-error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }


            public void Sort()
            {
                if (sortorder == Enums.SortOrder.DateAdded)
                {
                    logins = logins.OrderBy(x => x.date_added).ThenBy(x => x.name).ToList();
                }
                else if (sortorder == Enums.SortOrder.DateLastUsed)
                {
                    logins = logins.OrderByDescending(x => x.date_used).ThenBy(x => x.name).ToList();
                }
                else if (sortorder == Enums.SortOrder.Name)
                {
                    logins = logins.OrderBy(x => x.name).ToList();
                }
                else
                {
                    logins = logins.OrderByDescending(x => x.used).ThenBy(x => x.name).ToList();
                }
            }
        }


        public class Login
        {
            public Guid guid { get; set; }
            public string name { get; set; }
            public string secret { get; set; }
            public DateTime date_added { get; set; }
            public DateTime date_used { get; set; }
            public int used { get; set; }
        }


        public class ComboboxSortorder
        {
            public string name { get; set; }
            public Enums.SortOrder sortorder { get; set; }
        }
    }
}
