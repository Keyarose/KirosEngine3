using OpenTK.Windowing.GraphicsLibraryFramework;

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
        private string _previousContext = "";
        private static KeyboardState? _lastState;
        private static bool _capsLockState = false;
        private static bool _numLockState = false;

        /// <summary>
        /// The current input context, which decides what key notifications to send out.
        /// </summary>
        public static string CurrentContext
        { get { return Instance._context; } set { SetContext(value); } }

        /// <summary>
        /// The previous input context before the last assignment to the current context.
        /// </summary>
        public static string PreviousContext
        { get { return Instance._previousContext; } }

        /// <summary>
        /// The global program context for key events that always need to be sent regardless of the current context.
        /// </summary>
        public const string GLOBAL_CONTEXT = "global";

        /// <summary>
        /// Method signature for keyboard event handlers
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The keyboard event arguments</param>
        public delegate void KeyboardEventHandler(object sender, KeyboardEventArgs e);
        private Dictionary<string, Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>> _eventRegistry = [];

        /// <summary>
        /// Grouped collection of alpha numeric keys for registration.
        /// </summary>
        public static Keys[] AlphaNum => [Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
        Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0];

        /// <summary>
        /// Grouped collection of punctuation and symbol keys for registration.
        /// </summary>
        public static Keys[] PunctuationAndSymbols => [Keys.Apostrophe, Keys.Backslash, Keys.Comma, Keys.Equal, Keys.LeftBracket, Keys.Minus, Keys.Period, Keys.RightBracket, Keys.Semicolon, Keys.Slash];

        /// <summary>
        /// Basic constructor.
        /// </summary>
        private KeyboardEventManager()
        {
            if (OperatingSystem.IsWindows())
            {
                _capsLockState = Console.CapsLock;
                _numLockState = Console.NumberLock;
            }

            //todo: other platform support.
        }

        /// <summary>
        /// Set the current input context.
        /// </summary>
        /// <param name="context">The identifying string for the current context.</param>
        public static void SetContext(string context)
        {
            Instance._previousContext = Instance._context;
            Instance._context = context;
        }

        /// <summary>
        /// Revert the current input context to the previous context.
        /// </summary>
        public static void RevertContext()
        {
            Instance._context = Instance._previousContext;
        }

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
                    DebugConsole.WriteLine("Context key: {0} doesn't exist in KeyboardEventManager, adding it.", context);

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
        /// Subscribe to the keyboard event manager in the given context for the given keys and event type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="keys"></param>
        /// <param name="keyMode"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static bool SubscribeKeyboardEvents(string context, Keys[] keys, KeyboardEventType keyMode, KeyboardEventHandler callback)
        {
            foreach (var key in keys)
            {
                if (!SubscribeKeyboardEvent(context, key, keyMode, callback))
                    return false;
            }

            return true;
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
        /// <param name="time">Number of seconds since the previous call.</param>
        /// <exception cref="InvalidOperationException">Thrown when the KeyboardEventType for an unpacked
        /// event handler is an invalid value</exception>
        public static void Update(KeyboardState keyState, double time)
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
                    ProcessContextList(keyState, contextList, time);
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
                    ProcessContextList(keyState, contextList, time);
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
        /// <param name="time">Seconds since the last call.</param>
        /// <exception cref="InvalidOperationException">Thrown when the KeyboardEventType for an unpacked 
        /// event handler is an invalid value</exception>
        private static void ProcessContextList(KeyboardState keyState, Dictionary<Tuple<Keys, KeyboardEventType>, KeyboardEventHandler>? contextList, double time)
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
                                        KeyboardEventArgs args = new KeyboardEventArgs(kE.Key.Item1, kE.Key.Item2, amk, time);
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
                                        KeyboardEventArgs args = new KeyboardEventArgs(kE.Key.Item1, kE.Key.Item2, amk, time);
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
                                        KeyboardEventArgs args = new KeyboardEventArgs(kE.Key.Item1, kE.Key.Item2, amk, time);
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
        /// <summary>
        /// The key that triggered the event.
        /// </summary>
        public Keys Key { get; private set; }
        /// <summary>
        /// The type of event: pressed, held, released.
        /// </summary>
        public KeyboardEventType Type { get; private set; }

        /// <summary>
        /// Modifier keys active at the time of the event: Ctrl, Alt, Shift, etc.
        /// </summary>
        public ActiveModifierKeys ModifierKeys { get; private set; }

        /// <summary>
        /// The amount of time since the previous frame.
        /// </summary>
        public double Time { get; private set; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="key">The key involved in the event.</param>
        /// <param name="type">The type of event.</param>
        /// <param name="modKeys">Modifier keys active during the event.</param>
        /// <param name="time">The time since the previous frame.</param>
        public KeyboardEventArgs(Keys key, KeyboardEventType type, ActiveModifierKeys modKeys, double time)
        {
            Key = key;
            Type = type;
            ModifierKeys = modKeys;
            Time = time;
        }
    }

    /// <summary>
    /// Flags that show which modifier keys are active
    /// </summary>
    [Flags]
    public enum ActiveModifierKeys
    {
        /// <summary>
        /// No active keys.
        /// </summary>
        None = 0,
        /// <summary>
        /// Ether Ctrl key.
        /// </summary>
        Ctrl = 1,
        /// <summary>
        /// Ether Shift key.
        /// </summary>
        Shift = 2,
        /// <summary>
        /// Ether Alt key.
        /// </summary>
        Alt = 4,
        /// <summary>
        /// The Caps Lock key.
        /// </summary>
        CapsLock = 8,
        /// <summary>
        /// The Num Lock key.
        /// </summary>
        NumLock = 16,
    }

    /// <summary>
    /// The type of keyboard event, was the key pressed, held, or released
    /// </summary>
    public enum KeyboardEventType
    {
        /// <summary>
        /// The Key was Pressed.
        /// </summary>
        KeyPressed,
        /// <summary>
        /// The Key was Released.
        /// </summary>
        KeyReleased,
        /// <summary>
        /// The Key was Held.
        /// </summary>
        KeyHeld
    }
}
