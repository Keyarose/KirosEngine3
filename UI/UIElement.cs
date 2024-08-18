using KirosEngine3.Debug;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.UI
{
    /// <summary>
    /// Base class for UI elements.
    /// </summary>
    public abstract class UIElement : IDisposable
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

        /// <summary>
        /// The name of the shader to use in rendering the UI Element.
        /// </summary>
        protected string _shaderName = "";

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

        #region Background Fields
        /// <summary>
        /// The color to use for the UI Element's background. Defaults to Clear.
        /// </summary>
        protected Color4 _backgroundColor = Color4.Clear;

        /// <summary>
        /// The texture to use for the UI Element's background if it has one.
        /// </summary>
        protected Texture? _backgroundTexture;

        /// <summary>
        /// The texture coordinates for the UI Element's background if it has one.
        /// </summary>
        protected Vec2[]? _backgroundTexCoords;
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
        /// Flag that shows if the UIElement has been loaded.
        /// </summary>
        protected bool _loaded = false;
        /// <summary>
        /// Flag that prevents unloaded warning from being sent more than once
        /// </summary>
        protected bool _warnOnce = false;

        /// <summary>
        /// Flag that shows if the UIElement is being disposed of.
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// Flag that shows if the vertex array object is from the parent.
        /// </summary>
        protected bool _parentVAO = false;

        #region Properties
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
        /// The color of the element's background.
        /// </summary>
        public virtual Color4 BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }

        /// <summary>
        /// The texture of the element's background.
        /// </summary>
        public virtual Texture? BackgroundTexture { get { return _backgroundTexture; } set { _backgroundTexture = value; } }

        /// <summary>
        /// The texture coordinates of the element's background.
        /// </summary>
        public virtual Vec2[]? BackgroundTexCoords { get { return _backgroundTexCoords; } set { _backgroundTexCoords = value; } }

        /// <summary>
        /// The parent UI Element to this one.
        /// </summary>
        public virtual UIElement? Parent { get { return _containingElement; } set { _containingElement = value; } }
        #endregion

        #region Load
        /// <summary>
        /// Load the UI element and prepare it for rendering.
        /// </summary>
        /// <param name="VAO">The Vertex Array Object of the parent element if there is one.</param>
        /// <returns>True if successful.</returns>
        public virtual bool Init(int VAO = 0)
        {
            if (VAO == 0)
            {
                _VAO = GL.GenVertexArray();
                _parentVAO = false;
            }
            else
            {
                _VAO = VAO;
                _parentVAO = true;
            }

            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            _EBO = GL.GenBuffer();

            GL.BindVertexArray(0);

            _borderVerts = new Vertex2D[4];
            _borderVerts[0] = new Vertex2D { Position = Vec2.Zero };
            _borderVerts[1] = new Vertex2D { Position = new Vec2(Width, 0f) };
            _borderVerts[2] = new Vertex2D { Position = Size };
            _borderVerts[3] = new Vertex2D { Position = new Vec2(0f, Height) };

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
            if (_border)
            {
                DrawBorderGL(vm);
            }
            DrawBackgroundGL(vm, tu);
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
                Client.Report("Cannot draw border for {0} when default color shader is null.", GetType().Name);

                return;
            }

            //vertex array binding should happen in the sub class
            //GL.BindVertexArray(_VAO);

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

            //GL.BindVertexArray(0);
        }

        /// <summary>
        /// Draw the background of the UI Element.
        /// </summary>
        /// <param name="vm">The view matrices to use in rendering.</param>
        /// <param name="tu">The texture unit to use in rendering.</param>
        protected virtual void DrawBackgroundGL(ViewMatrixes vm, params TextureUnit[] tu)
        {
            //background is clear no rendering required.
            if (_backgroundTexture == null && (_backgroundColor == Color4.Transparent || _borderColor == Color4.Clear)) { return; }

            if (_backgroundTexture == null)
            {
                if (!ShaderManager.TryGetShader(ShaderManager.DefaultColor2DShaderName, out Shader? sh))
                {
                    Client.Report("Cannot draw background for {0} when default color shader is null.", GetType().Name);

                    return;
                }

                sh.UseGL();
                sh.SetUniformMat4GL("model", vm.Model);
                sh.SetUniformMat4GL("proj", vm.UIOrtho);
                sh.SetUniformVec4GL("aColor", (Vec4)_backgroundColor);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);//use the border vertices as they are the same data
                GL.BufferData(BufferTarget.ArrayBuffer, Vertex2D.SizeInBytesU * _borderVerts.Length, _borderVerts, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * 6, [0, 1, 3, 3, 1, 2], BufferUsageHint.StaticDraw);

                sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = Vertex2D.SizeInBytesU });
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                //todo: draw with texture
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Dispose of unmanaged resources.
        /// </summary>
        /// <param name="disposing">If true program is calling dispose, if false GC is.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //clear managed items
                }

                GL.DeleteBuffer(_VBO);
                GL.DeleteBuffer(_EBO);

                if (!_parentVAO)
                    GL.DeleteVertexArray(_VAO);

                _disposed = true;
            }
        }

        /// <summary>
        /// Release the UIElement's resources for unloading.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
