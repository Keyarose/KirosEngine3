using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using KirosEngine3.Math.Data;
using System.Drawing;

namespace KirosEngine3.Mesh
{
    public interface IVertex
    {
        public Vec3 Position { get; set; }

        public static readonly int SizeInBytesU;

        public abstract static void SetVertexAttribs(Shader sh, string[] attribNames);
    }
    //todo: vertex type checking against shader signature
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex : IVertex
    {
        /// <summary>
        /// The vertex's position
        /// </summary>
        public Vec3 Position { get; set; }

        /// <summary>
        /// The size of the vertex in bytes (Unsafe)
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<Vertex>();

        /// <summary>
        /// Defines the vertex attribute pointers for both position and color data at the named locations in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the locations from</param>
        /// <param name="attribNames">The names of the attributes in the shader, ordered by position then color</param>
        /// <exception cref="InvalidOperationException">Thrown if no names are supplied for the attributes</exception>
        public static void SetVertexAttribs(Shader sh, string[] attribNames)
        {
            if (attribNames.Length == 0)
            {
                throw new InvalidOperationException("No shader attribute names were defined.");
            }

            if (attribNames.Length == 1) //if only one is defined then it should be the position attrib
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
            }
        }

        /// <summary>
        /// Defines the vertex attribute pointer for position data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="attribName">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexPositionAttrib(Shader sh, string attribName)
        {
            int posAtt = sh.GetAttribLocationGL(attribName);

            if (posAtt == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", attribName, sh));
            }

            GL.VertexAttribPointer(posAtt, 3, VertexAttribPointerType.Float, false, SizeInBytesU, 0);
            GL.EnableVertexAttribArray(posAtt);
        }

        public override readonly string ToString()
        {
            return string.Format("Position: {0}", Position);
        }
    }

    /// <summary>
    /// Defines a Vertex with position and color with alpha
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ColorVertex : IVertex
    {
        /// <summary>
        /// The vertex's position
        /// </summary>
        public Vec3 Position { get; set; }

        /// <summary>
        /// The vertex's color with alpha
        /// </summary>
        public Color4 Color { get; set; }

        /// <summary>
        /// The size of the vertex in bytes (Unsafe)
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<ColorVertex>();

        /// <summary>
        /// Defines the vertex attribute pointers for both position and color data at the named locations in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the locations from</param>
        /// <param name="attribNames">The names of the attributes in the shader, ordered by position then color</param>
        /// <exception cref="InvalidOperationException">Thrown if no names are supplied for the attributes</exception>
        public static void SetVertexAttribs(Shader sh, string[] attribNames)
        {
            if (attribNames.Length == 0)
            {
                throw new InvalidOperationException("No shader attribute names were defined.");
            }

            if (attribNames.Length == 1) //if only one is defined then it should be the position attrib
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
            }
            else //if 2 or more are defined then both position and uv attributes are set, any further names are ignored
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
                SetVertexColorAttrib(sh, attribNames[1]);
            }
        }

        /// <summary>
        /// Defines the vertex attribute pointer for position data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="attribName">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexPositionAttrib(Shader sh, string attribName)
        {
            int posAtt = sh.GetAttribLocationGL(attribName);

            if (posAtt == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", attribName, sh));
            }

            GL.VertexAttribPointer(posAtt, 3, VertexAttribPointerType.Float, false, SizeInBytesU, 0);
            GL.EnableVertexAttribArray(posAtt);
        }

        /// <summary>
        /// Defines the vertex attribute pointer for color data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="attribName">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexColorAttrib(Shader sh, string attribName)
        {
            int colAtt = sh.GetAttribLocationGL(attribName);

            if (colAtt == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", attribName, sh));
            }

            GL.VertexAttribPointer(colAtt, 4, VertexAttribPointerType.Float, false, SizeInBytesU, Vec3.SizeInBytesU);
            GL.EnableVertexAttribArray(colAtt);
        }

        public override readonly string ToString()
        {
            return string.Format("Position: {0} \t Color: {1}", Position, Color);
        }
    }

    /// <summary>
    /// Defines a Vertex with position and texture coordinates
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TexturedVertex
    {
        /// <summary>
        /// The vertex's position
        /// </summary>
        public Vec3 Position;

        /// <summary>
        /// The vertex's uv coordinates
        /// </summary>
        public Vec2 UV;

        /// <summary>
        /// The size of the vertex in bytes (Unsafe)
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<TexturedVertex>();

        /// <summary>
        /// Sets the given shader's Vertex Attributes to match Vertex's format
        /// </summary>
        /// <param name="sh">The shader to set the Attributes</param>
        /// <param name="attribNames">The names of the Attributes in the shader in the order of Position, then UV</param>
        /// <exception cref="InvalidOperationException">Thrown if no names are supplied for the attributes</exception>
        public static void SetVertexAttribs(Shader sh, string[] attribNames)
        {
            if (attribNames.Length == 0)
            {
                throw new InvalidOperationException("No shader attribute names were defined.");
            }

            if (attribNames.Length == 1) //if only one is defined then it should be the position attrib
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
            }
            else //if 2 or more are defined then both position and uv attributes are set, any further names are ignored
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
                SetVertexUVAttrib(sh, attribNames[1]);
            }
        }

        /// <summary>
        /// Defines the vertex attribute pointer for position data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="attribName">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexPositionAttrib(Shader sh, string name)
        {
            int posAttrib = sh.GetAttribLocationGL(name);

            if (posAttrib == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, sh));
            }

            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, SizeInBytesU, 0);
            GL.EnableVertexAttribArray(posAttrib);
        }

        /// <summary>
        /// Defines the vertex attribute pointer for uv data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="name">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexUVAttrib(Shader sh, string name)
        {
            int uvAttrib = sh.GetAttribLocationGL(name);

            if (uvAttrib == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, sh));
            }

            GL.VertexAttribPointer(uvAttrib, 2, VertexAttribPointerType.Float, false, SizeInBytesU, Vec3.SizeInBytesU);
            GL.EnableVertexAttribArray(uvAttrib);
        }

        public override readonly string ToString()
        {
            return string.Format("Position: {0} \t UV: {1}", Position, UV);
        }
    }

    /// <summary>
    /// Defines a Vertex with position, color with alpha, and texture coordinates
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ColorTexVertex
    {
        /// <summary>
        /// The vertex's position
        /// </summary>
        public Vec3 Position { get; set; }

        /// <summary>
        /// The vertex's color
        /// </summary>
        public Color4 Color { get; set; }

        /// <summary>
        /// The vertex's texture coordinates
        /// </summary>
        public Vec2 UV { get; set; }

        /// <summary>
        /// The size of the vertex in bytes (Unsafe)
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<ColorTexVertex>();

        /// <summary>
        /// Defines the vertex attribute pointers for position, color, and uv data at the named locations in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the locations from</param>
        /// <param name="attribNames">The names of the attributes in the shader, ordered by position, color, then uv</param>
        /// <exception cref="InvalidOperationException">Thrown if no names are supplied for the attributes</exception>
        public static void SetVertexAttribs(Shader sh, string[] attribNames)
        {
            if (attribNames.Length == 0)
            {
                throw new InvalidOperationException("No shader attribute names were defined.");
            }

            if (attribNames.Length == 1) //if only one is defined then it should be the position attrib
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
            }
            else if(attribNames.Length == 2) 
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
                SetVertexColorAttrib(sh, attribNames[1]);
            }
            else //if 3 or more are defined then all attributes are set, any further names are ignored
            {
                SetVertexPositionAttrib(sh, attribNames[0]);
                SetVertexColorAttrib(sh, attribNames[1]);
                SetVertexUVAttrib(sh, attribNames[2]);
            }
        }

        /// <summary>
        /// Defines the vertex attribute pointer for position data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="attribName">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexPositionAttrib(Shader sh, string name)
        {
            int posAttrib = sh.GetAttribLocationGL(name);

            if (posAttrib == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, sh));
            }

            GL.VertexAttribPointer(posAttrib, 3, VertexAttribPointerType.Float, false, SizeInBytesU, 0);
            GL.EnableVertexAttribArray(posAttrib);
        }

        /// <summary>
        /// Defines the vertex attribute pointer for color data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="attribName">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexColorAttrib(Shader sh, string attribName)
        {
            int colAtt = sh.GetAttribLocationGL(attribName);

            if (colAtt == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", attribName, sh));
            }

            GL.VertexAttribPointer(colAtt, 4, VertexAttribPointerType.Float, false, SizeInBytesU, Vec3.SizeInBytesU);
            GL.EnableVertexAttribArray(colAtt);
        }

        /// <summary>
        /// Defines the vertex attribute pointer for uv data at the named location in the given shader
        /// </summary>
        /// <param name="sh">The shader to get the location from</param>
        /// <param name="name">The attribute's name in the shader</param>
        /// <exception cref="ArgumentException">Thrown if the attribute location cannot be found in the shader</exception>
        public static void SetVertexUVAttrib(Shader sh, string name)
        {
            int uvAttrib = sh.GetAttribLocationGL(name);

            if (uvAttrib == -1)
            {
                throw new ArgumentException(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, sh));
            }

            GL.VertexAttribPointer(uvAttrib, 2, VertexAttribPointerType.Float, false, SizeInBytesU, Vec3.SizeInBytesU + Vec4.SizeInBytesU);
            GL.EnableVertexAttribArray(uvAttrib);
        }

        public override readonly string ToString()
        {
            return string.Format("Position: {0} \t Color: {1} \t UV: {2}", Position, Color, UV);
        }
    }
}