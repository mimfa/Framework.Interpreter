using Microsoft.ClearScript;
using MiMFa.General;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Interpreters
{
    /// <summary>
    /// Interpreters Engine Interface
    /// </summary>
    public abstract class InterpreterBase : IInterpreter
    {
        /// <summary>
        /// The Interpreter Name
        /// </summary>
        public virtual string Name => "MiMFa Interpreter";
        /// <summary>
        /// The Interpreter Description
        /// </summary>
        public virtual string Description => "MiMFa Interpreter Base";
        public virtual string Extension => ".s";
        public virtual List<string> Libraries { get; private set; } = new List<string>();


        public HostFunctions HostFunctions = new HostFunctions();
        public ExtendedHostFunctions ExtendedHostFunctions = new ExtendedHostFunctions();

        /// <summary>
        /// Has an object in the interpreter
        /// </summary>
        /// <param name="propertyName">Variable Name</param>
        /// <returns></returns>
        public virtual bool HasObject(string propertyName) => false;

        /// <summary>
        /// Get an object of interpreter
        /// </summary>
        /// <param name="propertyName">Variable Name</param>
        /// <returns></returns>
        public virtual object GetObject(string propertyName) => null;
       
        /// <summary>
        /// Get an object of interpreter
        /// </summary>
        /// <param name="propertyName">Variable Name</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns></returns>
        public virtual T GetObject<T>(string propertyName, T defaultValue) 
        {
            try
            {
                return (T)GetObject(propertyName);
            }
            catch { return defaultValue; }
        }

        /// <summary>
        /// Get an object of interpreter
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<KeyValuePair<string, object>> GetObjects()
        {
            yield break;
        }

        /// <summary>
        /// Add an Internal Object to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="obj">The object to introduce to engine</param>
        /// <returns></returns>
        public virtual void InjectObject(string name, object obj) { }

        /// <summary>
        /// Add an Internal Type to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="type">The Type to introduce to engine</param>
        /// <returns></returns>
        public virtual void InjectType(string name, Type type) { }
        public virtual void InjectType(Type type) => InjectType(type.Name, type);

        /// <summary>
        /// Add an Internal Assembly to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="assemblyNames">Assembly names</param>
        /// <returns></returns>
        public void InjectAssembly(string name, params string[] assemblyNames)
        {
            InjectObject(name, new HostTypeCollection(assemblyNames));
        }

        /// <summary>
        /// Add an Internal Assembly to Engine
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <param name="assemblies">Assemblies</param>
        /// <returns></returns>
        public void InjectAssembly(string name, params Assembly[] assemblies)
        {
            InjectObject(name, new HostTypeCollection(assemblies));
        }



        /// <summary>
        /// Initialize the engine
        /// </summary>
        /// <returns></returns>
        public virtual void InitializeEngine()
        {
            Libraries = new List<string>();
        }

        /// <summary>
        /// Initialize all, contains engine, basic and default
        /// </summary>
        /// <returns></returns>
        public virtual void Initialize()
        {
            Initialize(true, true);
        }
        /// <summary>
        /// Initialize engine, basic and default
        /// </summary>
        /// <returns></returns>
        public void Initialize(bool injectBasic, bool injectDefault)
        {
            InitializeEngine();
            if (injectBasic) InjectBasics();
            if (injectDefault) InjectDefaults();
        }
        public void Initialize(params string[] rootAssemblies)
        {
            Initialize(true,true);
            InjectAssembly("Root", rootAssemblies);
        }
        public virtual void FinalizeEngine()
        {
            Libraries = new List<string>();
            Interrupt();
        }
        public virtual void Finalize()
        {
            FinalizeEngine();
        }

        public virtual void InjectBasics()
        {
            InjectType("MessageBox", typeof(MessageBox));
            InjectType("MessageMode", typeof(MessageMode));
            InjectType("DialogResult", typeof(DialogResult));
            InjectType("Var", typeof(Tool.Var));
            InjectType("Statement", typeof(Statement));
            InjectType("Default", typeof(Default));
            InjectType("Config", typeof(Config));
            InjectAssembly("Core",
                "mscorlib",
                "System",
                "System.Core",
                "System.Xml",
                "System.Linq",
                "System.Data",
                "System.Threading",
                "System.Collections",
                "System.Reflection",
                "System.Web",
                "System.Windows");
            InjectObject("Host", HostFunctions);
            InjectObject("ExHost", ExtendedHostFunctions);
        }
        public virtual void InjectDefaults()
        {
            InjectDefaultAssemblies();
            InjectDefaultTypes();
            InjectDefaultObjects();
        }
        public virtual void InjectDefaultAssemblies()
        {
            InjectAssembly("Library",
                                "MiMFa Framework",
                                "MiMFa Framework.Interpreter",
                                "System.Design",
                                "System.Drawing",
                                "System.Windows.Forms");
        }
        public virtual void InjectDefaultTypes()
        {
            InjectType("Engine", GetType());
            InjectType(typeof(Enumerable));
            InjectType(typeof(EnumerableQuery));
            InjectType(typeof(EnumerableExecutor));
            InjectType(typeof(ParallelEnumerable));
            InjectType(typeof(MiMFa.Model.Structure.Hierarchy));
            InjectType("Document", typeof(Model.IO.ChainedFile));
        }
        public virtual void InjectDefaultObjects()
        {
            InjectType("console", typeof(Console));
            InjectObject("engine", this);
        }

        public virtual void Interrupt()
        {
        }

        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        public virtual T Evaluate<T>(string codes, T defaultValue = default)
        {
            try { return ((dynamic)Evaluate(null, codes)) ?? defaultValue; } catch (Exception ex) { return defaultValue; }
        }

        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        public virtual object Evaluate(string documentName, string code) => null;
        /// <summary>
        /// Evaluate Codes
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        public object Evaluate(string codes) => Evaluate(null, codes);
        /// <summary>
        /// Evaluate Scripts
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public object[] Evaluate(string[] pathsOrScripts, bool withException = true) => Evaluate(withException,pathsOrScripts).ToArray();
        /// <summary>
        /// Evaluate Scripts
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public IEnumerable<object> Evaluate(bool withException, params string[] pathsOrScripts)
        {
            if (withException) foreach (var path in pathsOrScripts) yield return Evaluate(path, Script(path));
            else foreach (var path in pathsOrScripts)
                {
                    object obj = null;
                    try { obj = Evaluate(path, Script(path)); } catch { }
                    yield return obj;
                }
        }


        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        public virtual void Execute(string documentName, string code) { }
        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        public void Execute(string codes) => Execute(null, codes);
        /// <summary>
        /// Execute Scripts
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public void Execute(bool withException, params string[] pathsOrScripts) => Execute(pathsOrScripts, withException);
        /// <summary>
        /// Execute Scripts
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public void Execute(string[] pathsOrScripts, bool withException = true)
        {
            if (withException) foreach (var path in pathsOrScripts) Execute(path, Script(path));
            else foreach (var path in pathsOrScripts) try { Execute(path, Script(path)); } catch { }
        }

        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="code">The concurrent codes</param>
        /// <param name="documentName">The concurrent name</param>
        public virtual string ExecuteCommand(string documentName, string code) => null;
        /// <summary>
        /// Execute Codes
        /// </summary>
        /// <param name="codes">The code</param>
        /// <returns></returns>
        public string ExecuteCommand(string codes) => ExecuteCommand(null, codes);
        /// <summary>
        /// Execute Command
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public string[] ExecuteCommand(string[] pathsOrScripts, bool withException = true) => ExecuteCommand(withException, pathsOrScripts).ToArray();
        /// <summary>
        /// Execute Command
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public IEnumerable<string> ExecuteCommand(bool withException, params string[] pathsOrScripts)
        {
            if (withException) foreach (var path in pathsOrScripts) yield return ExecuteCommand(path, Script(path));
            else foreach (var path in pathsOrScripts)
                {
                    string obj = null;
                    try { obj = ExecuteCommand(path, Script(path)); } catch { }
                    yield return obj;
                }
        }

        /// <summary>
        /// Embed Module
        /// </summary>
        /// <param name="documentName">The codes filename</param>
        /// <param name="codes">The module code</param>
        public virtual object Module(string documentName, string codes) => Evaluate(documentName, codes);
        /// <summary>
        /// Embed Module
        /// </summary>
        /// <param name="code">The module code</param>
        public object Module(string code) => Module(null, code);
        /// <summary>
        /// Embed Modules
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public object[] Module(string[] pathsOrScripts, bool withException = true) => Module(withException,pathsOrScripts).ToArray();
        /// <summary>
        /// Embed Modules
        /// </summary>
        /// <param name="pathsOrScripts">The codes files</param>
        /// <param name="withException">Auto Exception Handler</param>
        /// <returns></returns>
        public IEnumerable<object> Module(bool withException, params string[] pathsOrScripts)
        {
            if (withException) foreach (var path in pathsOrScripts) yield return Module(path, Script(path));
            else foreach (var path in pathsOrScripts)
                {
                    object o = null;
                    try { o = Module(path, Script(path)); } catch { }
                    yield return o;
                }
        }


        public object[] Use(string[] pathsOrScripts, bool withExceptions = true) => Use(withExceptions,pathsOrScripts).ToArray();
        public IEnumerable<object> Use(bool withException, params string[] pathsOrScripts) => (from v in pathsOrScripts select Use(v, withException)).ToArray();
        public object Use(bool withExceptions = true) => Use(Config.PackageDirectory, withExceptions);
        public object Use(string pathsOrScripts, bool withExceptions = true) => Use(pathsOrScripts, Extension, withExceptions);
        public virtual object Use(string pathsOrScripts, string extension, bool withExceptions = true)
        {
            if (withExceptions) return Evaluate(Script(pathsOrScripts, extension));
            try { return Evaluate(Script(pathsOrScripts, extension)); }
            catch (Exception ex){ return null;  }
        }

        public Task[] UseAsync(string[] pathsOrScripts, bool withExceptions = true) => UseAsync(withExceptions,pathsOrScripts).ToArray();
        public IEnumerable<Task> UseAsync(bool withException, params string[] pathsOrScripts) => (from v in pathsOrScripts select UseAsync(v, withException)).ToArray();
        public Task UseAsync(bool withExceptions = true) => UseAsync(Config.PackageDirectory, withExceptions);
        public Task UseAsync(string pathsOrScripts, bool withExceptions = true) => UseAsync(pathsOrScripts, Extension, withExceptions);
        public virtual Task UseAsync(string pathsOrScripts, string extension, bool withExceptions = true) => ProcessService.RunTask(() => Use(pathsOrScripts, extension, withExceptions));

        public object[] Once(string[] pathsOrScripts, bool withExceptions = true) => Once(withExceptions, pathsOrScripts).ToArray();
        public IEnumerable<object> Once(bool withException, params string[] pathsOrScripts) => (from v in pathsOrScripts select Once(v, withException)).ToArray();
        public object Once(bool withExceptions = true) => Once(Config.PackageDirectory, withExceptions);
        public object Once(string pathsOrScripts, bool withExceptions = true) => Once(pathsOrScripts, Extension, withExceptions);
        public virtual object Once(string pathsOrScripts, string extension, bool withExceptions = true)
        {
            if (Libraries.Contains(pathsOrScripts)) return null;
            if (withExceptions)
            {
                var sc = Script(pathsOrScripts, extension);
                Libraries.Add(pathsOrScripts);
                return Evaluate(sc);
            }
            try
            {
                var sc = Script(pathsOrScripts, extension);
                Libraries.Add(pathsOrScripts);
                return Evaluate(sc);
            }
            catch (Exception ex) { return null; }
        }

        public Task[] OnceAsync(string[] pathsOrScripts, bool withExceptions = true) => OnceAsync(withExceptions, pathsOrScripts).ToArray();
        public IEnumerable<Task> OnceAsync(bool withException, params string[] pathsOrScripts) => (from v in pathsOrScripts select OnceAsync(v, withException)).ToArray();
        public Task OnceAsync(bool withExceptions = true) => OnceAsync(Config.PackageDirectory, withExceptions);
        public Task OnceAsync(string pathsOrScripts, bool withExceptions = true) => OnceAsync(pathsOrScripts, Extension, withExceptions);
        public virtual Task OnceAsync(string pathsOrScripts, string extension, bool withExceptions = true) => ProcessService.RunTask(() => Once(pathsOrScripts, extension, withExceptions));


        public string Script(string[] addressesOrScripts, string extension = null) => string.Join(Environment.NewLine, from pth in addressesOrScripts select Script(pth, extension));
        public virtual string Script(string addressOrScripts, string extension = null)
        {
            extension = extension ?? Extension;
            if (string.IsNullOrWhiteSpace(addressOrScripts)) return null;
            if(Path.InvalidPathChars.Any(v=> addressOrScripts.Contains(v)))return addressOrScripts;
            else if(File.Exists(addressOrScripts)) return File.ReadAllText(addressOrScripts);
            else if (Directory.Exists(addressOrScripts))
            {
                string[] files = Directory.GetFiles(addressOrScripts, "*" + extension, SearchOption.TopDirectoryOnly);
                if (files.Length > 0) return Script(files, extension);
                else return Script(Directory.GetDirectories(addressOrScripts), extension);
            }
            else if (InternetService.IsWellURL(addressOrScripts)) return Script(InternetService.Download(addressOrScripts), extension);
            else
            {
                string newPath = Path.Combine(Config.ApplicationDirectory, addressOrScripts);
                if (!newPath.EndsWith(extension)) newPath += extension;
                if (File.Exists(newPath) || Directory.Exists(newPath)) return Script(newPath, extension);
                else
                {
                    newPath = Path.Combine(Environment.CurrentDirectory, addressOrScripts);
                    if (!newPath.EndsWith(extension))  newPath += extension;
                    if (File.Exists(newPath) || Directory.Exists(newPath)) return Script(newPath, extension);
                    else return addressOrScripts;
                }
            }
        }
        #region Services
        public static bool IsHostTypeObject(dynamic args) => args == null ? false : args.GetType().FullName.StartsWith("Microsoft.ClearScript.");
        /// <summary>
        /// Convert the V8Array to IEnumerable
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="args">V8Array argument</param>
        /// <param name="func">Convert function</param>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumerable<T>(dynamic args, Func<object, T> func)
        {
            foreach (dynamic v in ToEnumerable(args))
                yield return func(v);
        }

        /// <summary>
        /// Convert the V8Array to IEnumerable
        /// </summary>
        /// <param name="args">V8Array argument</param>
        /// <returns></returns>
        public static IEnumerable<object> ToEnumerable(dynamic args)
        {
            if (args == null) yield break;
            if (args is IEnumerable<object>)
                foreach (var item in args)
                    yield return item;
            else if (IsHostTypeObject(args))
            {
                int len = args.PropertyIndices.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        yield return args[args.PropertyIndices[i]];
                len = args.PropertyNames.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        yield return args[args.PropertyNames[i]];
            }
            else yield return args;
        }

        /// <summary>
        /// Convert the V8Array to Dictionary
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="args">V8Array argument</param>
        /// <param name="kfunc">Convert Key function</param>
        /// <param name="vfunc">Convert Value function</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey,TValue>(dynamic args, Func<object, TKey> kfunc, Func<object, TValue> vfunc)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            if (args == null) return dic;
            if (args is IEnumerable<object>)
            {
                long l = 0;
                foreach (var item in args)
                    dic.Add(kfunc(l++), vfunc(item));
            }
            else if(IsHostTypeObject(args))
            {
                int len = args.PropertyIndices.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        dic.Add(kfunc(args.PropertyIndices[i]), vfunc(args[args.PropertyIndices[i]]));
                else
                {
                    len = args.PropertyNames.Length;
                    if (len > 0)
                        for (int i = 0; i < len; i++)
                           dic.Add(kfunc(args.PropertyNames[i]), vfunc(args[args.PropertyNames[i]]));
                }
            }
            else dic.Add(kfunc(0), vfunc(args));
            return dic;
        }

        /// <summary>
        /// Convert the V8Array to Dictionary
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="args">V8Array argument</param>
        /// <param name="func">Convert function</param>
        /// <returns></returns>
        public static Dictionary<object, T> ToDictionary<T>(dynamic args, Func<object, T> func)
        {
            Dictionary<object, T> dic = new Dictionary<object, T>();
            if (args == null) return dic;
            if (args is IEnumerable<object>)
            {
                long l = 0;
                foreach (var item in args)
                    dic.Add(l++, func(item));
            }
            else if(IsHostTypeObject(args))
            {
                int len = args.PropertyIndices.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        dic.Add(args.PropertyIndices[i], func( args[args.PropertyIndices[i]]));
                else
                {
                    len = args.PropertyNames.Length;
                    if (len > 0)
                        for (int i = 0; i < len; i++)
                            dic.Add(args.PropertyNames[i], func(args[args.PropertyNames[i]]));
                }
            }
            else dic.Add(0, func(args));
            return dic;
        }

        /// <summary>
        /// Convert the V8Array to Dictionary
        /// </summary>
        /// <param name="args">V8Array argument</param>
        /// <returns></returns>
        public static Dictionary<object, object> ToDictionary(dynamic args)
        {
            Dictionary<object, object> dic = new Dictionary<object, object>();
            if (args == null) return dic;
            if (args is IEnumerable<object>)
            {
                long l = 0;
                foreach (var item in args)
                    dic.Add(l++, item);
            }
            else if (IsHostTypeObject(args))
            {
                int len = args.PropertyIndices.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        dic.Add(args.PropertyIndices[i], args[args.PropertyIndices[i]]);
                else
                {
                    len = args.PropertyNames.Length;
                    if (len > 0)
                        for (int i = 0; i < len; i++)
                            dic.Add(args.PropertyNames[i], args[args.PropertyNames[i]]);
                }
            }
            else dic.Add(0, args);
            return dic;
        }

        /// <summary>
        /// Convert the V8Array to Dictionary
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="args">V8Array argument</param>
        /// <param name="kfunc">Convert Key function</param>
        /// <param name="vfunc">Convert Value function</param>
        /// <returns></returns>
        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(dynamic args, Func<object, TKey> kfunc, Func<object, TValue> vfunc)
        {
            SortedDictionary<TKey, TValue> dic = new SortedDictionary<TKey, TValue>();
            if (args == null) return dic;
            if (args is IEnumerable<object>)
            {
                long l = 0;
                foreach (var item in args)
                    dic.Add(kfunc(l++), vfunc(item));
            }
            else if (IsHostTypeObject(args))
            {
                int len = args.PropertyIndices.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        dic.Add(kfunc(args.PropertyIndices[i]), vfunc(args[args.PropertyIndices[i]]));
                else
                {
                    len = args.PropertyNames.Length;
                    if (len > 0)
                        for (int i = 0; i < len; i++)
                            dic.Add(kfunc(args.PropertyNames[i]), vfunc(args[args.PropertyNames[i]]));
                }
            }
            else dic.Add(kfunc(0), vfunc(args));
            return dic;
        }

        /// <summary>
        /// Convert the V8Array to Dictionary
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="args">V8Array argument</param>
        /// <param name="func">Convert function</param>
        /// <returns></returns>
        public static SortedDictionary<object, T> ToSortedDictionary<T>(dynamic args, Func<object, T> func)
        {
            SortedDictionary<object, T> dic = new SortedDictionary<object, T>();
            if (args == null) return dic;
            if (args is IEnumerable<object>)
            {
                long l = 0;
                foreach (var item in args)
                    dic.Add(l++, func(item));
            }
            else if (IsHostTypeObject(args))
            {
                int len = args.PropertyIndices.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        dic.Add(args.PropertyIndices[i], func(args[args.PropertyIndices[i]]));
                else
                {
                    len = args.PropertyNames.Length;
                    if (len > 0)
                        for (int i = 0; i < len; i++)
                            dic.Add(args.PropertyNames[i], func(args[args.PropertyNames[i]]));
                }
            }
            else dic.Add(0, func(args));
            return dic;
        }

        /// <summary>
        /// Convert the V8Array to Dictionary
        /// </summary>
        /// <param name="args">V8Array argument</param>
        /// <returns></returns>
        public static SortedDictionary<object, object> ToSortedDictionary(dynamic args)
        {
            SortedDictionary<object, object> dic = new SortedDictionary<object, object>();
            if (args == null) return dic;
            if (args is IEnumerable<object>)
            {
                long l = 0;
                foreach (var item in args)
                    dic.Add(l++, item);
            }
            else if (IsHostTypeObject(args))
            {
                int len = args.PropertyIndices.Length;
                if (len > 0)
                    for (int i = 0; i < len; i++)
                        dic.Add(args.PropertyIndices[i], args[args.PropertyIndices[i]]);
                else
                {
                    len = args.PropertyNames.Length;
                    if (len > 0)
                        for (int i = 0; i < len; i++)
                            dic.Add(args.PropertyNames[i], args[args.PropertyNames[i]]);
                }
            }
            else dic.Add(0, args);
            return dic;
        }
        #endregion
    }
}
