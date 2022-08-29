using Unity;
using System;
using Unity.Resolution;
using System.Windows.Forms;
using Microsoft.Xrm.Tooling.Connector;
using XrmMigrationUtility.Model.Interfaces;
using XrmMigrationUtility.Services.Interfaces;
using XrmMigrationUtility.Model.Implementations;
using XrmMigrationUtility.Services.Implementations;

namespace XrmMigrationUtility
{
    public static class Injection
    {
        //static readonly IUnityContainer unityContainer;

        //static Injection()
        //{
        //    unityContainer = new UnityContainer();
        //}

        public static ILogger GetLoggerInstance(TextBox txtLogs, string logPath)
        {
            //unityContainer.RegisterType<ILogger, Logger>();
            //ILogger logger = unityContainer.Resolve<ILogger>(new ResolverOverride[]
            //    {
            //            new ParameterOverride("txtLogs", txtLogs),
            //            new ParameterOverride("logsPath", logPath)
            //    });
            return new Logger(txtLogs, logPath);
        }

        public static IDataverseService GetDataverseServiceInstance(CrmServiceClient service)
        {
            //unityContainer.RegisterType<IDataverseService, DataverseService>();
            //IDataverseService dataverseService = unityContainer.Resolve<IDataverseService>(new ResolverOverride[]
            //    {
            //        new ParameterOverride("service", service)
            //    });

            return new DataverseService(service);
        }

        public static IResultItem GetResultItemInstance(string entityName)
        {
            //unityContainer.RegisterType<IResultItem, ResultItem>();
            //IResultItem resultItem = unityContainer.Resolve<IResultItem>(new ResolverOverride[]
            //    {
            //        new ParameterOverride("entityName", entityName)
            //    });

            return new ResultItem(entityName);
        }
    }
}