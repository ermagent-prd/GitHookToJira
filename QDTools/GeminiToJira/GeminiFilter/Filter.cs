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
                        //Issues = "|63446|65023|67269|",   
                        //Issues = "|59545|",   
                        //Issues = "|59844|",   
                        //Issues = "|61087|",   
                    };
                case FilterType.UAT:
                    return new IssuesFilter
                    {
                        Projects = UatConstants.UAT_PROJECT_ID,
                        //Issues = "|67704|"
                        //Issues = "|63715|"  
                        //Issues = "|62157|"  
                        //Issues = "|61636|"
                    };
                case FilterType.ERMBUG:
                    return new IssuesFilter
                    {
                        IncludeClosed = ErmBugConstants.ERMBUG_INCLUDED_CLOSED,
                        Projects = ErmBugConstants.ERMBUG_PROJECT_ID,
                        //Issues = "|63783|" 
                        
                    };
                default:
                    return new IssuesFilter();
            }
            
        }

        #region Private
        

        #endregion
    }
}
