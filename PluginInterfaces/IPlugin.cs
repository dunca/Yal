using System.Collections.Generic;
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
        /// Should be set by plugins that rely on special keywords to do their mojo. When this is not null, Yal will
        /// always try to match the user's input against item names starting from index 0
        /// </summary>
        string Activator { get; }

        PluginItemSortingOption SortingOption { get; }

        /// <summary>
        /// Instructions on how to use the plugin
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// This method should return a UserControl object that will be displayed by the main program in a dynamically 
        /// created tab page that will be located in the Options -> Plugins window. The user control should contain the plugin's user
        /// changeble settings, if any
        /// </summary>
        /// <returns></returns>
        UserControl PluginUserControl { get; }

        /// <summary>
        /// An icon that will be shown in the output window, next to the plugin provided entries
        /// </summary>
        Icon PluginIcon { get; }

        /// <summary>
        /// This method is called when the user hits the "Apply" button in the "Options" window. Its body should contain
        /// code that saves the plugin's own settings and/or its controls' state, if any
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Called every time the user's input changes. For plugins that use activators this will only
        /// get called when the user's input starts with the activator
        /// </summary>
        /// <param name="userInput">the string the user inserts in the launcher's text/search box</param>
        /// <returns>a list of the items it can provide, or 'null' (when the plugin can't provide any items)</returns>
        List<PluginItem> GetItems(string userInput);

        /// <summary>
        /// This method should take care of the execution of the specified parameter
        /// </summary>
        /// <param name="input">the Info (or Item) value of a PluginItem object</param>
        void HandleExecution(string input);

        /// <summary>
        /// This method is called when plugin specific items are fetched from the history database. 
        /// </summary>
        /// <param name="input">the Info (or Item) value of a PluginItem object</param>
        /// <returns>If the method returns false then the item will not be displayed in the output list</returns>
        bool CanHandle(string input);
    }
}
