namespace GeminiIssueProducer
{
    public static class GeminiConstants
    {
        public const string OPT_ADD_OR_UPDATE = "add-or-update";
        public const string OPT_UPDATE_NOT_CLOSED = "update-not-closed";

        public const int PROJ_ESF_UAT_ID = 37;
        public const int TYPE_INTERNAL_UAT_ID = 177;
        public const int SEVERITY_MEDIUM_ID = 85; // Low=84, Medium=85, High=86, Blocking=87

        public const int CF_ISSUE_TYPE_ID = 162;
        public const int CF_AFFECTED_BUILD_ID = 164;
        public const int CF_FUNCTIONALITY_ID = 287;

        public const string PROJ_ESF_UAT = "ESF-UAT";
        public const string TYPE_INTERNAL_UAT = "Internal UAT";
        public const string SEVERITY_MEDIUM = "Medium";
        public const string ISSUE_TYPE_REGRESSION = "Regression";
        public const string FUNCTIONALITY_ERMAS = "ERMAS";

        public const int OK = 0;
        public const int NOT_FOUND = 1;

        public const int ERR_MISSING_ARGS = -1;
        public const int ERR_WRONG_OPTION = -2;
        public const int ERR_TIMEOUT_CONNECTION = -3;
        public const int ERR_CALL_FAIL = -4;

        public const int ERR_LOGIN_FAIL = -10;
        public const int ERR_FIND_FAIL = -11;
        public const int ERR_FILE_NOT_EXISTS = -12;
        public const int ERR_NO_RES_USER = -13;
        public const int ERR_CANNOT_CREATE_ISSUE = -14;
        public const int ERR_CANNOT_UPDATE_CF_ISSUETYPE = -15;
        public const int ERR_CANNOT_UPDATE_CF_BUILD = -16;
        public const int ERR_CANNOT_UPDATE_CF_FUNCTIONALITY = -17;
        public const int ERR_CANNOT_READ_FILE = -18;
        public const int ERR_CANNOT_UPDATE_ATTACHMENTS = -19;

        public const int ERR_EMPTY_TITLE = -20;

        public const int ERR_WRONG_ID = -30;
        public const int ERR_EMPTY_COMMENT = -31;
        public const int ERR_CANNOT_UPDATE_COMMENT = -32;

        public const int ERR_NO_STATE = -100;


    }
}
