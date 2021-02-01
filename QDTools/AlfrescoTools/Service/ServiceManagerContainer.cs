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
            SessionFactory sessionFactory = SessionFactory.NewInstance();
            IDictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add(SessionParameter.User, parContainer.UserName);
            parameters.Add(SessionParameter.Password, parContainer.Password);
            parameters.Add(SessionParameter.AtomPubUrl, parContainer.ServerUrl);
            parameters.Add(SessionParameter.BindingType, BindingType.AtomPub);
            parameters.Add(SessionParameter.Compression, "true");
            parameters.Add(SessionParameter.CacheTTLObjects, "0");

            // If there is only one repository exposed (e.g. Alfresco),
            // these lines will help detect it and its ID
            var repositories = sessionFactory.GetRepositories(parameters);
            
            // Create a new session with the Alfresco repository
            return repositories[0].CreateSession();
        }
    }
}
