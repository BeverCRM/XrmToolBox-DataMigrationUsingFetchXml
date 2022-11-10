using Unity;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using XrmToolBox.Extensibility.Interfaces;

namespace DataMigrationUsingFetchXml
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Data Migration using FetchXml"),
        ExportMetadata("Description", "Tool helps to transfer entity records between environments by using FetchXml queries."),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "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"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class DataMigrationUsingFetchXml : PluginBase
    {
        private readonly IUnityContainer _unityContainer;

        public override IXrmToolBoxPluginControl GetControl()
        {
            UnityConfig.RegisterTypes(_unityContainer);

            return _unityContainer.Resolve<DataMigrationUsingFetchXmlControl>();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public DataMigrationUsingFetchXml()
        {
            _unityContainer = new UnityContainer();
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.MyPlugin 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}