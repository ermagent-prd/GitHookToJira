using AlfrescoTools.Container;
using AlfrescoTools.Engine;
using AlfrescoTools.Parameters;
using GeminiToJira.Engine;
using GeminiToJira.Engine.Common;
using GeminiToJira.Engine.Common.Alfresco;
using GeminiToJira.Engine.DevStory;
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
            container.RegisterType<EpicIssueMapper>();
            

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
            container.RegisterType<MainEngine>();
            container.RegisterType<GeminiUserMapper>();
            container.RegisterType<OriginalAccountLogger>();
            container.RegisterType<URLChecker>();
            container.RegisterType<JiraRemoteLinkerEngine>();
            container.RegisterType<AlfrescoUrlsEngine>();
            container.RegisterType<StoryGroupIssueMapper>();
            container.RegisterType<StoryOtherTasksIssueMapper>();
           
            container.RegisterType<ReporterSaveEngine>();
            container.RegisterType<StorySaveEngine>();
            container.RegisterType<SubtaskSaveEngine>();
            container.RegisterType<ImportStoryGroupItemEngine>();
            container.RegisterType<ImportStoryNoGroupEngine>();




            container.AddNewExtension<JiraToolsContainerExtension>();
            container.AddNewExtension<GeminiToolsContainerExtension>();
            container.AddNewExtension<AlfrescoToolsContainerExtension>();

            return container;
        }
    }
}
