using KirosEngine3.Math.Vector;
using KirosEngine3.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.UI
{
    /// <summary>
    /// A textbox UI element.
    /// </summary>
    public class TextBox
    {
        //origin at upper left
        /// <summary>
        /// The position of the textbox in screen coordinates.
        /// </summary>
        protected Vec2 _position;

        /// <summary>
        /// The size of the textbox in screen coordinates.
        /// </summary>
        protected Vec2 _size;

        /// <summary>
        /// The font to be used in the textbox.
        /// </summary>
        protected Font _font;

        /// <summary>
        /// The collection of lines of text in the textbox.
        /// </summary>
        protected Queue<string> _lines;
        /// <summary>
        /// The number of lines that fit within the textbox.
        /// </summary>
        protected float _visibleLines;

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

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="position">The position in screen coordinates.</param>
        /// <param name="size">The size in screen coordinates.</param>
        /// <param name="font">The font to be used.</param>
        /// <param name="maxLines">The maximum number of lines.</param>
        public TextBox(Vec2 position, Vec2 size, Font font, int maxLines)
        {
            _position = position;
            _size = size;

            //todo: clamp size to fit on screen

            _font = font;
            _visibleLines = _size.Y / _font.Size;

            _lines = new Queue<string>(maxLines);
        }
    }
}
