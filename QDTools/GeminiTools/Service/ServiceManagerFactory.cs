using Countersoft.Gemini.Api;

namespace GeminiTools
{
    public class ServiceManagerFactory
    {
        public ServiceManager Execute(string geminiUrl)
        {
            return new ServiceManager(geminiUrl);
        }
    }
}
