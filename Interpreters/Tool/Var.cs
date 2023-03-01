using MiMFa.Interpreters;
using MiMFa.Interpreters.Engine;
using MiMFa.Model;
using MiMFa.Model.IO;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Interpreters.Tool
{
    public class Var
    {
        public static Type GetType(ref string name, Type rootType, object obj)
        {
            if (!string.IsNullOrWhiteSpace(name))
                if (rootType.FullName.StartsWith("Microsoft.ClearScript"))
                {
                    if (rootType.FullName.EndsWith(".HostTypeCollection"))
                    {
                        foreach (var o in ((dynamic)obj))
                            if (NestedOf(o.Key, ".", ref name))
                                return GetType(ref name, o.Value.GetType(), o.Value);
                    }
                    else if (rootType.FullName.EndsWith(".PropertyBag"))
                    {
                        foreach (var o in ((dynamic)obj))
                            if (NestedOf(o.Key, ".", ref name))
                                return GetType(ref name, o.Value, o.Value);
                    }
                }
                else
                    try
                    {
                        foreach (var item in rootType.GetProperties())
                            if (NestedOf(item.Name, ".", ref name))
                                return GetType(ref name, item.PropertyType, item.GetValue(obj));
                        foreach (var item in rootType.GetFields())
                            if (NestedOf(item.Name, ".", ref name))
                                return GetType(ref name, item.FieldType, item.GetValue(obj));
                        foreach (var item in rootType.GetEvents())
                            if (NestedOf(item.Name, ".", ref name))
                                return GetType(ref name, item.EventHandlerType, null);
                        if (name.Contains("()"))
                            foreach (var item in rootType.GetMethods())
                                if (NestedOf(item.Name, "()", ref name))
                                    return GetType(ref name, item.ReturnType, null);
                    }
                    catch { }
            return rootType;
        }
        public static bool NestedOf(string word, string nestSeparator, ref string nest)
        {
            if (nest == word)
            {
                nest = "";
                return true;
            }
            if (nest.StartsWith(word + nestSeparator))
            {
                nest = nest.Substring(word.Length + nestSeparator.Length);
                return true;
            }
            return false;
        }


        public static object Object(object obj = null) => obj == null ? new Object() : (object)obj;
        public static bool Bool(object obj = null) => obj == null ? false : Convert.ToBoolean(obj);
        public static short Short(object obj = null) => obj == null ? new Int16() : Convert.ToInt16(obj);
        public static int Int(object obj = null) => obj == null ? new Int32() : Convert.ToInt32(obj);
        public static long Long(object obj = null) => obj == null ? new Int64() : Convert.ToInt64(obj);
        public static float Float(object obj = null) => obj == null ? new Single() : Convert.ToSingle(obj);
        public static double Double(object obj = null) => obj == null ? new Double() : Convert.ToDouble(obj);
        public static decimal Decimal(object obj = null) => obj == null ? new Decimal() : Convert.ToDecimal(obj);
        public static char Char(object obj = null) => obj == null ? ' ' : Convert.ToChar(obj);
        public static string String(object obj = null) => obj == null ? string.Empty : Convert.ToString(obj);
        public static IEnumerable<object> IEnumerable(object obj = null) => obj == null? (new object[]{}).AsEnumerable(): InterpreterBase.ToEnumerable(obj);
        public static IEnumerable<T> IEnumerable<T>(T type, int capasity) => new T[capasity].AsEnumerable();
        public static IEnumerable<T> IEnumerable<T>(T type, object obj) => obj == null ? (new T[] { }).AsEnumerable() : InterpreterBase.ToEnumerable(obj,o=>(T)o);
        public static IEnumerable<T> IEnumerable<T>(object obj, dynamic func) => InterpreterBase.ToEnumerable(obj,o=> (T)func(o));
        public static Matrix<T> Matrix<T>(T obj, int m = -1, int n =-1) => new Matrix<T>(m,n<0?m:n,obj);
        public static Dictionary<object, object> Dictionary(object obj = null) => InterpreterBase.ToDictionary(obj);
        public static Dictionary<TKey, TValue> Dictionary<TKey,TValue>(TKey key, TValue value) => new Dictionary<TKey, TValue>();
        public static Dictionary<TKey, TValue> Dictionary<TKey,TValue>(object obj,dynamic keyFunc, dynamic valueFunc) => InterpreterBase.ToDictionary(obj, o => (TKey)keyFunc(o), o => (TValue)keyFunc(o));
        public static SortedDictionary<object, object> SortedDictionary(object obj = null) => InterpreterBase.ToSortedDictionary(obj);
        public static SortedDictionary<TKey, TValue> SortedDictionary<TKey, TValue>(TKey key, TValue value) => new SortedDictionary<TKey, TValue>();
        public static SortedDictionary<TKey, TValue> SortedDictionary<TKey, TValue>(object obj, dynamic keyFunc, dynamic valueFunc) => InterpreterBase.ToSortedDictionary(obj, o => (TKey)keyFunc(o), o => (TValue)keyFunc(o)); 
        public static KeyValuePair<TKey, TValue> KeyValuePair<TKey,TValue>(TKey key, TValue value) => new KeyValuePair<TKey, TValue>(key, value);
        public static List<object> List(object obj = null) => IEnumerable(obj).ToList();
        public static List<T> List<T>(T type, int capasity) => new List<T>(capasity);
        public static List<T> List<T>(T type, object obj) => IEnumerable<T>(type,obj).ToList();
        public static List<T> List<T>(object obj, dynamic func) => IEnumerable<T>(obj,func).ToList();
        public static LinkedList<object> LinkedList(object obj = null) => new LinkedList<object>(IEnumerable(obj));
        public static LinkedList<T> LinkedList<T>(T type, int capasity) => new LinkedList<T>();
        public static LinkedList<T> LinkedList<T>(T type, object obj) => new LinkedList<T>(IEnumerable<T>(type,obj));
        public static LinkedList<T> LinkedList<T>(object obj, dynamic func) => new LinkedList<T>(IEnumerable<T>(obj,func));
        public static LinkedListNode<T> LinkedListNode<T>(T obj) => new LinkedListNode<T>(obj);
        public static Stack<object> Stack(object obj = null) => new Stack<object>(IEnumerable(obj));
        public static Stack<T> Stack<T>(T type, int capasity) => new Stack<T>(capasity);
        public static Stack<T> Stack<T>(T type, object obj) => new Stack<T>(IEnumerable<T>(type,obj));
        public static Stack<T> Stack<T>(object obj, dynamic func) => new Stack<T>(IEnumerable<T>(obj, func));
        public static Queue<object> Queue(object obj = null) => new Queue<object>(IEnumerable(obj));
        public static Queue<T> Queue<T>(T type, int capasity) => new Queue<T>(capasity); 
        public static Queue<T> Queue<T>(T type, object obj) => new Queue<T>(IEnumerable<T>(type,obj));
        public static Queue<T> Queue<T>(object obj, dynamic func) => new Queue<T>(IEnumerable<T>(obj, func)); 
        public static object[] Array(object obj = null) => IEnumerable(obj).ToArray();
        public static T[] Array<T>(T type, int capasity) => new T[capasity];
        public static T[] Array<T>(T type, object obj) => IEnumerable<T>(type,obj).ToArray();
        public static T[] Array<T>(object obj, dynamic func) => IEnumerable<T>(obj, func).ToArray();
        public static Font Font(string fontFamily, float size = 8.25f) => new Font(fontFamily, size);
        public static Color Color(int r, int g, int b, int a = 255) => System.Drawing.Color.FromArgb(a,r,g,b);
        public static Color Color(Color color, int a) => System.Drawing.Color.FromArgb(a, color);
        public static Color Color(int c = 0) => System.Drawing.Color.FromArgb(c);
        public static Color Color(string name) => System.Drawing.Color.FromKnownColor(ConvertService.ToEnum<KnownColor>(name));
        public static Size Size(int width = 0, int height = 0) => new Size(width, height);
        public static Point Point(int x = 0, int y = 0) => new Point(x, y);
        public static LongPoint LongPoint(long x = 0, long y = 0) => new LongPoint(x, y);
        public static Location Location(int x = 0, int y = 0, int z = 0) => new Location(x, y, z);
        public static LongLocation LongLocation(long x = 0, long y = 0, long z = 0) => new LongLocation(x, y, z);
        public static Icon Icon(object obj = null)
        {
            if (obj == null) return null;
            Image img = Image(obj);
            if (img == null) return null;
            return ConvertService.ToIcon(img);
        }
        public static Image Image(object obj = null)
        {
            if (obj == null) return null;
            if (obj is System.IO.Stream) return System.Drawing.Image.FromStream(obj as System.IO.Stream);
            if (obj is string && System.IO.File.Exists(obj as string)) return System.Drawing.Image.FromFile(obj as string);
            return null;
        }
        public static string Address(object obj = null)
        {
            if (obj == null) return Config.ApplicationDirectory;
            if (obj is string) return PathService.GetFullAddress(obj+"");
            return null;
        }
        public static ChainedFile Document() => new ChainedFile();
        public static ChainedFile Document(string path, bool cie = true) => new ChainedFile(path, cie);
        public static MatrixFile<T> GenericMatrixFile<T>(T d) => new MatrixFile<T>(Document(), d, d + "");
    }
}
