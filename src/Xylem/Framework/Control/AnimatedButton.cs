
using Xylem.Vectors;

namespace Xylem.Framework.Control
{
    public abstract class AnimatedButton : Button
    {
        protected virtual Stopwatch HoverTimer { get; set; }

        public bool PauseOnFocus { get; set; }

        public double HoverTimerProgress => HoverTimer.GetProgress();

        protected AnimatedButton()
        {
            HoverTimer = new Stopwatch(limit: 0.1D);

            HoveredProperty.AddObserver((prev, next) =>
            {
                if (next)
                    HoverTimer?.Start();
                else 
                    HoverTimer?.Reverse();
            });

            FocusedProperty.AddObserver((prev, next) =>
            {
                if (PauseOnFocus)
                    HoverTimer.SetPaused(next);
            });
        }
    }
}