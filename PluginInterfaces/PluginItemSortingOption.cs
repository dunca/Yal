namespace PluginInterfaces
{
    public enum PluginItemSortingOption
    {
        ByNameLength, // items will be displayed by their name's length. This assures that the items with the highest number of matching characters will show up first
        ByOriginalPosition // items with be displayed by their indices in the returned array. Items with higher indices will show up first
    }
}
