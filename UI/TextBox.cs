using KirosEngine3.Math.Vector;
using KirosEngine3.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.UI
{
    public class TextBox
    {
        //origin at upper left
        protected Vec2 _position;

        protected Vec2 _size;

        protected Font _font;

        protected Queue<Text> _lines;
        protected float _visibleLines;//how many lines can be rendered in the box space

        /// <summary>
        /// The on screen position of the textbox.
        /// </summary>
        public Vec2 Position
        {
            get { return _position; }
            set { _position = value; }//todo: clamp to prevent the box from going off screen
        }

        /// <summary>
        /// The size of the textbox.
        /// </summary>
        public Vec2 Size
        {
            get { return _size; }
            set { _size = value; }//todo: clamp to prevent off screen flow
        }

        /// <summary>
        /// The width of the textbox.
        /// </summary>
        public float Width
        {
            get { return _size.X; }
            set { _size.X = value; }//todo: clamp to prevent off screen flow
        }

        /// <summary>
        /// The height of the textbox.
        /// </summary>
        public float Height
        {
            get { return _size.Y; }
            set { _size.Y = value; }//todo: clamp
        }

        public TextBox(Vec2 position, Vec2 size, Font font, int maxLines)
        {
            _position = position;
            _size = size;

            //todo: clamp size to fit on screen

            _font = font;
            _visibleLines = _size.Y / _font.Size;

            _lines = new Queue<Text>(maxLines);
        }
    }
}
