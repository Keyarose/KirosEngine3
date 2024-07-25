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
    /// Defines a CheckBox UI Element.
    /// </summary>
    public class CheckBox : UIElement
    {
        /// <summary>
        /// State flag for the checkbox.
        /// </summary>
        protected bool _checked = false;

        /// <summary>
        /// The name of the texture used for the unchecked state of the checkbox.
        /// </summary>
        protected string _uncheckedTexture;
        /// <summary>
        /// The name of the texture used for the checked state of the checkbox.
        /// </summary>
        protected string _checkedTexture;
        /// <summary>
        /// The method of drawing to be used for the checkbox's state, defaults to replace.
        /// </summary>
        protected CheckBDrawMethod _drawMethod = CheckBDrawMethod.Replace;

        /// <summary>
        /// The label for the checkbox.
        /// </summary>
        protected Label? _cbLabel;

        /// <summary>
        /// Denotes if the checkbox currently has input focus.
        /// </summary>
        protected bool _hasFocus;

        #region Properties
        /// <summary>
        /// Get or set if the checkbox has input focus.
        /// </summary>
        public bool HasFocus { get { return _hasFocus; } set { _hasFocus = value; } }

        /// <summary>
        /// Get the name of the texture to be used when the checkbox isn't checked.
        /// </summary>
        public string UncheckedTexture { get { return _uncheckedTexture; } }

        /// <summary>
        /// Get the name of the texture to be used when the checkbox is checked.
        /// </summary>
        public string CheckedTexture { get { return _checkedTexture; } }

        /// <summary>
        /// The draw method used when the checkbox is checked.
        /// </summary>
        public CheckBDrawMethod DrawMethod { get { return _drawMethod; } set { _drawMethod = value; } }

        /// <summary>
        /// Get or set the checked state of the checkbox.
        /// </summary>
        public bool Checked { get { return _checked; } set { _checked = value; } }
        #endregion

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="position">The position in screen coordinates.</param>
        /// <param name="size">The size in screen coordinates.</param>
        /// <param name="name">The name of the checkbox component.</param>
        /// <param name="ucTextureName">The name of the texture to use when unchecked.</param>
        /// <param name="cTextureName">The name of the texture to use when checked.</param>
        /// <param name="font">The font to use for the label.</param>
        /// <param name="labelText">The text for the label.</param>
        public CheckBox(Vec2 position, Vec2 size, string name, string ucTextureName, string cTextureName, Font? font, string labelText)
        {
            _position = position;
            _size = size;
            _name = name;
            _uncheckedTexture = ucTextureName;
            _checkedTexture = cTextureName;

            _cbLabel = new Label(new Vec2(), new Vec2(), font, labelText, this);//todo: pos and size for label
        }

        #region Load

        #endregion

        #region Draw
        /// <summary>
        /// Draw the checkbox.
        /// </summary>
        public override void Draw()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draw the checkbox using the DirectX API.
        /// </summary>
        public override void DrawDX()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// The different methods of drawing the checked state of the checkbox.
    /// </summary>
    public enum CheckBDrawMethod
    {
        /// <summary>
        /// Replace the unchecked texture with the checked texture.
        /// </summary>
        Replace,
        /// <summary>
        /// Draw the checked texture overtop of the unchecked texture.
        /// </summary>
        DrawOver
    }
}
