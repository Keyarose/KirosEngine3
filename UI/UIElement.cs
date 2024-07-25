using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Shaders;
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
        protected Vec2 _position;

        /// <summary>
        /// The size of the element.
        /// </summary>
        protected Vec2 _size;

        /// <summary>
        /// The name of the UI Element.
        /// </summary>
        protected string _name = "";

        #region Border Fields
        /// <summary>
        /// Enable or disable the drawing of the element's border.
        /// </summary>
        protected bool _border;

        /// <summary>
        /// The width of the border lines in px.
        /// </summary>
        protected int _borderWidth;

        /// <summary>
        /// The color of the border, defaults to black.
        /// </summary>
        protected Color4 _borderColor = Color4.Black;

        /// <summary>
        /// The vertices for the element's border.
        /// </summary>
        protected Vertex2D[] _borderVerts = [];

        /// <summary>
        /// The indices for the element's border.
        /// </summary>
        protected uint[] _borderIndices = [];
        #endregion

        /// <summary>
        /// The vertex array object.
        /// </summary>
        protected int _VAO;

        /// <summary>
        /// The vertex buffer object.
        /// </summary>
        protected int _VBO;

        /// <summary>
        /// The index buffer object.
        /// </summary>
        protected int _EBO;

        /// <summary>
        /// The element that contains this one.
        /// </summary>
        protected UIElement? _containingElement;

        /// <summary>
        /// The UI element's position.
        /// </summary>
        public virtual Vec2 Position { get { return _position; } set { _position = value; } }

        /// <summary>
        /// The size of the UI element.
        /// </summary>
        public virtual Vec2 Size { get { return _size; } set { _size = value; } }

        /// <summary>
        /// The width of the UI element.
        /// </summary>
        public virtual float Width { get { return _size.X; } set { _size.X = value; } }

        /// <summary>
        /// The height of the UI element.
        /// </summary>
        public virtual float Height { get { return _size.Y; } set { _size.Y = value; } }

        /// <summary>
        /// The name of the UI Element.
        /// </summary>
        public virtual string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// Enable or disable the drawing of a border around the UIElement.
        /// </summary>
        public virtual bool Border { get { return _border; } set { _border = value; } }

        /// <summary>
        /// The width of the border in px.
        /// </summary>
        public virtual int BorderWidth { get { return _borderWidth; } set { _borderWidth = value; } }

        /// <summary>
        /// The color of the element's border.
        /// </summary>
        public virtual Color4 BorderColor { get { return _borderColor; } set { _borderColor = value; } }

        /// <summary>
        /// The parent UI Element to this one.
        /// </summary>
        public virtual UIElement? Parent { get { return _containingElement; } set { _containingElement = value; } }

        #region Load
        /// <summary>
        /// Load the UI element and prepare it for rendering.
        /// </summary>
        /// <returns>True if successful.</returns>
        public virtual bool Init()
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            _EBO = GL.GenBuffer();

            GL.BindVertexArray(0);

            _borderVerts = new Vertex2D[4];
            _borderVerts[0] = new Vertex2D { Position = Position };
            _borderVerts[1] = new Vertex2D { Position = Position + new Vec2(Width, 0f)};
            _borderVerts[2] = new Vertex2D { Position = Position + Size};
            _borderVerts[3] = new Vertex2D { Position = Position + new Vec2(0f, Height) };

            _borderIndices = [0, 1, 1, 2, 2, 3, 3, 0];
            return true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw the UI element.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Draw the UI element using the OpenGL API.
        /// </summary>
        /// <param name="vm">The view matrices to use in rendering.</param>
        /// <param name="tu">The texture units to use for rendering.</param>
        public virtual void DrawGL(ViewMatrixes vm, params TextureUnit[] tu)
        {
            GL.BindVertexArray(_VAO);
            if (_border)
            {
                DrawBorderGL(vm);
            }
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Draw the UI element using the DirectX API.
        /// </summary>
        public abstract void DrawDX();

        /// <summary>
        /// Method for drawing the border of the UI Element.
        /// </summary>
        /// <param name="vm">The view matrices to use in rendering.</param>
        protected virtual void DrawBorderGL(ViewMatrixes vm)
        {
            if (!ShaderManager.TryGetShader(ShaderManager.DefaultColor2DShaderName, out Shader? sh))
            {
                Console.WriteLine("Cannot draw border for {0} when default color shader is null.", GetType().Name);
                Logger.WriteToLog("Cannot draw border for {0} when default color shader is null.", GetType().Name);
                DebugConsole.WriteLine("Cannot draw border for {0} when default color shader is null.", GetType().Name);

                return;
            }

            GL.BindVertexArray(_VAO);

            sh.UseGL();
            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("proj", vm.UIOrtho);
            sh.SetUniformVec4GL("aColor", (Vec4)_borderColor);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex2D.SizeInBytesU * _borderVerts.Length, _borderVerts, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _borderIndices.Length, _borderIndices, BufferUsageHint.StaticDraw);

            sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = Vertex2D.SizeInBytesU });
            GL.DrawElements(PrimitiveType.Lines, _borderIndices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }
        #endregion
    }
}
