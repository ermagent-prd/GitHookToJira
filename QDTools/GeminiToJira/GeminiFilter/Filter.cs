using Countersoft.Gemini.Commons.Dto;
using Countersoft.Gemini.Commons.Entity;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GeminiToJira.GeminiFilter
{
    public static class Filter
    {
        public static IssuesFilter GetFilter(FilterType fType)
        {
            switch(fType)
            {
                case FilterType.Development:
                    return new IssuesFilter
                    {
                        IncludeClosed = DevelopmentConstants.DEVELOPMENT_INCLUDED_CLOSED,
                        Projects = DevelopmentConstants.DEVELOPMENT_PROJECT_ID,
                        Types = DevelopmentConstants.DEVELOPMENT_TYPES,
                    };
                case FilterType.UAT:
                    return new IssuesFilter
                    {
                        Projects = UatConstants.UAT_PROJECT_ID,
                    };
                case FilterType.ERMBUG:
                    return new IssuesFilter
                    {
                        IncludeClosed = ErmBugConstants.ERMBUG_INCLUDED_CLOSED,
                        Projects = ErmBugConstants.ERMBUG_PROJECT_ID,                        
                    };
                default:
                    return new IssuesFilter();
            }
            
        }

        #region Private
        

        #endregion
    }
}
