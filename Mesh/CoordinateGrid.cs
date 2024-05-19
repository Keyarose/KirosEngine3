using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh.Primitives;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Mesh
{
    /// <summary>
    /// Defines a renderable grid of lines that represent a coordinate space
    /// </summary>
    public class CoordinateGrid : IRenderable, IDisposable
    {
        private readonly List<Line> _lines = [];

        private Vec3 _origin;

        protected int _VAO;

        protected int _VBO;

        protected bool _loaded = false;
        protected bool _grouped = false;//flag for if the grid has been loaded in group draw mode or not
        protected bool _disposed = false;

        protected Matrix4 _rotation = Matrix4.Identity;
        protected Matrix4 _scale = Matrix4.Identity;

        public Vec3 Origin { get { return _origin; } set { _origin = value; } }

        public Matrix4 Rotation { get { return _rotation; } set { _rotation = value; } }

        public Matrix4 Scale { get { return _scale; } set { _scale = value; } }

        public PrimitiveType DrawMode
        {
            get { return _lines.FirstOrDefault()?.DrawMode ?? PrimitiveType.Lines; }
            set
            {
                foreach (var line in _lines)
                {
                    line.DrawMode = value;
                }
            }
        }

        #region UnitGridData
        /// <summary>
        /// Data for a 10x10 grid on the X-Y plane
        /// </summary>
        private static readonly Line[] _unitGridDataXY =
            [
                new Line(new Vec3(-5.0f, 0.0f, 0.0f), new Vec3(5.0f, 0.0f, 0.0f), Color4.Red),//x-axis
                new Line(new Vec3(-5.0f, 1.0f, 0.0f), new Vec3(5.0f, 1.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, -1.0f, 0.0f), new Vec3(5.0f, -1.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 2.0f, 0.0f), new Vec3(5.0f, 2.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, -2.0f, 0.0f), new Vec3(5.0f, -2.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 3.0f, 0.0f), new Vec3(5.0f, 3.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, -3.0f, 0.0f), new Vec3(5.0f, -3.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 4.0f, 0.0f), new Vec3(5.0f, 4.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, -4.0f, 0.0f), new Vec3(5.0f, -4.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 5.0f, 0.0f), new Vec3(5.0f, 5.0f, 0.0f), Color4.Black),
                new Line(new Vec3(-5.0f, -5.0f, 0.0f), new Vec3(5.0f, -5.0f, 0.0f), Color4.Black),

                new Line(new Vec3(0.0f, 5.0f, 0.0f), new Vec3(0.0f, -5.0f, 0.0f), Color4.Green),//y-axis
                new Line(new Vec3(1.0f, 5.0f, 0.0f), new Vec3(1.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-1.0f, 5.0f, 0.0f), new Vec3(-1.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(2.0f, 5.0f, 0.0f), new Vec3(2.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-2.0f, 5.0f, 0.0f), new Vec3(-2.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(3.0f, 5.0f, 0.0f), new Vec3(3.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-3.0f, 5.0f, 0.0f), new Vec3(-3.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(4.0f, 5.0f, 0.0f), new Vec3(4.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(-4.0f, 5.0f, 0.0f), new Vec3(-4.0f, -5.0f, 0.0f), Color4.Gray),
                new Line(new Vec3(5.0f, 5.0f, 0.0f), new Vec3(5.0f, -5.0f, 0.0f), Color4.Black),
                new Line(new Vec3(-5.0f, 5.0f, 0.0f), new Vec3(-5.0f, -5.0f, 0.0f), Color4.Black)
            ];

        /// <summary>
        /// Data for a 10x10 grid on the X-Z plane
        /// </summary>
        private static readonly Line[] _unitGridDataXZ =
            [
                new Line(new Vec3(-5.0f, 0.0f, 0.0f), new Vec3(5.0f, 0.0f, 0.0f), Color4.Red),//x-axis
                new Line(new Vec3(-5.0f, 0.0f, 1.0f), new Vec3(5.0f, 0.0f, 1.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, -1.0f), new Vec3(5.0f, 0.0f, -1.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, 2.0f), new Vec3(5.0f, 0.0f, 2.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, -2.0f), new Vec3(5.0f, 0.0f, -2.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, 3.0f), new Vec3(5.0f, 0.0f, 3.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, -3.0f), new Vec3(5.0f, 0.0f, -3.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, 4.0f), new Vec3(5.0f, 0.0f, 4.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, -4.0f), new Vec3(5.0f, 0.0f, -4.0f), Color4.Gray),
                new Line(new Vec3(-5.0f, 0.0f, 5.0f), new Vec3(5.0f, 0.0f, 5.0f), Color4.Black),
                new Line(new Vec3(-5.0f, 0.0f, -5.0f), new Vec3(5.0f, 0.0f, -5.0f), Color4.Black),

                new Line(new Vec3(0.0f, 0.0f, 5.0f), new Vec3(0.0f, 0.0f, -5.0f), Color4.Blue),//z-axis
                new Line(new Vec3(1.0f, 0.0f, 5.0f), new Vec3(1.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(-1.0f, 0.0f, 5.0f), new Vec3(-1.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(2.0f, 0.0f, 5.0f), new Vec3(2.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(-2.0f, 0.0f, 5.0f), new Vec3(-2.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(3.0f, 0.0f, 5.0f), new Vec3(3.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(-3.0f, 0.0f, 5.0f), new Vec3(-3.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(4.0f, 0.0f, 5.0f), new Vec3(4.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(-4.0f, 0.0f, 5.0f), new Vec3(-4.0f, 0.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(5.0f, 0.0f, 5.0f), new Vec3(5.0f, 0.0f, -5.0f), Color4.Black),
                new Line(new Vec3(-5.0f, 0.0f, 5.0f), new Vec3(-5.0f, 0.0f, -5.0f), Color4.Black)
            ];

        /// <summary>
        /// Data for a 10x10 grid on the Y-Z plane
        /// </summary>
        private static readonly Line[] _unitGridDataYZ =
           [
                new Line(new Vec3(0.0f, -5.0f, 0.0f), new Vec3(0.0f, 5.0f, 0.0f), Color4.Green),//y-axis
                new Line(new Vec3(0.0f, -5.0f, 1.0f), new Vec3(0.0f, 5.0f, 1.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, -1.0f), new Vec3(0.0f, 5.0f, -1.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, 2.0f), new Vec3(0.0f, 5.0f, 2.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, -2.0f), new Vec3(0.0f, 5.0f, -2.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, 3.0f), new Vec3(0.0f, 5.0f, 3.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, -3.0f), new Vec3(0.0f, 5.0f, -3.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, 4.0f), new Vec3(0.0f, 5.0f, 4.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, -4.0f), new Vec3(0.0f, 5.0f, -4.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -5.0f, 5.0f), new Vec3(0.0f, 5.0f, 5.0f), Color4.Black),
                new Line(new Vec3(0.0f, -5.0f, -5.0f), new Vec3(0.0f, 5.0f, -5.0f), Color4.Black),

                new Line(new Vec3(0.0f, 0.0f, 5.0f), new Vec3(0.0f, 0.0f, -5.0f), Color4.Blue),//z-axis
                new Line(new Vec3(0.0f, 1.0f, 5.0f), new Vec3(0.0f, 1.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -1.0f, 5.0f), new Vec3(0.0f, -1.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, 2.0f, 5.0f), new Vec3(0.0f, 2.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -2.0f, 5.0f), new Vec3(0.0f, -2.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, 3.0f, 5.0f), new Vec3(0.0f, 3.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -3.0f, 5.0f), new Vec3(0.0f, -3.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, 4.0f, 5.0f), new Vec3(0.0f, 4.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, -4.0f, 5.0f), new Vec3(0.0f, -4.0f, -5.0f), Color4.Gray),
                new Line(new Vec3(0.0f, 5.0f, 5.0f), new Vec3(0.0f, 5.0f, -5.0f), Color4.Black),
                new Line(new Vec3(0.0f, -5.0f, 5.0f), new Vec3(0.0f, -5.0f, -5.0f), Color4.Black)
           ];
        #endregion

        /// <summary>
        /// Predefined 10x10 grid on the X-Y plane
        /// </summary>
        public static CoordinateGrid UnitGridXY => new CoordinateGrid(_unitGridDataXY, Vec3.Zero);

        /// <summary>
        /// Predefined 10x10 grid on the X-Z plane
        /// </summary>
        public static CoordinateGrid UnitGridXZ => new CoordinateGrid(_unitGridDataXZ, Vec3.Zero);

        /// <summary>
        /// Predefined 10x10 grid on the Y-Z plane
        /// </summary>
        public static CoordinateGrid UnitGridYZ => new CoordinateGrid(_unitGridDataYZ, Vec3.Zero);

        public CoordinateGrid(Line[] lines, Vec3 origin)
        {
            _lines.AddRange(lines);
            _origin = origin;
        }

        #region Loading
        public void Init()
        {
            ColorVertex[] verts = new ColorVertex[_lines.Count * 2];

            for (int i = 0; i < _lines.Count; i++)
            {
                ColorVertex[] lineV = _lines[i].GetVertexData();

                verts[i * 2] = lineV[0];
                verts[i * 2 + 1] = lineV[1];
            }

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * verts.Length, verts, BufferUsageHint.StaticDraw);

            Shader sh = ShaderManager.Instance["color"];//todo: better shader handling

            sh.SetAttribsGL<ColorVertex>();

            GL.BindVertexArray(0);

            _loaded = true;
            _grouped = true;
        }

        /// <summary>
        /// Load the lines for independent drawing
        /// </summary>
        public void InitLines()
        {
            foreach (var line in _lines)
            {
                line.Init();
            }

            _loaded = true;
            _grouped = false;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw the lines together as a single unit
        /// </summary>
        /// <param name="vm"></param>
        public void DrawGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded || !_grouped)
            {
                Logger.WriteToLog("Attempt to draw unloaded Coordinate Grid object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded Coordinate Grid object: {0}", this);
                //todo: write to debug console
                return;
            }

            //can't draw, the shader failed to be added to the pipeline, logged in TryGetShader
            if (!ShaderManager.TryGetShader(_lines.FirstOrDefault()?.ShaderName ?? "color", out Shader? sh))
            {
                return;
            }
            sh.UseGL();

            sh.SetUniformMat4GL("model", vm.Model * _rotation);
            sh.SetUniformMat4GL("view", vm.View);
            sh.SetUniformMat4GL("proj", vm.Projection);

            GL.BindVertexArray(_VAO);

            GL.DrawArrays(DrawMode, 0, _lines.Count * 2);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Draw each line of the grid independently
        /// </summary>
        /// <param name="vm"></param>
        public void DrawLinesGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded || _grouped)
            {
                Logger.WriteToLog("Attempt to draw unloaded Coordinate Grid object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded Coordinate Grid object: {0}", this);
                //todo: write to debug console
                return;
            }

            vm.Model *= _rotation;

            foreach (var line in _lines)
            {
                line.DrawGL(vm);
            }
        }

        public ColorVertex[] GetVertexData()
        {
            ColorVertex[] verts = new ColorVertex[_lines.Count * 2];

            for (int i = 0; i < _lines.Count; i++)
            {
                ColorVertex[] lineV = _lines[i].GetVertexData();

                verts[i * 2] = lineV[0];
                verts[i * 2 + 1] = lineV[1];
            }

            return verts;
        }

        public PrimitiveType GetDrawMode()
        {
            return DrawMode;
        }

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

                if (_loaded && _grouped)
                {
                    GL.DeleteBuffer(_VBO);
                    GL.DeleteVertexArray(_VAO);
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Dispose of the grid's unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CoordinateGrid()
        {
            Dispose(false);
        }
        #endregion
    }
}
