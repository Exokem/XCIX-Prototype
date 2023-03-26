# XCIX

Shield: [![CC BY-NC-SA 4.0][cc-by-nc-sa-shield]][cc-by-nc-sa]

## License

This work is licensed under a
[Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License][cc-by-nc-sa].

[![CC BY-NC-SA 4.0][cc-by-nc-sa-image]][cc-by-nc-sa]

[cc-by-nc-sa]: http://creativecommons.org/licenses/by-nc-sa/4.0/
[cc-by-nc-sa-image]: https://licensebuttons.net/l/by-nc-sa/4.0/88x31.png
[cc-by-nc-sa-shield]: https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg

<br/>

## Overview

XCIX (99) is an exploratory project that addresses some of the various provisions of a
game framework.

The project makes primary use of the graphics pipeline provided by the 
[MonoGame](https://www.monogame.net) framework, but does not rely on the content
pipeline that MonoGame provides for packaging game assets. 

The source for the project is split into two modules, Xylem and Vitreous. The 'Xylem' 
module contains most of the library code for the project, while the 'Vitreous' module
contains a prototype implementation of that library code.

After archiving the XCIX project, I decided that its library component should be split
into two parts: a modular content pipeline called Xylem that I am currently working on,
and a separate untitled graphics framework that I am still planning. The present
iteration of the Xylem project is not yet visible publicly, but offers several dramatic
improvements over what is visible in this repository.

<br/>

## Current Features

### Integrated UI Framework

The XCIX project includes a prototype graphical UI framework that is modeled roughly
after the functionality of such libraries as [JavaFX](https://openjfx.io) and Java's
Swing toolkit. 

Some components include: 

- [Frame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Frame.cs) (the default component)
- [Label](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Control/Controls.cs) (displays some text)
- [TextInput](https://github.com/Exokem/XCIX/blob/dc81728f742410f106c2ac0e0db291a776e3bd92/src/Xylem/Framework/Control/Controls.cs#L90) (a label that can be edited, with a movable cursor)
- [GridFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Layout/GridFrame.cs) (arranges contents within a CxR grid, similarly to the [WPF Grid](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.grid?view=windowsdesktop-7.0))
    - [PartitionGridFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Layout/PartitionGridFrame.cs) (the same as a GridFrame, but rows and columns are assigned a specific portion of the whole width or height of the grid)
    - [SelectionGridFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Layout/SelectionGridFrame.cs) (a GridFrame where only one item in the grid can be selected at once)
- [ListFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Layout/ListFrame.cs) (vertical scrollable list of frames)
    - [DynamicListFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Layout/DynamicListFrame.cs) (a ListFrame with some extra dynamic sizing functionality)
- [SplitFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Layout/SplitFrame.cs) (a horizontal or vertical list of frames with a consistent spacing between them)
- [TabFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Layout/TabFrame.cs) (a frame with tabs at the top that change its content - like web browser tabs)
- [Button](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Control/Buttons.cs)
    - [SwitchButton](https://github.com/Exokem/XCIX/blob/dc81728f742410f106c2ac0e0db291a776e3bd92/src/Xylem/Framework/Control/Buttons.cs#L85) (a check button)
    - [AnimatedButton](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Control/AnimatedButton.cs)
    - [Menu](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Control/Menus.cs) (a dropdown menu, like the 'File' or 'Edit' menus at the top of many applications)
- [ContextMenu](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Control/ContextMenu.cs) (a dropdown menu that can be displayed in a specific place)
- [DialogFrame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Control/DialogFrame.cs) (a popup dialog)

<br/>

#### Visual Example - ListFrame with Buttons

![](/Resources/list_frame_example.png)

#### Visual Example - Label & Numeric TextInput

![](/Resources/text_input.png)

#### Visual Example - PartitionGridFrame with ListFrames

![](/Resources/partition_grid.png)

#### Visual Example - TabFrame & SelectionGridFrame

![](/Resources/tab_frame.png)

The thin vertical panel on the left contains four buttons arranged in a 
SelectionGridFrame. Each button has a tooltip to indicate what it does.
The tab frame has two tabs, one for the sector editor and one for the area editor, which
is currently selected.

#### Visual Example - Tilemap Editor Overview

![](/Resources/tilemap_editor.png)

This example shows a functional tilemap editor. The control tab on the left side
features four tools (top to bottom): a tile inspector, a structure placer, a floor
placer, and an element placer. The element placer is not visibly functional, but the 
structure and floor placers can be used - a ghost of the structure/floor will be 
displayed under the cursor.

![](/Resources/zoomed_in.png)

The editor has a zoom feature (mouse wheel scroll) that always maintains the detail of
the tile sprites - this does mean the maximum zoom out is limited. The editor also has 
a shift+scroll to pan feature that supports horizontal scrolling.

![](/Resources/inspector.png)

The inspector tool can be used to change properties of placed structures. This example 
displays the properties of a selected door structure, which can be rotated with its
'axis' property to display North-South or East-West, and can be opened or closed.

#### Theming

![](/Resources/light_mode.png)

All of the colors used by this example are referenced from just a few configurable 
values, meaning it is easy to set up a theme to display the interface differently.

#### Fonts

![](/Resources/font_debug.png)

The project features a custom font rendering system and font file format - this format
is just a png image with a json file that specifies where each of the glyphs are.
The font rendering system supports both monospace and variable space fonts, but does
not support any standard font formats (out of scope).
All displayed fonts are custom made.
The way the font rendering system works is actually surprisingly similar to how
MonoGame's pixel fonts work - I didn't realize this until after I had already written
it.

The debug overlay (activated by the F10 key) will display an outline over any rendered 
character when the mouse moves over it, even if the text is displayed as a part of the
debug overlay.

<br/>

### Dynamic Texture Generation

The graphical provisions of this project as they apply to games are largely centered
around 2D tile-based aesthetics. As such, a substantial effort went into designing a
system to make tile connections more seamless - this takes shape mostly through
[PatchworkConnectors](https://github.com/Exokem/XCIX/tree/development-1/src/Xylem/Graphics/Patchwork).

PatchworkConnectors provide a way to overlay extra textures over the base texture of a
tile conditionally, like when two tiles should visually connect in some way. 

Take these two tiles, representing a door and a wall:

![A sci-fi door](/Resources/door_texture.png)
![A sci-fi wall](/Resources/wall_texture.png)

Without a PatchworkConnector, placing a door between two walls looks like this: 

![A door between two walls](/Resources/door_without_connections.png)

However, with a PatchworkConnector and some extra connection textures, the same door
between two walls looks like this: 

![A better door between two walls](/Resources/door_with_connections.png)

It's not a huge difference, but the second image better portrays the angle and depth of
the textures.

<br/>

For connecting the same tiles, there is also a neat debug overlay that shows where each
connection texture is overlaid.

![](/Resources/single_horizontal_overlay.png)

In the above image, the blue rectangle highlights where an overlay was placed over the 
highlighted tile (see the thin orange outline) to better connect it to the wall tile
on the left.

![](/Resources/single_horizontal_overlay_left.png)

The debug overlay for the left wall shows the same thing, but on the opposite side.

![](/Resources/l1_debug.png)

The overlay for a third wall added above the right wall shows an overlay for the 
bottom half of the new tile.

![](/Resources/l2_debug.png)

Highlighting the bottom right wall this time reveals three overlapping overlays. There
is the blue rectangle on the left side of the wall from the first example, another 
blue rectangle on the top side connecting it to the above wall, and a red rectangle
indicating where a final texture was placed to ensure the corned between the bottom 
left and top right tiles is displayed correctly. 

<br/>

### Modular Content Pipeline

The content pipeline component of the project facilitates the production of runtime 
objects from serialized data.

The iteration of the pipeline that is available in this repository is heavily coupled
to the JSON data format.

```json
{
    "idn": "rowui",
    "res": "Row-UI",
    "base_scale": 2.0,
    "height": 10,
    "space": 1,
    "layout": [
        "QWERTYUIOP[]{}|\\ -",
        "ASDFGHJKL;:'\"ZXCVBNM,<.>/?",
        "qwertyuiop",
        "asdfghjkl",
        "zxcvbnm",
        "0123456789"
    ]
}
```

The above JSON data, for example, would be translated into an instance of the [Typeface](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Component/Typeface.cs)
class: 

```cs
public class Typeface : RegistryEntry
{
    protected readonly Dictionary<char, Rectangle> _glyphs = 
        new Dictionary<char, Rectangle>();

    protected readonly Texture2D _texture;
        
    public readonly bool Monospace;
    public readonly int MaxWidth;
    public readonly int Height;
    public readonly int Space;

    public readonly float BaseScale;

    public Typeface (JObject data) : base(data)
    {
        // ...
    }
}
```

<br/>

#### Terminology

[JsonComposites](https://github.com/Exokem/XCIX/blob/9cc5e33e043aeed980ba9bd064670cf1846ab53b/src/Xylem/Reference/Json.cs#L190) are types that are deserialized from JSON, and serialized to JSON.

[Registries](https://github.com/Exokem/XCIX/blob/9cc5e33e043aeed980ba9bd064670cf1846ab53b/src/Xylem/Registration/Registry.cs#L34) are essentially dictionaries indexing components of a specific type 
against unique identifiers.

[RegistryEntries](https://github.com/Exokem/XCIX/blob/9cc5e33e043aeed980ba9bd064670cf1846ab53b/src/Xylem/Registration/Registry.cs#L14) are JsonComposites that can only be registered once per identifier.
They are conventionally immutable, though some of the code may not reflect this.

[Instances](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Component/Instance.cs) are JsonComposites that contain a reference to some RegistryEntry as well 
as mutable states related to that entry.

Modules (no code representation) are collections of data files that are intended to
resemble game mods. The idea is that you can swap them out as needed to change which 
content is available at runtime.

<br/>

#### Clarifications

Registry entries and instances in this case are analogous to classes and class
instances, from OOP terminology.

The structure of components in the current state of this project does not exactly 
illustrate the principles described above, primarily because I didn't understand what
I was trying to accomplish with the separation between registry entries and instances
until I started working on the second iteration.


<br/>

#### The Pipeline - Indexing

The content pipeline begins with indexing. The [Modules](https://github.com/Exokem/XCIX/tree/development-1/Modules) directory is the root data 
directory - the [Importer](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Data/Importer.cs) begins indexing by reading the module import ordering
from the [module_priorities](https://github.com/Exokem/XCIX/blob/development-1/Modules/module_priorities.jsonc) file within this directory.

Next, the Importer scans each subdirectory of the Modules directory for a [module_info](https://github.com/Exokem/XCIX/blob/development-1/Modules/Xylem/module_info.jsonc)
file, whose existence indicates that the containing directory is a module.

Once all of the module directories have been checked, the Importer verifies that all
required modules are present. If any modules listed in the [module_priorities](https://github.com/Exokem/XCIX/blob/development-1/Modules/module_priorities.jsonc) file 
or as dependencies of a scanned module, an error will be produced.

<br/>

#### The Pipeline - Registry Initializing

After indexing, registries need to be initialized so that the importer has a place to 
store all of the data it imports from present modules. The [ModuleRegistryInitializer](https://github.com/Exokem/XCIX/blob/9cc5e33e043aeed980ba9bd064670cf1846ab53b/src/Xylem/Reflection/ModuleAttributes.cs#L19)
attribute is used to locate 'registry initializer' types, whose static constructors are
subsequently forcibly invoked under the presumption that they will initialize any 
necessary registries. See [Xylem.Registration.R](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Registration/Registries.cs) for an example.

<br/>

#### The Pipeline - Importing

The pipeline swings back to the Importer after all registries are initialized in order
to actually import the data contained in each module.

For each module, in the order they were listed in the module_priorities file, the
importer scans the module's data directory for folders whose names correspond to 
existing registries. If, for example, a registry was initialized in the previous step
with the directory name 'Items', the importer will at one point look for an 'Items' subdirectory
in each module directory. In some cases, the registry directory name may be something like
'Resources/Textures', in which case the importer will be looking for data in the
'[module]/Resources/Textures' directory instead.

The contents of registry directories are imported recursively, so the directory
structure only matters up to that point. Even so, the importer does require data files
to follow a specific format.

Each data file in a registry directory must specify the key of the registry its content
is intended for, as well as a set of entries to be loaded. See [framework_colors.jsonc](https://github.com/Exokem/XCIX/blob/development-1/Modules/Xylem/Options/framework_colors.jsonc)
for an example data file.

When a data file is parsed (using the [Json.NET library](https://www.newtonsoft.com/json)), the raw JObject data for each entry is passed directly to the constructor
of the derived RegistryEntry type for the current registry.

After all data files have been processed, the application begins.


<br/>

## Limitations & Retrospective

This project is rife with limitations, most directly because it is not finished (and 
never will be in this form). 

I'm currently working on a much improved version of the
content pipeline system that will be its own library separate from any graphical tools,
and am looking forward to designing a much improved version of the graphical UI 
framework as another separate library.

<br/>

### The Graphical UI Framework

I did not design the framework that is present in this repository. It's essentially just
an evolution of a previous GUI prototype that became so complicated for its lack of
forethought that I felt it would be easier to start again. 

It was definitely easier to
begin with, but as I added more features and more complex frames, the logic became 
similarly jumbled until any changes would have unpredictable cascading effects.

At one point I wanted to add a margin property to my frames, similar to the insets
they already had. The initial design was so rigid that I quickly gave up on this idea;
all of the existing rendering logic was based on there only being insets to account for.

More to the point, the base [Frame](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Framework/Frame.cs) class has become so bloated that it's extremely
difficult to add anything new without breaking something, or everything, else.

<br/>

### The Content Pipeline

There are a lot of problems I can come up with to describe the content pipeline. Most
of them come from my underestimation of the necessary scope of the thing, and from my 
emphasis on making the prototype application look good before designing its skeleton.
That said, I definitely don't regret the way anything looks (except the code).

First and foremost, the component structure and pipeline are irrevocably coupled to the 
JSON format. I feel that greater flexibility in this regard would be best, if the goal
is to switch to an entirely different format (like XML) or even just a more efficient 
JSON library.

Another point against the pipeline is its reliance on manual conversion of data into
the relevant properties. If you look at the [Typeface](https://github.com/Exokem/XCIX/blob/development-1/src/Xylem/Component/Typeface.cs) 
class, for example, there is a huge amount of logic in the constructor that is just
devoted to reading from a JObject. Even if the logic is unavoidable, it probably belongs
elsewhere. Plus, it would be neater if all of the required properties could just be 
marked with an attribute to be imported automatically. 

Secondarily, a huge portion of the RegistryEntry-JsonComposite-Instance structure is
really confused. I didn't initially realize there was a need to separate instances from 
entries so it was all a bit of an afterthought that I didn't fully understand the 
intent of until I started working on the latest iteration (not in this repo).

<br/>

## Future Iterations

**NOTE:** This section refers to features that are not present in this repository, and are either planned or implemented within other projects (that may not be publicly visible yet).

### Xylem

Planned Features

* Cross-Referencing (when some data depends on other data that hasn't been loaded yet)

Implemented Features

* A custom data serialization format 
* Flexible automatic deserialization
    * Simply marking a property with an attribute is enough for it to be deserialized
    * The system also supports custom deserialization strategies for nonstandard property types
* Interchangeable serialization formats
    * This uses an intermediate data layer that each format must be converted to
* A property inheritance model for registry entries
* A patching model for registry entries (overwriting specific properties of existing entries)
* Consistent versioning schema for data files
* Expansion of the modules system
    * Modules are organized into profiles that can be switched between
* A more well-defined pipeline 
    * Improved error handling 
    * Improved module dependency checking
    * Better distribution of responsibilities
* Automatic registry instantiation
    * Simply marking a registry entry class with an attribute is enough for a registry to be added for it
* Registry import prioritization
* Various other substantial improvements

<br/>

### Graphical UI Library (Final Name Pending)

Goals

* Composition-based (versus frames that extend frames that extend frames; composition may be more modular and easier to test)
* Limited dependence on a specific underlying graphics library (the current setup is coupled to MonoGame)
* A better set of default UI elements (things like buttons and layout controls)
* Raster and vector based rendering
* Support for rendering custom bitmap fonts and standard formats (otf, ttf, etc.)
* Support for rendering a variety of image formats
* Potentially, a domain specific language to simplify UI creation (something like WPF's XAML, but neater and easier to read)


