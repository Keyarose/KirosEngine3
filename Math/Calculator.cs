using KirosEngine3.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math
{
    internal class Calculator
    {
        private static Calculator? _instance;

        private Dictionary<string, long> _variables = [];

        public static Calculator Instance { get { return _instance ??= new Calculator(); } }

        public Action<object, string>? OutputTarget { get; set; }

        private Calculator() { }

        public static void ParseInput(object sender, string input)
        {
            if (input == "") { return; }

            string[] substrings = input.Split(' ');

            switch (substrings[0]) 
            {
                case "var":
                case "Var":
                case "VAR":
                    AddVariable(substrings[1..]);
                    break;
                case "listVar":
                case "listvar":
                case "LISTVAR":
                    ListVariables();
                    break;
                case "add":
                case "Add":
                case "ADD":
                    Evaluate(substrings[1..], (v1, v2) => v1 + v2);
                    break;
                case "sub":
                case "Sub":
                case "SUB":
                    Evaluate(substrings[1..], (v1, v2) => v1 - v2);
                    break;
                case "mul":
                case "Mul":
                case "MUL":
                    Evaluate(substrings[1..], (v1, v2) => v1 * v2);
                    break;
                case "help":
                case "Help":
                case "HELP":
                    if (substrings.Length == 1)
                    {
                        Help("");
                    }
                    else
                    {
                        Help(substrings[1]);
                    }
                    break;
            }
        }

        public static void AddVariable(string[] input)
        {
            if (input.Length < 3) { return; }
            if (input[1] != "=") { return; }
            if (!long.TryParse(input[2], out var value) ) { return; }

            string name = input[0];

            Instance._variables.Add(name, value);
            Instance.Output(string.Format("{0} set to {1}", name, value));
        }

        public static void ListVariables()
        {
            foreach(var v in Instance._variables)
            {
                Instance.Output($"{v}");
            }
        }

        public static void Evaluate(string[] input, Func<long, long, long> eval)
        {
            bool v1IsValue = false;
            long val1 = 0, val2 = 0;
            bool v2IsValue = false;

            if (input.Length < 3) { return; }
            if (!Instance._variables.ContainsKey(input[0]))
            {
                if (!long.TryParse(input[0], out val1))
                {
                    return;
                }
                v1IsValue = true;
            }

            if (!Instance._variables.ContainsKey(input[1]))
            {
                if (!long.TryParse(input[1], out val2))
                {
                    return;
                }
                v2IsValue = true;
            }

            if (!v1IsValue && !v2IsValue)
            {
                long result = eval.Invoke(Instance._variables[input[0]], Instance._variables[input[1]]);
                Instance._variables.Add(input[2], result);
                Instance.Output(string.Format(input[2] + " = {0}", result));
            }
            else if (v1IsValue && !v2IsValue)
            {
                long result = eval.Invoke(val1, Instance._variables[input[1]]);
                Instance._variables.Add(input[2], result);
                Instance.Output(string.Format(input[2] + " = {0}", result));
            }
            else if (v1IsValue && v2IsValue)
            {
                long result = eval.Invoke(val1, val2);
                Instance._variables.Add(input[2], result);
                Instance.Output(string.Format(input[2] + " = {0}", result));
            }
            else
            {
                long result = eval.Invoke(Instance._variables[input[0]], val2);
                Instance._variables.Add(input[2], result);
                Instance.Output(string.Format(input[2] + " = {0}", result));
            }
        }

        public static void Help(string input)
        {
            switch (input)
            {
                case "":
                    Instance.Output("Commands:");
                    Instance.Output("Var");
                    Instance.Output("ListVar");
                    Instance.Output("Add");
                    Instance.Output("Sub");
                    Instance.Output("Mul");
                    break;
                case "var":
                case "Var":
                case "VAR":
                    Instance.Output("Var [Name] = [Value]");
                    break;
                case "listvar":
                case "ListVar":
                case "listVar":
                case "LISTVAR":
                    Instance.Output("List all variables.");
                    break;
                case "add":
                case "Add":
                case "ADD":
                    Instance.Output("Add [val 1/var 1] [val 2/var 2] [var]");
                    break;
                case "sub":
                case "Sub":
                case "SUB":
                    Instance.Output("Sub [val 1/var 1] [val 2/var 2] [var]");
                    break;
                case "mul":
                case "Mul":
                case "MUL":
                    Instance.Output("Mul [val 1/var 1] [val 2/var 2] [var]");
                    break;
            }
        }

        public void Output(string text)
        {
            if (OutputTarget == null) { return; }

            OutputTarget.Invoke(this, text);
        }
    }
}
