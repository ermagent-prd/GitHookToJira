using System;
using Countersoft.Gemini.Api;
using GeminiTools.Parameters;

namespace GeminiTools.Service
{
    public class ServiceManagerContainer
    {
        private readonly Lazy<ServiceManager> svc;

        public ServiceManagerContainer(IGeminiToolsParameters parContainer)
        {
            this.svc = new Lazy<ServiceManager>(() => new ServiceManager(parContainer.ServerUrl));
        }

        public ServiceManager Service { get { return this.svc.Value; } }
    }
}
