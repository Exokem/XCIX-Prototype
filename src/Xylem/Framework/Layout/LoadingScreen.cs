
using Microsoft.Xna.Framework;
using Xylem.Framework.Control;
using Xylem.Graphics;
using Xylem.Functional;
using Xylem.Registration;

namespace Xylem.Framework.Layout
{
    public class LoadingFrame : Frame
    {
        private static readonly Resizer LoadingFrameResizer = new HeightRatio(secondary: new WidthRatio());

        private static readonly VerticalAlignment Vertical = new CenteredVerticalAlignment();
        private static readonly HorizontalAlignment Horizontal = new CenteredHorizontalAlignment();

        public override Resizer Resizer => LoadingFrameResizer;

        private long _fadeInCounter = 0L;
        public virtual long StartDelay { get; set; } = 100L;

        public virtual byte FadeIncrement { get; set; } = 2;

        private long _fadeOutCounter = 0L;
        public virtual long FadeOutDelay { get; set; } = 100L;

        public bool HasEnded { get; protected set; }

        public Update OnEnded { get; set; } = () => {};

        private IconFrame _mainFrame;

        public IconFrame MainFrame 
        {
            get => _mainFrame;
            set
            {
                if (_mainFrame != null)
                {
                    Remove(_mainFrame);
                    _mainFrame.ActivationAction = frame => {};
                }
                
                _mainFrame = value;
                _mainFrame.VerticalAlignment = Vertical;
                _mainFrame.HorizontalAlignment = Horizontal;
                _mainFrame.OverlayColor = Color.Black;
                _mainFrame.ActivationAction = frame => this.OnActivated();
                Add(_mainFrame);
            }
        }

        public LoadingFrame(string iconIdentifier)
        {
            IconFrame icon = new IconFrame(R.Textures[iconIdentifier])
            {
                Borderless = true,
                OverlayColor = Color.White,
                Resizer = new HeightRatio(0.35F, new WidthRatio(0.25F))
            };
            MainFrame = icon;
        }

        public override void Resize()
        {
            if (_mainFrame != null)
            {
                _mainFrame.ResourceScale = GraphicsConfiguration.PixelScale;
            }

            base.Resize();
        }

        protected override void OnActivated()
        {
            if (MainFrame == null)
                return;

            if (_fadeInCounter < StartDelay)
            {
                _fadeInCounter = StartDelay;
                MainFrame.OverlayColor = Color.White;
            }

            else if (_fadeOutCounter < FadeOutDelay)
            {
                _fadeOutCounter = FadeOutDelay;
                HasEnded = true;
                OnEnded?.Invoke();
                MainFrame.OverlayColor = Color.Black;
            }
        }

        protected override void PostUpdate()
        {
            if (MainFrame == null)
                return;

            if (_fadeInCounter < StartDelay)
                _fadeInCounter ++;
            
            if (StartDelay <= _fadeInCounter && _fadeOutCounter == 0)
                DoFade();
            else if (_fadeOutCounter != 0 && _fadeOutCounter < FadeOutDelay)
                _fadeOutCounter ++;

            if (FadeOutDelay <= _fadeOutCounter)
                DoFade(true);
        }

        protected virtual void DoFade(bool reverse = false)
        {
            Color color = MainFrame.OverlayColor;

            if (reverse)
            {
                if (0 <= color.R - FadeIncrement)
                {
                    color.R -= FadeIncrement;
                    color.G -= FadeIncrement;
                    color.B -= FadeIncrement;
                }

                else 
                {
                    HasEnded = true;
                    OnEnded?.Invoke();
                }
            }

            else
            {
                if (color.R + FadeIncrement <= 255)
                {
                    color.R += FadeIncrement;
                    color.G += FadeIncrement;
                    color.B += FadeIncrement;
                }

                else 
                    _fadeOutCounter ++;
            }

            MainFrame.OverlayColor = color;
        }
    }
}