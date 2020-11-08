using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace TitleParser
{
    interface Setting
    {
        string Read(string key, string section);
    }
    
    public class IniFile : Setting
    {
        string Path;
        
        // for UTF-8
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);

        public IniFile(string IniPath = null)
        {
            Path = IniPath;
        }

        public string Read(string Key, string Section = null)
        {
            return ReadString(Section, Key, "", Path);
        }

        private static byte[] getBytes(string s, string encodingName)
        {
            return null == s ? null : Encoding.GetEncoding(encodingName).GetBytes(s);
        }

        public static string ReadString(string section, string key, string def, string fileName, string encodingName = "utf-8", int size = 1024)
        {
            byte[] buffer = new byte[size];
            int count = GetPrivateProfileString(getBytes(section, encodingName), getBytes(key, encodingName), getBytes(def, encodingName), buffer, size, fileName);
            return Encoding.GetEncoding(encodingName).GetString(buffer, 0, count).Trim();
        }
    }

    public class JsonFile : Setting
    {
        string context;

        public JsonFile(string path)
        {
            context = File.ReadAllText(path);
        }

        public string Read(string Key, string Section = null)
        {
            dynamic json = JValue.Parse(context);
            var val = json.Property(Key)?.Value;
            return (string)val;
        }
    }
}
