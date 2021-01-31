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

        private readonly ServiceManagerContainer requestFactory;

        #endregion

        #region Constructor

        public FolderGetterEngine(ServiceManagerContainer requestFactory)
        {
            this.requestFactory = requestFactory;

        }

        #endregion

        #region Public methods

        public IFolder Execute()
        {
            var alfresco = requestFactory.Session;
            
            // get all folders  
            IFolder root = alfresco.GetRootFolder();

            

            return root;
        }

        #endregion
    }
}
