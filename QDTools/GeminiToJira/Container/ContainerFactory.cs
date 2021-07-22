﻿using AlfrescoTools.Container;
using AlfrescoTools.Engine;
using AlfrescoTools.Parameters;
using GeminiToJira.Engine;
using GeminiToJira.Log;
using GeminiToJira.Mapper;
using GeminiToJira.Parameters;
using GeminiTools.Container;
using GeminiTools.Parameters;
using JiraTools.Container;
using JiraTools.Parameters;
using Unity;

namespace GeminiToJira.Container
{
    internal static class ContainerFactory
    {
        public static IUnityContainer Execute()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IGeminiToolsParameters, GeminiParamContainer>();
            container.RegisterType<IJiraToolsParameters, JiraParamContainer>();
            container.RegisterType<IAlfrescoToolsParameters, AlfrescoParamContainer>();

            container.RegisterType<CommentMapper>();
            container.RegisterType<JiraAccountIdEngine>(TypeLifetime.Singleton);

            container.RegisterType<ImportTaskEngine>();
            container.RegisterType<ImportStoryEngine>();
            container.RegisterType<ImportUatEngine>();
            container.RegisterType<ImportBugEngine>();
            container.RegisterType<TimeLogEngine>();
            container.RegisterType<BugIssueMapper>();
            container.RegisterType<UATIssueMapper>();
            container.RegisterType<TaskIssueMapper>();

            container.RegisterType<FolderGetterEngine>();
            container.RegisterType<FolderCreateEngine>();
            container.RegisterType<UploadDocumentEngine>();

            container.RegisterType<FilteredGeminiIssueListGetter>();
            container.RegisterType<GeminiIssueChecker>();
            container.RegisterType<AddWatchersEngine>();
            container.RegisterType<AssigneeEngine>();
            container.RegisterType<AffectedVersionsEngine>();
            container.RegisterType<LogManager>();
            container.RegisterType<DebugLogManager>();
            container.RegisterType<StoryIssueMapper>();
            







            container.RegisterType<ParseCommentEngine>();
            container.AddNewExtension<JiraToolsContainerExtension>();
            container.AddNewExtension<GeminiToolsContainerExtension>();
            container.AddNewExtension<AlfrescoToolsContainerExtension>();
            







            return container;
        }
    }
}
