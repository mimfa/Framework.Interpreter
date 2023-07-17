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
using MiMFa.General;
using MiMFa.Service;
using System.IO;

namespace MiMFa.Interpreters.Engine
{
    /// <summary>
    /// Engine Base
    /// </summary>
    public class EngineBase : InterpreterBase
    {
        /// <summary>
        /// Interpreter Name
        /// </summary>
        public override string Name => "MiMFa";
        /// <summary>
        /// Interpreter Description
        /// </summary>
        public override string Description => "MiMFa Interpreter";

        protected int _NestNumber = 0;
        public virtual int NestNumber { get => _NestNumber; protected set { if ((_NestNumber = value) <= 0) ContinueSwitch = true; } }
        public virtual bool Nested { get => _NestNumber > 0; protected set { if (value) _NestNumber++; else _NestNumber--; } }
        public virtual bool ContinueSwitch { get; set; } = true;
        public virtual bool InterruptSwitch { get => !ContinueSwitch && !(ContinueSwitch = true); set => ContinueSwitch = !value; }

        /// <summary>
        /// Main Engine
        /// </summary>
        public virtual ScriptEngine Engine { get; set;}

        /// <summary>
        /// Create Instance of Java Script V8
        /// </summary>
        public EngineBase()
        {
            Engine.AllowReflection = true;
            Engine.EnforceAnonymousTypeAccess = true;
            Engine.ExposeHostObjectStaticMembers = true;
            Engine.EnableAutoHostVariables = true;
            Engine.EnableNullResultWrapping = false;
            Engine.DisableTypeRestriction = false;
            Engine.DisableListIndexTypeRestriction = false;
            Engine.DisableExtensionMethods = false;
            Engine.DisableFloatNarrowing = false; 
            Engine.UseReflectionBindFallback = false;
            Engine.ContinuationCallback = new ContinuationCallback(()=> ContinueSwitch);
            Engine.DocumentSettings.SearchPath = Config.ApplicationDirectory;
            Engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
            InitializeEngine();
            InjectDefaults();
        }

        /// <summary>
        /// Add an Internal Object to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="obj">The object to introduce to engine</param>
        /// <returns></returns>
        public override void InjectObject(string name, object obj)
        {
            Engine.AddHostObject(name, HostItemFlags.DirectAccess, obj);
        }
        /// <summary>
        /// Add an Internal Object to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="obj">The object to introduce to engine</param>
        /// <returns></returns>
        public void InjectObjectByDialog(string name, object obj)
        {
             ProcessService.RunDialog("Preparing "+name,(s,a)=> InjectObject(name, obj));
        }

        /// <summary>
        /// Add an Internal Type to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="type">The Type to introduce to engine</param>
        /// <returns></returns>
        public override void InjectType(string name, Type type)
        {
            Engine.AddHostType(name,HostItemFlags.HideDynamicMembers, type);
        }

        /// <summary>
        /// Has an object in the interpreter
        /// </summary>
        /// <param name="propertyName">Variable Name</param>
        /// <returns></returns>
        public override bool HasObject(string propertyName)
        {
            return ((string[])Engine.Script.PropertyNames).Contains(propertyName);
        }


        /// <summary>
        /// Get an object of interpreter
        /// </summary>
        /// <param name="propertyName">Variable Name</param>
        /// <returns></returns>
        public override dynamic GetObject(string propertyName)
        {
            return Engine.Script[propertyName];
        }

        /// <summary>
        /// Convert an object by interpreter
        /// </summary>
        /// <param name="obj">Variable</param>
        /// <returns></returns>
        public virtual object ToObject(dynamic obj)
        {
            if (obj == null) return null;
            if (obj.GetType().FullName == "Microsoft.ClearScript.HostType")
                try { return HostFunctions.newObj(obj).GetType(); }
                catch (Exception ex)
                {
                    //try { return (Type)obj; } catch { }
                    //string nam = (obj + "").Replace("HostType:", "");
                    //try { return HostFunctions.newObj(nam).GetType(); }
                    //catch (Exception e) { }
                }
            return obj;
        }

        /// <summary>
        /// Get an objects of interpreter
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, object>> GetObjects()
        {
            return from v in (string[])Engine.Script.PropertyNames
                   where Engine.Script[v] != null
                   select new KeyValuePair<string, object>(v,
                   //Engine.Script[v] is HostTypeCollection? ((HostTypeCollection)Engine.Script[v]).Values :
                   Engine.Script[v]
                );
        }


        public override void Interrupt()
        {
            if (Nested) Engine.Interrupt();
            InterruptSwitch = true;
            base.Interrupt();
        }

        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        public override object Evaluate(string documentName, string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return null;
                Nested = true;
                if (string.IsNullOrWhiteSpace(documentName)) return Engine.Evaluate(code);
                return Engine.Evaluate(documentName, code);
            }
            catch (AccessViolationException) { return null; }
            finally { Nested = false; }
        }

        /// <summary>
        /// Execute Command Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        public override string ExecuteCommand(string documentName, string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return null;
                Nested = true;
                return Engine.ExecuteCommand(code);
            }
            catch (AccessViolationException) { return null; }
            finally { Nested = false; }
        }

        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        public override void Execute(string documentName, string code)
        {
            try
            { 
                if (string.IsNullOrWhiteSpace(code)) return;
                Nested = true;
                if (string.IsNullOrWhiteSpace(documentName)) Engine.Execute(code);
                else Engine.Execute(documentName, code);
            } catch (AccessViolationException) {  }
            finally { Nested = false; }
        }

        /// <summary>
        /// Embed Module
        /// </summary>
        /// <param name="documentName">The module path name</param>
        /// <param name="code">The module codes</param>
        public override object Module(string documentName, string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return null;
                Nested = true;
                return Engine.Evaluate(new DocumentInfo(documentName) { Category = ModuleCategory.Standard }, code);
            }
            catch (AccessViolationException ex) { return null;  }
            finally { Nested = false; }
        }


        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        public virtual object Compile(string documentName, string code) => Evaluate(documentName, code);

        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        public object Compile(string codes) => Compile(null, codes);

        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="codes">The codes</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public IEnumerable<object> CompileCodes(bool withException, params string[] codes)
        {
            if (withException) foreach (var code in codes) yield return Compile(code);
            else foreach (var code in codes)
                {
                    object obj = null;
                    try { obj = Compile(code); } catch { }
                    yield return obj;
                }
        }

        /// <summary>
        /// Evaluate Codes in the Files
        /// </summary>
        /// <param name="paths">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public IEnumerable<object> CompileFiles(bool withException, params string[] paths)
        {
            if (withException) foreach (var path in paths) yield return Compile(path, System.IO.File.ReadAllText(path));
            else foreach (var path in paths)
                {
                    object obj = null;
                    try { obj = Compile(path, System.IO.File.ReadAllText(path)); } catch { }
                    yield return obj;
                }
        }
     
        /// <summary>
        /// Parallel Execute Codes
        /// </summary>
        /// <param name="codes">The concurrent codes</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public void ConcurrentCompileCodes(bool withException, params string[] codes)
        {
            if (withException)
                foreach (var code in codes)
                    System.Threading.Tasks.Task.Run(new Action(() => Compile(code)));
            else
                foreach (var code in codes)
                    System.Threading.Tasks.Task.Run(new Action(() => { try { Compile(code); } catch { } }));
        }

        /// <summary>
        /// Parallel Execute Codes in the Files
        /// </summary>
        /// <param name="paths">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public void ConcurrentCompileFiles(bool withException, params string[] paths)
        {
            if (withException)
                foreach (var path in paths)
                    System.Threading.Tasks.Task.Run(new Action(() => Compile(path, System.IO.File.ReadAllText(path))));
            else
                foreach (var path in paths)
                    if (System.IO.File.Exists(path))
                        System.Threading.Tasks.Task.Run(new Action(() => { try { Compile(path, System.IO.File.ReadAllText(path)); } catch { } }));
        }

    }
}
