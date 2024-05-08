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
        protected string _name = string.Empty;

        protected List<SceneObject> _objects = new List<SceneObject>();

        public string Name { get { return _name; } set { _name = value; } }
        //todo: fill stub

        public Scene(string file)
        {
            //todo: implement
        }

        public Scene(XDocument sceneXml)
        {
            //todo: implement
        }

        public Scene(string name, string file)
        {
            _name = name;
            //todo: implement
        }

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
