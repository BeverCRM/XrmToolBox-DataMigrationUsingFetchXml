using Unity;
using XrmMigrationUtility.Services.Interfaces;
using XrmMigrationUtility.Services.Implementations;

namespace XrmMigrationUtility
{
    internal static class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer unityContainer)
        {
            unityContainer.RegisterType<DataMigrationUtilityControl>(TypeLifetime.Singleton);
            unityContainer.RegisterType<ITransferOperation, TransferOperation>(TypeLifetime.Singleton);
            unityContainer.RegisterType<ILogger, Logger>(TypeLifetime.Singleton);
        }
    }
}