using System;
using CredentialManagement;

namespace TwoFactor.Classes
{
    public class WindowsPassword
    {
        public static void SetPassword(string password)
        {
            using (var cred = new Credential())
            {
                cred.Password = password;
                cred.Target = Globals.UniqueKey;
                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }


        public static string GetPassword()
        {
            using (var cred = new Credential())
            {
                cred.Target = Globals.UniqueKey;
                cred.Load();

                return cred.Password;
            }
        }
    }
}
