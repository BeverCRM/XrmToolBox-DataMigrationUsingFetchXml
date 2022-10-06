using Unity;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using XrmToolBox.Extensibility.Interfaces;

namespace XrmMigrationUtility
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Data Migration Utility"),
        ExportMetadata("Description", "Data Migration Utility helps migrating data between environments."),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAMAAABEpIrGAAAAAXNSR0IArs4c6QAAAppQTFRFAAAAAAAAAP////8AAAAAAAAAAFWqAAAAv4BAJCQkAIC/HBwcAICzIiIRICAgDXO/GhoaJCQkGhoaKioqIiIiDni8ZkYcJCQky4AeDne9JSUlHx8ccHBzDne5HR0d4I4fHBwcISQnKSkmMCgbD3W8Ly8tnqCjcUwdhFYds3MgzM7Q3+TmD3a9q24gD3i7lZeXJycnIyMhKyQcEHe8PDAbD3i985YiD3S3D3e8Hj5SMnCS5o8g8pUi8pciKCgm+vr6s3swvnkhJSUl2YghD3e9IiIgxn4i440gxcjJd4WMfYCAVz8bEHa8EHe8sLCvv8LEqaqqFFeComgeqqysra2spGofEHe9EHi9bk4izM/SEHe9eFEfq6uq8JYhEHW4EHa6EHe7EHe8EHe9EHi8EHi9EXGxEXKyEXW5Em+tEnCvE16QE2miE2umE2yoE22qFFeFFHe5FmKVF0ViGFmFGFuHGHi3GXCpGk1uGni1HUVgHkNbH0BWID9TITpLLIbDNXmdOIi9OX2gQX6bSn+VUFBPUZrLUomtU5bBVYCNVYGNVYquVYywVoKeX42rYWFfYWFhYpm+aoN/bG5ucGtjd3d3d4Z2eHp6ebDVgIB/hISDh4hqiYqLjY2MlpiampyenYpboKGioaesoa63oqKhoqirpIxXpKeprK6vt45Kubm5vb29vngfwpBCxMTExX0gx8jIycrLyn8gy4Eizc3Nz5E5z5E6z5I60ZE42Ygg29vb3d3c3d3d3uLk35Qu39/f4OHi4Y4i4Y4j444h45Al5pQq5ubm5urt55Up6Ojo661c7ZMi75Ui7+/v8JUi8PL08ZYh8pYh8pYi8pci85Yh85ci8/Pz+Pj4+Pn5+fn5+vr6+/v7/Pz8/f39/v7+wqlKDAAAAF50Uk5TAAEBAQIDAwQEBwgJCg8QFBQVHTA0NTc4RElMUVRYWFpbXF1fZmZmaGhoaGh3gISEiZGTlJaXl5iYmJiYmJiampucoKOlpaamq63GzM/c6evv8fP09ff8/P39/v7+/kaE+D0AAAIISURBVBgZBcGxblt1GAfQ87v3bztxEsctUaESCkWCASFV0AqEGBjZmRCPwAADEjMPwQuwsbEyIF4ApQg2qlZApUKgbQpOSVzX8f04B5BAEi1JIo0EAZCSCqCQIgRASEBAhARACqKIQlAAACEIAgAQIEAiIgAJARAg0ACwd2WUYXU83ZllvfibgghF1I2D/Y+mT75+vD3+ZPrl4odHKWgqJeXSW5c+GyXHb2xmN+6tP/xqqpBqFLL35pVPxxm+/Wvh1fW7Q9/3pJQGTF/6YtaG01u3u34zar89e+2D4b+HBV2gdq7/m4vh3k+pIaPpi5c3m8OrY0In0H1+WE7uBzXZX55vsr45TkQr2L4Z6Va/pMjxN6PuaHXhne8pLSVV59UNQ3WV4n5tJi8sLrrnqFSrYP3rotvsFwL96uyJ2lFKU5jPf26btw36oSK1tZXKQ0Ej5fWPl1vLUX/waBAM/UIcQXWJaGe7k929+TVFVZf5UmdRpdKK+Ofij7P5sP38td87G/PJovrhu6cp0Ug52HlP1/pXHi8f1HjcnzzrT388rqL0YPb+0d0/D+/svdwNbVYXT5cnd24PQhLI1eu7W6eLs0myajUM/fruKZUigdT8cn/+YJ0SFEGlokgISEQIIkIIQRAAEAFIJBIAEiEIiIAkCQiBhAAQAIDQIYAgSEBAh4LQV9InIQjB/3yJzfbiFSI9AAAAAElFTkSuQmCC"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class DataMigrationUtility : PluginBase
    {
        private readonly IUnityContainer _unityContainer;

        public override IXrmToolBoxPluginControl GetControl()
        {
            UnityConfig.RegisterTypes(_unityContainer);

            return _unityContainer.Resolve<DataMigrationUtilityControl>();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public DataMigrationUtility()
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