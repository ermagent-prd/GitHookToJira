using System.Collections.Generic;

namespace GeminiToJira.GeminiFilter
{
    public static class UatConstants
    {
        public static readonly string UAT_PROJECT_ID = "|37|";  //uat project

        public static readonly string UAT_CREATED_FROM = "12/03/2020";

        public static readonly List<string> UAT_FUNCTIONALITY = new List<string>() { "PYTHO" };
        public static readonly bool UAT_GROUP_DEPENDENCIES = true;
        public static readonly bool UAT_INCLUDED_CLOSED = true;
    }
}
