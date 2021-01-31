using System;
using System.Collections.Generic;
using AlfrescoTools.Parameters;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;

namespace AlfrescoTools.Service
{
    public class ServiceManagerContainer
    {
        private readonly Lazy<ISession> session;

        public ServiceManagerContainer(IAlfrescoToolsParameters parContainer)
        {
            this.session = new Lazy<ISession>(() => CreateSession(parContainer));
        }

        public ISession Session { get { return this.session.Value; } }


        private ISession CreateSession(IAlfrescoToolsParameters parContainer)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            // define binding type, in our example we are using ATOMPUB as stated above  
            parameters[DotCMIS.SessionParameter.BindingType] = BindingType.AtomPub;

            // define CMIS available path which is already available under alfresco  
            parameters[DotCMIS.SessionParameter.AtomPubUrl] = parContainer.ServerUrl;

            // alfresco portal admin user name  
            parameters[DotCMIS.SessionParameter.User] = parContainer.UserName;

            // alfresco portal admin password  
            parameters[DotCMIS.SessionParameter.Password] = parContainer.Password;

            // define session factory  
            SessionFactory factory = SessionFactory.NewInstance();

            return factory.GetRepositories(parameters)[0].CreateSession();
        }
    }
}
