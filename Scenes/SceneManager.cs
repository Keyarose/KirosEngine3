using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KirosEngine3.Exceptions;

namespace KirosEngine3.Scenes
{
    /// <summary>
    /// Manages all scenes defined for a program in a single easy to access place
    /// </summary>
    public sealed class SceneManager
    {
        private static SceneManager? _instance;
        
        private readonly Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private SceneManager() { }

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static SceneManager Instance
        { get { return _instance ??= new SceneManager(); } }

        /// <summary>
        /// Add the given scene to the manager
        /// </summary>
        /// <param name="scene">The scene to be added to the manager</param>
        /// <exception cref="ArgumentException">Thrown if the name for the scene is already in use</exception>
        public static void AddScene(Scene scene)
        {
            if (!Instance._scenes.TryAdd(scene.Name, scene))
            {
                throw new ArgumentException(string.Format("Scene name: {0} is already in use.", scene.Name));
            }
        }

        /// <summary>
        /// Add a scene to the manager from an xml doc that defines the scene
        /// </summary>
        /// <param name="name">The name of the scene</param>
        /// <param name="scene">The xml document containing the scene data</param>
        /// <exception cref="ArgumentException">Thrown if the name for the scene is already in use</exception>
        public static void AddScene(string name, XDocument scene)
        {
            if (!Instance._scenes.TryAdd(name, new Scene(name, scene)))
            {
                throw new ArgumentException(string.Format("Scene name: {0} is already in use.", name));
            }
        }

        /// <summary>
        /// Add a scene to the manager from a file that defines the scene
        /// </summary>
        /// <param name="name">The name of the scene</param>
        /// <param name="file">The file containing the scene data</param>
        /// <exception cref="ArgumentException">Thrown if the name for the scene is already in use</exception>
        public static void AddScene(string name, string file)
        {
            if (!Instance._scenes.TryAdd(name, new Scene(name, file)))
            {
                throw new ArgumentException(string.Format("Scene name: {0} is already in use.", name));
            }
        }

        /// <summary>
        /// Add the given scene to the manager
        /// </summary>
        /// <param name="scene">The scene to be added to the manager</param>
        /// <returns>True if the scene is successfully added to the manager, false otherwise</returns>
        public static bool TryAddScene(Scene scene)
        {
            if (Instance._scenes.TryAdd(scene.Name, scene))
            {
                return true;
            }

            Logger.Instance.WriteToLog(string.Format("Scene name: {0} is already in use.", scene.Name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Add a scene to the manager from an xml doc that defines the scene
        /// </summary>
        /// <param name="name">The name of the scene to be added</param>
        /// <param name="scene">The xml doc containing the scene data</param>
        /// <returns>True if the scene is successfully added to the manager, false otherwise</returns>
        public static bool TryAddScene(string name, XDocument scene)
        {
            if (Instance._scenes.TryAdd(name, new Scene(name, scene)))
            {
                return true;
            }

            Logger.Instance.WriteToLog(string.Format("Scene name: {0} is already in use.", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Add a scene to the manager from a file that defines the scene
        /// </summary>
        /// <param name="name">The name of the scene to be added</param>
        /// <param name="file">The file containing the scene data</param>
        /// <returns>True if the scene is successfully added to the manager, false otherwise</returns>
        public static bool TryAddScene(string name, string file)
        {
            if (Instance._scenes.TryAdd(name, new Scene(file)))
            {
                return true;
            }

            Logger.Instance.WriteToLog(string.Format("Scene name: {0} is already in use.", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Try to remove the scene from the manager using the given name
        /// </summary>
        /// <param name="name">The name of the scene to be removed</param>
        /// <returns>True if the scene is removed, false if no scene is found for the name</returns>
        public static bool TryRemoveScene(string name)
        {
            //if the scene exists clean up it's resources before removing it
            if (Instance._scenes.TryGetValue(name, out var scene))
            {
                scene.Unload();
                return Instance._scenes.Remove(name);
            }

            return false;
        }

        /// <summary>
        /// Try to get the scene for the given name
        /// </summary>
        /// <param name="name">The name of the scene to get</param>
        /// <param name="scene">The scene for the name if it exists</param>
        /// <returns>True if the scene is found, false otherwise</returns>
        public static bool TryGetScene(string name, out Scene? scene)
        {
            if (Instance._scenes.TryGetValue(name, out var sce))
            {
                scene = sce;
                return true;
            }

            scene = null;
            return false;
        }

        /// <summary>
        /// Clean up the scenes before unloading the program
        /// </summary>
        /// <exception cref="CollectionNotEmptyException">Thrown when the shader dictionary is not properly emptied</exception>
        public static void OnUnload()
        {
            foreach (var key in Instance._scenes.Keys)
            {
                Instance._scenes[key].Unload();
            }
            Instance._scenes.Clear();

            if (Instance._scenes.Count > 0) 
            {
                throw new CollectionNotEmptyException("The Scene Manager collection is not empty after running OnUnload, something is wrong.");
            }
        }
    }
}
