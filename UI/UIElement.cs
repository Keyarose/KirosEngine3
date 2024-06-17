using KirosEngine3.Math.Vector;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.UI
{
    /// <summary>
    /// Base class for UI elements.
    /// </summary>
    public abstract class UIElement
    {
        /// <summary>
        /// The position of the element on screen.
        /// </summary>
        protected Vec2 _screenPos;

        /// <summary>
        /// The size of the element.
        /// </summary>
        protected Vec2 _size;

        /// <summary>
        /// The element that contains this one.
        /// </summary>
        protected UIElement? _containingElement;

        /// <summary>
        /// The UI element's position.
        /// </summary>
        public virtual Vec2 Position { get { return _screenPos; } set { _screenPos = value; } }

        /// <summary>
        /// The size of the UI element.
        /// </summary>
        public Vec2 Size { get { return _size; } set { _size = value; } }

        /// <summary>
        /// The width of the UI element.
        /// </summary>
        public float Width { get { return _size.X; } set { _size.X = value; } }

        /// <summary>
        /// The height of the UI element.
        /// </summary>
        public float Height { get { return _size.Y; } set { _size.Y = value; } }

        /// <summary>
        /// The parent UI Element to this one.
        /// </summary>
        public UIElement? Parent { get { return _containingElement; } set { _containingElement = value; } }

        /// <summary>
        /// Load the UI element and prepare it for rendering.
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();

        /// <summary>
        /// Draw the UI element.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Draw the UI element using the OpenGL API.
        /// </summary>
        /// <param name="vm">The view matrices to use in rendering.</param>
        /// <param name="tu">The texture units to use for rendering.</param>
        public abstract void DrawGL(ViewMatrixes vm, TextureUnit[]? tu);

        /// <summary>
        /// Draw the UI element using the DirectX API.
        /// </summary>
        public abstract void DrawDX();
    }
}
