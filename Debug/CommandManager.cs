using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Debug
{
    /// <summary>
    /// Manages application commands and processing of those commands.
    /// </summary>
    public class CommandManager
    {
        private static CommandManager? _instance;

        private static CommandManager Instance
        { get { return _instance ??= new CommandManager(); } }

        //todo: List<Delegate> => List<(Delegate,string)>delegate, help info string
        private readonly Dictionary<string, List<Delegate>> _commands = new Dictionary<string, List<Delegate>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Basic constructor.
        /// </summary>
        private CommandManager() { }

        /// <summary>
        /// Register a command with the manager.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="method">The delegate method the command performs.</param>
        /// <returns>True if successful.</returns>
        public static bool RegisterCommand(string commandName, Delegate method)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(commandName, nameof(commandName));

            if (Instance._commands.TryGetValue(commandName, out List<Delegate>? commands)) 
            {
                commands.Add(method);
            }
            else
            {
                List<Delegate> newCmdList = [];
                newCmdList.Add(method);
                Instance._commands.Add(commandName, newCmdList);
            }

            return true;
        }

        /// <summary>
        /// Process a string and invoke the method represented by that string.
        /// </summary>
        /// <param name="command">The command string.</param>
        public static void ProcessCommand(string command)
        {
            if (command == null || command[0] != '\\')
            {
                Client.Report("Received command is null or does not start with \\");
                return;
            }

            string[] subs = command.Split([' ']);

            if (subs.Length == 0) 
            {
                Client.Report("'{0}', is not a valid command.", command);
                return;
            }

            //log the command
            Logger.WriteToLog(command);

            string cmdName = subs[0].Replace("\\", string.Empty);//strip out the \\

            subs = subs[1 .. subs.Length];//drop the cmd name

            if (Instance._commands.TryGetValue(cmdName, out List<Delegate>? result))//get the list of delegates related to the command
            {
                foreach (var action in result) //go through the list until a match for the number of parameters is found.
                {
                    ParameterInfo[] cmdParams = action.GetMethodInfo().GetParameters();

                    int nonOptional = 0;

                    foreach (var par in cmdParams)//get a count of the non optional params
                    {
                        if (!par.IsOptional)
                            nonOptional++;
                    }

                    bool match = false;

                    if (nonOptional <= subs.Length && subs.Length <= cmdParams.Length)//check the number of args against the number of non optional params
                    {
                        if (cmdParams.Length == 0)//no parameters and args
                        {
                            action.DynamicInvoke();
                            return;
                        }

                        object?[] parameters = new object?[cmdParams.Length];

                        for (int i = 0; i < cmdParams.Length; i++)
                        {
                            if (i < subs.Length)//while we have args left
                            {
                                try
                                {
                                    parameters[i] = TypeDescriptor.GetConverter(cmdParams[i].ParameterType).ConvertFromString(subs[i]);
                                }
                                catch (NotSupportedException)
                                {
                                    //cannot convert to the required parameter type thus not valid cmd match
                                    //break out of the parameter check and move on to the next one
                                    match = false;
                                    break;
                                }
                                catch (ArgumentException)
                                {
                                    if (cmdParams[i].Name != null)
                                        Client.Report("{0} is not a valid value for argument {1} of {2}", subs[i], cmdParams[i].Name!, cmdName);
                                    else
                                        Client.Report("{0} is not a valid argument for {1}", subs[i], cmdName);

                                    match = false;
                                    break;
                                }
                            }
                            else //we're out of args and only optional params should be left
                            {
                                parameters[i] = cmdParams[i].DefaultValue;
                            }
                            

                            //check that the converted value actually fits the parameter
                            if (parameters[i] != null && parameters[i]?.GetType() != cmdParams[i].ParameterType && parameters[i] != Type.Missing)
                            {
                                match = false;
                                break;
                            }

                            match = true;
                        }

                        if (match) //if all parameters successfully convert
                        {
                            action.DynamicInvoke(parameters);
                            return;
                        }
                    }
                }

                Client.Report("Missing arguments for command {0}.", cmdName);
                return;
            }
            else
            {
                Client.Report("'{0}', is not a valid command.", command);
            }
        }
    }
}
