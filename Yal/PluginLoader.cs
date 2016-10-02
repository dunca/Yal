using System;
using System.IO;
using PluginInterfaces;
using System.Reflection;
using System.Collections.Generic;

namespace Yal
{
    internal static class PluginLoader
    {
        internal static List<IPlugin> InstantiatePlugins(List<Type> pluginTypes)
        {
            var pluginInstances = new List<IPlugin>();
            foreach (var type in pluginTypes)
            {
                pluginInstances.Add(Activator.CreateInstance(type) as IPlugin);
            }
            return pluginInstances;
        }

        internal static List<Type> Load(string path)
        {
            if (!Directory.Exists(path))
            {
                return null;
            }

            var assemblies = new List<Assembly>();
            foreach (string dll in Directory.EnumerateFiles(path, "*.dll"))
            {
                assemblies.Add(Assembly.LoadFrom(dll));
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
