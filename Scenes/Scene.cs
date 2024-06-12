using KirosEngine3.Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Scenes
{
    /// <summary>
    /// Defines a group of objects, resources, and data to be used in the rendering of a scene
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// The name of the scene.
        /// </summary>
        protected string _name = string.Empty;

        /// <summary>
        /// The collection of objects in the scene.
        /// </summary>
        protected List<SceneObject> _objects = new List<SceneObject>();

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        //todo: fill stub

        /// <summary>
        /// Basic constructor for loading from a scene file.
        /// </summary>
        /// <param name="file">The file path for the scene.</param>
        public Scene(string file)
        {
            //todo: implement
        }

        /// <summary>
        /// Basic constructor for loading from a XML Doc.
        /// </summary>
        /// <param name="sceneXml">The XML document.</param>
        public Scene(XDocument sceneXml)
        {
            //todo: implement
        }

        /// <summary>
        /// Constructor with the scene name.
        /// </summary>
        /// <param name="name">The name of the scene.</param>
        /// <param name="file">The file path for the scene.</param>
        public Scene(string name, string file)
        {
            _name = name;
            //todo: implement
        }

        /// <summary>
        /// Constructor with the scene name.
        /// </summary>
        /// <param name="name">The name of the scene.</param>
        /// <param name="sceneXml">The XML document.</param>
        public Scene(string name, XDocument sceneXml)
        {
            _name = name;
            //todo: implement
        }

        /// <summary>
        /// Called to unload a scene's resources
        /// </summary>
        public void Unload()
        {
            //todo: implement
            throw new NotImplementedException();
        }
    }
}
