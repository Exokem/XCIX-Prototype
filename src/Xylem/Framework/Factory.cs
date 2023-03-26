
using Microsoft.Xna.Framework;
using Xylem.Registration;
using Xylem.Framework.Control;
using Xylem.Framework.Layout;
using Xylem.Functional;

namespace Xylem.Framework
{
    public abstract class FrameFactory<V> where V : Frame
    {
        protected readonly V Frame;

        protected FrameFactory()
        {
            Frame = DefaultFrameConstructor();
        }

        public FrameFactory<V> Border(Border border)
        {
            Frame.Border = border;
            return this;
        }

        public FrameFactory<V> Resizer(Resizer resizer)
        {
            Frame.Resizer = resizer;
            return this;
        }

        public FrameFactory<V> HorizontalAlignment(HorizontalAlignment alignment)
        {
            Frame.HorizontalAlignment = alignment;
            return this;
        }

        public FrameFactory<V> VerticalAlignment(VerticalAlignment alignment)
        {
            Frame.VerticalAlignment = alignment;
            return this;
        }

        public FrameFactory<V> ExpandVertical()
        {
            Frame.ExpandVertical = true; 
            return this;
        }

        public FrameFactory<V> ExpandHorizontal()
        {
            Frame.ExpandHorizontal = true; 
            return this;
        }

        public FrameFactory<V> Tooltip(string tooltip)
        {
            Frame.TooltipText = tooltip;
            return this;
        }

        public FrameFactory<V> BackgroundColor(Color background)
        {
            Frame.BackgroundColor = background;
            return this;
        }

        public FrameFactory<V> ContentInsets(Insets insets)
        {
            Frame.ContentInsets = insets;
            return this;
        }

        public FrameFactory<V> ContentMargin(Insets insets)
        {
            Frame.ContentMargin = insets;
            return this;
        }

        public V Assemble()
        {
            V frame = DefaultFrameConstructor();
            frame.Border = Frame.Border;
            frame.TooltipText = Frame.TooltipText;
            frame.Resizer = Frame.Resizer;
            frame.HorizontalAlignment = Frame.HorizontalAlignment;
            frame.VerticalAlignment = Frame.VerticalAlignment;
            frame.ExpandHorizontal = Frame.ExpandHorizontal;
            frame.ExpandVertical = Frame.ExpandVertical;
            frame.BackgroundColor = Frame.BackgroundColor;

            frame.ContentMargin = Frame.ContentMargin;
            frame.ContentInsets = Frame.ContentInsets;

            return frame;
        }

        protected abstract V DefaultFrameConstructor();
    }

    [System.Obsolete]
    public class IconButtonFactory : FrameFactory<IconButton>
    {
        public IconButtonFactory() : base()
        {
        }

        protected override IconButton DefaultFrameConstructor() => new IconButton();

        public IconButtonFactory IconScale(float scale)
        {
            Frame.ResourceScale = scale;
            return this;
        }

        public IconButton Assemble(string resource, string tooltip = "", Update selectionReceiver = null)
        {
            IconButton button = base.Assemble();

            // button.Resource = R.Textures[resource];
            // button.TooltipText = tooltip;
            // button.ResourceScale = Frame.ResourceScale;

            // if (selectionReceiver != null)
            //     button.OnSelected = selectionReceiver;

            return button;
        }
    }

    public class ListFrameFactory<V> : FrameFactory<ListFrame<V>>
    {
        public ListFrameFactory() : base() {}

        protected override ListFrame<V> DefaultFrameConstructor() => new ListFrame<V>();

        public ListFrameFactory<V> ItemHeight(int itemHeight)
        {
            Frame.ItemHeight = itemHeight;
            return this;
        }

        public ListFrameFactory<V> ItemConstructor(Function<V, IListItem<V>> constructor)
        {
            Frame.ItemConstructor = constructor;
            return this;
        }

        public ListFrameFactory<V> LabelResizer(Resizer labelResizer)
        {
            Frame.LabelResizer = labelResizer;
            return this;
        }

        public ListFrameFactory<V> AllowDeselection(bool allow)
        {
            Frame.AllowDeselection = allow;
            return this;
        }

        public ListFrame<R> Assemble<R>(string labelText = null, Receiver<R> onSelected = null, Function<R, IListItem<R>> constructor = null) where R : V
        {
            ListFrame<R> frame = new ListFrame<R>(labelText);

            frame.Border = Frame.Border;
            frame.TooltipText = Frame.TooltipText;
            frame.Resizer = Frame.Resizer;
            frame.HorizontalAlignment = Frame.HorizontalAlignment;
            frame.VerticalAlignment = Frame.VerticalAlignment;
            frame.ExpandHorizontal = Frame.ExpandHorizontal;
            frame.ExpandVertical = Frame.ExpandVertical;
            frame.BackgroundColor = Frame.BackgroundColor;

            frame.ItemHeight = Frame.ItemHeight;
            frame.AllowDeselection = Frame.AllowDeselection;

            frame.ContentMargin = Frame.ContentMargin;
            frame.ContentInsets = Frame.ContentInsets;
            
            if (Frame.ListLabel != null)
                frame.LabelResizer = Frame.ListLabel.Resizer;

            if (onSelected != null)
                frame.OnSelected = onSelected;

            if (constructor != null)
                frame.ItemConstructor = constructor;

            return frame;
        }
    }

    public class LabelFactory : FrameFactory<Label>
    {
        protected override Label DefaultFrameConstructor() => new Label("");

        public LabelFactory TextColor(Color color)
        {
            Frame.TextColor = color;
            return this;
        }

        public LabelFactory TextScale(float scale)
        {
            Frame.TextScale = scale;
            return this;
        }

        public LabelFactory CenterText(bool center)
        {
            Frame.CenterText = center;
            return this;
        }

        public Label Assemble(string text)
        {
            Label label = base.Assemble();
            label.LabelText = text;
            label.TextColor = Frame.TextColor;
            label.TextScale = Frame.TextScale;
            label.CenterText = Frame.CenterText;

            return label;
        }
    }

    public class InputFactory : FrameFactory<TextInput>
    {
        protected override TextInput DefaultFrameConstructor() => new TextInput();

        public TextInput Assemble(string defaultText = "", bool numeric = false)
        {
            TextInput input = base.Assemble();
            input.LabelText = defaultText;

            input.Numeric = numeric;

            return input;
        }
    }
}