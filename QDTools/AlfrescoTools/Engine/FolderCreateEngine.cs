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
    public class FolderCreateEngine
    {
        #region Private properties
        private readonly ISession alfresco;
        private readonly FolderGetterEngine folderGetterEngine;

        #endregion

        #region Constructor

        public FolderCreateEngine(ServiceManagerContainer requestFactory, FolderGetterEngine folderGetterEngine)
        {
            this.alfresco = requestFactory.Session;
            this.folderGetterEngine = folderGetterEngine;
        }

        #endregion

        #region Public methods

        public IFolder Execute(string parentFolderName, string newFolderName)
        {
            //retrieve the parent folder under the root
            var parentFolder = (IFolder)folderGetterEngine.Execute(parentFolderName);

            // Check if folder already exist
            IFolder newFolder = null;
            var children = parentFolder.GetChildren();
            foreach(var child in children)
            {
                if (child.Name == newFolderName)
                    newFolder = (IFolder)child;
            }

            //if not create it
            if (newFolder == null)
            {
                Dictionary<String, Object> newFolderProps = new Dictionary<String, Object>();
                newFolderProps.Add(PropertyIds.ObjectTypeId, "cmis:folder");
                newFolderProps.Add(PropertyIds.Name, newFolderName);
                newFolder = parentFolder.CreateFolder(newFolderProps);
            }
            

            return newFolder;
        }

        #endregion
    }
}
