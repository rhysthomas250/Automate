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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingLib
{
    public class Tokenizer
    {
        public char[] WhitespaceChars { get; set; }
        public char[] SymbolChars { get; set; }
        public char[] LiteralDelimiters { get; set; }
        public string[] Keywords { get; set; }
        public char LiteralEscapeChar { get; set; }
        public bool KeepWhitespace { get; set; }
        public bool AutoParseLiteral { get; set; }

        public Tokenizer()
        {
            WhitespaceChars = new char[] { ' ', '\t', '\r', '\n' };
            SymbolChars = new char[] { ',', ':', ';', '+', '-', '*', '/', '^', '=', '<', '>', '(', ')', '[', ']', '{', '}', '?', '!', '&', '|' };
            LiteralDelimiters = new char[] { '"', '\'' };
            LiteralEscapeChar = '\\';
            Keywords = new string[] { "if", "else", "return" };
            KeepWhitespace = false;
        }

        public List<Token> Tokenize(string text)
        {
            List<Token> tokens = new List<Token>();
            Token currentToken = null;
            char currentChar;

            for (int i = 0; i < text.Length; i++)
            {
                currentChar = text[i];

                if (WhitespaceChars.Contains(currentChar)) // Whitespace
                {
                    CheckIdentifier(tokens, currentToken);

                    if (KeepWhitespace)
                    {
                        currentToken = new Token(TokenType.Whitespace, currentChar.ToString(), i);
                        tokens.Add(currentToken);
                    }
                    else
                    {
                        currentToken = new Token();
                    }
                }
                else if (SymbolChars.Contains(currentChar)) // Symbol
                {
                    CheckIdentifier(tokens, currentToken);

                    currentToken = new Token(TokenType.Symbol, currentChar.ToString(), i);
                    tokens.Add(currentToken);
                }
                else if (LiteralDelimiters.Contains(currentChar)) // Literal
                {
                    CheckIdentifier(tokens, currentToken);

                    currentToken = new Token(TokenType.Literal, currentChar.ToString(), i);
                    tokens.Add(currentToken);

                    char delimeter = currentChar;

                    for (i++; i < text.Length; i++)
                    {
                        currentChar = text[i];
                        currentToken.Text += currentChar;

                        if (currentChar == delimeter && !IsEscaped(currentToken.Text))
                        {
                            if (AutoParseLiteral)
                            {
                                currentToken.Text = ParseString(currentToken.Text);
                            }

                            break;
                        }
                    }
                }
                else // Identifier, Numeric, Keyword
                {
                    if (currentToken != null && currentToken.Type == TokenType.Identifier)
                    {
                        currentToken.Text += currentChar;
                    }
                    else
                    {
                        currentToken = new Token(TokenType.Identifier, currentChar.ToString(), i);
                        tokens.Add(currentToken);
                    }

                    if (i + 1 >= text.Length) // EOF
                    {
                        CheckIdentifier(tokens, currentToken);
                    }
                }
            }

            return tokens;
        }

        public string ParseString(string text)
        {
            if (!string.IsNullOrEmpty(text) && text.Length > 1)
            {
                StringBuilder sb = new StringBuilder();
                char delimeter = text[0];
                int length;

                if (text[text.Length - 1] == delimeter && !IsEscaped(text))
                {
                    length = text.Length - 1;
                }
                else
                {
                    length = text.Length;
                }

                for (int i = 1; i < length; i++)
                {
                    if (text[i] != LiteralEscapeChar || text[i - 1] == LiteralEscapeChar)
                    {
                        sb.Append(text[i]);
                    }
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        private void CheckIdentifier(List<Token> tokens, Token token)
        {
            if (token != null && token.Type == TokenType.Identifier)
            {
                double result;

                if (double.TryParse(token.Text, out result))
                {
                    token.Type = TokenType.Numeric;
                }
                else if (Keywords.Contains(token.Text))
                {
                    token.Type = TokenType.Keyword;
                }
            }
        }

        private bool IsEscaped(string text, int position = -1)
        {
            if (position == -1)
            {
                position = text.Length - 1;
            }

            if (position > 0 && text[position - 1] == LiteralEscapeChar)
            {
                if (position > 1 && text[position - 2] == LiteralEscapeChar)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}