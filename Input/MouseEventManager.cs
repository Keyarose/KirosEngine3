using KirosEngine3.Math.Vector;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Input
{
    /// <summary>
    /// Manages mouse input and notifies subscribed objects of input events.
    /// </summary>
    public class MouseEventManager
    {
        private static MouseEventManager? _instance;

        private static MouseEventManager Instance
        { get { return _instance ??= new MouseEventManager(); } }

        private string _context = "";
        private string _previousContext = "";
        private static OpenTK.Windowing.GraphicsLibraryFramework.MouseState? _lastState;

        /// <summary>
        /// The current input context, which decides what button notifications to send out.
        /// </summary>
        public static string CurrentContext
        { get { return Instance._context; } }

        /// <summary>
        /// The global program context for mouse events that always need to be sent regardless of the current context.
        /// </summary>
        public const string GLOBAL_CONTEXT = "global";

        /// <summary>
        /// Method signature for mouse event handlers.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">The mouse event arguments.</param>
        public delegate void MouseEventHandler(object sender, MouseEventArgs args);
        private Dictionary<string, Dictionary<MouseEventType, MouseEventHandler>> _eventRegistry = [];

        /// <summary>
        /// Basic constructor.
        /// </summary>
        private MouseEventManager() { }

        /// <summary>
        /// Set the current input context.
        /// </summary>
        /// <param name="context">The identifying string for the current context.</param>
        public static void SetContext(string context)
        {
            Instance._previousContext = Instance._context;
            Instance._context = context;
        }

        #region Add/Remove
        /// <summary>
        /// Subscribe to the mouse event manager in the given context for the given key and event type.
        /// </summary>
        /// <param name="context">The program context in which the subscriber wants to receive events.</param>
        /// <param name="button">The button to receive events for.</param>
        /// <param name="type">The type of event.</param>
        /// <param name="callback">The delegate to handle the event.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentException">Thrown if context is an empty string.</exception>
        public static bool SubscribeMouseEvent(string context, MouseButton button, MouseEventType type, MouseEventHandler callback)
        {
            if (context != string.Empty) 
            {
                Dictionary<MouseEventType, MouseEventHandler> contextList = [];

                try
                {
                    contextList = Instance._eventRegistry[context];
                }
                catch (KeyNotFoundException)
                {
                    Logger.WriteToLog("Context key: {0} doesn't exist in MouseEventManager, adding it.", context);
                    Console.WriteLine("Context key: {0} doesn't exist in MouseEventManager, adding it.", context);
                    //todo:write to debug

                    //create it
                    Instance._eventRegistry[context] = contextList;
                }
                finally
                {
                    if (contextList.TryGetValue(type, out MouseEventHandler? list))
                    {
                        //unpack the handler, add the new one, and put it away
                        list += new MouseEventHandler(callback);
                        contextList[type] = list;
                    }
                    else
                    {
                        //none exist so create a new one
                        contextList.Add(type, new MouseEventHandler(callback));
                    }
                }
                return true;
            }

            throw new ArgumentException("The context string for a mouse event cannot be empty.", nameof(context));
        }
        #endregion

        /// <summary>
        /// Check the mouse state and send callbacks to all registered watchers who's events have happened.
        /// </summary>
        /// <param name="state">The current mouse state.</param>
        /// <param name="time">Number of seconds since the previous call.</param>
        public static void Update(OpenTK.Windowing.GraphicsLibraryFramework.MouseState state, double time)
        {
            Dictionary<MouseEventType, MouseEventHandler>? contextList;

            //global context check
            if (Instance._eventRegistry.TryGetValue(GLOBAL_CONTEXT, out contextList))
            {
                try
                {
                    ProcessContextList(state, contextList, time);
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
                    //process context list
                }
                catch
                {
                    throw;
                }
            }

            _lastState = state.GetSnapshot();
        }

        private static void ProcessContextList(MouseState mouseState, Dictionary<MouseEventType, MouseEventHandler>? contextList, double time)
        {
            if (contextList != null)
            {
                foreach (var eventHandler in contextList)
                {
                    switch (eventHandler.Key) 
                    {
                        case MouseEventType.Moved:
                            {
                                if (_lastState != null && mouseState.Position != _lastState.Position)
                                {
                                    MouseEventHandler handler = eventHandler.Value;
                                    if (handler != null) 
                                    {
                                        MouseEventArgs args = new MouseEventArgs(MouseButton.None, mouseState.Position, eventHandler.Key, time);
                                        handler.Invoke(Instance, args);
                                    }
                                }
                                break;
                            }
                        default:
                            throw new InvalidOperationException(string.Format("Invalid value for MouseEventType: {0}", eventHandler.Key));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Event arguments for mouse events.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// The mouse button the triggered the event.
        /// </summary>
        public MouseButton MouseButton { get; private set; }

        /// <summary>
        /// The position of the mouse on screen.
        /// </summary>
        public Vec2 ScreenPosition { get; private set; }

        /// <summary>
        /// The type of mouse event.
        /// </summary>
        public MouseEventType Type { get; private set; }

        /// <summary>
        /// The amount of time since the previous frame.
        /// </summary>
        public double Time { get; private set; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="mouseButton">The button involved in the event.</param>
        /// <param name="screenPosition">The position on the screen.</param>
        /// <param name="type">The type of the event.</param>
        /// <param name="time">The time since the previous frame.</param>
        public MouseEventArgs(MouseButton mouseButton, Vec2 screenPosition, MouseEventType type, double time)
        {
            MouseButton = mouseButton;
            ScreenPosition = screenPosition;
            Type = type;
            Time = time;
        }
    }

    /// <summary>
    /// The buttons of a mouse.
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        /// Button 1, or left.
        /// </summary>
        Button1 = 0,
        /// <summary>
        /// Button 2, or right.
        /// </summary>
        Button2 = 1,
        /// <summary>
        /// Button 3, or middle.
        /// </summary>
        Button3 = 2,
        /// <summary>
        /// Button 4, commonly left side.
        /// </summary>
        Button4 = 3,
        /// <summary>
        /// Button 5, commonly right side.
        /// </summary>
        Button5 = 4,
        /// <summary>
        /// Button 6.
        /// </summary>
        Button6 = 5,
        /// <summary>
        /// Button 7.
        /// </summary>
        Button7 = 6,
        /// <summary>
        /// Button 8.
        /// </summary>
        Button8 = 7,
        /// <summary>
        /// The left button, or button 1.
        /// </summary>
        Left = Button1,
        /// <summary>
        /// The right button, or button 2.
        /// </summary>
        Right = Button2,
        /// <summary>
        /// The middle button, or button 3.
        /// </summary>
        Middle = Button3,
        /// <summary>
        /// The highest available button number.
        /// </summary>
        Last = Button8,
        /// <summary>
        /// No button, or unknown button.
        /// </summary>
        None = 8
    }

    

    /// <summary>
    /// The type of mouse event, button pressed, held, released, scroll up, scroll down
    /// </summary>
    public enum MouseEventType
    {
        /// <summary>
        /// The mouse was moved.
        /// </summary>
        Moved,
        /// <summary>
        /// The button was pressed.
        /// </summary>
        ButtonPressed,
        /// <summary>
        /// The button was released.
        /// </summary>
        ButtonReleased,
        /// <summary>
        /// The button was held.
        /// </summary>
        ButtonHeld,
        /// <summary>
        /// The scroll wheel moved up.
        /// </summary>
        ScrollUp,
        /// <summary>
        /// The scroll wheel moved down.
        /// </summary>
        ScrollDown,
    }
}
