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
using Microsoft.ClearScript.Windows;

namespace MiMFa.Interpreters.Engine
{
    /// <summary>
    /// JavaScript Engine
    /// </summary>
    public class VisualBasic : EngineBase
    {
        /// <summary>
        /// Interpreter Name
        /// </summary>
        public override string Name => "MiMFa VisualBasic";
        /// <summary>
        /// Interpreter Description
        /// </summary>
        public override string Description => "MiMFa VisualBasic Interpreter";
        /// <summary>
        /// Interpreter Script File Extension
        /// </summary>
        public override string Extension => ".vb";

        /// <summary>
        /// Main Engine
        /// </summary>
        public override ScriptEngine Engine { get; set;} = new VBScriptEngine();

        /// <summary>
        /// Create Instance of Java Script V8
        /// </summary>
        public VisualBasic() : base() { }


    }
}
