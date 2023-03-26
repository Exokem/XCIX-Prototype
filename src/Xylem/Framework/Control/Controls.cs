
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Data;
using Xylem.Input;
using Xylem.Component;
using Xylem.Graphics;
using Xylem.Functional;
using Xylem.Reference;

namespace Xylem.Framework.Control
{
    public class Label : Frame
    {
        public Typeface Typeface = XylemOptions.DefaultTypeface;

        public bool CenterText { get; set; } = true;

        protected StringBuilder _textBuilder = new StringBuilder();
        public string LabelText
        {
            get => _textBuilder.ToString();
            set
            {
                _textBuilder.Clear();
                _textBuilder.Append(value);
            }
        }

        private float _textScale = 1F;

        public float TextScale 
        {
            get => _textScale;
            set
            {
                _textScale = value;
                FitText();
            }
        }

        public override Insets ContentInsets 
        { 
            get => base.ContentInsets; 
            set 
            {
                base.ContentInsets = value; 
                FitText();
            }
        }

        protected virtual void FitText()
        {
            int textHeight = (int) Typeface.CharHeight(TextScale);
            if (CH < textHeight)
                H = textHeight + ContentInsets.Height;
        }

        public virtual Color TextColor { get; set; }

        public Label(string text, Color? textColor = null)
        {
            LabelText = text;
            
            if (textColor.HasValue)
                TextColor = textColor.Value;
            else 
                TextColor = PrimaryColor;
        }

        protected override void PostUpdate()
        {
            // FitText();
        }

        protected override void RenderExtraContent()
        {
            if (LabelText != null)
                Graphics.Text.RenderTextWithin(Typeface, LabelText, ContentArea, TextColor, TextScale, centerHorizontal: CenterText);
        }

        public override int MeasuredWidth => base.MeasuredWidth + (int) Typeface.StringWidth(LabelText, TextScale);

        public override int MeasuredHeight => base.MeasuredHeight + (int) Typeface.ScaledHeight(TextScale);
    }

    public class TextInput : Label
    {
        public static readonly IntervalPublisher<bool> CursorBlinkPublisher;

        public static bool CursorBlinkOn { get; private set; }

        protected override MouseCursor HoveredCursor => MouseCursor.IBeam;

        public virtual Color TextInputTextBackground { get; set; } = XFK.TextInputTextBackground;
        public virtual Color TextInputTextBackgroundHovered { get; set; } = XFK.TextInputTextBackgroundHovered;

        public override Color BackgroundColor { get; set; } = XFK.TextInputBackground;

        public virtual Color TextInputCursor { get; set; } = XFK.TextInputCursor;

        static TextInput()
        {
            CursorBlinkPublisher = new IntervalPublisher<bool>(true, TimeSpan.FromMilliseconds(700), cursorVisible => !cursorVisible, autoUpdate: false);

            CursorBlinkPublisher.AttachReceiver(cursorBlinkedOn => CursorBlinkOn = cursorBlinkedOn);
        }

        public Receiver<string> OnTextChanged;

        public bool Numeric { get; set; } = false;

        private Property<int> _cursor = new Property<int>(0);
        private int Cursor
        {
            get => _cursor.Value;
            set => _cursor.Value = value;
        }

        private int _start;
        private int _chars;

        private Queue<char> _keyQueue = new Queue<char>();

        public string RenderedText => LabelText.Substring(_start, _chars);

        

        public TextInput(string defaultText = "") : base(defaultText)
        {
            Cursor = _start = _chars = 0;

            ActivationAction = frame => 
            {
                if (frame.Activated)
                    FocusManager.SetFocused(this);
            };

            // Reset cursor blink interval when focused or cursor moved
            FocusedProperty.AddObserver((p, n) => CursorBlinkPublisher.Reset());
            _cursor.AddObserver((p, n) => CursorBlinkPublisher.Reset());

            Focusable = true;
        }

        public override void ReceiveTextInput(TextInputEventArgs args)
        {
            char c = args.Character;

            if (Numeric && ('9' < c || c < '0'))
                return;
            
            if (Typeface.SupportsChar(c))
                _keyQueue.Enqueue(c);

            CursorBlinkPublisher.Reset();
        }

        protected override void OnActivated()
        {
            if (!InputProcessor.CursorWithin(ContentArea))
                return;

            int rx = InputProcessor.X - CX;

            if (WidthOf(RenderedText) < rx)
            {
                Cursor = LabelText.Length;
                return;
            }

            int w = 0;

            for (int i = _start; i < _start + _chars; i ++)
            {
                char c = LabelText[i];
                int cw = WidthOf(c);

                if (rx < w + cw)
                {
                    if (rx < w + (cw / 2))
                        Cursor = i;
                    else 
                        Cursor = i + 1;
                    return;
                }

                w += cw + Typeface.ScaledSpaceWidth(TextScale);
            }
        }

        public void Insert(char c) => Insert(c.ToString());
        public void Insert(string text)
        {
            _textBuilder.Insert(Cursor, text);
            Cursor += text.Length;
            OnTextChanged?.Invoke(_textBuilder.ToString());
        }

        public void Delete(int amount = 1)
        {
            if (Cursor == 0 || _textBuilder.Length == 0)
                return;

            Cursor--;

            _textBuilder.Remove(Cursor, 1);
        }

        protected int SpacedWidthOf(char input) => (int) Typeface.SpacedCharWidth(input, TextScale);

        protected int WidthOf(char input) => (int) Typeface.CharWidth(input, TextScale);

        protected int WidthOf(string input) => (int) Typeface.StringWidth(input, TextScale);

        private void AdjustVisibleChars() => AdjustVisibleChars(() => {});
        private void AdjustVisibleChars(Action extraAction)
        {
            while (0 < _chars && CW < WidthOf(RenderedText))
            {
                extraAction();
                _chars--;
            }

            while (_start + _chars < LabelText.Length && WidthOf(RenderedText) + WidthOf(LabelText[_start + _chars]) < CW)
            {
                _chars++;
            }
        }

        public override void Resize()
        {
            base.Resize();
            AdjustVisibleChars();
        }

        protected override void UpdateContentFrames()
        {
            int textWidth = WidthOf(RenderedText);

            if (Focused)
                CursorBlinkPublisher.ForceUpdate();

            // Insert next queued key
            if (_keyQueue.Count != 0)
            {
                char c = _keyQueue.Dequeue();
                int charWidth = SpacedWidthOf(c);

                // If the character will overflow:
                // 1. Shift start right by 1 character until it does not overflow, also
                //    decreasing the number of visible characters by 1 each time

                Insert(c);

                if (textWidth + charWidth < CW)
                    _chars++;
                // The character has been inserted in the middle of the text
                else if (Cursor != _textBuilder.Length)
                    AdjustVisibleChars();
                // The character has been added at the end of the text, and does not fit
                else AdjustVisibleChars(() => _start++);
            }

            // 1. Move cursor when key first clicked
            // 2. If key pressed for windup time X, start fast movement
            // 3. If fast movement is active, move cursor for every Y seconds key held

            if (Cursor != _textBuilder.Length && (InputProcessor.Clicked(Keys.Right) || InputProcessor.RepeatClicked(Keys.Right)))
                Cursor++;
            else if (Cursor != 0 && (InputProcessor.Clicked(Keys.Left) || InputProcessor.RepeatClicked(Keys.Left)))
                Cursor--;
            else if (InputProcessor.Clicked(Keys.Home))
                Cursor = 0;
            else if (InputProcessor.Clicked(Keys.End))
                Cursor = _textBuilder.Length;
            else if (InputProcessor.Clicked(Keys.Back) || InputProcessor.RepeatClicked(Keys.Back))
                Delete();

            // Update displayed char count after a deletion
            if (_textBuilder.Length < _chars)
                _chars = _textBuilder.Length;
            if (_textBuilder.Length < _start + _chars)
                _chars = _textBuilder.Length - _start;

            // Shift start left; also adjust visible chars to prevent overflow
            if (Cursor < _start)
            {
                _start = Cursor;
                AdjustVisibleChars();
            }

            // Shift start right
            if (_start + _chars < Cursor)
            {
                _start = Cursor - _chars;
                AdjustVisibleChars();
            }

            // Probably redundant bounds checking
            if (_chars < 0)
                _chars = 0;

            // Ensure that at least one char is visible when deleting from the end
            if (_chars == 0 && LabelText.Length != 0)
            {
                _start--;
                _chars++;
            }

            // Ensure that at least one char is visible before the cursor when deleting
            // from the middle
            else if (Cursor != 0 && Cursor == _start)
            {
                _start--;
                AdjustVisibleChars();
            }

            // Show entire string if it can fit after deleting
            else if (_chars < LabelText.Length && Cursor != 0 && _start != 0 && Typeface.StringWidth(LabelText, TextScale) < CW)
            {
                _start = 0;
                _chars = LabelText.Length;
            }

            // else if (_cursor != 0)

            _start = System.Math.Clamp(_start, 0, _textBuilder.Length);
            _chars = System.Math.Clamp(_chars, 0, _textBuilder.Length - _start);
        }

        protected override void RenderBackground()
        {
            Pixel.FillRect(RenderArea, BackgroundColor);

            if (Hovered)
                Pixel.FillRect(MarginArea, TextInputTextBackgroundHovered);
            else
                Pixel.FillRect(MarginArea, TextInputTextBackground);
        }

        protected override void RenderExtraContent()
        {
            if (LabelText.Length != 0)
                Graphics.Text.RenderTextWithin(Typeface, RenderedText, ContentArea, PrimaryColor, TextScale, centerHorizontal: false);

            if (Focused)
            {
                string substring = LabelText.Substring(_start, Cursor - _start);

                if (CursorBlinkOn)
                {
                    int cursorHeight = (int) Typeface.CharHeight(TextScale);
                    float cursorSpace = (CH - cursorHeight) / 2F;

                    int cursorOffset = (int) Typeface.StringWidth(substring, TextScale);

                    Graphics.Pixel.FrameRect(CX + cursorOffset, CY + (int) cursorSpace, 1, cursorHeight, TextInputCursor);
                }
            }

            if (GraphicsConfiguration.Debug)
            {
                Graphics.Text.RenderTextAt(XylemOptions.DefaultTypeface, $"s:{_start} x:{Cursor} c:{_chars} t:{LabelText}", RX, RY - (int) Typeface.CharHeight());
            }
        }
    }
}