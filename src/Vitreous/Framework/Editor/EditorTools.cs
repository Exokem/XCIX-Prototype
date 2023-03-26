
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Input;
using Xylem.Graphics;
using Xylem.Vectors;
using Xylem.Framework;
using Xylem.Functional;
using Vitreous.Component.Composite;
using Vitreous.Reference;
using Vitreous.Component.Spatial;

namespace Vitreous.Framework.Editor
{
    public interface IEditorTool<V>
    {
        string IconResourceIdentifier { get; }
        string Identifier { get; }
        string Description { get; }

        // 
        // Summary:
        //      Renders hovered data associated with this tool within the provided area.
        //      This function should return false when the standard overlay should be used.
        bool RenderHoverData(Rectangle area);

        // 
        // Summary:
        //      Queries whether this tool is applicable to the current state of the system.
        bool Applicable(V component);

        //
        // Summary:
        //      Edits the positions within the provided component according to the 
        //      discretion of this tool. 
        //      This function should return true when the tool has modified the component.
        bool Edit(V component, params Vec2i[] positions);

        //
        // Summary:
        //      Restores the state of the provided component to what it was before the 
        //      'Edit' function was previously invoked on this tool. 
        void Restore(V component, params Vec2i[] positions);

        //
        // Summary:
        //      Provides a list of the options available for this editor tool.
        virtual List<EditorToolOption> ToolOptions() => new List<EditorToolOption>();

        virtual MouseCursor ToolCursor() => MouseCursor.Arrow;

        //
        // Summary: 
        //      End the invokation of this instance of the tool, providing a new one.
        IEditorTool<V> End();
    }

    public sealed class EditorToolOption
    {
        public EditorToolOption(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public bool Enabled { get; set; }
    }

    public class StructurePlaceTool : IEditorTool<Area>
    {
        public readonly StructureEntry Structure;
        private readonly Dictionary<Vec2i, Structure> _previousStructures;

        public static EditorToolOption TestOption = new EditorToolOption("Test");

        public string IconResourceIdentifier => "Icon-Structures";
        public string Identifier => VK.StructurePlacement;
        public string Description => "Place Structures";

        public StructurePlaceTool(StructureEntry structure)
        {
            Structure = structure;
            _previousStructures = new Dictionary<Vec2i, Structure>();
        }

        public bool RenderHoverData(Rectangle area)
        {
            if (Structure != null && !InputProcessor.MouseLeft.Pressed)
            {
                if (!InputProcessor.MouseRight.Pressed && Structure.HasTexture)
                {
                    GraphicsContext.Render(Structure.Texture.Texture, area, Color.White * 0.5F);
                }

                else 
                {
                    // Pixel.FrameRect(area, Color.White * 0.1F, Color.Red);
                    Pixel.FrameRect(area, Color.Red);
                }

                return true;
            }

            return false;
        }

        public bool Applicable(Area component)
        {
            return Structure != null && InputProcessor.MouseLeft.Pressed || InputProcessor.MouseRight.Pressed;
        }

        public bool Edit(Area areaComponent, params Vec2i[] positions)
        {
            bool hasEdited = false;

            StructureEntry structureEntry = InputProcessor.MouseRight.Pressed ? StructureEntry.Empty : Structure;

            foreach (Vec2i v in positions)
            {
                Structure prev = areaComponent.GetStructure(v);
                Structure next = new Structure(structureEntry);

                if (next.Reference != prev.Reference)
                {
                    _previousStructures[v] = areaComponent.GetStructure(v);
                    areaComponent.SetStructure(v, new Structure(structureEntry));

                    hasEdited = true;
                }
            }

            return hasEdited;
        }

        public void Restore(Area areaComponent, params Vec2i[] positions)
        {
            foreach ((Vec2i v, Structure prev) in _previousStructures)
                areaComponent.SetStructure(v, prev);
        }

        public List<EditorToolOption> ToolOptions() => new List<EditorToolOption>{TestOption};

        public IEditorTool<Area> End() => new StructurePlaceTool(Structure);
    }

    public class FloorPlaceTool : IEditorTool<Area>
    {
        public readonly FloorEntry Floor;
        private readonly Dictionary<Vec2i, Floor> _previousFloors;

        public string IconResourceIdentifier => "Icon-Floors";
        public string Identifier => VK.FloorPlacement;
        public string Description => "Place Floors";

        public FloorPlaceTool(FloorEntry floor)
        {
            Floor = floor;
            _previousFloors = new Dictionary<Vec2i, Floor>();
        }

        public bool RenderHoverData(Rectangle area)
        {
            if (Floor != null && !InputProcessor.MouseLeft.Pressed)
            {
                if (!InputProcessor.MouseRight.Pressed && Floor.Texture != null)
                {
                    GraphicsContext.Render(Floor.Texture.Texture, area, Color.White * 0.5F);
                }

                else 
                {
                    // Pixel.FrameRect(area, Color.White * 0.1F, Color.Red);
                    Pixel.FrameRect(area, Color.Red);
                }

                return true;
            }

            return false;
        }

        public bool Applicable(Area component)
        {
            return Floor != null && InputProcessor.MouseLeft.Pressed || InputProcessor.MouseRight.Pressed;
        }

        public bool Edit(Area areaComponent, params Vec2i[] positions)
        {
            bool hasEdited = false;

            FloorEntry floorEntry = InputProcessor.MouseRight.Pressed ? FloorEntry.Empty : Floor;

            foreach (Vec2i v in positions)
            {
                Floor prev = areaComponent.GetFloor(v);
                Floor next = new Floor(floorEntry);

                if (next.Reference != prev.Reference)
                {
                    _previousFloors[v] = areaComponent.GetFloor(v);
                    areaComponent.SetFloor(v, new Floor(floorEntry));

                    hasEdited = true;
                }
            }

            return hasEdited;
        }

        public void Restore(Area areaComponent, params Vec2i[] positions)
        {
            foreach ((Vec2i v, Floor prev) in _previousFloors)
                areaComponent.SetFloor(v, prev);
        }

        public IEditorTool<Area> End() => new FloorPlaceTool(Floor);
    }

    public class ElementAddTool : IEditorTool<Area>
    {
        public readonly ElementEntry Element;
        private readonly Dictionary<Vec2i, Element> _addedElements;

        public string IconResourceIdentifier => "Icon-Elements";
        public string Identifier => VK.ElementAddition;
        public string Description => "Add Elements";

        public ElementAddTool(ElementEntry element)
        {
            Element = element;
            _addedElements = new Dictionary<Vec2i, Element>();
        }

        public bool RenderHoverData(Rectangle area)
        {
            // TODO: Render element textures once implemented

            return false;
        }

        public bool Applicable(Area component)
        {
            return Element != null && InputProcessor.MouseLeft.Clicked;
        }

        public bool Edit(Area areaComponent, params Vec2i[] positions)
        {
            if (Element == ElementEntry.Empty)
                return false;

            bool hasEdited = false;

            foreach (Vec2i v in positions)
            {
                Element element = new Element(Element);
                if (areaComponent.AddElement(v, element))
                {
                    _addedElements[v] = element;
                    NotificationManager.EnqueueNotification($"Added {element.Identifier} at {v}");
                    
                    hasEdited = true;
                }
            }

            return hasEdited;
        }

        public void Restore(Area areaComponent, params Vec2i[] positions)
        {
            foreach (Vec2i v in positions)
            {
                if (_addedElements.ContainsKey(v) && areaComponent.RemoveElement(v, _addedElements[v]))
                {
                    NotificationManager.EnqueueNotification($"Removed {_addedElements[v].Identifier} from {v}");
                }
            }
        }

        public IEditorTool<Area> End() => new ElementAddTool(Element);
    }

    public class TileInspectorTool : IEditorTool<Area>
    {
        // Ideas: add structure/element to inspected tile when they are selected from list
        // Allow elements to be removed visually

        public string IconResourceIdentifier => "Icon-Inspector";
        public string Identifier => VK.TileInspection;
        public string Description => "Inspect Tiles";

        public Vec2i? InspectedTile { get; private set; }

        public Receiver<Tile> TileReceiver { get; set; }

        public bool Applicable(Area component) => InputProcessor.MouseLeft.Clicked;

        public bool Edit(Area areaComponent, params Vec2i[] positions)
        {
            if (positions.Length != 0)
            {
                Vec2i pos = positions[0];
                InspectedTile = pos;

                Tile state = areaComponent[pos];

                TileReceiver?.Invoke(state);

                // Display structure in tile

                // Display each element in the tile
            }

            return false;
        }

        public IEditorTool<Area> End() => this;

        public bool RenderHoverData(Rectangle area) => false;

        public void Restore(Area areaComponent, params Vec2i[] positions) {}
    }

    // public class ElementPlaceTool : IEditorTool
    // {
    //     private readonly ElementEntry _element;
    //     private readonly Dictionary<Vec2i, InstanceMap<ElementEntry, Element>> _previousTileElements;

    //     public ElementPlaceTool()
    // }
}