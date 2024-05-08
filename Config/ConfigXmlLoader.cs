using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Config
{
    /// <summary>
    /// Abstract definition of a Configuration loader. Implemented by projects making use of the 
    /// engine to load their custom config files.
    /// </summary>
    public abstract class ConfigXmlLoader
    {
        private string _configFile;

        public ConfigXmlLoader(string configFile) 
        {
            _configFile = configFile;
        }

        /// <summary>
        /// Process the xml file and store data in config vars
        /// </summary>
        /// <returns>True if successful false otherwise</returns>
        public abstract bool LoadFromXml();
    }
}
