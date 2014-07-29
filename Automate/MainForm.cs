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

using ScriptingLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Automate
{
    public partial class MainForm : Form
    {
        private FunctionManager functionManager = new FunctionManager();
        private Tokenizer tokenizer = new Tokenizer();
        private bool isWorking;

        public MainForm()
        {
            InitializeComponent();
            tokenizer.Keywords = FunctionManager.Functions.Select(x => x.Key).ToArray();
            Tokenize();
            cbFunctions.Items.AddRange(tokenizer.Keywords);
        }

        private void rtbInput_TextChanged(object sender, EventArgs e)
        {
            Tokenize();
        }

        private void Tokenize()
        {
            if (!string.IsNullOrEmpty(rtbInput.Text))
            {
                List<Token> tokens = tokenizer.Tokenize(rtbInput.Text);
                SyntaxHighlighting(tokens);
            }
        }

        private void SyntaxHighlighting(List<Token> tokens)
        {
            int start = rtbInput.SelectionStart;
            int length = rtbInput.SelectionLength;

            rtbInput.BeginUpdate();

            foreach (Token token in tokens)
            {
                Color color;

                switch (token.Type)
                {
                    default:
                        continue;
                    case TokenType.Symbol:
                        color = Color.Red;
                        break;
                    case TokenType.Literal:
                        color = Color.Brown;
                        break;
                    case TokenType.Identifier:
                        color = Color.DarkBlue;
                        break;
                    case TokenType.Numeric:
                        color = Color.Blue;
                        break;
                    case TokenType.Keyword:
                        color = Color.Green;
                        break;
                }

                rtbInput.SelectionStart = token.Position;
                rtbInput.SelectionLength = token.Text.Length;
                rtbInput.SelectionColor = color;
            }

            rtbInput.Select(start, length);
            rtbInput.EndUpdate();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (!isWorking)
            {
                isWorking = true;
                btnRun.Enabled = false;
                string[] lines = rtbInput.Lines;
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.RunWorkerAsync(lines);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] lines = e.Argument as string[];

            try
            {
                functionManager.Compile(lines);
                functionManager.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isWorking = false;
            btnRun.Enabled = true;
        }
    }
}