using System.Drawing;
using System.Windows.Forms;

namespace PluginInterfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Description { get; }

        /// <summary>
        /// Should be set to 'true' by plugins that rely on special keywords to do their mojo.
        /// </summary>
        bool RequiresActivator { get; }

        /// <summary>
        /// Instructions on how to use the plugin
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// This method should return a UserControl object that will be displayed by the main program in a dynamically 
        /// created tab page that will be located in the Options>Plugins window. The user control should contain the plugin's user
        /// changeble settings, if any
        /// </summary>
        /// <returns></returns>
        UserControl PluginUserControl { get; }

        /// <summary>
        /// An icon that will be shown in the output window, next to the plugin provided entries
        /// </summary>
        Icon PluginIcon { get; }

        /// <summary>
        /// This method is called when the user hits the "Apply" button in the "Options" window. It's body should contain
        /// code that saves the plugin's own settings and/or it's control's state, if any
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// This method should return an array with the identifiers of all the items that the plugin can provide or 'null'
        /// when the plugin can't provide any items
        /// </summary>
        /// <param name="input">the string the user inserts in the launcher's text/search box</param>
        /// <param name="itemInfo">an optional array of additional strings, one for each of the items in the returned array</param>
        /// <returns></returns>
        string[] GetResults(string input, out string[] itemInfo);

        /// <summary>
        /// This method should take care of the execution of the specified parameter
        /// </summary>
        /// <param name="input">one of the strings returned by the GetResults method, with additional user input
        /// in case of plugins that require activators</param>
        void HandleExecution(string input);
    }
}
