using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PluginInterfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
        Icon PluginIcon { get; }
        List<string> Activators { get; }

        void SaveSettings();
        UserControl GetUserControl();
        bool TryParseInput(string input, out string output);
        void HandleExecution(string input);
    }
}
