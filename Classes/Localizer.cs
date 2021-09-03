using System;
using System.Collections.Generic;
using System.Linq;

namespace TwoFactor
{
    public class Localizer
    {
        public static List<LanguageEntry> LocalizationData { get; set; }


        /// <summary>
        /// Get the correct localized text based on the key
        /// </summary>
        /// <param name="key">The key for the localized text</param>
        /// <returns>String</returns>
        public static string GetLocalized(string key)
        {
            if (LocalizationData == null)
                CreateLanguageData();
                
            var item = LocalizationData.Where(x => x.key == key).FirstOrDefault();

            //key not found
            if (item == null)
            {
                return "KEY-NOT-FOUND" + key;
            }

            string value;
            if (Classes.Globals.Language == Classes.Enums.Language.NL)
            {
                value = item.nl;
            }
            else if (Classes.Globals.Language == Classes.Enums.Language.DE)
            {
                value = item.de;
            }
            else
            {
                value = item.en;
            }

            if (value == null)
            {
                return "VALUE-NOT-FOUND";
            }
            else
            {
                return value;
            }
        }


        /// <summary>
        /// Creates a list with all the localized items. This could be modified to create a list from another internal or external source
        /// </summary>
        public static void CreateLanguageData()
        {
            LocalizationData = new List<LanguageEntry>()
            {
                new LanguageEntry()
                {
                    key = "mainwindow-error",
                    nl = "Kritieke fout!",
                    en = "Critical error!",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-closeapp",
                    nl = "Weet je zeker dat je {APPNAME} wilt afsluiten?",
                    en = "Are you sure you want to close {APPNAME}?",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-closing",
                    nl = "Afsluiten...",
                    en = "Closing...",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-close",
                    nl = "{APPNAME} afsluiten",
                    en = "Exit {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-restore",
                    nl = "{APPNAME} openen",
                    en = "Restore {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-minimize",
                    nl = "{APPNAME} minimaliseren",
                    en = "Minimize {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-maximize",
                    nl = "{APPNAME} maximaliseren",
                    en = "Maximize {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-about",
                    nl = "Over {APPNAME}",
                    en = "About {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-ontop",
                    nl = "Bovenste venster aan/uit",
                    en = "Toggle always on top",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "about-version",
                    nl = "Versie",
                    en = "Version",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "about-ok",
                    nl = "OK",
                    en = "OK",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-cancel",
                    nl = "Annuleren",
                    en = "Cancel",
                    de = ""
                },                  
                

                //appspecifiek
                new LanguageEntry()
                {
                    key = "app-read-error",
                    nl = "Het lezen van de instellingen is mislukt.",
                    en = "The settings failed to load.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "app-write-error",
                    nl = "Het opslaan van de instellingen is mislukt.",
                    en = "The settings failed to save.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "app-error",
                    nl = $"{Classes.Globals.AppName} Fout",
                    en = $"{Classes.Globals.AppName} Error",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "inputpassword-welcome-to",
                    nl = "Welkom bij",
                    en = "Welcome to",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "inputpassword-create-password",
                    nl = "Verzin eenmalig een wachtwoord van minimaal 6 tekens.\nDit zal gebruikt worden om je logins te beveiligen.",
                    en = "Create a one time password with a minimum length of 6 characters.\nThis will be used to protect your logins.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-title",
                    nl = "Bewerk login: ",
                    en = "Edit login: ",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-save",
                    nl = "Opslaan",
                    en = "Save",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-delete",
                    nl = "Verwijder",
                    en = "Delete",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-delete-confirm",
                    nl = "Weet je zeker dat je login \"{0}\" wil verwijderden?",
                    en = "Are you sure you want to delete the login \"{0}\"",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-delete-title",
                    nl = "Login verwijderen?",
                    en = "Remove login?",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-tt-qr-desktop",
                    nl = "Scan de de desktop voor een QR code.",
                    en = "Scan the desktop for a QR code.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-tt-qr-file",
                    nl = "Open een QR code afbeelding vanaf de computer.",
                    en = "Open a QR code image from the computer.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-copied",
                    nl = "Code gekopieerd naar klembord.",
                    en = "Code copied to clipboard.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-date-added",
                    nl = "Datum toegevoegd:",
                    en = "Date added:",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-date-used",
                    nl = "Datum laatst gebruikt:",
                    en = "Date last used:",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editlogin-usage",
                    nl = "Aantal keer gebruikt:",
                    en = "Numer of times used:",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-saveqr-warning",
                    nl = "Fout bij opslaan QR code op schijf.\nSla de QR code handmatig op.",
                    en = "Error on saving QR code to disk.\nSave the QR code manually.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-saveqr-title",
                    nl = "Disk fout",
                    en = "Disc eror",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-qrerror-title",
                    nl = "QR code leesfout",
                    en = "QR code read error",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-qrerror",
                    nl = "Geen Secret Key gevonden in de QR code.",
                    en = "No Secret Key found in the QR code.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-duplicate-title",
                    nl = "Dubbele QR code",
                    en = "Duplicate QR code",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-duplicate",
                    nl = "Deze QR code is al een keer toegevoegd.",
                    en = "This QR code has already been added.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-new-login",
                    nl = "Nieuwe Login",
                    en = "New Login",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "inputpassword-password",
                    nl = "Wachtwoord",
                    en = "Password",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "inputpassword-password-repeat",
                    nl = "Herhaal wachtwoord",
                    en = "Repeat password",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-export",
                    nl = "Export",
                    en = "Export",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sorting-mostused",
                    nl = "Meest gebruikt",
                    en = "Most used",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sorting-name",
                    nl = "Naam",
                    en = "Name",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sorting-dateadded",
                    nl = "Datum toegevoegd",
                    en = "Date added",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sorting-datelastused",
                    nl = "Datum laatst gebruikt",
                    en = "Date last used",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-title",
                    nl = "Instellingen",
                    en = "Settings",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-tooltip",
                    nl = string.Format("Instellingen wijzigen van {0}.", Classes.Globals.AppName),
                    en = string.Format("Change the {0} settings.", Classes.Globals.AppName),
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sortorder",
                    nl = "Sorteervolgorde",
                    en = "Sort order",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-saveqr",
                    nl = "Bewaar QR code afbeelding",
                    en = "Save QR code image",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-close",
                    nl = "Sluiten",
                    en = "Close",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-instructions",
                    nl = $"Om een backup terug te zetten naar het huidige om een ander device moet overal hetzelfde wachtwoord gebruikt zijn dat je bij eerste keer opstarten van {Classes.Globals.AppDeveloper}_{Classes.Globals.AppName}.exe is gevraagd in te voeren.\nAls dat het geval is dan kan de \"{Classes.Globals.AppDeveloper}_{Classes.Globals.AppName}.settings\" file door beide devices gelezen worden.",
                    en = $"To restore a backup to the current or another device, the same password must be used that was asked when you first started {Classes.Globals.AppDeveloper}_{Classes.Globals.AppName}.exe.\nIf that is the case then the \"{Classes.Globals.AppDeveloper}_{Classes.Globals.AppName}.settings\" can be read by both devices.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-warning",
                    nl = "Als je de optie \"Bewaar afbeelding\" aan zet dan zal er een kopie afbeelding van de QR code in de applicatie folder bewaard worden bij het invoeren van een nieuwe Login.\n Je kan dan eventueel later dezelfde QR code login aan een ander apparaat koppelen.\n\nDit is mogelijk een beveiligingsrisico!",
                    en = "If you enable the option \"Save image\" then an image of the QR code will be saved in the application folder when adding a new Login.\nYou could then add the QR code to a different device if you wanted.\n\nThis is a potential security risk!",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-askpass",
                    nl = "Vraag wachtwoord bij openen",
                    en = "Ask password on open",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "askpassword-title",
                    nl = "Vul je wachtwoord in",
                    en = "Please enter your password",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "askpassword-3times",
                    nl = "Je hebt 3x een foutief wachtwoord ingevoerd.\r\nDe app zal nu sluiten.",
                    en = "You've entered an incorrect password 3 times.\r\nThe app will now close.",
                    de = ""
                },
            };

            LocalizationData.ForEach(x => x.nl = x.nl.Replace("{APPNAME}", Classes.Globals.AppName));
            LocalizationData.ForEach(x => x.en = x.en.Replace("{APPNAME}", Classes.Globals.AppName));
        }


        public class LanguageEntry
        {
            public string key { get; set; }
            public string nl { get; set; }
            public string en { get; set; }
            public string de { get; set; }
        }
    }
}