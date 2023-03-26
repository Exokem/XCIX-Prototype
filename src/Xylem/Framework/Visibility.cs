
namespace Xylem.Framework
{
    public interface IVisibility
    {
        bool ShouldUpdate();
        bool ShouldRender();
        bool ShouldOccupySpace();
    }

    public sealed class Visibilities : IVisibility
    {
        private readonly bool _shouldUpdate, _shouldRender, _shouldOccupySpace;

        public static readonly Visibilities Visible, Invisible, None;

        static Visibilities()
        {
            Visible = new Visibilities(true, true, true);
            Invisible = new Visibilities(true, false, true);
            None = new Visibilities(false, false, false);
        }

        private Visibilities(bool shouldUpdate, bool shouldRender, bool shouldOccupySpace)
        {
            _shouldUpdate = shouldUpdate;
            _shouldRender = shouldRender;
            _shouldOccupySpace = shouldOccupySpace;
        }

        public bool ShouldUpdate() => _shouldUpdate;
        public bool ShouldRender() => _shouldRender;
        public bool ShouldOccupySpace() => _shouldOccupySpace;
    }
}