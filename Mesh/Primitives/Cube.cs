using KirosEngine3.Math.Data;
using KirosEngine3.Math.Geometry;
using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Mesh.Primitives
{
    /// <summary>
    /// Defines a primitive Cube mesh that can be rendered with color
    /// </summary>
    public class Cube : IDisposable, IRenderable, IFormattable
    {
        protected ColorVertex[] _verts = new ColorVertex[8];

        protected uint[] _indices = new uint[36];

        protected int _VAO;

        protected int _VBO;

        protected int _EBO;

        protected string _shaderName;

        protected bool _loaded = false;
        protected bool _disposed = false;

        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        /// <summary>
        /// The points that define the Cube
        /// </summary>
        public Vec3[] Points
        {
            get
            {
                return _verts.Select(vert => vert.Position).ToArray();
            }
        }

        /// <summary>
        /// The mathematical representation of the Cube
        /// </summary>
        public RectCuboid MathCube { get { return new RectCuboid(Points); } }

        /// <summary>
        /// The name of the shader to use in rendering
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// The draw mode to be used during rendering
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }

        #region UnitCubeData
        private static readonly Vec3[] cPoints =
        [
            new Vec3(-0.5f, 0.5f, 0.5f),//ftl
            new Vec3(0.5f, 0.5f, 0.5f),//ftr
            new Vec3(0.5f, -0.5f, 0.5f),//fbr
            new Vec3(-0.5f, -0.5f, 0.5f),//fbl

            new Vec3(-0.5f, 0.5f, -0.5f),//rtl
            new Vec3(0.5f, 0.5f, -0.5f),//rtr
            new Vec3(0.5f, -0.5f, -0.5f),//rbr
            new Vec3(-0.5f, -0.5f, -0.5f)//rbl
        ];

        private static readonly uint[] cIndices =
        [
        0, 1, 2, 2, 3, 0,//front face
        4, 0, 7, 7, 0, 3,//left face
        5, 4, 7, 7, 6, 5,//back face
        1, 5, 2, 2, 5, 6,//right face
        4, 5, 0, 0, 5, 1,//top face
        3, 2, 7, 7, 2, 6//bottom face
        ];
        #endregion
        /// <summary>
        /// Unit sized predefined Cube with a color of red
        /// </summary>
        public static readonly Cube UnitCube = new Cube(cPoints, cIndices, Color4.Red);

        /// <summary>
        /// Construct a Cube using the given points, indices, and colors.
        /// </summary>
        /// <param name="points">The points that define the cube</param>
        /// <param name="indices">The vertex indices used to define the cube's triangles</param>
        /// <param name="colors">The list of colors to be applied to each point</param>
        /// <param name="shaderName">Optional, name of the shader to be used in drawing the Cube.
        /// Defaults to "color"</param>
        /// <exception cref="ArgumentException">Thrown if any of the array parameters are not of the correct length.</exception>
        public Cube(Vec3[] points, uint[] indices, Color4[] colors, string shaderName = "color")
        {
            if (points.Length != _verts.Length || indices.Length != _indices.Length || colors.Length != _verts.Length)
                throw new ArgumentException(string.Format("Cube requires {0} vertices and colors, and {1} indices.", _verts.Length, _indices.Length));

            if (shaderName == string.Empty)
                throw new ArgumentException("No shader name specified.", nameof(shaderName));

            for (int i = 0; i < _verts.Length; i++)
            {
                _verts[i].Position = points[i];
                _verts[i].Color = colors[i];
            }

            _indices = indices;
            _shaderName = shaderName;
        }

        /// <summary>
        /// Construct a Cube using the given points, indices, and colors.
        /// </summary>
        /// <param name="points">The points that define the cube</param>
        /// <param name="indices">The vertex indices used to define the cube's triangles</param>
        /// <param name="color">The color to be applied to every point</param>
        /// <param name="shaderName">Optional, name of the shader to be used in drawing the Cube.
        /// Defaults to "color"</param>
        /// <exception cref="ArgumentException">Thrown if any of the array parameters are not of the correct length.</exception>
        public Cube(Vec3[] points, uint[] indices, Color4 color, string shaderName = "color")
        {
            if (points.Length != _verts.Length || indices.Length != _indices.Length)
                throw new ArgumentException(string.Format("Cube requires {0} vertices, and {1} indices.", _verts.Length, _indices.Length));

            if (shaderName == string.Empty)
                throw new ArgumentException("No shader name specified.", nameof(shaderName));

            for (int i = 0; i < _verts.Length; i++)
            {
                _verts[i].Position = points[i];
                _verts[i].Color = color;
            }

            _indices = indices;
            _shaderName = shaderName;
        }

        /// <summary>
        /// Sets all vertices to the given color, the cube needs to be reinitialized for it to 
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
        /// updated, re-init if already loaded
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

            ColorVertex.SetVertexAttribs(sh, ["aPosition", "aColor"]);

            GL.BindVertexArray(0);

            _loaded = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the Cube on the screen using the draw mode and named shader (OpenGL)
        /// </summary>
        public void DrawGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded cube object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded cube object: {0}", this);
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

            GL.BindVertexArray(0);
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
        /// Draws the Cube on the screen using the named shader (DirectX)
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
            string result = string.Format("Cube mesh: \n\tPoints: ");

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
        /// <param name="disposing">If true the user code is calling, false if the GC system is</param>
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
        /// Release the Cube's resources for unloading
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Cube()
        {
            Dispose(false);
        }
        #endregion
    }
}
