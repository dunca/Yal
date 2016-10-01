namespace PluginInterfaces
{
    public struct PluginItem
    {
        // The item's name. The user's input is usually matched against this
        public string Name { get; set; }

        // Optional item identifier. When set, matching will be done against it and not against the Name property
        public string AlternateInfo { get; set; }

        // Shows up under the item's name. It defaults to the plugin's name if set to null
        public string SubitemName { get; set; }

        // Shows up next to the item's name. It defaults to the plugin's icon if set to null
        public string IconLocation { get; set; }
    }
}
