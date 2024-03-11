using KirosEngine3.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3
{
    internal class DebugConsole
    {
        protected List<string> _lines = [];

        protected int _xPos;
        protected int _yPos;

        protected int _width;
        protected int _height;

        protected int _charWidth;
        protected int _linesHeight;//how many lines fit in the space

        protected Font _defaultFont;

        protected bool _visible;

        /// <summary>
        /// Get and Set the console's visibility
        /// </summary>
        public bool IsVisible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public DebugConsole(int xPos,  int yPos, int width, int height)
        {
            _xPos = xPos;
            _yPos = yPos;
            _width = width;
            _height = height;

            //todo: load the default font based on config

            _visible = false; //default to not visible
        }

        /// <summary>
        /// Draw the visible lines only if the console is open, and thus visible
        /// </summary>
        public void Draw()
        {
            if(_visible) 
            {
                //todo: draw
            }
        }
    }
}
