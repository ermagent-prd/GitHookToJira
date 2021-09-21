using System;
using TCALauncher;

namespace TCAProcess
{
    internal class TCACheckEngine
    {
        #region Private fields

        private readonly TCAQueryHelper queryHelper;

        #endregion

        #region Constructors

        public TCACheckEngine(TCAQueryHelper queryHelper)
        {
            this.queryHelper = queryHelper;
        }

        #endregion

        #region Public methods

        public TCACheckResult Execute(string build, string ermasDB)
        {
            TCACheckResult query1Result =
                    CheckTCAResults(build, ermasDB);

            if (!query1Result.Phase.PhasePassed)
                return query1Result;

            return CheckTCAErrors(build, ermasDB);
        }

        #endregion

        #region Private methods

        private TCACheckResult CheckTCAResults(string build, string ermasDB)
        {
            string query1 =
               "select COUNT(*) FROM [dbo].[T_OUT_RSLT] tca left join (select items.UID_ELBS_ITM, ermdb.UCOD_ERMDB, ermdb.DB_SW_VERSION " +
               "from T_AAA_ELBS_ITEM items left join T_AAA_ERMDB ermdb " +
               $"on items.UID_DB_V2 = ermdb.UID_ERMDB) db1 on tca.UID_ELBS_ITM = db1.UID_ELBS_ITM where SIMPROV_2 = '{build}' and db1.UCOD_ERMDB = '{ermasDB}'";

            int returnValue = TCALauncherConstants.OK;
            string returnMessage = null;
            bool phasePassed = false;

            try
            {
                int query1Result = queryHelper.GetRowCount(query1);

                if (query1Result == 0)
                {
                    returnValue = TCALauncherConstants.PLAN_NO_TCA;
                    returnMessage = $"No tca results found";
                }
                else
                    phasePassed = true;

            }
            catch (Exception exc)
            {
                returnValue = TCALauncherConstants.ERR_SQL;
                returnMessage = $"{exc.Message} Query {query1}";
            }

            return new TCACheckResult(phasePassed, returnValue, returnMessage, null);
        }

        private TCACheckResult CheckTCAErrors(string build, string ermasDB)
        {
            string query2 =
                "select COUNT(*) FROM [dbo].[T_OUT_RSLT] tca left join (select items.UID_ELBS_ITM, ermdb.UCOD_ERMDB, ermdb.DB_SW_VERSION " +
                "from T_AAA_ELBS_ITEM items left join T_AAA_ERMDB ermdb on items.UID_DB_V2 = ermdb.UID_ERMDB) db1 on tca.UID_ELBS_ITM = db1.UID_ELBS_ITM " +
                $"where tca.OUT_RSLT in (0,2) and tca.valid <> 1 and SIMPROV_2 = '{build}' and db1.UCOD_ERMDB = '{ermasDB}'";

            int returnValue = TCALauncherConstants.OK;
            string returnMessage = null;
            bool phasePassed = false;

            try
            {
                int query2Result = queryHelper.GetRowCount(query2);

                phasePassed = true;

                if (query2Result == 0)
                    returnMessage = $"Checks ok";

                if (query2Result > 0)
                    return GetErrors(build, ermasDB);
            }
            catch (Exception exc)
            {
                returnValue = TCALauncherConstants.ERR_SQL;
                returnMessage = $"{exc.Message} Query {query2}";
            }

            return new TCACheckResult(phasePassed, returnValue, returnMessage);
        }

        private TCACheckResult GetErrors(string build, string ermasDB)
        {
            string query3 =
                "SELECT cs.UID_CSTD, cs.UCOD_CSTD, cs.SESS_REF, tca.[DATE], " +
                "CASE tca.OUT_RSLT WHEN 0 THEN 'Error' WHEN 1 THEN 'Ok' WHEN 2 THEN 'Warning' ELSE 'Undefined' END [Result], " +
                "CASE tca.VALID  WHEN 0 THEN 'No' WHEN 1 THEN 'Yes' ELSE 'Undefined' END [Validation], db1.UCOD_ERMDB [DB1], DB1.DB_SW_VERSION [DBVERSION1], " +
                "db2.UCOD_ERMDB [DB2], DB2.DB_SW_VERSION [DBVERSION2], tca.[SIMPROV_1], tca.[SIMPROV_2] FROM [dbo].[T_OUT_RSLT] tca left join " +
                "(select items.UID_ELBS_ITM, ermdb.UCOD_ERMDB, ermdb.DB_SW_VERSION from T_AAA_ELBS_ITEM items left join T_AAA_ERMDB ermdb on items.UID_DB_V1 = ermdb.UID_ERMDB) db1 on " +
                "tca.UID_ELBS_ITM = db1.UID_ELBS_ITM left join " +
                "(select items.UID_CSTD, items.UID_ELBS_ITM, ermdb.UCOD_ERMDB, ermdb.DB_SW_VERSION from " +
                "T_AAA_ELBS_ITEM items left join T_AAA_ERMDB ermdb on items.UID_DB_V2 = ermdb.UID_ERMDB) db2 on tca.UID_ELBS_ITM = db2.UID_ELBS_ITM  left join " +
                $"[dbo].[T_AAA_CSTD] cs on db2.UID_CSTD = cs.UID_CSTD where tca.OUT_RSLT in (0,2) and tca.valid <> 1 and SIMPROV_2 = '{build}' and db2.UCOD_ERMDB = '{ermasDB}'";

            int returnValue = TCALauncherConstants.TCA_REGR_FOUND;
            string returnMessage = $"Regression found";

            bool phasePassed = false;
            TCAResultObj returnObj = null;
            
            try
            {
                returnObj =
                    queryHelper.GetErrors(query3);

                phasePassed = true;
            }
            catch (Exception exc)
            {
                returnValue = TCALauncherConstants.ERR_SQL;
                returnMessage = $"{exc.Message} Query {query3}";
            }

            return new TCACheckResult(phasePassed, returnValue, returnMessage, returnObj);
        }

        #endregion
    }
}
