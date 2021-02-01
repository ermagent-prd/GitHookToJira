using AlfrescoTools.Parameters;
using AlfrescoTools.Service;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlfrescoTools.Engine
{
    public class FolderGetterEngine
    {
        #region Private properties

        
        private readonly ISession alfresco;

        #endregion

        #region Constructor

        public FolderGetterEngine(ServiceManagerContainer requestFactory)
        {
            this.alfresco = requestFactory.Session;

        }

        #endregion

        #region Public methods

        public IEnumerable<ICmisObject> Execute()
        {
            
            // get all folders  
            IFolder root = alfresco.GetRootFolder();
                       

            return root.GetChildren();
        }

        public ICmisObject Execute(string folderName)
        {
            var folderList = this.Execute();
            ICmisObject found = null;

            foreach (var folder in folderList)
            {
                if (folder.Name == folderName)
                {
                    found = folder;
                    break;
                }
            }

            return found;
        }

        #endregion
    }
}
