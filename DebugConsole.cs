using KirosEngine3.Config;
using KirosEngine3.Math.Vector;
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
        protected int _maxLines;//maximum number of allowed lines

        protected int _xPos;
        protected int _yPos;

        protected int _width;
        protected int _height;

        protected int _linesHeight;//how many lines fit in the space

        protected string _defaultFontName = "";

        protected bool _visible;

        /// <summary>
        /// Get or Set the maximum number of lines the console is to remember
        /// </summary>
        public int MaxLines
        {
            get { return _maxLines; }
            set { _maxLines = value; }
        }

        /// <summary>
        /// Get and Set the console's visibility
        /// </summary>
        public bool IsVisible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public DebugConsole(Vec2 pos, Vec2 size, int maxLines) :
            this((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, maxLines)
        { }

        public DebugConsole(int xPos,  int yPos, int width, int height, int maxLines)
        {
            _xPos = xPos;
            _yPos = yPos;
            _width = width;
            _height = height;

            _lines = [];
            _maxLines = maxLines;
            
            if (ConfigVars.TryGetVar(ConfigKeys.D_FONT_NAME_KEY, out string? fontName))
            {
                _defaultFontName = fontName!;
                if (FontManager.TryGetFont(_defaultFontName, out Font? df))
                {
                    _linesHeight = df.Size;
                }
            }
            else
            {
                Console.WriteLine("Warning: Default font is not configured.");
                Logger.WriteToLog("Warning: Default font is not configured.");
            }

            _visible = false; //default to not visible
        }

        /// <summary>
        /// DrawGL the visible lines only if the console is open, and thus visible
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
