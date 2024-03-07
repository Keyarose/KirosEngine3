using KirosEngine3.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Shaders
{
    /// <summary>
    /// Organizes and provides access to shaders while preventing duplicates
    /// </summary>
    public sealed class ShaderManager
    {
        private static ShaderManager? _instance;
        
        private readonly Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private ShaderManager() { }

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static ShaderManager Instance
        {
            get { return _instance ??= new ShaderManager(); }
        }

        /// <summary>
        /// Add a shader to the manager
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="vertPath">The path for the vertex shader</param>
        /// <param name="fragPath">The path for the fragment shader</param>
        /// <exception cref="ArgumentException">Thrown if the name for the shader is already in use</exception>
        public static void AddShader(string name, string vertPath, string fragPath)
        {
            if (!Instance._shaders.TryAdd(name, new Shader(name, vertPath, fragPath)))
            {
                throw new ArgumentException(string.Format("Shader name: {0} is already in use.", name), name);
            }
        }

        /// <summary>
        /// Add a shader to the manager
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="shader">The shader to be added</param>
        /// <exception cref="ArgumentException">Thrown if the name for the shader is already in use</exception>
        public static void AddShader(string name, Shader shader) 
        {
            if (!Instance._shaders.TryAdd(name, shader))
            {
                throw new ArgumentException(string.Format("Shader name: {0} is already in use.", name), name);
            }
        }

        /// <summary>
        /// Try to add a shader to the manager without throwing an exception on failure
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="vertPath">The path for the vertex shader</param>
        /// <param name="fragPath">The path for the fragment shader</param>
        /// <returns>True if the shader is successfully added, false if the name is already in use</returns>
        public static bool TryAddShader(string name, string vertPath, string fragPath)
        {
            if (Instance._shaders.TryAdd(name, new Shader(name, vertPath, fragPath)))
            { return true; }

            Logger.Instance.WriteToLog(string.Format("Shader name: {0} is already in use.", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Try to add a shader to the manager without throwing an exception on failure
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="shader">The shader to be added</param>
        /// <returns>True if the shader is successfully added, false if the name is already in use</returns>
        public static bool TryAddShader(string name, Shader shader)
        {
            if (Instance._shaders.TryAdd(name, shader))
            { return true; }

            Logger.Instance.WriteToLog(string.Format("Shader name: {0} is already in use.", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Try to remove the shader from the manager using the given name
        /// </summary>
        /// <param name="name">The name of the shader to be removed</param>
        /// <returns>True if the shader is removed, false if no shader is found for the name</returns>
        public static bool TryRemoveShader(string name)
        {
            //if the shader exists dispose of it before removing
            if (Instance._shaders.TryGetValue(name, out var shader))
            {
                shader.Dispose();
                return Instance._shaders.Remove(name);
            }

            return false;
        }

        /// <summary>
        /// Try to get the shader for the given name
        /// </summary>
        /// <param name="name">The name of the shader to get</param>
        /// <param name="shader">The Shader for the name if it exists</param>
        /// <returns>True if the shader is found, false otherwise</returns>
        public static bool TryGetShader(string name, out Shader? shader)
        {
            if (Instance._shaders.TryGetValue(name, out var sha))
            {
                shader = sha;
                return true;
            }

            shader = null;
            return false;
        }

        /// <summary>
        /// Try to get the handle for the given shader by name
        /// </summary>
        /// <param name="name">The name of the shader to get the handle of</param>
        /// <returns>The handle of the shader for the given name or -1 if not found</returns>
        public static int TryGetShaderHandle(string name)
        {
            if (Instance._shaders.TryGetValue(name, out var shader))
            {
                return shader.Handle;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Try to set the shader to be used in the rendering pipeline
        /// </summary>
        /// <param name="name">The name of the shader to use</param>
        /// <returns>True if the shader is successfully set for use, false otherwise</returns>
        public static bool TryUseShader(string name)
        {
            if (Instance._shaders.TryGetValue(name, out var shader))
            {
                if (RuntimeVars.Instance[Client.GRAPHICSMODE_KEY] is string gm && gm.Equals(Client.GRAPHICSMODE_GL_VAL))
                {
                    shader.UseGL();
                }
                else
                {
                    //todo: replace with DX equivalent
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clean up the shaders before unloading the program
        /// </summary>
        /// <exception cref="CollectionNotEmptyException">Thrown when the shader dictionary is not properly emptied</exception>
        public static void OnUnload()
        {
            //dispose of all shaders then clear the dictionary
            foreach (var key in Instance._shaders.Keys)
            {
                Instance._shaders[key].Dispose();
            }
            Instance._shaders.Clear();

            if (Instance._shaders.Count > 0)
            {
                throw new CollectionNotEmptyException("The Shader Manager collection is not empty after running OnUnload, something is wrong.");
            }
        }
    }
}
