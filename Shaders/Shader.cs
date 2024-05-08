using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Shaders
{
    public class Shader : IDisposable
    {
        protected int _handle;
        protected bool _disposed;
        protected string _name;

        protected string _vertPath;
        protected string _fragPath;

        protected readonly Dictionary<string, int> _uniformLocations = [];
        protected ShaderSignature _signature;

        public int Handle
        { get { return _handle; } }

        public string Name
        { get { return _name; } }

        //todo: attrib pointers for position, color, texture, ect.

        /// <summary>
        /// Construct a shader from a vertex shader and a fragment shader
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="vertPath">The path to the vertex shader file</param>
        /// <param name="fragPath">The path to the fragment shader file</param>
        public Shader(string name, string vertPath, string fragPath)
        {
            _name = name;
            _vertPath = vertPath;
            _fragPath = fragPath;

            LoadShaderGL();
        }

        /// <summary>
        /// Constructor for co-located shader programs with identical names
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="dualPath">The path to shader programs located in the same folder with the same name</param>
        /// <param name="vertexExt">Optional file extension for the vertex shader file, defaults to .vert</param>
        /// <param name="fragmentExt">Optional file extension for the fragment shader file, defaults to .frag</param>
        public Shader(string name, string dualPath, string vertexExt = ".vert", string fragmentExt = ".frag")
        {
            _name = name;
            _vertPath = dualPath + vertexExt;
            _fragPath = dualPath + fragmentExt;

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
            _signature.Attributes = new Tuple<int, string, ActiveAttribType>[attributesCount];
            for (int i = 0; i < attributesCount; i++)
            {
                string key = GL.GetActiveAttrib(_handle, i, out _, out ActiveAttribType type);
                int location = GL.GetAttribLocation(_handle, key);

                _signature.Attributes[i] = new Tuple<int, string, ActiveAttribType>(location, key, type);
            }
        }

        //todo: get signature attribute for ActiveAttribType

        //todo: get signature attribute for index

        //todo: get signature attribute for name

        /// <summary>
        /// Provide access to the shader attribute locations (OpenGL)
        /// </summary>
        /// <param name="name">The name of the attribute to get the location of</param>
        /// <returns>The location integer of the attribute named</returns>
        public int GetAttribLocationGL(string name)
        {
            return _signature.Attributes.Where(attrib => attrib.Item2 == name)
                .Select(attrib => attrib.Item1).First();
        }

        /// <summary>
        /// Activates the shader for use in the rendering pipeline (OpenGL)
        /// </summary>
        public void UseGL()
        {
            GL.UseProgram(_handle);
        }

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
    }

    public struct ShaderSignature
    {
        public Tuple<int, string, ActiveAttribType>[] Attributes;
    }
}
