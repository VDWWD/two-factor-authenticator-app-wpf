using System;

namespace TwoFactor.Classes
{
    public class Enums
    {
        public enum Language
        {
            NL,
            EN,
            DE
        }


        public enum SortOrder
        {
            MostUsed,
            Name,
            DateAdded,
            DateLastUsed
        }


        public enum Icon
        {
            None,
            About,
            Browse,
            Check,
            Close,
            Delete,
            Desktop,
            Export,
            Maximize,
            Pin,
            Save,
            Settings,
            Unpin
        }
    }
}
