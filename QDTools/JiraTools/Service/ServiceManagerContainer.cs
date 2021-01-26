﻿using System;
using Atlassian.Jira;
using JiraTools.Parameters;

namespace JiraTools.Service
{
    public class ServiceManagerContainer
    {
        private readonly Lazy<Jira> svc;

        public ServiceManagerContainer(IJiraToolsParameters parContainer)
        {
            var settings = new JiraRestClientSettings();
            settings.EnableUserPrivacyMode = true;

            this.svc = new Lazy<Jira>(() => Jira.CreateRestClient(parContainer.ServerUrl, parContainer.User, parContainer.Token, settings));
            
        }

        public Jira Service { get { return this.svc.Value; } }
    }
}
