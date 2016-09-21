using System.Drawing;
using System.Windows.Forms;

namespace PluginInterfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }
        Icon PluginIcon { get; }
        bool FileLikeOutput { get; }

        void SaveSettings();
        UserControl GetUserControl();
        string[] GetResults(string input, out string[] itemInfo);
        void HandleExecution(string input);
    }
}
