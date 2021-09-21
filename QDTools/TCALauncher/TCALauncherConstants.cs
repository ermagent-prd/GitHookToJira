namespace TCALauncher
{
    internal class TCALauncherConstants
    {
        public const int OK = 0;
        public const int FAIL = -1;

        public const int PLAN_FILE_READ_ERR = -10;
        public const int PLAN_EXC = -11;
        public const int NO_PLAN_LIST = -12;
        public const int ERR_PLAN_EXE = -13;
        public const int ERR_TCA_EXE = -14;
        public const int ERR_SCRIPT_EXE = -15;
        public const int ERR_SQL = -17;
        public const int PLAN_NO_TCA = -18;
        public const int TCA_REGR_FOUND = -19;
        public const int ERR_REPORT_EXP = -20;

        public const int DISPATCH_UNAV = 1;
        public const int PLAN_RUNNING = 2;
        public const int PLAN_WAIT = 3;
        public const int PLAN_FAIL = 4;
        public const int HEAD_UNREACH = 5;
        public const int ERR_CONNECT_DB = 6;
        public const int ERR_APP = 7;
    }
}
