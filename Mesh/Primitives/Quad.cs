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
    /// <summary>
    /// Defines a primitive Quad mesh that can be rendered with color
    /// </summary>
    public class Quad : IDisposable, IRenderable, IFormattable
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

        #region UnitQuadData
        private static readonly Vec3[] qPoints =
        [
            new Vec3(-0.5f, 0.5f, 0.0f),//tl
            new Vec3(0.5f, 0.5f, 0.0f),//tr
            new Vec3(0.5f, -0.5f, 0.0f),//br
            new Vec3(-0.5f, -0.5f, 0.0f)//bl
        ];
        #endregion

        /// <summary>
        /// Unit sized predefined Quad with a color of red
        /// </summary>
        public static Quad UnitQuad => new Quad(qPoints, [0, 1, 2, 2, 3, 0], Color4.Red);

        public Quad(Vec3[] points, uint[] indices, Color4[] colors, string shaderName = "color")
        {
            if (points.Length != _verts.Length || indices.Length != _indices.Length || colors.Length != _verts.Length)
                throw new ArgumentException(string.Format("Quad requires {0} vertices and colors, and {1} indices.", _verts.Length, _indices.Length));

            if (shaderName == string.Empty)
                throw new ArgumentException("No shader name specified.", nameof(shaderName));

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

        /// <summary>
        /// Sets all vertices to the given color, the quad needs to be reinitialized for it to 
        /// take effect if Init has already been called
        /// </summary>
        /// <param name="color">The color to set the vertices</param>
        public void SetColor(Color4 color)
        {
            for (int i = 0; i < _verts.Length; i++)
            {
                _verts[i].Color = color;
            }
        }

        /// <summary>
        /// Sets the vertices to the given colors, cycling through the array until all vertices are
        /// updated
        /// Init needs to be called again after setting the colors
        /// </summary>
        /// <param name="colors">The colors to set the vertices</param>
        public void SetColors(Color4[] colors)
        {
            int j = 0;
            for (int i = 0; i < _verts.Length; i++)
            {
                _verts[i].Color = colors[j];

                j = (j + 1) % colors.Length;
            }
        }

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

            sh.SetAttribsGL<ColorVertex>();
            
            GL.BindVertexArray(0);

            _loaded = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the Quad on the screen using the draw mode and named shader (OpenGL)
        /// </summary>
        public void DrawGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded quad object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded quad object: {0}", this);
                //todo: write to debug console
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
            sh.SetUniformMat4GL("proj", vm.Projection);

            GL.BindVertexArray(_VAO);

            GL.DrawElements(_drawMode, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);//clear bound vertex array
        }

        /// <inheritdoc/>
        public ColorVertex[] GetVertexData()
        {
            return _verts;
        }

        /// <inheritdoc/>
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


        #region ToString
        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public string ToString(string? format)
        {
            return ToString(format, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public string ToString(IFormatProvider? formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            string result = string.Format("Quad mesh: \n\tPoints: ");

            foreach (var v in Points)
            {
                result += v.ToString(format, formatProvider) + "\n\t\t";
            }

            return result;
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
