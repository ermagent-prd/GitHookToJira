using System;
using System.Diagnostics;
using GeminiToJira.Engine.Common;
using GeminiToJira.Parameters.Import;

namespace GeminiToJira.Engine
{
    internal class MainEngine
    {
        #region Private properties

        private readonly ConfigurationContainer configurationManager;

        private readonly ImportUatEngine uatEngine;

        private readonly ImportTaskEngine taskEngine;

        private readonly ImportStoryEngine storyEngine;

        private readonly ImportBugEngine bugEngine;

        private readonly OriginalAccountLogger accountLogger;

        #endregion

        #region Constructor

        public MainEngine(
            ConfigurationContainer configurationManager,
            OriginalAccountLogger accountLogger,
            ImportUatEngine uatEngine, 
            ImportTaskEngine taskEngine, 
            ImportStoryEngine storyEngine, 
            ImportBugEngine bugEngine)
        {
            this.accountLogger = accountLogger;
            this.configurationManager = configurationManager;
            this.uatEngine = uatEngine;
            this.taskEngine = taskEngine;
            this.storyEngine = storyEngine;
            this.bugEngine = bugEngine;
        }


        #endregion

        #region Public methods

        public void Execute()
        {
            GeminiToJiraParameters configurationSetup = this.configurationManager.Configuration;

            var storyLogFile = configurationSetup.LogDirectory + "ImportLog_" + configurationSetup.JiraProjectCode + "_" + DateTime.Now.ToString("yyyyMMdd-hh_mm") + ".log";

            Trace.AutoFlush = true;
            Trace.Listeners.Add(new TextWriterTraceListener(storyLogFile, "myListener"));

            Stopwatch timer = new Stopwatch();

            var startMessage = "[" + DateTime.Now + "] Started...";

            Console.WriteLine(startMessage);
            Trace.TraceInformation(startMessage);


            #region Task

            if (configurationSetup.Jira.ImportTask)
                ImportTask(configurationSetup,  timer);

            #endregion

            #region Story

            if (configurationSetup.Jira.ImportStory)
                ImportDevelopment(configurationSetup,  timer);

            #endregion

            #region UAT

            if (configurationSetup.Jira.ImportUat)
                ImportUat(configurationSetup,  timer);

            #endregion

            #region BUG

            if (configurationSetup.Jira.ImportBug)
                ImportBug(configurationSetup,  timer);

            #endregion

            this.accountLogger.SaveLog();

            var endLog = "[" + DateTime.Now + "] Finished";


            Trace.TraceInformation(endLog);

            Console.WriteLine(endLog);


            //Console.WriteLine("Press a key to close");
            //Console.ReadLine();

        }

        #endregion

        #region Private methods

        private  void ImportTask(GeminiToJiraParameters configurationSetup, Stopwatch timer)
        {
            timer.Start();
            Console.WriteLine("[" + DateTime.Now + "] Start Task");
            this.taskEngine.Execute(configurationSetup);
            timer.Stop();
            Console.WriteLine("[" + DateTime.Now + "] Task imported in " + timer.Elapsed);
        }

        private  void ImportDevelopment(GeminiToJiraParameters configurationSetup, Stopwatch timer)
        {
            timer.Start();
            var startMsg = "[" + DateTime.Now + "] Start Development";
            Console.WriteLine(startMsg);
            Trace.TraceInformation(startMsg);
            this.storyEngine.Execute(configurationSetup);
            timer.Stop();
            var endMsg = "[" + DateTime.Now + "] Development imported in " + timer.Elapsed;

            Console.WriteLine(endMsg);
            Trace.TraceInformation(endMsg);

        }

        private  void ImportUat(GeminiToJiraParameters configurationSetup, Stopwatch timer)
        {
            timer.Restart();
            var startmsg = "[" + DateTime.Now + "] Start UAT";
            Console.WriteLine(startmsg);
            Trace.TraceInformation(startmsg);
            this.uatEngine.Execute(configurationSetup);
            timer.Stop();
            var stopMsg = "[" + DateTime.Now + "] UAT imported in " + timer.Elapsed;
            Console.WriteLine("[" + DateTime.Now + "] UAT imported in " + timer.Elapsed);
            Trace.TraceInformation(stopMsg);
        }

        private void ImportBug(GeminiToJiraParameters configurationSetup,  Stopwatch timer)
        {
            timer.Restart();
            var startMsg = "[" + DateTime.Now + "] Start BUG";
            Console.WriteLine(startMsg);
            Trace.TraceInformation(startMsg);
            this.bugEngine.Execute(configurationSetup);
            timer.Stop();
            var stopMsg = "[" + DateTime.Now + "] BUG imported in " + timer.Elapsed;
            Console.WriteLine(stopMsg);
            Trace.TraceInformation(stopMsg);

        }


        #endregion
    }
}
