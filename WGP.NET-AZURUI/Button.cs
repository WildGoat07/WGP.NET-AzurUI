﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGP;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace WGP.AzurUI
{
    public class Button : Widget
    {
        #region Protected Fields

        protected VertexArray _gradient;
        protected VertexArray _lines;
        protected Text _text;

        #endregion Protected Fields

        #region Private Fields

        private bool oldMouseState;

        #endregion Private Fields

        #region Public Constructors

        public Button() : base()
        {
            _text = new Text("", Engine.BaseFont, Engine.CharacterSize);
            _text.FillColor = Engine.BaseFontColor;
            _lines = new VertexArray(PrimitiveType.Lines);
            _gradient = new VertexArray(PrimitiveType.Quads);
            Hue = Engine.DefaultHue;
            Pressing = false;
            Hovered = false;
            oldMouseState = false;
            Clicked = null;
        }

        #endregion Public Constructors

        #region Public Properties

        public Action Clicked { get; set; }
        public Vector2f HalfSize { get; set; }
        public bool Hovered { get; protected set; }
        public float Hue { get; set; }
        public override FloatRect LocalBounds => new FloatRect(-HalfSize, HalfSize * 2);
        public bool Pressing { get; protected set; }
        public string Text { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void DrawOn(RenderTarget target)
        {
            Transform tr = Transform.Identity;
            tr.Translate(Position);
            target.Draw(_gradient, new RenderStates(tr));
            target.Draw(_lines, new RenderStates(tr));
            target.Draw(_text, new RenderStates(tr));
        }

        public override void Update(RenderWindow app)
        {
            bool oldHover = Hovered;
            bool oldPress = Pressing;
            Hovered = GlobalBounds.Contains(app.MapPixelToCoords(Mouse.GetPosition(app)));
            if (oldMouseState != Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                oldMouseState = Mouse.IsButtonPressed(Mouse.Button.Left);
                if (Pressing && !oldMouseState && Clicked != null)
                    Clicked();
                if (oldMouseState && Hovered)
                    Pressing = true;
                else
                    Pressing = false;
            }
            if (oldHover != Hovered || (!oldPress && Pressing))
                _chronometer.Restart();
            float s = .3f;
            float bonusV = 0;
            if (Hovered)
            {
                s = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, Time.Zero, Time.FromMilliseconds(500)), .3f, .5f);
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, Time.Zero, Time.FromMilliseconds(500)), 0f, .2f);
            }
            else
                Pressing = false;
            if (Pressing)
            {
                s = .6f;
                bonusV = .4f;
            }
            _text.DisplayedString = Text;
            _text.Origin = (Vector2f)((Vector2i)(_text.GetGlobalBounds().Size() / 2)) + new Vector2f(0, _text.GetLocalBounds().Top);
            _gradient.Clear();
            if (Pressing)
            {
                _gradient.Append(new Vertex(new Vector2f(-HalfSize.X + 1, -HalfSize.Y + 1), new HSVColor(Hue, s / 2, .47f,
                    (byte)Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, Time.Zero, Time.FromMilliseconds(1000)), 0f, 255))));
                _gradient.Append(new Vertex(new Vector2f(HalfSize.X - 2, -HalfSize.Y + 1), new HSVColor(Hue, s / 2, .47f,
                    (byte)Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, Time.Zero, Time.FromMilliseconds(1000)), 0f, 255))));
            }
            else if (Hovered)
            {
                _gradient.Append(new Vertex(new Vector2f(-HalfSize.X + 1, -HalfSize.Y + 1), new HSVColor(Hue, s / 2, .47f,
                    (byte)Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, Time.Zero, Time.FromMilliseconds(500)), 0f, 60))));
                _gradient.Append(new Vertex(new Vector2f(HalfSize.X - 2, -HalfSize.Y + 1), new HSVColor(Hue, s / 2, .47f,
                    (byte)Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, Time.Zero, Time.FromMilliseconds(500)), 0f, 60))));
            }
            else
            {
                _gradient.Append(new Vertex(new Vector2f(-HalfSize.X + 1, -HalfSize.Y + 2), new HSVColor(Hue, s / 2, .47f, 0)));
                _gradient.Append(new Vertex(new Vector2f(HalfSize.X - 2, -HalfSize.Y + 2), new HSVColor(Hue, s / 2, .47f, 0)));
            }
            _gradient.Append(new Vertex(new Vector2f(HalfSize.X - 2, HalfSize.Y - 2), new HSVColor(Hue, s, .47f + bonusV)));
            _gradient.Append(new Vertex(new Vector2f(-HalfSize.X + 1, HalfSize.Y - 2), new HSVColor(Hue, s, .47f + bonusV)));

            _lines.Clear();
            var actualBonus = bonusV * 1.2f;
            var bonus0 = 0;
            Time threshold = Time.Zero;
            Time increase = Time.FromSeconds(1f) / 9f;

            #region light

            if (Pressing)
            {
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));
            }
            else
            {
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y - 1), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, HalfSize.Y - 3), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 1, -HalfSize.Y + 3), new HSVColor(Hue, s, .47f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y + 1), new HSVColor(Hue, s, .47f + bonusV)));
            }

            #endregion light

            #region dark

            threshold = Time.Zero;
            if (Pressing)
            {
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));
                threshold += increase;
                bonusV = Utilities.Interpolation(Utilities.Percent(_chronometer.ElapsedTime, threshold, threshold + increase), bonus0, actualBonus);
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
            }
            else
            {
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(HalfSize.X - 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, HalfSize.Y - 3), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));

                _lines.Append(new Vertex(new Vector2f(-HalfSize.X, -HalfSize.Y + 3), new HSVColor(Hue, s, .27f + bonusV)));
                _lines.Append(new Vertex(new Vector2f(-HalfSize.X + 3, -HalfSize.Y), new HSVColor(Hue, s, .27f + bonusV)));
            }

            #endregion dark
        }

        #endregion Public Methods
    }
}