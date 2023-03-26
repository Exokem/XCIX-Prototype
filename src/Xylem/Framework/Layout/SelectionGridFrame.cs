
using Xylem.Functional;

namespace Xylem.Framework.Layout
{
    public interface ISelected
    {
        public bool Selected { get; set; }

        public Update OnSelected { get; set; }
    }

    public class SelectionGridFrame<V> : GridFrame<V> where V : Frame, ISelected
    {
        public bool AllowDeselection { get; set; } = true;

        public V SelectedItem { get; private set; } = null;

        public Receiver<V> OnSelected { get; set; }

        public SelectionGridFrame(int columns, int rows) : base(columns, rows)
        {
        }

        internal virtual void Select(V item)
        {
            if (item.Selected)
            {
                if (AllowDeselection)
                {
                    if (SelectedItem != null)
                        SelectedItem.Selected = false;
                    SelectedItem = null;
                }
            }
            else
            {
                if (SelectedItem != null)
                    SelectedItem.Selected = false;

                SelectedItem = item;
                SelectedItem.Selected = true;
                SelectedItem.OnSelected();
                OnSelected?.Invoke(SelectedItem);
            }
        }

        public override V this[int x, int y] 
        { 
            get => base[x, y]; 
            set 
            {
                if (base[x, y] != null)
                    base[x, y].ActivationAction = frame => {};

                base[x, y] = value; 
                // value.ActivatedProperty.AddObserver((wasActivated, isActivated) => 
                // {
                //     if (isActivated && !wasActivated)
                //         Select(value);
                // });
                value.ActivationAction = frame => Select(value);

                if (SelectedItem == null && !AllowDeselection)
                    Select(value);
            }
        }
    }
}