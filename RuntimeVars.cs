using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3
{
    /// <summary>
    /// Define a collection for application wide accessible variables
    /// </summary>
    internal class RuntimeVars
    {
        private static RuntimeVars? _instance;

        private Dictionary<string, object> _vars = new Dictionary<string, object>();

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private RuntimeVars() { }

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static RuntimeVars Instance
        { get { return _instance ??= new RuntimeVars(); } }

        /// <summary>
        /// Accessor for the variables collection
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The variable's value</returns>
        public object this[string name]
        {
            get
            {
                return _vars[name];
            }

            set
            {
                _vars[name] = value;
            }
        }

        /// <summary>
        /// Add a variable for app wide access
        /// </summary>
        /// <param name="name">The name for the variable</param>
        /// <param name="value">The value to be stored in the variable</param>
        /// <exception cref="ArgumentException">Thrown if the name is already in use</exception>
        public static void AddVar(string name, object value)
        {
            if (!Instance._vars.TryAdd(name, value)) 
            {
                throw new ArgumentException(string.Format("Variable already registered for the name: {0}", name));
            }
        }

        /// <summary>
        /// Try to add a variable for app wide access
        /// </summary>
        /// <param name="name">The name for the variable</param>
        /// <param name="value">The value to be store in the variable</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryAddVar(string name, object value) 
        {
            if (Instance._vars.TryAdd(name, value))
            {
                return true;
            }

            Logger.Instance.WriteToLog(string.Format("Variable already registered for the name: {0}", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Try to remove a variable from app wide access
        /// </summary>
        /// <param name="name">The name of the variable to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryRemoveVar(string name) 
        {
            return Instance._vars.Remove(name);
        }

        /// <summary>
        /// Get the variable for the given name
        /// </summary>
        /// <param name="name">The name of the variable to get</param>
        /// <returns>The value of the variable</returns>
        public static object? GetVar(string name) 
        {
            if (Instance._vars.TryGetValue(name, out var obj))
            {
                return obj;
            }
            return null;
        }
        
        /// <summary>
        /// Try to get a variable using the given name
        /// </summary>
        /// <param name="name">The name of the variable to get</param>
        /// <param name="value">The value returned</param>
        /// <returns>True if the variable is found, false otherwise</returns>
        public static bool TryGetVar(string name, out object? value) 
        {
            if (Instance._vars.TryGetValue(name, out value))
            {
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try to set the variable for the given name
        /// </summary>
        /// <param name="name">The name to set the variable as</param>
        /// <param name="value">The value of the variable</param>
        public static void SetVar(string name, object value)
        {
            if (Instance._vars.ContainsKey(name))
            {
                Instance._vars[name] = value;
            }
            else
            {
                AddVar(name, value);
            }
        }
    }
}
