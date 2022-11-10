using Unity;
using DataMigrationUsingFetchXml.Services.Interfaces;
using DataMigrationUsingFetchXml.Services.Implementations;

namespace DataMigrationUsingFetchXml
{
    internal static class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<DataMigrationUsingFetchXmlControl>(TypeLifetime.Singleton);
            unityContainer.RegisterType<ITransferOperation, TransferOperation>(TypeLifetime.Singleton);
            unityContainer.RegisterType<ILogger, Logger>(TypeLifetime.Singleton);
        }
    }
}