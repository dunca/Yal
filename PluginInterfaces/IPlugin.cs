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
        string[] GetResults(string input, bool matchAnywhere, bool fuzzyMatch);
        bool CouldProvideResults(string input, bool matchAnywhere, bool fuzzyMatch);
        void HandleExecution(string input);
    }
}
