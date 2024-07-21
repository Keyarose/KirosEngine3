using KirosEngine3.Math.Data;
using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Mesh.Primitives
{
    /// <summary>
    /// Defines a drawable point with color
    /// </summary>
    public class Point : IDisposable, IRenderable
    {
        /// <summary>
        /// The vertex data.
        /// </summary>
        protected ColorVertex[] _point = new ColorVertex[1];

        /// <summary>
        /// The vertex array object
        /// </summary>
        protected int _VAO;
        /// <summary>
        /// The vertex buffer object
        /// </summary>
        protected int _VBO;

        /// <summary>
        /// The name of the shader to use in rendering.
        /// </summary>
        protected string _shaderName;

        /// <summary>
        /// Flag denoting if the point has been loaded.
        /// </summary>
        protected bool _loaded = false;
        /// <summary>
        /// Flag denoting if the point has been unloaded.
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// The draw mode to use in rendering.
        /// </summary>
        protected PrimitiveType _drawMode = PrimitiveType.Points;

        /// <summary>
        /// The position of the point
        /// </summary>
        public Vec3 Position { get { return _point[0].Position; } set { _point[0].Position = value; } }

        /// <summary>
        /// The color of the point
        /// </summary>
        public Color4 Color { get { return _point[0].Color; } set { _point[0].Color = value; } }

        /// <summary>
        /// The name of the shader used to render the line
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// The draw mode to be used during rendering
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }

        /// <summary>
        /// Basic constructor for a renderable point object
        /// </summary>
        /// <param name="position">The point's position</param>
        /// <param name="color">The color of the point</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Point(Vec3 position, Color4 color, string shaderName = "color")
        {
            if (shaderName == string.Empty)
                throw new ArgumentException("No shader name specified.", nameof(shaderName));

            _point[0].Position = position;
            _point[0].Color = color;

            _shaderName = shaderName;
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
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _point.Length, _point, BufferUsageHint.DynamicDraw);

            Shader sh = ShaderManager.Instance[_shaderName];

            sh.SetAttribsGL<ColorVertex>();

            GL.BindVertexArray(0);

            _loaded = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the point on the screen using the named shader (OpenGL)
        /// </summary>
        public void DrawGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded point object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded point object: {0}", this);
                DebugConsole.WriteLine("Attempt to draw unloaded point object: {0}", this);

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

            GL.DrawArrays(_drawMode, 0, 1);
            GL.BindVertexArray(0);
        }

        /// <inheritdoc/>
        public ColorVertex[] GetVertexData()
        {
            return _point;
        }

        /// <inheritdoc/>
        public PrimitiveType GetDrawMode()
        {
            return _drawMode;
        }

        /// <summary>
        /// Draws the point on the screen using the named shader (DirectX)
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
            return string.Format("Point mesh: \n\tPoint: {0}", Position.ToString(format, formatProvider));
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
                    GL.DeleteBuffer(_VBO);
                    GL.DeleteVertexArray(_VAO);
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Release the point's resources for unloading
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        ~Point()
        {
            Dispose(false);
        }
        #endregion
    }
}
