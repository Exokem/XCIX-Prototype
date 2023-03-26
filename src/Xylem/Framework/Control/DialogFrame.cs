
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Framework.Layout;
using Xylem.Reference;
using Xylem.Functional;
using Xylem.Graphics;
using Xylem.Input;

namespace Xylem.Framework.Control
{
    public abstract class DialogFrame : Frame
    {
        protected Frame Content;

        // Provide true to this receiver when the dialog should be closed
        private readonly DualReceiver<bool, DialogFrame> _endReceiver;

        public virtual bool Resolved { get; set; }
        public virtual string Title { get; set; }

        public override Color BackgroundColor { get; set; } = XFK.Tertiary;

        protected DialogFrame(DualReceiver<bool, DialogFrame> endReceiver, string title, Frame content = null)
        {
            Title = title;
            Content = content;
            Add(Content);

            _endReceiver = endReceiver;

            GraphicsConfiguration.AddResizeReceiver((x, y) => Resize());
        }

        protected abstract void OnCancel();
        protected abstract void OnResolve();

        public void Cancel()
        {
            OnCancel();
            _endReceiver(true, this);
        }

        public void Resolve()
        {
            OnResolve();
            _endReceiver(true, this);
        }

        public override void Resize()
        {
            base.Resize();

            W = Math.Max(Content.RW, W);
            H = Math.Max(Content.RH, H);

            CenteredVerticalAlignment.Instance.Align(this);
            CenteredHorizontalAlignment.Instance.Align(this);
        }

        protected override void UpdateContentFrames()
        {
            Content.Update();
            W = Math.Max(Content.RW, W);
            H = Math.Max(Content.RH, H);
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();

            if (InputProcessor.Clicked(Keys.Enter))
                Resolve();
            else if (InputProcessor.Clicked(Keys.Escape))
                Cancel();
        }

        protected override void RenderContentFrames()
        {
            Content.Render();
        }
    }

    public class FormDialog : DialogFrame
    {
        public static void Post(String title, bool cancelable, string[] fields, string[] values, Receiver<Dictionary<string, string>> receiver)
        {
            FormDialog form = new FormDialog(BlockingUpdater.CloseDialog, title, fields, values, cancelable)
            {
                Receiver = receiver
            };

            BlockingUpdater.PostDialog(form);
        }

        // public static void Post(String title, bool cancelable, string[] fields, string[] values, Receiver<Dictionary<string, string>> receiver)
        // {
        //     FormDialog form = new FormDialog(BlockingUpdater.CloseDialog, title, fields, values, cancelable)
        //     {
        //         Receiver = receiver
        //     };

        //     BlockingUpdater.PostDialog(form);
        // }

        public Receiver<Dictionary<string, string>> Receiver { get; set; }

        public FormDialog(DualReceiver<bool, DialogFrame> endReceiver, string title, bool cancelable = true, params string[] fields) : base(endReceiver, title)
        {
            Content = BuildFormLayout(title, fields, new string[0], cancelable);
            Add(Content);
        }

        public FormDialog(DualReceiver<bool, DialogFrame> endReceiver, string title, string[] fields, string[] values, bool cancelable = true) : base(endReceiver, title)
        {
            Content = BuildFormLayout(title, fields, values, cancelable);
            Add(Content);
        }

        protected override void OnCancel()
        {
        }

        protected override void OnResolve()
        {
            if (Receiver == null)
                return;

            Dictionary<string, string> entries = new Dictionary<string, string>();

            if (Content is GridFrame layout)
            {
                for (int i = 1; i < layout.Rows - 1; i ++)
                {
                    if (layout[0, i] is Label label && layout[1, i] is TextInput input)
                        entries[label.LabelText] = input.LabelText;
                }
            }

            Receiver(entries);
        }

        public GridFrame BuildFormLayout(String title, string[] fields, string[] values, bool cancelable = true)
        {
            GridFrame layout = new GridFrame(2, fields.Length + 2)
            {
            };

            Label titleLabel = new Label(title)
            {
                ContentInsets = new Insets(5),
                SpanColumns = 2
            };

            Relocator relocator = MouseDragRelocator.Left;
            relocator.RelocationTarget = () => layout.Container;
            titleLabel.Relocator = relocator;

            layout[0, 0] = titleLabel;

            int maxWidth = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                string field = fields[i];
                Label label = new Label(field)
                {
                    CenterText = false,
                    ContentInsets = new Insets(7),
                    ExpandHorizontal = true,
                    Borderless = true
                };

                TextInput input = new TextInput()
                {
                    ContentInsets = new Insets(5),
                    ExpandHorizontal = true,
                    Borderless = true,
                    TextInputTextBackground = XFK.Secondary,
                    BackgroundColor = Color.Transparent
                };

                if (i < values.Length)
                    input.LabelText = values[i];

                layout[0, i + 1] = label;
                layout[1, i + 1] = input;

                maxWidth = Math.Max(maxWidth, label.MeasuredWidth);
            }

            Button cancel = new Button("Cancel")
            {
                ContentInsets = new Insets(5),
                ActivationAction = v => Cancel()
            };

            Button submit = new Button("Submit")
            {
                ContentInsets = new Insets(5),
                ActivationAction = v => Resolve()
            };

            submit.Border.Left = 0;

            layout[0, fields.Length + 1] = cancel;
            layout[1, fields.Length + 1] = submit;

            layout.W = maxWidth * 4;

            return layout;
        }
    }
}