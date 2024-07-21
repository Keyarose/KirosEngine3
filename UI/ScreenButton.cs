using KirosEngine3.Math.Data;
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
    /// a UI object that defines a renderable button.
    /// </summary>
    public class ScreenButton
    {
        /// <summary>
        /// The position of the button in screen coordinates.
        /// </summary>
        protected Vec2 _position;

        /// <summary>
        /// The background color of the button.
        /// </summary>
        protected Color4 _backColor;

        /// <summary>
        /// The size of the button in screen coordinates.
        /// </summary>
        protected Vec2 _size;

        /// <summary>
        /// The vertex data.
        /// </summary>
        protected ColorVertex[] _verts = new ColorVertex[4];
        /// <summary>
        /// The index list.
        /// </summary>
        protected uint[] _indices = new uint[6];

        /// <summary>
        /// The name of the shader to be used in rendering.
        /// </summary>
        protected string _shaderName;

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
        /// Flag to denote if the button has been unloaded.
        /// </summary>
        protected bool _disposed = false;
        /// <summary>
        /// Flag to denote if the button has been loaded.
        /// </summary>
        protected bool _loaded = false;

        /// <summary>
        /// The position of the Button in screen coordinates.
        /// </summary>
        public Vec2 Position { get { return _position; } set {  _position = value; } }

        /// <summary>
        /// The background color of the Button.
        /// </summary>
        public Color4 BackgroundColor { get { return _backColor; } set { _backColor = value; } }

        /// <summary>
        /// The size of the Button in screen coordinates.
        /// </summary>
        public Vec2 Size { get { return _size; } set { _size = value; } }//todo: resize event?

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="pos">The position in screen coordinates.</param>
        /// <param name="color">The background color.</param>
        /// <param name="size">The size in screen coordinates.</param>
        /// <param name="shader">Optional. The name of the shader to use in rendering. Defaults to color.</param>
        public ScreenButton(Vec2 pos, Color4 color, Vec2 size, string shader = "color") 
        {
            _position = pos;
            _backColor = color;
            _size = size;
            _shaderName = shader;

            _verts[0].Position = pos.AsVec3();
            _verts[1].Position = pos.AsVec3() + new Vec3(size.X, 0.0f, 0.0f);
            _verts[2].Position = pos.AsVec3() + new Vec3(size.X, size.Y, 0.0f);
            _verts[3].Position = pos.AsVec3() + new Vec3(0.0f, size.Y, 0.0f);

            _indices[0] = 0;
            _indices[1] = 1;
            _indices[2] = 2;
            _indices[3] = 2;
            _indices[4] = 3;
            _indices[5] = 0;

            /*_verts[0].Color = Color4.Yellow;
            _verts[1].Color = Color4.Blue;
            _verts[2].Color = Color4.Red;
            _verts[3].Color = Color4.Green;*/


            for (int i = 0; i < _verts.Length; i++)
            {
                _verts[i].Color = color; 
            }
        }

        #region Loading
        /// <summary>
        /// Load the Button for rendering.
        /// </summary>
        public void Init()
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.StaticDraw);

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices.Length, _indices, BufferUsageHint.StaticDraw);

            Shader sh = ShaderManager.Instance[_shaderName];
            sh.SetAttribsGL<ColorVertex>();

            GL.BindVertexArray(0);

            _loaded = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw the button using the OpenGL API.
        /// </summary>
        /// <param name="vm">The view matrices to use.</param>
        public void DrawGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded ScreenButton object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded ScreenButton object: {0}", this);
                DebugConsole.WriteLine("Attempt to draw unloaded ScreenButton object: {0}", this);

                return;
            }

            //if the shader fails to be added to the pipeline log it
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }
            sh.UseGL();

            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("view", vm.View);
            sh.SetUniformMat4GL("proj", vm.Orthographic);

            GL.BindVertexArray(_VAO);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);//clear bound vertex array
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Disposal of unmanaged objects
        /// </summary>
        /// <param name="disposing">If true the user code is calling, false means the GC system is</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //clear managed items
                }

                if (_loaded)
                {
                    GL.DeleteBuffers(2, [_VBO, _EBO]);
                    GL.DeleteVertexArray(_VAO);
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Release the Quad's resources for unloading
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~ScreenButton()
        {
            Dispose(false);
        }
        #endregion
    }
}
