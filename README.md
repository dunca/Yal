# Yal
Yet Another Launcher

![s0](http://i.imgur.com/XmhkR0x.png)

Yal can help you **speed up the way you interact with your Windows based PC**.
You can easily **access** your favorite **programs**, **documents**, even **browser bookmarks**.
You don't even have to type their full name. **Yal can usually tell what you actually want, 
without expecting many input characters**. There's also TAB completion that could come handy in some cases.

**The more you use Yal, the better it gets**. It keeps count of the keywords you enter and the entries you
execute, thus it will always give you the best possible suggestions. 

You can also perform simple math calculations, **access your clipboard's history**, **termitate processes**, you can quickly **switch between opened windows**, **perform searches using your favourite search engines**...and more to come.

Here's a crappy video presentation:
[![ScreenShot](http://i.imgur.com/o7cmOwv.png)](https://vimeo.com/185518455)


How to use:
-----------
* Download the latest release (https://github.com/boobcoder/Yal/releases)
* Extract it somewhere
* Run Yal.exe
* if Yal loses focus, you can either click on the system tray icon, or hit a key combination. By default,
the key combination is <kbd>ALT+SPACE</kbd>


Options
-------
The **Options** window can be accessed by hitting <kbd>CTRL+O</kbd> when focused on Yal, or by right-clicking
on Yal's border, and selecting the relevant context menu entry.


**The general options tab**

![general options](http://i.imgur.com/cGIYr10.png)

* **Opacity**: lets you change Yal's opacity
* **Pick a color**: opens a color dialog which lets you change Yal's border color
* **Always on top**: if checked, Yal won't hide when you switch to another window, it will be the topmost window
* **Horizontal/Vertical alignment**: aligns Yal's window at start-up
* **Only move while CTRL is pressed**: if this is checked, you'll only be able to move Yal's window while pressing the CTRL key, and dragging the window with your mouse, as usual
* **Hotkey**: the key combination used to bring Yal to the top if it loses focus
* **Launch at system startup**: it will try to automatically run Yal when you start your PC
* **Keep plugin items in history**: When you run something through Yal, it remembers the input keyword and the launched entry. The next time you write the same keyword, the entries that were previously launched using that keyword, will show up in the output list on top of everyting else. For plugin items this is optional. If you uncheck this option you won't get suggestions based on previously used entries.
* **Fuzzily match file names**: if this is checked, a keyword like ffx will match 'Mozilla Firefox.lnk' (if this file exists in your PC and it's indexed), because Firefox contains all those letters, in order. If you uncheck it, Yal will try to match only exact substrings in file names.
* **Fuzzily match plugin items**: same as above, but for plugin returned entries.
* **Match anywhere in item names**: if this is unchecked, Yal will look for entries that start with the keyword you enter, thus fox will not match 'Mozilla Firefox.lnk', but moz will.
* **Auto indexing (minutes)**: this allows you to set an interval after which Yal will automatically re-index your files. Uncheck this option if you want to disable auto indexing. Disabling this (or setting a really high interval) is especially recommended if you've selected lots of directories to index.
* **Total items to keep in history**: Yal will only keep that many items in the history. If the number of history items goes over the specified threshold, the oldest/least accessed history items will be removed.
* **Max visible items**: specifies the maximum number of items to show at once in the output window. If there are more items available, as scrollbar will show up.
* **Total number of items**: specifies the maximum number of items that will be fetched from the data sources.
* **Trim long names to (chars)**: specifies the maximum number of characters entries can have. Entries longer than the specified value will be trimmed and an ellipsis will be appended (eg.: Super cool Firefox -> Super cool F...)
* **Load and show item icons**: if this is enabled, each entry will be accompanied by an icon. Icons are cool, but this will have a slight impact on Yal's performance, especially on slower systems with mechanical hard drives.
* **Show extensions in file names**: uncheck this option if you don't want to see extensions in the entries provided by Yal's index reader.
* **Plugin items have higher priority**: if checked, plugin generated items will show up first in the output list.

Yal will automatically index some files by default. It does this to in order to provide faster searches.
You can alter the indexing settings in the Indexing tab.

![s2](http://i.imgur.com/ZEAb9wZ.png)

Make sure you apply your new settings (if any) before hitting the 'Rebuild' button.

Plugins
-------
Plugins are pieces of software that extend Yal's functionality. 
They come in two flavors: 
* plugins that require activators
* and those that don't

What's an activator you might ask? It's a special
keyword that has to be always specified when dealing with plugins that require it.
For example, YalBookmark (the plugin that allows you to search through you Firefox and Google Chrome bookmarks) uses the **bk** keyword. Usage: **bk text_to_search_for**.
Other activators: **$** -> for the window switching plugin, **kill** -> for the process temination plugin, **cb** -> for the plugin that lists your clipboard history, **!a**, **!b**, **!g** and others.. for YalWeb (the plugin that let's you search using your favourite search engine).

Here are some of Yal's plugins. Some of them have short help texts.
![s3](http://i.imgur.com/RhpqfGd.png)

![s4](http://i.imgur.com/iMKVb0E.png)

![s5](http://i.imgur.com/tJJLjBH.png)

![s6](http://i.imgur.com/wbkzEbv.png)

![s7](http://i.imgur.com/wBUJtJw.png)

![s8](http://i.imgur.com/IfM1Ndd.png)

![s9](http://i.imgur.com/yOlUaEd.png)

![s10](http://i.imgur.com/wHXC5LC.png)

![s11](http://i.imgur.com/KQ7Gu8C.png)

FAQ
---
**Q**: What operating systems does Yal support?
**A**: MS Windows 7 SP1 and later, because (for now) it is compiled for .NET Framework 4.6.1.

**Q**: Do you plan on adding more features?
**A**: Yes, please suggest any (feasable) features you have in mind.

**Q**: How do I manually clear the index (history) database(s)?
**A**: Simply delete index.sqlite (history.sqlite) found within the folder in which Yal resides.
