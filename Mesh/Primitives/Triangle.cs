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
    /// Defines a primitive Triangle mesh that can be rendered with color
    /// </summary>
    public class Triangle : IDisposable, IRenderable, IFormattable
    {
        protected ColorVertex[] _verts = new ColorVertex[3];

        protected int _VAO;

        protected int _VBO;

        protected string _shaderName;

        protected bool _loaded = false;
        protected bool _disposed = false;

        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        /// <summary>
        /// The points of the triangle
        /// </summary>
        public Vec3[] Points 
        { 
            get { return [_verts[0].Position, _verts[1].Position, _verts[2].Position]; }
        }

        /// <summary>
        /// The mathematical representation of the triangle
        /// </summary>
        public Triangle3D MathTriangle { get { return new Triangle3D(Points); } }

        /// <summary>
        /// The name of the shader to use in rendering
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// The draw mode to be used during rendering
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }

        public Triangle(Vec3[] points, Color4 color, string shaderName = "color")
        {
            if (points.Length < 3)
            {
                throw new ArgumentException(string.Format("Triangle requires three points, only {0} were provided.", points.Length));
            }

            if (shaderName == string.Empty)
                throw new ArgumentException("No shader name specified.", nameof(shaderName));

            _shaderName = shaderName;

            _verts[0].Position = points[0];
            _verts[0].Color = color;

            _verts[1].Position = points[1];
            _verts[1].Color = color;

            _verts[2].Position = points[2];
            _verts[2].Color = color;
        }

        /// <summary>
        /// Sets all vertices to the given color
        /// Init needs to be called again after setting the color
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
        /// Called to initialize the renderable object if it is not being drawn as part of a group
        /// </summary>
        public void Init()
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);

            Shader sh = ShaderManager.Instance[_shaderName];

            sh.SetAttribsGL<ColorVertex>();

            GL.BindVertexArray(0);

            _loaded = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the triangle on the screen using the named shader (OpenGL)
        /// </summary>
        public void DrawGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded triangle object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded triangle object: {0}", this);
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

            GL.DrawArrays(_drawMode, 0, _verts.Length);
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
        /// Draws the triangle on the screen using the named shader (DirectX)
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
            string result = string.Format("Triangle mesh: \n\tPoints: ");

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
        /// <param name="disposing">If true the user code is calling, false the GC system</param>
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
        /// Release the triangle's resources for unloading
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Triangle()
        {
            Dispose(false);
        }
        #endregion
    }
}
