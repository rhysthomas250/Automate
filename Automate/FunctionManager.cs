﻿#region License Information (GPL v3)

/*
    Copyright (C) Jaex

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScriptingLib;

namespace Automate
{
    public class FunctionManager
    {
        public static readonly Dictionary<string, Type> Functions = new Dictionary<string, Type>()
        {
            { "Func", typeof(Function) },
            { "Goto", typeof(Function_Goto) },
            { "Wait", typeof(Function_Wait) },
            { "KeyDown", typeof(Function_KeyDown) },
            { "KeyUp", typeof(Function_KeyDown) },
            { "KeyPress", typeof(Function_KeyPress) },
            { "KeyPressText", typeof(Function_KeyPressText) },
            { "MouseDown", typeof(Function_MouseDown) },
            { "MouseUp", typeof(Function_MouseUp) },
            { "MouseClick", typeof(Function_MouseClick) },
            { "MouseMove", typeof(Function_MouseMove) },
            { "MouseWheel", typeof(Function_MouseWheel) }
        };

        public int LineDelay { get; set; }

        private List<Function> FunctionList;
        private bool stopRequest;

        public Function GetFunction(string name, string[] parameters)
        {
            if (Functions.ContainsKey(name))
            {
                Function func = (Function)Activator.CreateInstance(Functions[name]);
                func.Name = name;
                func.Parameters = parameters;
                return func;
            }

            return null;
        }

        public int FindFunctionIndex(string name)
        {
            for (int i = 0; i < FunctionList.Count; i++)
            {
                Function function = FunctionList[i];

                if (function != null && function.GetType() == typeof(Function) && function.Parameters[0].
                    Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Compile(string[] lines)
        {
            FunctionList = new List<Function>();

            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    Tokenizer tokenizer = new Tokenizer();
                    tokenizer.AutoParseLiteral = true;
                    tokenizer.Keywords = FunctionManager.Functions.Select(x => x.Key).ToArray();
                    List<Token> tokens = tokenizer.Tokenize(line);

                    int loop = 1;

                    if (tokens[0].Type == TokenType.Numeric)
                    {
                        loop = int.Parse(tokens[0].Text);
                        tokens.RemoveAt(0);
                    }

                    if (tokens[0].Type == TokenType.Keyword)
                    {
                        string name = tokens[0].Text;
                        string[] parameters = tokens.Skip(1).Select(x => x.Text).ToArray();
                        Function tempFunction = GetFunction(name, parameters);

                        if (tempFunction != null)
                        {
                            tempFunction.FunctionManager = this;
                            tempFunction.Loop = loop;
                            FunctionList.Add(tempFunction);
                            continue;
                        }
                    }
                }

                FunctionList.Add(null);
            }

            foreach (Function function in FunctionList)
            {
                function.Prepare();
            }

            return true;
        }

        public void Run(int startIndex = 0)
        {
            if (FunctionList != null)
            {
                Function function;

                for (int i = startIndex; i < FunctionList.Count && !stopRequest; i++)
                {
                    function = FunctionList[i];

                    if (function == null)
                    {
                        break;
                    }

                    function.Run();

                    if (LineDelay > 0)
                    {
                        Thread.Sleep(LineDelay);
                    }
                }
            }
        }

        public void Stop()
        {
            stopRequest = true;
        }
    }
}