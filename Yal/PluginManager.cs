using System;
using System.IO;
using PluginInterfaces;
using System.Reflection;
using System.Collections.Generic;

namespace Yal
{
    internal static class PluginManager
    {
        internal static List<IPlugin> InstantiatePlugins()
        {
            var pluginInstances = new List<IPlugin>();
            foreach (var type in LoadPluginTypes())
            {
                pluginInstances.Add(Activator.CreateInstance(type) as IPlugin);
            }
            return pluginInstances;
        }

        private static List<Type> LoadPluginTypes(string path = "plugins")
        {
            if (!Directory.Exists(path))
            {
                return null;
            }

            var assemblies = new List<Assembly>();
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                var pluginFilePath = Path.Combine(dir, string.Concat(Path.GetFileName(dir), ".dll"));
                if (File.Exists(pluginFilePath))
                {
                    assemblies.Add(Assembly.LoadFrom(pluginFilePath));
                }
            }

            var pluginTypes = new List<Type>();
            var interfaceName = typeof(IPlugin).FullName;

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.GetInterface(interfaceName) != null)
                    {
                        pluginTypes.Add(type);
                    }
                }
            }
            return pluginTypes;
        }

        internal static bool PluginIsDisabled(IPlugin plugin)
        {
            return Properties.Settings.Default.DisabledPlugins.Contains(plugin.Name);
        }
    }
}
