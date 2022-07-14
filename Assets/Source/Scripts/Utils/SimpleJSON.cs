#if !UNITY_WEBPLAYER
#define USE_FileIO
#endif

/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Features / attributes:
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser ignores data types. Each value is a string.
 * - only double quotes (") are used for quoting strings.
 * - values and names are not restricted to quoted strings. They simply add up and are trimmed.
 * - There are only 3 types: arrays(JSONArray), objects(JSONClass) and values(JSONData)
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser try to avoid errors, but if malformed JSON is parsed the result is undefined
 * 
 * 
 * 2012-12-17 Update:
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * * * * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Utility
{
    public enum JSONBinaryTag
    {
        Array = 1,
        Class = 2,
        Value = 3,
        IntValue = 4,
        DoubleValue = 5,
        BoolValue = 6,
        FloatValue = 7
    }

    public class JSONNode
    {
        #region common interface

        public virtual void Add(string aKey, JSONNode aItem)
        {
        }

        public virtual JSONNode this[int aIndex]
        {
            get => null;
            set { }
        }

        public virtual JSONNode this[string aKey]
        {
            get => null;
            set { }
        }

        public virtual string Value
        {
            get => "";
            set { }
        }

        public virtual int Count => 0;

        public virtual void Add(JSONNode aItem)
        {
            Add("", aItem);
        }

        public virtual JSONNode Remove(string aKey)
        {
            return null;
        }

        public virtual JSONNode Remove(int aIndex)
        {
            return null;
        }

        public virtual JSONNode Remove(JSONNode aNode)
        {
            return aNode;
        }

        public virtual IEnumerable<JSONNode> Childs
        {
            get { yield break; }
        }

        public IEnumerable<JSONNode> DeepChilds
        {
            get
            {
                foreach (var C in Childs)
                    foreach (var D in C.DeepChilds)
                        yield return D;
            }
        }

        public override string ToString()
        {
            return "JSONNode";
        }

        public virtual string ToString(string aPrefix)
        {
            return "JSONNode";
        }

        #endregion common interface

        #region typecasting properties

        public virtual int AsInt
        {
            get
            {
                var v = 0;
                if (int.TryParse(Value, out v))
                    return v;
                return 0;
            }
            set => Value = value.ToString();
        }

        public virtual float AsFloat
        {
            get
            {
                var v = 0.0f;
                if (float.TryParse(Value, out v))
                    return v;
                return 0.0f;
            }
            set => Value = value.ToString();
        }

        public virtual double AsDouble
        {
            get
            {
                var v = 0.0;
                if (double.TryParse(Value, out v))
                    return v;
                return 0.0;
            }
            set => Value = value.ToString();
        }

        public virtual bool AsBool
        {
            get
            {
                var v = false;
                if (bool.TryParse(Value, out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set => Value = value ? "true" : "false";
        }

        public virtual JSONArray AsArray => this as JSONArray;

        public virtual JSONClass AsObject => this as JSONClass;

        #endregion typecasting properties

        #region operators

        public static implicit operator JSONNode(string s)
        {
            return new JSONData(s);
        }

        public static implicit operator string(JSONNode d)
        {
            return d == null ? null : d.Value;
        }

        public static bool operator ==(JSONNode a, object b)
        {
            if (b == null && a is JSONLazyCreator)
                return true;
            return ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion operators

        internal static string Escape(string aText)
        {
            var result = "";
            foreach (var c in aText)
                switch (c)
                {
                    case '\\':
                        result += "\\\\";
                        break;
                    case '\"':
                        result += "\\\"";
                        break;
                    case '\n':
                        result += "\\n";
                        break;
                    case '\r':
                        result += "\\r";
                        break;
                    case '\t':
                        result += "\\t";
                        break;
                    case '\b':
                        result += "\\b";
                        break;
                    case '\f':
                        result += "\\f";
                        break;
                    default:
                        result += c;
                        break;
                }
            return result;
        }

        public static JSONNode Parse(string aJSON)
        {
            var stack = new Stack<JSONNode>();
            JSONNode ctx = null;
            var i = 0;
            var token = "";
            var tokenName = "";
            var quoteMode = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        if (quoteMode)
                        {
                            token += aJSON[i];
                            break;
                        }

                        stack.Push(new JSONClass());
                        if (ctx != null)
                        {
                            tokenName = tokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (tokenName != "")
                                ctx.Add(tokenName, stack.Peek());
                        }

                        tokenName = "";
                        token = "";
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (quoteMode)
                        {
                            token += aJSON[i];
                            break;
                        }

                        stack.Push(new JSONArray());
                        if (ctx != null)
                        {
                            tokenName = tokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (tokenName != "")
                                ctx.Add(tokenName, stack.Peek());
                        }

                        tokenName = "";
                        token = "";
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (quoteMode)
                        {
                            token += aJSON[i];
                            break;
                        }

                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (token != "")
                        {
                            tokenName = tokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(token);
                            else if (tokenName != "")
                                ctx.Add(tokenName, token);
                        }

                        tokenName = "";
                        token = "";
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (quoteMode)
                        {
                            token += aJSON[i];
                            break;
                        }

                        tokenName = token;
                        token = "";
                        break;

                    case '"':
                        quoteMode ^= true;
                        break;

                    case ',':
                        if (quoteMode)
                        {
                            token += aJSON[i];
                            break;
                        }

                        if (token != "")
                        {
                            if (ctx is JSONArray)
                                ctx.Add(token);
                            else if (tokenName != "")
                                ctx.Add(tokenName, token);
                        }

                        tokenName = "";
                        token = "";
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (quoteMode)
                            token += aJSON[i];
                        break;

                    case '\\':
                        ++i;
                        if (quoteMode)
                        {
                            var C = aJSON[i];
                            switch (C)
                            {
                                case 't':
                                    token += '\t';
                                    break;
                                case 'r':
                                    token += '\r';
                                    break;
                                case 'n':
                                    token += '\n';
                                    break;
                                case 'b':
                                    token += '\b';
                                    break;
                                case 'f':
                                    token += '\f';
                                    break;
                                case 'u':
                                {
                                    var s = aJSON.Substring(i + 1, 4);
                                    token += (char) int.Parse(s, NumberStyles.AllowHexSpecifier);
                                    i += 4;
                                    break;
                                }
                                default:
                                    token += C;
                                    break;
                            }
                        }

                        break;

                    default:
                        token += aJSON[i];
                        break;
                }

                ++i;
            }

            if (quoteMode) throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            return ctx;
        }

        public virtual void Serialize(BinaryWriter aWriter)
        {
        }

        public void SaveToStream(Stream aData)
        {
            var W = new BinaryWriter(aData);
            Serialize(W);
        }

#if USE_SharpZipLib
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
            {
                gzipOut.IsStreamOwner = false;
                SaveToStream(gzipOut);
                gzipOut.Close();
            }
        }
 
        public void SaveToCompressedFile(string aFileName)
        {
            #if USE_FileIO
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using(var F = System.IO.File.OpenWrite(aFileName))
            {
                SaveToCompressedStream(F);
            }
            #else
            throw new Exception("Can't use File IO stuff in webplayer");
            #endif
        }
        public string SaveToCompressedBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                SaveToCompressedStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }
 
#else
        public void SaveToCompressedStream(Stream aData)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public string SaveToCompressedBase64()
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public void SaveToFile(string aFileName)
        {
#if USE_FileIO
            Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
            using (var F = File.OpenWrite(aFileName))
            {
                SaveToStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }

        public string SaveToBase64()
        {
            using (var stream = new MemoryStream())
            {
                SaveToStream(stream);
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static JSONNode Deserialize(BinaryReader aReader)
        {
            var type = (JSONBinaryTag) aReader.ReadByte();
            switch (type)
            {
                case JSONBinaryTag.Array:
                {
                    var count = aReader.ReadInt32();
                    var tmp = new JSONArray();
                    for (var i = 0; i < count; i++)
                        tmp.Add(Deserialize(aReader));
                    return tmp;
                }
                case JSONBinaryTag.Class:
                {
                    var count = aReader.ReadInt32();
                    var tmp = new JSONClass();
                    for (var i = 0; i < count; i++)
                    {
                        var key = aReader.ReadString();
                        var val = Deserialize(aReader);
                        tmp.Add(key, val);
                    }

                    return tmp;
                }
                case JSONBinaryTag.Value:
                {
                    return new JSONData(aReader.ReadString());
                }
                case JSONBinaryTag.IntValue:
                {
                    return new JSONData(aReader.ReadInt32());
                }
                case JSONBinaryTag.DoubleValue:
                {
                    return new JSONData(aReader.ReadDouble());
                }
                case JSONBinaryTag.BoolValue:
                {
                    return new JSONData(aReader.ReadBoolean());
                }
                case JSONBinaryTag.FloatValue:
                {
                    return new JSONData(aReader.ReadSingle());
                }

                default:
                {
                    throw new Exception("Error deserializing JSON. Unknown tag: " + type);
                }
            }
        }

#if USE_SharpZipLib
        public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
            return LoadFromStream(zin);
        }
        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
            #if USE_FileIO
            using(var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromCompressedStream(F);
            }
            #else
            throw new Exception("Can't use File IO stuff in webplayer");
            #endif
        }
        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromCompressedStream(stream);
        }
#else
        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JSONNode LoadFromCompressedStream(Stream aData)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public static JSONNode LoadFromStream(Stream aData)
        {
            using (var R = new BinaryReader(aData))
            {
                return Deserialize(R);
            }
        }

        public static JSONNode LoadFromFile(string aFileName)
        {
#if USE_FileIO
            using (var F = File.OpenRead(aFileName))
            {
                return LoadFromStream(F);
            }
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }

        public static JSONNode LoadFromBase64(string aBase64)
        {
            var tmp = Convert.FromBase64String(aBase64);
            var stream = new MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromStream(stream);
        }
    } // End of JSONNode

    public class JSONArray : JSONNode, IEnumerable
    {
        #region Fields

        private readonly List<JSONNode> m_List = new List<JSONNode>();

        #endregion

        #region Public Properties

        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (var N in m_List)
                    yield return N;
            }
        }

        public override int Count => m_List.Count;

        #endregion

        #region Public Indexers

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return new JSONLazyCreator(this);
                return m_List[aIndex];
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    m_List.Add(value);
                else
                    m_List[aIndex] = value;
            }
        }

        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this);
            set => m_List.Add(value);
        }

        #endregion

        #region Public Methods and Operators

        public override void Add(string aKey, JSONNode aItem)
        {
            m_List.Add(aItem);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var N in m_List)
                yield return N;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return null;
            var tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }

        public override void Serialize(BinaryWriter aWriter)
        {
            aWriter.Write((byte) JSONBinaryTag.Array);
            aWriter.Write(m_List.Count);
            for (var i = 0; i < m_List.Count; i++) m_List[i].Serialize(aWriter);
        }

        public override string ToString()
        {
            var result = "[ ";
            foreach (var N in m_List)
            {
                if (result.Length > 2)
                    result += ", ";
                result += N.ToString();
            }

            result += " ]";
            return result;
        }

        public override string ToString(string aPrefix)
        {
            var result = "[ ";
            foreach (var N in m_List)
            {
                if (result.Length > 3)
                    result += ", ";
                result += "\n" + aPrefix + "   ";
                result += N.ToString(aPrefix + "   ");
            }

            result += "\n" + aPrefix + "]";
            return result;
        }

        #endregion
    } // End of JSONArray

    public class JSONClass : JSONNode, IEnumerable
    {
        #region Fields

        private readonly Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

        #endregion

        #region Public Properties

        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (var N in m_Dict)
                    yield return N.Value;
            }
        }

        public override int Count => m_Dict.Count;

        #endregion

        #region Public Indexers

        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey))
                    return m_Dict[aKey];
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = value;
                else
                    m_Dict.Add(aKey, value);
            }
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return null;
                return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return;
                var key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Add(string aKey, JSONNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = aItem;
                else
                    m_Dict.Add(aKey, aItem);
            }
            else
            {
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var N in m_Dict)
                yield return N;
        }

        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
                return null;
            var tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            var item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                var item = m_Dict.Where(k => k.Value == aNode).First();
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override void Serialize(BinaryWriter aWriter)
        {
            aWriter.Write((byte) JSONBinaryTag.Class);
            aWriter.Write(m_Dict.Count);
            foreach (var K in m_Dict.Keys)
            {
                aWriter.Write(K);
                m_Dict[K].Serialize(aWriter);
            }
        }

        public override string ToString()
        {
            var result = "{";
            foreach (var N in m_Dict)
            {
                if (result.Length > 2)
                    result += ", ";
                result += "\"" + Escape(N.Key) + "\":" + N.Value;
            }

            result += "}";
            return result;
        }

        public override string ToString(string aPrefix)
        {
            var result = "{ ";
            foreach (var N in m_Dict)
            {
                if (result.Length > 3)
                    result += ", ";
                result += "\n" + aPrefix + "   ";
                result += "\"" + Escape(N.Key) + "\" : " + N.Value.ToString(aPrefix + "   ");
            }

            result += "\n" + aPrefix + "}";
            return result;
        }

        #endregion
    } // End of JSONClass

    public class JSONData : JSONNode
    {
        #region Fields

        private bool isNotString;
        private string m_Data;

        #endregion

        #region Constructors and Destructors

        public JSONData(string aData)
        {
            m_Data = aData;
            isNotString = false;
        }

        public JSONData(float aData)
        {
            AsFloat = aData;
            isNotString = true;
        }

        public JSONData(double aData)
        {
            AsDouble = aData;
            isNotString = true;
        }

        public JSONData(bool aData)
        {
            AsBool = aData;
            isNotString = true;
        }

        public JSONData(int aData)
        {
            AsInt = aData;
            isNotString = true;
        }

        #endregion

        #region Public Properties

        public override string Value
        {
            get => m_Data;
            set
            {
                m_Data = value;
                isNotString = false;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override void Serialize(BinaryWriter aWriter)
        {
            var tmp = new JSONData("");

            tmp.AsInt = AsInt;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte) JSONBinaryTag.IntValue);
                aWriter.Write(AsInt);
                return;
            }

            tmp.AsFloat = AsFloat;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte) JSONBinaryTag.FloatValue);
                aWriter.Write(AsFloat);
                return;
            }

            tmp.AsDouble = AsDouble;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte) JSONBinaryTag.DoubleValue);
                aWriter.Write(AsDouble);
                return;
            }

            tmp.AsBool = AsBool;
            if (tmp.m_Data == m_Data)
            {
                aWriter.Write((byte) JSONBinaryTag.BoolValue);
                aWriter.Write(AsBool);
                return;
            }

            aWriter.Write((byte) JSONBinaryTag.Value);
            aWriter.Write(m_Data);
        }

        public override string ToString()
        {
            if (isNotString)
                return Escape(m_Data);
            return "\"" + Escape(m_Data) + "\"";
        }

        public override string ToString(string aPrefix)
        {
            if (isNotString)
                return Escape(m_Data);
            return "\"" + Escape(m_Data) + "\"";
        }

        #endregion
    } // End of JSONData

    internal class JSONLazyCreator : JSONNode
    {
        #region Fields

        private readonly string m_Key;
        private JSONNode m_Node;

        #endregion

        #region Constructors and Destructors

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }

        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        #endregion

        #region Public Properties

        public override JSONArray AsArray
        {
            get
            {
                var tmp = new JSONArray();
                Set(tmp);
                return tmp;
            }
        }

        public override bool AsBool
        {
            get
            {
                var tmp = new JSONData(false);
                Set(tmp);
                return false;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override double AsDouble
        {
            get
            {
                var tmp = new JSONData(0.0);
                Set(tmp);
                return 0.0;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override float AsFloat
        {
            get
            {
                var tmp = new JSONData(0.0f);
                Set(tmp);
                return 0.0f;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override int AsInt
        {
            get
            {
                var tmp = new JSONData(0);
                Set(tmp);
                return 0;
            }
            set
            {
                var tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override JSONClass AsObject
        {
            get
            {
                var tmp = new JSONClass();
                Set(tmp);
                return tmp;
            }
        }

        #endregion

        #region Public Indexers

        public override JSONNode this[int aIndex]
        {
            get => new JSONLazyCreator(this);
            set
            {
                var tmp = new JSONArray();
                tmp.Add(value);
                Set(tmp);
            }
        }

        public override JSONNode this[string aKey]
        {
            get => new JSONLazyCreator(this, aKey);
            set
            {
                var tmp = new JSONClass();
                tmp.Add(aKey, value);
                Set(tmp);
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool operator ==(JSONLazyCreator a, object b)
        {
            if (b == null)
                return true;
            return ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }

        public override void Add(JSONNode aItem)
        {
            var tmp = new JSONArray();
            tmp.Add(aItem);
            Set(tmp);
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            var tmp = new JSONClass();
            tmp.Add(aKey, aItem);
            Set(tmp);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return true;
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "";
        }

        public override string ToString(string aPrefix)
        {
            return "";
        }

        #endregion

        #region Methods

        private void Set(JSONNode aVal)
        {
            if (m_Key == null)
                m_Node.Add(aVal);
            else
                m_Node.Add(m_Key, aVal);
            m_Node = null; // Be GC friendly.
        }

        #endregion
    } // End of JSONLazyCreator

    public static class JSON
    {
        #region Public Methods and Operators

        public static JSONNode Parse(string aJSON)
        {
            return JSONNode.Parse(aJSON);
        }

        #endregion
    }
}