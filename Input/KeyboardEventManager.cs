using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Input
{
    public class KeyboardEventManager
    {
        private static KeyboardEventManager? _instance;

        private static KeyboardEventManager Instance
        { get { return _instance ??= new KeyboardEventManager(); } }

        /// <summary>
        /// Method signature for keyboard event handlers
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The keyboard event arguments</param>
        public delegate void KeyboardEventHandler(object sender, KeyboardEventArgs e);
        private Dictionary<string, Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>> _eventRegistry = [];

        /// <summary>
        /// Subscribe to the keyboard event manager in the given context for the given key and event type
        /// </summary>
        /// <param name="context">The program context in which the subscriber wants to receive events</param>
        /// <param name="key">The key to receive events for</param>
        /// <param name="type">The type of event</param>
        /// <param name="callback">The delegate to handle the event</param>
        /// <returns>True if successful</returns>
        /// <exception cref="ArgumentException">Thrown if context is an empty string</exception>
        public static bool SubscribeKeyboardEvent(string context, Keys key, KeyboardEventType type, KeyboardEventHandler callback)
        {
            //empty string is not allowed as a context
            if (context != string.Empty) 
            {
                Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler> contextList = [];

                try
                {
                    //get the collection of handlers for the given context
                    contextList = Instance._eventRegistry[context];
                }
                catch (KeyNotFoundException)
                {
                    Logger.WriteToLog("Context key: {0} doesn't exist in KeyboardEventManager, adding it.", context);
                    Console.WriteLine("Context key: {0} doesn't exist in KeyboardEventManager, adding it.", context);
                    //todo: write to debug

                    //create the collection for the context
                    Instance._eventRegistry[context] = contextList;
                }
                finally
                {
                    //the composite key that references the appropriate handler
                    Tuple<Keys, KeyboardEventType> keyMode = Tuple.Create(key, type);

                    if (contextList.TryGetValue(keyMode, out KeyboardEventHandler? list))
                    {
                        //unpack the handler, add the new one to it, and put it away
                        list += new KeyboardEventHandler(callback);
                        contextList[keyMode] = list;
                    }
                    else
                    {
                        //no handler exists so create a new one
                        contextList.Add(keyMode, new KeyboardEventHandler(callback));
                    }
                }
                return true;
            }

            throw new ArgumentException("The context string for a keyboard event cannot be empty.", nameof(context));
        }

        ///<inheritdoc cref="SubscribeKeyboardEvent(string, Keys, KeyboardEventType, KeyboardEventHandler)"/>
        public static bool SubscribeKeyboardEvent(string context, Tuple<Keys, KeyboardEventType> keyMode, KeyboardEventHandler callback)
        {
            return SubscribeKeyboardEvent(context, keyMode.Item1, keyMode.Item2, callback);
        }

        /// <summary>
        /// Unsubscribe from the keyboard event manager in the given context for the given key mode
        /// </summary>
        /// <param name="context">The program context in which the subscriber no longer wants to receive events</param>
        /// <param name="key">The key to remove events for</param>
        /// <param name="type">The type of event</param>
        /// <param name="callback">The delegate to remove from the event</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool UnSubscribeKeyboardEvent(string context, Keys key, KeyboardEventType type, KeyboardEventHandler callback)
        {
            if (Instance._eventRegistry.TryGetValue(context, out Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>? cList))
            {
                Tuple<Keys, KeyboardEventType> keyMode = Tuple.Create(key, type);
                if (cList.TryGetValue(keyMode, out KeyboardEventHandler? keh))
                {
                    keh -= new KeyboardEventHandler(callback);
                    cList[keyMode] = keh!;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// List the delegate count registered for the given context
        /// </summary>
        /// <param name="context">The context to list the delegates for</param>
        /// <returns>The list of delegates as a string</returns>
        public static string ListDelegates(string context)
        {
            string result = string.Format("No delegates for context: {0}", context);

            if (Instance._eventRegistry.TryGetValue(context, out Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>? contextList))
            {
                result = "";
                foreach (var keh in contextList) 
                {
                    result += string.Format("Key: {1} EventType: {2} Count: {0} \n",
                        keh.Value.GetInvocationList().Length, keh.Key.Item1.ToString(), keh.Key.Item2.ToString());
                    result += "\n";
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Event arguments for keyboard events
    /// </summary>
    public class KeyboardEventArgs : EventArgs
    {
        public Keys Key { get; private set; }
        public KeyboardEventType Type { get; private set; }
    }

    /// <summary>
    /// The type of keyboard event, was the key pressed, held, or released
    /// </summary>
    public enum KeyboardEventType
    {
        KeyPressed,
        KeyReleased,
        KeyHeld
    }
}
