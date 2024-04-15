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

        public int Handle
        { get { return _handle; } }

        public string Name
        { get { return _name; } }

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
            //todo: cache all uniform see opentk shader
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
        }

        //todo: setup and attrib handling making use of GL.GetActiveAttrib

        /// <summary>
        /// Provide access to the shader attribute locations (OpenGL)
        /// </summary>
        /// <param name="name">The name of the attribute to get the location of</param>
        /// <returns>The location integer of the attribute named</returns>
        public int GetAttribLocationGL(string name)
        {
            return GL.GetAttribLocation(_handle, name);
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
            int location = GL.GetUniformLocation(Handle, name);
            GL.UseProgram(Handle);
            GL.Uniform1(location, value);
        }

        public void SetUniformVec3GL(string name, Vec3 value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UseProgram(Handle);
            GL.Uniform3(location, value);
        }

        public void SetUniformVec4GL(string name, Vec4 value)
        {
            int loc = GL.GetUniformLocation(Handle, name);
            GL.UseProgram(Handle);
            GL.Uniform4(loc, value);
        }

        /// <summary>
        /// Set a uniform in the shader program to the given Matrix4 (OpenGL)
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="value">The matrix4 to set it to</param>
        public void SetUniformMat4GL(string name, Matrix4 value)
        {
            OpenTK.Mathematics.Matrix4 v2 = value;//convert to openTK format
            int loc = GL.GetUniformLocation(Handle, name);
            GL.UseProgram(Handle);
            GL.UniformMatrix4(loc, true, ref v2);
        }
        #endregion

        #region ProgramUniformsGL
        //todo: more uniform types
        public void ProgramUniformVec4GL(string name, Vec4 value)
        {
            int loc = GL.GetUniformLocation(Handle, name);
            GL.ProgramUniform4(Handle, loc, value);
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
            }
        }
    }
}
