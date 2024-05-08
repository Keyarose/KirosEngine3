using KirosEngine3.Math.Data;
using KirosEngine3.Math.Geometry;
using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Mesh.Primitives
{
    public class Quad : IDisposable, IRenderable
    {
        protected ColorVertex[] _verts = new ColorVertex[4];

        protected uint[] _indices = new uint[6];

        protected int _VAO;

        protected int _VBO;

        protected int _EBO;

        protected string _shaderName;

        protected bool _loaded = false;
        protected bool _disposed = false;

        protected PrimitiveType _drawMode = PrimitiveType.TriangleStrip;//todo: replace with custom type that can be converted to PrimitiveType

        /// <summary>
        /// The points that define the Quad
        /// </summary>
        public Vec3[] Points
        {
            get { return [_verts[0].Position, _verts[1].Position, _verts[2].Position, _verts[3].Position]; }
        }

        /// <summary>
        /// The mathematical representation of the Quad
        /// </summary>
        public Rect3D MathQuad { get { return new Rect3D(Points); } }

        /// <summary>
        /// The name of the shader to use in rendering
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// The draw mode to be used during rendering
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }

        public Quad(Vec3[] points, uint[] indices, Color4[] colors, string shaderName = "color")
        {
            //todo: handle input arrays being too short
            _shaderName = shaderName;

            for (int i = 0; i < 4; i++) 
            {
                _verts[i].Position = points[i];
                _verts[i].Color = colors[i];
            }

            _indices = indices;
        }

        public Quad(Rect3D rec, uint[] indices, Color4[] colors, string shaderName = "color") :
            this(rec.Vertices, indices, colors, shaderName)
        { }

        public Quad(Vec3[] points, uint[] indices, Color4 color, string shaderName = "color") :
            this(points, indices, [color, color, color, color], shaderName)
        { }

        #region Loading
        /// <summary>
        /// Initialize the renderable object if it is not being drawn as part of a group
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

            ColorVertex.SetVertexPositionAttrib(sh, "aPosition");//todo: get the shader attrib names from the shader
            ColorVertex.SetVertexColorAttrib(sh, "aColor");

            GL.BindVertexArray(0);

            _loaded = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the Quad on the screen using the draw mode and named shader (OpenGL)
        /// </summary>
        public void DrawGL()
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded quad object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded quad object: {0}", this);
                //todo: write to debug console
                return;
            }

            ShaderManager.TryUseShader(_shaderName);

            GL.BindVertexArray(_VAO);

            GL.DrawElements(_drawMode, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);//clear bound vertex array
        }

        public ColorVertex[] GetVertexData()
        {
            return _verts;
        }

        public PrimitiveType GetDrawMode()
        {
            return _drawMode;
        }

        /// <summary>
        /// Draws the Quad on the screen using the named shader (DirectX)
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void DrawDX()
        {
            throw new NotImplementedException();
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

        ~Quad()
        {
            Dispose(false);
        }
        #endregion
    }
}
