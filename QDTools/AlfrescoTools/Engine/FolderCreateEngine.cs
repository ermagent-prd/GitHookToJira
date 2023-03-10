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

        public IFolder Execute(string rootFolder, string newFolderName, string storyFolder)
        {
            

            //retrieve the parent folder under the root
            var root = (IFolder)folderGetterEngine.Execute(rootFolder);

            // Check if folder already exist
            IFolder folder = null;

            var parentFolder = GetChildren(root, FolderCheck(storyFolder));
            var children = parentFolder.GetChildren();

            foreach(var child in children)
            {
                if (child.Name == FolderCheck(newFolderName))
                    folder = (IFolder)child;
            }

            //if not create it
            if (folder == null)
            {
                Dictionary<String, Object> newFolderProps = new Dictionary<String, Object>();
                newFolderProps.Add(PropertyIds.ObjectTypeId, "cmis:folder");
                newFolderProps.Add(PropertyIds.Name, FolderCheck(newFolderName));
                folder = parentFolder.CreateFolder(newFolderProps);
            }
            

            return folder;
        }

        private string FolderCheck(string folder)
        {
            string result = folder.Replace(':', '-').Replace('/', '_').TrimEnd();

            result = result.Replace('\\', ' ').TrimEnd();
            result = result.Replace('"', ' ').TrimEnd();

            if (result.EndsWith("."))
                result = result.Substring(0, result.Length - 1);

            return result;
        }

        private IFolder GetChildren(IFolder root, string storyFolder)
        {
            IFolder parentForlder = null;

            //Find sub directory if exist
            if (storyFolder != null && storyFolder != "")
            {
                foreach(var child in root.GetChildren())
                {
                    if (child.Name == storyFolder)
                        parentForlder = (IFolder)child;
                }
                return parentForlder;
            }
            else
                return root;
        }

        #endregion
    }
}
