using System;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace MiMFa.Interpreters.Engine
{
    /// <summary>
    /// V8 JavaScript Engine
    /// </summary>
    public class JavaScriptV8 : EngineBase
    {
        /// <summary>
        /// Interpreter Name
        /// </summary>
        public override string Name => "MiMFa JavaScript V8";
        /// <summary>
        /// Interpreter Description
        /// </summary>
        public override string Description => "MiMFa JavaScript V8 Interpreter";
        /// <summary>
        /// Interpreter Script File Extension
        /// </summary>
        public override string Extension => ".js";

        /// <summary>
        /// Main Engine
        /// </summary>
        public override ScriptEngine Engine { get; set;} = new V8ScriptEngine();

        /// <summary>
        /// Create Instance of Java Script V8
        /// </summary>
        public JavaScriptV8() : base() { }

        public override object Compile(string documentName, string code)
        {
            try
            {
                Nested = true;
                if (string.IsNullOrWhiteSpace(code)) return null;
                if (string.IsNullOrWhiteSpace(documentName)) return ((V8ScriptEngine)Engine).Compile(code);
                return ((V8ScriptEngine)Engine).Compile(documentName, code);
            }
            catch (AccessViolationException) { return null; }
            finally
            {
                Nested = false;
            }
        }
    }
}
