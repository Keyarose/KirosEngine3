using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using OpenTK.Graphics.OpenGL4;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using KirosEngine3.Mesh;

namespace KirosEngine3.Shaders
{
    public class Shader : IDisposable
    {
        protected int _handle;
        protected bool _disposed;
        protected string _name;

        protected string _vertPath;
        protected string _fragPath;

        protected ShaderAttribNames _attribNames;

        protected readonly Dictionary<string, int> _uniformLocations = [];
        protected Dictionary<string, Tuple<int, ActiveAttribType>> _attribList = [];

        public int Handle
        { get { return _handle; } }

        public string Name
        { get { return _name; } }

        /// <summary>
        /// The name of the attrib used in the shader for position data, empty string if unused.
        /// </summary>
        public string PositionAttribName
        { get { return _attribNames.Position; } }

        /// <summary>
        /// The name of the attrib used in the shader for color data, empty string if unused.
        /// </summary>
        public string ColorAttribName
        { get { return _attribNames.Color; } }

        /// <summary>
        /// The name of the attrib used in the shader for uv data, empty string if unused.
        /// </summary>
        public string UVAttribName
        { get { return _attribNames.UV; } }

        /// <summary>
        /// The name of the attrib used in the shader for normal data, empty string if unused.
        /// </summary>
        public string NormalAttribName
        { get { return _attribNames.Normal; } }

        /// <summary>
        /// Construct a shader from a vertex shader and a fragment shader
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="vertPath">The path to the vertex shader file</param>
        /// <param name="fragPath">The path to the fragment shader file</param>
        public Shader(string name, string vertPath, string fragPath) 
            : this(name, vertPath, fragPath, new ShaderAttribNames())
        {
        }

        /// <summary>
        /// Constructor for co-located shader programs with identical names
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="dualPath">The path to shader programs located in the same folder with the same name</param>
        /// <param name="vertexExt">Optional file extension for the vertex shader file, defaults to .vert</param>
        /// <param name="fragmentExt">Optional file extension for the fragment shader file, defaults to .frag</param>
        public Shader(string name, string dualPath, string vertexExt = ".vert", string fragmentExt = ".frag") 
            : this (name, dualPath, new ShaderAttribNames(), vertexExt, fragmentExt)
        { 
        }

        /// <summary>
        /// Construct a shader from a vertex shader and a fragment shader
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="vertPath">The path to the vertex shader file</param>
        /// <param name="fragPath">The path to the fragment shader file</param>
        /// <param name="attribNames">The names of the attribute fields in the shader</param>
        public Shader(string name, string vertPath, string fragPath, ShaderAttribNames attribNames) 
        {
            _name = name;
            _vertPath = vertPath;
            _fragPath = fragPath;

            _attribNames = attribNames;

            LoadShaderGL();
        }

        /// <summary>
        /// Construct a shader from a vertex shader and a fragment shader
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="dualPath">The path to shader programs located in the same folder with the same name</param>
        /// <param name="attribNames">The names of the attribute fields in the shader</param>
        /// <param name="vertexExt">Optional file extension for the vertex shader file, defaults to .vert</param>
        /// <param name="fragmentExt">Optional file extension for the fragment shader file, defaults to .frag</param>
        public Shader(string name, string dualPath, ShaderAttribNames attribNames, string vertexExt = ".vert", string fragmentExt = ".frag")
        {
            _name = name;
            _vertPath = dualPath + vertexExt;
            _fragPath = dualPath + fragmentExt;

            _attribNames = attribNames;

            LoadShaderGL();
        }

        /// <summary>
        /// Handle loading and error checking the shader objects and program (OpenGL)
        /// </summary>
        protected void LoadShaderGL()
        {
            int vertexShader;
            int fragmentShader;
            
            string vertShaderSource;
            string fragShaderSource;

            try
            {
                //load the shader programs from their files
                vertShaderSource = File.ReadAllText(_vertPath);
                fragShaderSource = File.ReadAllText(_fragPath);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex.Message);
                Console.WriteLine(ex.Message);
                return;
            }

            //create the shader objects
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertShaderSource);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragShaderSource);


            //compile the vertex shader
            GL.CompileShader(vertexShader);

            //check for errors
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int checkV);
            if (checkV == 0)
            {
                string info = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine(info);
                Logger.WriteToLog(info);
            }

            //compile the fragment shader
            GL.CompileShader(fragmentShader);

            //check for errors
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int checkF);
            if (checkF == 0)
            {
                string info = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine(info);
                Logger.WriteToLog(info);
            }

            //create the shader program handle
            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);

            GL.LinkProgram(_handle);
            GL.ValidateProgram(_handle);

            //check for program errors
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int checkP);
            if (checkP == 0)
            {
                string info = GL.GetProgramInfoLog(_handle);
                Console.WriteLine("Shader program failed to link: {0}", info);
                Logger.WriteToLog("Shader program failed to link: {0}", info);
            }

            //cleanup
            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            //uniform caching
            GL.GetProgram(_handle, GetProgramParameterName.ActiveUniforms, out var uniformCount);
            for (int i = 0; i < uniformCount; i++) 
            {
                string key = GL.GetActiveUniform(_handle, i, out _, out _);
                int location = GL.GetUniformLocation(_handle, key);

                _uniformLocations.Add(key, location);
            }

            //form shader signature from attributes
            GL.GetProgram(_handle, GetProgramParameterName.ActiveAttributes, out var attributesCount);
            for (int i = 0; i < attributesCount; i++) 
            {
                string key = GL.GetActiveAttrib(_handle, i, out _, out ActiveAttribType type);
                int location = GL.GetAttribLocation(_handle, key);

                _attribList.Add(key, new Tuple<int, ActiveAttribType>(location, type));
            }
        }

        /// <summary>
        /// Provide access to the shader attribute locations (OpenGL)
        /// </summary>
        /// <param name="name">The name of the attribute to get the location of</param>
        /// <returns>The location integer of the attribute named or -1 if it failed</returns>
        public int GetAttribLocationGL(string name)
        {
            if (_attribList.TryGetValue(name, out var value))
            {
                return value.Item1;
            }

            return -1;
        }

        /// <summary>
        /// Activates the shader for use in the rendering pipeline (OpenGL)
        /// </summary>
        public void UseGL()
        {
            GL.UseProgram(_handle);
        }

        #region SetGivenAttribs
        /// <summary>
        /// If the shader has a position attribute defined set it using the given settings, and no normalization
        /// </summary>
        /// <param name="size">The size of the data</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of this attribute in the data</param>
        public void SetPositionAttribGL(int size, int stride, int offset)
        {
            SetPositionAttribGL(size, false, stride, offset);
        }

        /// <summary>
        /// If the shader has a position attribute defined set it using the given settings
        /// </summary>
        /// <param name="size">The size of the data</param>
        /// <param name="normalized">Flag to enable or disable normalization on data access</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of this attribute in the data</param>
        public void SetPositionAttribGL(int size, bool normalized, int stride, int offset)
        {
            if (PositionAttribName.Equals(string.Empty))
            {
                Console.WriteLine("Shader: {0} does not have a Position attribute name defined.", Name);
                Logger.WriteToLog("Shader: {0} does not have a Position attribute name defined.", Name);
                //todo: write to debug
                return;
            }

            SetAttribGL(PositionAttribName, size, normalized, stride, offset);
        }

        /// <summary>
        /// If the shader has a position attribute defined set it using the given settings
        /// </summary>
        /// <param name="settings">The settings for the attribute</param>
        public void SetPositionAttribGL(ShaderAttribSettings settings)
        {
            SetPositionAttribGL(settings.Size, settings.Stride, settings.Offset);
        }

        /// <summary>
        /// If the shader has a color attribute defined set it using the given settings
        /// </summary>
        /// <param name="size">The size of the data</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of this attribute in the data</param>
        public void SetColorAttribGL(int size, int stride, int offset)
        {
            SetColorAttribGL(size, false, stride, offset);
        }

        /// <summary>
        /// If the shader has a color attribute defined set it using the given settings
        /// </summary>
        /// <param name="size">The size of the data</param>
        /// <param name="normalized">Flag to enable or disable normalization on data access</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of this attribute in the data</param>
        public void SetColorAttribGL(int size, bool normalized, int stride, int offset)
        {
            if (ColorAttribName.Equals(string.Empty))
            {
                Console.WriteLine("Shader: {0} does not have a Color attribute name defined.", Name);
                Logger.WriteToLog("Shader: {0} does not have a Color attribute name defined.", Name);
                //todo: write to debug
                return;
            }

            SetAttribGL(ColorAttribName, size, normalized, stride, offset);
        }

        /// <summary>
        /// If the shader has a color attribute defined set it using the given settings
        /// </summary>
        /// <param name="settings">The settings for the attribute</param>
        public void SetColorAttribGL(ShaderAttribSettings settings)
        {
            SetColorAttribGL(settings.Size, settings.Stride, settings.Offset);
        }

        /// <summary>
        /// If the shader has a UV attribute defined set it using the given settings
        /// </summary>
        /// <param name="size">The size of the data</param>
        /// <param name="normalized">Flag to enable or disable normalization on data access</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of this attribute in the data</param>
        public void SetUVAttribGL(int size, bool normalized, int stride, int offset)
        {
            if (UVAttribName.Equals(string.Empty))
            {
                Console.WriteLine("Shader: {0} does not have a UV attribute name defined.", Name);
                Logger.WriteToLog("Shader: {0} does not have a UV attribute name defined.", Name);
                //todo: write to debug
                return;
            }

            SetAttribGL(UVAttribName, size, normalized, stride, offset);
        }

        /// <summary>
        /// If the shader has a UV attribute defined set it using the given settings
        /// </summary>
        /// <param name="size">The size of the data</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of this attribute in the data</param>
        public void SetUVAttribGL(int size, int stride, int offset)
        {
            SetUVAttribGL(size, false, stride, offset);
        }

        /// <summary>
        /// If the shader has a UV attribute defined set it using the given settings
        /// </summary>
        /// <param name="settings">The settings for the attribute</param>
        public void SetUVAttribGL(ShaderAttribSettings settings)
        {
            SetUVAttribGL(settings.Size, settings.Stride, settings.Offset);
        }
        #endregion

        #region SetAttribsGL
        /// <summary>
        /// Set the shader attributes for the provided vertex type
        /// </summary>
        /// <typeparam name="T">The type of vertex to set attributes for, IVertex implementations only</typeparam>
        public void SetAttribsGL<T>() where T : IVertex
        {
            if (typeof(T) == typeof(TexturedVertex))
            {
                SetPositionAttribGL(new ShaderAttribSettings { Size = 3, Stride = TexturedVertex.SizeInBytesU, Offset = 0 });
                SetUVAttribGL(new ShaderAttribSettings { Size = 2, Stride = TexturedVertex.SizeInBytesU, Offset = TexturedVertex.UVOffset });
            }
            else if (typeof(T) == typeof(ColorTexVertex))
            {
                SetPositionAttribGL(new ShaderAttribSettings { Size = 3, Stride = ColorTexVertex.SizeInBytesU, Offset = 0 });
                SetColorAttribGL(new ShaderAttribSettings { Size = 4, Stride = ColorTexVertex.SizeInBytesU, Offset = ColorTexVertex.ColorOffset });
                SetUVAttribGL(new ShaderAttribSettings { Size = 2, Stride = ColorTexVertex.SizeInBytesU, Offset = ColorTexVertex.UVOffset });
            }
            else if (typeof(T) == typeof(ColorVertex))
            {
                SetPositionAttribGL(new ShaderAttribSettings { Size = 3, Stride = ColorVertex.SizeInBytesU, Offset = 0 });
                SetColorAttribGL(new ShaderAttribSettings { Size = 4, Stride = ColorVertex.SizeInBytesU, Offset = ColorVertex.ColorOffset });
            }
            else if (typeof(T) == typeof(Vertex))
            {
                SetPositionAttribGL(new ShaderAttribSettings { Size = 3, Stride = Vertex.SizeInBytesU, Offset = 0 });
            }
            else
            {
                Console.WriteLine("SetAttribsGL<T> does not support vertex type of: {0}", typeof(T));
            }
        }

        //todo: parameter checking
        public void SetAttribsGL(ShaderAttribSettings[] settings)
        {
            foreach (var attrib in settings)
            {
                if (attrib.Size == 0)
                {
                    SetAttribGL(attrib.Name, attrib.Stride, attrib.Offset);
                }
                else
                {
                    SetAttribGL(attrib.Name, attrib.Size, attrib.Stride, attrib.Offset);
                }
            }
        }

        /// <summary>
        /// Set the shader attrib with the given settings
        /// </summary>
        /// <param name="settings">The collection of settings for the attrib</param>
        public void SetAttribGL(ShaderAttribSettings settings)
        {
            SetAttribGL(settings.Name, settings.Size, settings.Stride, settings.Offset);
        }

        /// <summary>
        /// Set the shader attrib with the given settings
        /// </summary>
        /// <param name="name">The name of the attribute</param>
        /// <param name="stride">The stride of the data</param>
        /// <param name="offset">The offset of this attribute in the data</param>
        public void SetAttribGL(string name, int stride, int offset)
        {
            if (_attribList.TryGetValue(name, out var value))
            {
                VertexAttribPointerType type = FromActiveAttribType(value.Item2, out int size);
                GL.VertexAttribPointer(value.Item1, size, type, false, stride, offset);
                GL.EnableVertexAttribArray(value.Item1);

                return;
            }

            Console.WriteLine(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            Logger.WriteToLog(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            //todo: write to debug
        }

        /// <summary>
        /// Set the shader attrib with the given name, and no normalization on access
        /// </summary>
        /// <param name="name">The name of the attrib to set</param>
        /// <param name="size">The size of the data to set it for</param>
        /// <param name="stride">The stride of the data to set it for</param>
        /// <param name="offset">The offset of this attrib in the data</param>
        public void SetAttribGL(string name, int size, int stride, int offset)
        {
            SetAttribGL(name, size, false, stride, offset);
        }

        /// <summary>
        /// Set the shader attrib with the given name
        /// </summary>
        /// <param name="name">The name of the attrib to set</param>
        /// <param name="size">The size of the data to set it for</param>
        /// <param name="normalized">Flag to enable or disable normalization on data access</param>
        /// <param name="stride">The stride of the data to set it for</param>
        /// <param name="offset">The offset of this attrib in the data</param>
        public void SetAttribGL(string name, int size, bool normalized, int stride, int offset)
        {
            if (_attribList.TryGetValue(name, out var value))
            {
                GL.VertexAttribPointer(value.Item1, size, FromActiveAttribType(value.Item2, out int _), normalized, stride, offset);
                GL.EnableVertexAttribArray(value.Item1);

                return;
            }

            Console.WriteLine(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            Logger.WriteToLog(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            //todo: write to debug
        }

        /// <summary>
        /// Set the shader attrib with the given name
        /// </summary>
        /// <param name="name">The name of the attrib to set</param>
        /// <param name="normalized">Flag to enable or disable normalization on data access</param>
        /// <param name="stride">The stride of the data to set it for</param>
        /// <param name="offset">The offset of this attrib in the data</param>
        public void SetAttribGL(string name, bool normalized, int stride, int offset)
        {
            if (_attribList.TryGetValue(name, out var value))
            {
                VertexAttribPointerType type = FromActiveAttribType(value.Item2, out int size);
                GL.VertexAttribPointer(value.Item1, size, type, normalized, stride, offset);
                GL.EnableVertexAttribArray(value.Item1);

                return;
            }

            Console.WriteLine(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            Logger.WriteToLog(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            //todo: write to debug
        }

        /// <summary>
        /// Set the shader attrib with the given name, and no normalization on access
        /// </summary>
        /// <param name="name">The name of the attrib to set</param>
        /// <param name="size">The size of the data to set it for</param>
        /// <param name="type">The attrib pointer type</param>
        /// <param name="stride">The stride of the data to set it for</param>
        /// <param name="offset">The offset of this attrib in the data</param>
        public void SetAttribGL(string name, int size, VertexAttribPointerType type, int stride, int offset)
        {
            SetAttribGL(name, size, type, false, stride, offset);
        }

        /// <summary>
        /// Set the shader attrib with the given name
        /// </summary>
        /// <param name="name">The name of the attrib to set</param>
        /// <param name="size">The size of the data to set it for</param>
        /// <param name="type">The attrib pointer type</param>
        /// <param name="normalized">Flag to enable or disable normalization on data access</param>
        /// <param name="stride">The stride of the data to set it for</param>
        /// <param name="offset">The offset of this attrib in the data</param>
        public void SetAttribGL(string name, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            if (_attribList.TryGetValue(name, out var value))
            {
                GL.VertexAttribPointer(value.Item1, size, type, normalized, stride, offset);
                GL.EnableVertexAttribArray(value.Item1);

                return;
            }

            Console.WriteLine(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            Logger.WriteToLog(string.Format("Failed to acquire attribute location named: {0} in shader: {1}", name, _name));
            //todo: write to debug
        }
        #endregion

        #region SetUniformsGL
        //todo: more uniform types
        /// <summary>
        /// Set a uniform in the shader program to the given integer value (OpenGL)
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="value">The value to set it to</param>
        public void SetUniformIntGL(string name, int value)
        {
            if (_uniformLocations.TryGetValue(name, out int loc))
            {
                GL.UseProgram(Handle);
                GL.Uniform1(loc, value);
            }
            else
            {
                Logger.WriteToLog("No uniform named: {0} found in shader: {1}", name, _name);
                Console.WriteLine("No uniform named: {0} found in shader: {1}", name, _name);
                //write debug
            }
        }

        public void SetUniformVec3GL(string name, Vec3 value)
        {
            if (_uniformLocations.TryGetValue(name, out int loc))
            {
                GL.UseProgram(Handle);
                GL.Uniform3(loc, value);
            }
            else
            {
                Logger.WriteToLog("No uniform named: {0} found in shader: {1}", name, _name);
                Console.WriteLine("No uniform named: {0} found in shader: {1}", name, _name);
                //write debug
            }
        }

        public void SetUniformVec4GL(string name, Vec4 value)
        {
            if (_uniformLocations.TryGetValue(name, out int loc))
            {
                GL.UseProgram(Handle);
                GL.Uniform4(loc, value);
            }
            else
            {
                Logger.WriteToLog("No uniform named: {0} found in shader: {1}", name, _name);
                Console.WriteLine("No uniform named: {0} found in shader: {1}", name, _name);
                //write debug
            }
        }

        /// <summary>
        /// Set a uniform in the shader program to the given Matrix4 (OpenGL)
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="value">The matrix4 to set it to</param>
        public void SetUniformMat4GL(string name, Matrix4 value)
        {
            OpenTK.Mathematics.Matrix4 v2 = value;//convert to openTK format
            if (_uniformLocations.TryGetValue(name, out int loc))
            {
                GL.UseProgram(Handle);
                GL.UniformMatrix4(loc, true, ref v2);
            }
            else
            {
                Logger.WriteToLog("No uniform named: {0} found in shader: {1}", name, _name);
                Console.WriteLine("No uniform named: {0} found in shader: {1}", name, _name);
                //write debug
            }
        }
        #endregion

        #region ProgramUniformsGL
        //todo: more uniform types
        public void ProgramUniformVec4GL(string name, Vec4 value)
        {
            if (_uniformLocations.TryGetValue(name, out int loc))
            {
                GL.ProgramUniform4(Handle, loc, value);
            }
        }
        #endregion

        /// <summary>
        /// Simple mapping of ActiveAttribType to VertexAttribPointerType
        /// </summary>
        /// <param name="at"></param>
        /// <param name="size">The number of values of the returned type in the attribute, returns 0 if no conversion was found,
        /// cannot be more than 4</param>
        /// <returns></returns>
        private static VertexAttribPointerType FromActiveAttribType(ActiveAttribType at, out int size)
        {
            //todo: return a size based on input
            switch (at)
            {
                case ActiveAttribType.UnsignedIntVec2:
                    size = 2;
                    return VertexAttribPointerType.UnsignedInt;
                case ActiveAttribType.UnsignedIntVec3:
                    size = 3;
                    return VertexAttribPointerType.UnsignedInt;
                case ActiveAttribType.UnsignedIntVec4:
                    size = 4;
                    return VertexAttribPointerType.UnsignedInt;
                case ActiveAttribType.UnsignedInt:
                    size = 1;
                    return VertexAttribPointerType.UnsignedInt;

                case ActiveAttribType.FloatVec2:
                    size = 2;
                    return VertexAttribPointerType.Float;
                case ActiveAttribType.FloatVec3:
                    size = 3;
                    return VertexAttribPointerType.Float;
                case ActiveAttribType.FloatVec4:
                    size = 4;
                    return VertexAttribPointerType.Float;
                case ActiveAttribType.Float:
                    size = 1;
                    return VertexAttribPointerType.Float;

                case ActiveAttribType.DoubleMat3:
                    size = 3;
                    return VertexAttribPointerType.Double;
                case ActiveAttribType.DoubleMat4:
                    size = 4;
                    return VertexAttribPointerType.Double;
                case ActiveAttribType.DoubleVec2:
                    size = 2;
                    return VertexAttribPointerType.Double;
                case ActiveAttribType.DoubleVec3:
                    size = 3;
                    return VertexAttribPointerType.Double;
                case ActiveAttribType.DoubleMat2:
                case ActiveAttribType.DoubleVec4:
                    size = 4;
                    return VertexAttribPointerType.Double;
                case ActiveAttribType.Double:
                    size = 1;
                    return VertexAttribPointerType.Double;

                case ActiveAttribType.IntVec2:
                    size = 2;
                    return VertexAttribPointerType.Int;
                case ActiveAttribType.IntVec3:
                    size = 3;
                    return VertexAttribPointerType.Int;
                case ActiveAttribType.IntVec4:
                    size = 4;
                    return VertexAttribPointerType.Int;
                case ActiveAttribType.Int:
                    size = 1;
                    return VertexAttribPointerType.Int;

                case ActiveAttribType.FloatMat2:
                case ActiveAttribType.FloatMat3:
                case ActiveAttribType.FloatMat4:
                case ActiveAttribType.FloatMat2x3:
                case ActiveAttribType.FloatMat2x4:
                case ActiveAttribType.FloatMat3x2:
                case ActiveAttribType.FloatMat3x4:
                case ActiveAttribType.FloatMat4x2:
                case ActiveAttribType.FloatMat4x3:
                case ActiveAttribType.DoubleMat2x3:
                case ActiveAttribType.DoubleMat2x4:
                case ActiveAttribType.DoubleMat3x2:
                case ActiveAttribType.DoubleMat3x4:
                case ActiveAttribType.DoubleMat4x2:
                case ActiveAttribType.DoubleMat4x3:
                default:
                    size = 0;
                    return VertexAttribPointerType.Float;
            }
        }

        #region Dispose
        /// <summary>
        /// Disposes of the shader program and marks the shader as disposed (OpenGL)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void DisposeGL(bool disposing)
        {
            if (!_disposed)
            {
                GL.DeleteProgram(_handle);
                _disposed = true;
            }
        }

        /// <summary>
        /// Public access to dispose of the shader
        /// </summary>
        public void Dispose()
        {
            DisposeGL(true);
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            if (_disposed == false)
            {
                Console.WriteLine("Shader named: " + _name + ", not properly disposed of.");
                Logger.WriteToLog("Shader named: " + _name + ", not properly disposed of.");
                //todo: write to debug
            }
        }
        #endregion
    }

    /// <summary>
    /// Container struct for shader attribute settings
    /// </summary>
    public struct ShaderAttribSettings
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Stride { get; set; }
        public int Offset { get; set; }
    }

    /// <summary>
    /// Storage struct for the names of common shader attribute fields. Each field defaults to empty string
    /// and should only be assigned to if the shader makes use of that field type
    /// </summary>
    public struct ShaderAttribNames
    {
        public string Position { get; set; }
        public string Color { get; set; }
        public string UV { get; set; }
        public string Normal { get; set; }

        public ShaderAttribNames()
        {
            Position = "";
            Color = "";
            UV = "";
            Normal = "";
        }
    }
}
