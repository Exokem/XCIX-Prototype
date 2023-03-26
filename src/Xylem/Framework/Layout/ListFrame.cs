
using System.Collections.Generic;
using Xylem.Input;
using Xylem.Graphics;
using Xylem.Registration;
using Xylem.Reference;
using Xylem.Data;
using Xylem.Vectors;
using Xylem.Framework.Control;
using Xylem.Functional;

namespace Xylem.Framework.Layout
{
    public class ListFrame<V> : Frame
    {
        protected readonly List<IListItem<V>> ItemList = new List<IListItem<V>>();

        protected int StartIndex = 0;
        protected int DisplayedItems;

        // Adjust content position if there is a label assigned to this list
        // public override int CY => base.CY + (ListLabel != null ? ListLabel.RH : 0);
        // public override int CH => base.CH - (ListLabel != null ? ListLabel.RH : 0);

        public int LH => ListLabel == null ? 0 : ListLabel.RH;

        public Function<V, IListItem<V>> ItemConstructor { get; set; }

        public bool AllowDeselection { get; set; } = true;

        public bool Empty => ItemList.Count == 0;
        public int Count => ItemList.Count;

        public int ItemOffset { get; set; } = 0;

        public int ItemHeight { get; set; }
        public Receiver<V> OnSelected = v => {};

        private IListItem<V> _selectedItem = null;
        public V SelectedItem => _selectedItem == null ? default(V) : _selectedItem.Item;

        // Caused list to be given more space than the size of its containing gridframe
        // public override int MeasuredHeight => base.MeasuredHeight + ItemHeight * _items.Count;

        public override int MeasuredWidth => base.MeasuredWidth + (ItemList.Count != 0 ? ItemList[0].Frame.MeasuredWidth : 0);

        public Resizer LabelResizer
        {
            set 
            {
                if (ListLabel != null)
                    ListLabel.Resizer = value;
            }
        }

        public Border LabelBorder 
        {
            set 
            {
                if (ListLabel != null)
                    ListLabel.Border = value;
            }
        }

        // private Label _listLabel;
        public Label ListLabel { get; set; }

        public ListFrame(string label = null) 
        {
            if (label != null)
            {
                ListLabel = DefaultLabel(label);
                Add(ListLabel);
            }

            Border = new SimpleBorder(0);
        }

        public ListFrame(int itemHeight, Function<V, IListItem<V>> itemConstructor, Label listLabel = null)
        {
            ItemHeight = itemHeight;
            ItemConstructor = itemConstructor;
            // Focusable = true;
            ListLabel = listLabel;
            Add(ListLabel);
        }

        public ListFrame(int itemHeight, Function<V, IListItem<V>> itemConstructor, IEnumerable<V> itemSource, Label listLabel = null) : this(itemHeight, itemConstructor, listLabel)
        {
            foreach (V item in itemSource)
                AddItem(item);
        }

        protected override void UpdateContentFrames()
        {
            ListLabel?.Update();

            if (ItemList.Count == 0 || ItemHeight == 0)
                return;
            
            DisplayedItems = (CH - LH) / ItemHeight;

            for (int i = StartIndex; i < StartIndex + DisplayedItems && i < ItemList.Count; i ++)
            {
                Frame frame = ItemList[i].Frame;
                
                frame.W = CW;
                frame.Update();
            }
        }

        protected override void UpdateGlobalInputs()
        {
            if (Focused || CursorWithin)
            {
                if (0 < StartIndex && InputProcessor.ScrolledUp)
                    StartIndex --;
                else if (StartIndex + DisplayedItems < ItemList.Count && InputProcessor.ScrolledDown)
                    StartIndex ++;
            }
        }

        protected override void RenderContentFrames()
        {
            int y = (CY - RY - BY) + LH;

            if (ListLabel != null)
            {
                ListLabel.W = CW;
                ListLabel.Render();
            }

            for (int i = StartIndex; i < StartIndex + DisplayedItems && y + ItemHeight <= CY + CH; i ++)
            {
                if (0 <= i && i < ItemList.Count)
                {
                    IListItem<V> item = ItemList[i];
                    Frame frame = item.Frame;
                    frame.OX = ItemOffset;
                    frame.OY = y;
                    frame.W = CW;
                    frame.H = ItemHeight;
                    frame.Render();
                    y -= frame.BY;
                }

                if (ItemList.Count <= i)
                    break;

                y += ItemHeight;
            }
        }

        public void AddItem(V item) 
        {
            IListItem<V> frameItem = ItemConstructor(item);
            frameItem.ItemContainer = this;

            ItemList.Add(frameItem);
            Add(frameItem.Frame);

            if (ItemHeight == 0)
                ItemHeight = frameItem.Frame.MeasuredHeight;

            W = System.Math.Max(W, frameItem.Frame.MeasuredWidth);
        }

        public void AddAllItems(params V[] items)
        {
            foreach (V item in items)
                AddItem(item);
        }

        public void AddAllItems(IEnumerable<V> items)
        {
            foreach (V item in items)
                AddItem(item);
        }

        public void RemoveItem(V item)
        {
            IListItem<V> match = null;
            foreach (var entry in ItemList)
            {
                if (entry.Item.Equals(item))
                    match = entry;
            }

            ItemList.Remove(match);
            base.Remove(match?.Frame);
        }

        public override void Remove(Frame frame)
        {
            base.Remove(frame);
            IListItem<V> match = null;
            foreach (var item in ItemList)
            {
                if (item.Frame == frame)
                    match = item;
            }

            if (match != null)
                ItemList.Remove(match);
        }

        internal void SelectFirst()
        {
            if (ItemList.Count != 0)
                Select(ItemList[0]);
        }

        internal void Select(IListItem<V> item)
        {
            if (item.Selected)
            {
                if (AllowDeselection)
                {
                    item.Selected = false;
                    _selectedItem = null;
                }
            }
            else
            {
                if (_selectedItem != null)
                    _selectedItem.Selected = false;
                
                item.Selected = true;
                item.OnSelected();
                _selectedItem = item;
            }

            OnSelected(SelectedItem);
        }

        public IEnumerable<V> Items()
        {
            for (int i = 0; i < ItemList.Count; i ++)
                yield return this[i];
        }

        public V this [int index] => ItemList[index].Item;

        public static Label DefaultLabel(string text)
        {
            return new Label(text)
            {
                ContentInsets = new Insets(5),
                BackgroundColor = XFK.ListFrameLabelBackground,
                Border = new SimpleBorder(1, 0, 1, 0)
            };
        }
    }

    public interface IListItem<V>
    {
        public bool Selected { get; set; }

        public ListFrame<V> ItemContainer { get; set; }

        public V Item { get; }

        public Frame Frame { get; }

        public void OnSelected();
    }

    public abstract class ListFrameItem<V> : AnimatedButton, IListItem<V>
    {
        protected int Shift => Focused ? 10 : (int) (HoverTimerProgress * 10.0D);

        public override int CX => base.CX + Shift;

        public bool RenderDefaultContent = true;
        
        public bool Selected { get => Focused; set => Focused = value; }

        protected ListFrame<V> _itemContainer;
        public ListFrame<V> ItemContainer 
        { 
            get => _itemContainer; 
            set => _itemContainer = value;    
        }

        public V Item { get; }

        public Frame Frame => this;

        protected ListFrameItem(V item)
        {
            Item = item;
            ActivatedProperty.AddObserver((prev, next) =>
            {
                if (!prev && next)
                    ItemContainer?.Select(this);
            });

            BackgroundColor = XFK.Secondary;
            Border = new SimpleBorder(0, 0, 1, 0);
        }
        
        public void OnSelected() {}

        protected override void RenderContentFrames()
        {
            if (Shift != 0)
                Pixel.FillRect(RX, RY, Shift, RH, XFK.ListFrameItemAccent);

            if (RenderDefaultContent)
                base.RenderContentFrames();
        }
    }

    public class RegistryEntryItem<V> : ListFrameItem<V> where V : RegistryEntry
    {
        public RegistryEntryItem(V entry) : base(entry)
        {
            
        }

        protected override void PostUpdate()
        {
            LabelText = Item.Identifier;
            
        }
    }

    public class FrameListItem : IListItem<Frame>
    {
        public bool Selected { get => Frame.Focused; set => Frame.Focused = value; }

        public ListFrame<Frame> ItemContainer { get; set; }

        public Frame Item { get; private set; }

        public Frame Frame { get; private set; }

        public FrameListItem(Frame frame)
        {
            Item = Frame = frame;
        }

        public void OnSelected()
        {
            
        }
    }
}