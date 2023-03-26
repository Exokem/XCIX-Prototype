
using System.Collections.Generic;
using Xylem.Framework;
using Xylem.Framework.Layout;
using Vitreous.Component.Spatial;
using Vitreous.Registration;
using Vitreous.Component.Composite;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    public partial class Editor : Frame
    {
        internal void SectorSelected(Sector sector)
        {

        }

        internal void AreaSelected(Area area)
        {
            if (CurrentEditor == VK.Sector)
            {
                // Handle for Sector editor
            }

            else if (CurrentEditor == VK.Area)
            {
                // Handle for Area editor
            }
        }

        internal void TileInspected() {} // TODO

        internal void StructureSelected(StructureEntry structure)
        {

        }

        internal void FloorSelected(FloorEntry floor)
        {

        }

        internal void ElementSelected(ElementEntry element)
        {
            
        }

        protected override void UpdateContentFrames()
        {
            base.UpdateContentFrames();

            if (SectorSectorList.Empty)
                SectorSectorList.AddAllItems(EditorData.EditorSectors);
            if (SectorAreaList.Empty)
                SectorAreaList.AddAllItems(EditorData.EditorAreas);

            if (AreaAreaList.Empty)
                AreaAreaList.AddAllItems(EditorData.EditorAreas);

            if (AreaStructures.Empty)
                AreaStructures.AddAllItems(Registries.Structures.Entries());
            if (AreaFloors.Empty)
                AreaFloors.AddAllItems(Registries.Floors.Entries());
            if (AreaElements.Empty)
                AreaElements.AddAllItems(Registries.Elements.Entries());
        }

        internal void AddSector(string identifier)
        {
            Sector sector = new Sector(identifier);
            EditorData.EditorSectors.Add(sector);
            SectorSectorList.AddItem(sector);
        }

        internal void AddArea(string identifier)
        {
            Area area = new Area(identifier);
            EditorData.EditorAreas.Add(area);
            SectorAreaList.AddItem(area);
            AreaAreaList.AddItem(area);
        }
    }
}