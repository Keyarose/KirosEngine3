using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Input
{
    /// <summary>
    /// Manages keyboard input and notifies subscribed objects of input events.
    /// Intended to be tied to the frame update cycle of the client.
    /// </summary>
    public class KeyboardEventManager
    {
        private static KeyboardEventManager? _instance;

        private static KeyboardEventManager Instance
        { get { return _instance ??= new KeyboardEventManager(); } }

        private string _context = "";
        private static KeyboardState? _lastState;
        private static bool _capsLockState = false;
        private static bool _numLockState = false;

        public static string CurrentContext
        { get { return Instance._context; } set { Instance._context = value; } }

        public const string GLOBAL_CONTEXT = "global";

        /// <summary>
        /// Method signature for keyboard event handlers
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The keyboard event arguments</param>
        public delegate void KeyboardEventHandler(object sender, KeyboardEventArgs e);
        private Dictionary<string, Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>> _eventRegistry = [];

        #region Add/Remove
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

        /// <inheritdoc cref="SubscribeKeyboardEvent(string, Keys, KeyboardEventType, KeyboardEventHandler)"/>
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

        /// <inheritdoc cref="UnSubscribeKeyboardEvent(string, Keys, KeyboardEventType, KeyboardEventHandler)"/>
        public static bool UnSubscribeKeyboardEvent(string context, Tuple<Keys, KeyboardEventType> keyMode, KeyboardEventHandler callback)
        {
            return UnSubscribeKeyboardEvent(context, keyMode.Item1, keyMode.Item2, callback);
        }
        #endregion

        /// <summary>
        /// On each frame update check the keyboard state and send callbacks to all registered watchers
        /// who's events have happened.
        /// Intended to be called from Client.OnUpdateFrame
        /// </summary>
        /// <param name="keyState">The current keyboard state</param>
        /// <exception cref="InvalidOperationException">Thrown when the KeyboardEventType for an unpacked
        /// event handler is an invalid value</exception>
        public static void Update(KeyboardState keyState)
        {
            //check special key states like caps-lock
            if (keyState.IsKeyPressed(Keys.CapsLock)) { _capsLockState = !_capsLockState; }
            if (keyState.IsKeyPressed(Keys.NumLock)) { _numLockState = !_numLockState; }

            Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>? contextList;
            //global context check
            if (Instance._eventRegistry.TryGetValue(GLOBAL_CONTEXT, out contextList))
            {
                try
                {
                    ProcessContextList(keyState, contextList);
                }
                catch
                {
                    throw;
                }
            }

            //current context check
            if (Instance._eventRegistry.TryGetValue(Instance._context, out contextList))
            {
                try
                {
                    ProcessContextList(keyState, contextList);
                }
                catch
                {
                    throw;
                }
            }
            //else there are no keys being watched in the current context

            //last save the state for comparison against next frame
            _lastState = keyState.GetSnapshot();
        }

        /// <summary>
        /// Process the given list of watchers
        /// </summary>
        /// <param name="keyState">The current key state</param>
        /// <param name="contextList">The list of watchers to process</param>
        /// <exception cref="InvalidOperationException">Thrown when the KeyboardEventType for an unpacked 
        /// event handler is an invalid value</exception>
        private static void ProcessContextList(KeyboardState keyState, Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>? contextList)
        {
            if (contextList != null)
            {
                //for each registered watcher check the key state
                foreach (var kE in contextList)
                {
                    //check modifier keys
                    ActiveModifierKeys amk = ActiveModifierKeys.None;
                    if (_capsLockState) { amk |= ActiveModifierKeys.CapsLock; }

                    if (_numLockState) { amk |= ActiveModifierKeys.NumLock; }

                    if (keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift)) { amk |= ActiveModifierKeys.Shift; }

                    if (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl)) { amk |= ActiveModifierKeys.Ctrl; }

                    if (keyState.IsKeyDown(Keys.LeftAlt) || keyState.IsKeyDown(Keys.RightAlt)) { amk |= ActiveModifierKeys.Alt; }

                    //decide which check to perform based on event type
                    switch (kE.Key.Item2)
                    {
                        case KeyboardEventType.KeyPressed:
                            {
                                //check the relevant key and invoke the callback if true
                                if (keyState.IsKeyPressed(kE.Key.Item1))
                                {
                                    KeyboardEventHandler temp = kE.Value;
                                    if (temp != null)
                                    {
                                        KeyboardEventArgs args = new KeyboardEventArgs(kE.Key.Item1, kE.Key.Item2, amk);
                                        temp.Invoke(Instance, args);
                                    }
                                }
                                break;
                            }
                        case KeyboardEventType.KeyReleased:
                            {
                                if (keyState.IsKeyReleased(kE.Key.Item1))
                                {
                                    KeyboardEventHandler temp = kE.Value;
                                    if (temp != null)
                                    {
                                        KeyboardEventArgs args = new KeyboardEventArgs(kE.Key.Item1, kE.Key.Item2, amk);
                                        temp.Invoke(Instance, args);
                                    }
                                }
                                break;
                            }
                        case KeyboardEventType.KeyHeld:
                            {
                                //if the key is down now and was down last check, if there was no last check then it's false
                                if ((_lastState?.IsKeyDown(kE.Key.Item1) ?? false) && keyState.IsKeyDown(kE.Key.Item1))
                                {
                                    KeyboardEventHandler temp = kE.Value;
                                    if (temp != null)
                                    {
                                        KeyboardEventArgs args = new KeyboardEventArgs(kE.Key.Item1, kE.Key.Item2, amk);
                                        temp.Invoke(Instance, args);
                                    }
                                }
                                break;
                            }
                        default:
                            throw new InvalidOperationException(string.Format("Invalid value for KeyboardEventType: {0}", kE.Key.Item2));
                    }
                }
            }
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

        public ActiveModifierKeys ModifierKeys { get; private set; }

        public KeyboardEventArgs(Keys key, KeyboardEventType type, ActiveModifierKeys modKeys)
        {
            Key = key;
            Type = type;
            ModifierKeys = modKeys;
        }
    }

    /// <summary>
    /// Flags that show which modifier keys are active
    /// </summary>
    [Flags]
    public enum ActiveModifierKeys
    {
        None = 0,
        Ctrl = 1,
        Shift = 2,
        Alt = 4,
        CapsLock = 8,
        NumLock = 16,
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
