using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleParser
{
    public interface IMyReader
    {
        void Read(StreamReader sr);
    }

    public abstract class MyReader : IMyReader
    {
        public string id;

        public abstract void Read(StreamReader sr);
    }

    public class TbTitle : MyReader
    {
        public string[] str = new string[3];

        public override void Read(StreamReader sr)
        {
            for (int j = 0; j < 3; j++)
                str[j] = sr.ReadLine();
        }
    }

    public class TbTitleInfo : MyReader
    {
        public string[] str = new string[22];

        public override void Read(StreamReader sr)
        {
            for (int j = 0; j < 22; j++)
                str[j] = sr.ReadLine();
        }
    }

    public interface WriteFormat
    {
        void WriteHead(StreamWriter sw, string title);
        void WriteTail(StreamWriter sw);
        void WriteBegin(StreamWriter sw);
        void WriteEnd(StreamWriter sw);
        void WriteNumber(StreamWriter sw, string number);
        void WriteCharacter(StreamWriter sw, string name);
        void WriteTitle(StreamWriter sw, string title);
        void WriteDescription(StreamWriter sw, string text);
        void WriteObtain(StreamWriter sw, string method);
        void WriteNoEffect(StreamWriter sw);
        void WriteEffect(StreamWriter sw, string[] effects);
        void Write(StreamWriter sw, string str);
    }

    public class WriteText : WriteFormat
    {
        public void WriteHead(StreamWriter sw, string title) { }
        public void WriteTail(StreamWriter sw) { }
        public void WriteBegin(StreamWriter sw) { }
        public void WriteEnd(StreamWriter sw)
        {
            sw.WriteLine($"");
        }

        public void WriteNumber(StreamWriter sw, string number)
        {
            sw.WriteLine($"編號：{number}");
        }

        public void WriteCharacter(StreamWriter sw, string name)
        {
            sw.WriteLine($"角色：{name}");
        }

        public void WriteTitle(StreamWriter sw, string title)
        {
            sw.WriteLine($"名稱：{title}");
        }

        public void WriteDescription(StreamWriter sw, string text)
        {
            sw.WriteLine($"說明：{text}");
        }

        public void WriteObtain(StreamWriter sw, string method)
        {
            sw.WriteLine($"獲得方法：{method}");
        }

        public void WriteNoEffect(StreamWriter sw)
        {
            sw.WriteLine($"效果：無");
        }

        public void WriteEffect(StreamWriter sw, string [] effects)
        {
            sw.WriteLine($"效果：");
            foreach (var eff in effects)
                sw.WriteLine(eff);
        }

        public void Write(StreamWriter sw, string str)
        {
            sw.WriteLine(str);
        }
    }
    
    public class WriteCSV : WriteFormat
    {
        public void WriteHead(StreamWriter sw, string title) { }
        public void WriteTail(StreamWriter sw) { }
        public void WriteBegin(StreamWriter sw) { }
        public void WriteEnd(StreamWriter sw)
        {
            sw.WriteLine($"");
        }

        public void WriteNumber(StreamWriter sw, string number)
        {
            sw.Write($"{number}, ");
        }

        public void WriteCharacter(StreamWriter sw, string name)
        {
            sw.Write($"{name}, ");
        }

        public void WriteTitle(StreamWriter sw, string title)
        {
            sw.Write($"{title}, ");
        }

        public void WriteDescription(StreamWriter sw, string text)
        {
            sw.Write($"{text}, ");
        }

        public void WriteObtain(StreamWriter sw, string method)
        {
            sw.Write($"{method}, ");
        }

        public void WriteNoEffect(StreamWriter sw)
        {
            sw.Write($"無, ");
        }

        public void WriteEffect(StreamWriter sw, string[] effects)
        {
            sw.Write(String.Join("; ", effects));
        }

        public void Write(StreamWriter sw, string str)
        {
            sw.WriteLine(str);
        }
    }

    public class WriteHTML : WriteFormat
    {
        public void WriteHead(StreamWriter sw, string title)
        {
            sw.WriteLine($"<!DOCTYPE html>");
            sw.WriteLine($"<html>");
            sw.WriteLine($"<head>");
            sw.WriteLine($"<title>{title}</title>");
            sw.WriteLine($"</head>");
            sw.WriteLine($"<body>");
            sw.WriteLine($"<table cellpadding=\"3\" border = '1'>");
            sw.WriteLine($"<tr><td>ID</td><td>腳色</td><td>稱號</td><td>描述</td><td>獲得方法</td><td>屬性</td></tr>");
        }

        public void WriteTail(StreamWriter sw)
        {
            sw.WriteLine($"</table>");
            sw.WriteLine($"</body>");
            sw.WriteLine($"</html>");
        }

        public void WriteBegin(StreamWriter sw)
        {
            sw.WriteLine($"<tr>");
        }

        public void WriteEnd(StreamWriter sw)
        {
            sw.WriteLine($"</tr>");
        }

        public void WriteNumber(StreamWriter sw, string number)
        {
            sw.Write($"<td>{number}</td>");
        }

        public void WriteCharacter(StreamWriter sw, string name)
        {
            sw.Write($"<td>{name}</td>");
        }

        public void WriteTitle(StreamWriter sw, string title)
        {
            sw.Write($"<td>{title}</td>");
        }

        public void WriteDescription(StreamWriter sw, string text)
        {
            text = text.Replace(@"\n\n", "<br>");
            text = text.Replace(@"\n", "<br>");
            sw.Write($"<td>{text}</td>");
        }

        public void WriteObtain(StreamWriter sw, string method)
        {
            sw.Write($"<td>{method}</td>");
        }

        public void WriteNoEffect(StreamWriter sw)
        {
            sw.Write($"<td>無</td>");
        }

        public void WriteEffect(StreamWriter sw, string[] effects)
        {
            sw.Write($"<td>");
            foreach (var eff in effects)
                sw.Write($"{eff}<br>");
            sw.Write($"</td>");
        }

        public void Write(StreamWriter sw, string str)
        {
            sw.WriteLine(str);
        }
    }

    public struct WritePackage
    {
        public WriteFormat method;
        public string ext;
    }

    public class WritePackageFactory
    {
        public enum Type
        {
            TEXT,
            CSV,
            HTML,
        }

        public static WritePackage GetInstance(Type type)
        {
            switch (type)
            {
                default:
                case Type.TEXT: return new WritePackage { method = new WriteText(), ext = "txt" };
                case Type.CSV: return new WritePackage { method = new WriteCSV(), ext = "csv" };
                case Type.HTML: return new WritePackage { method = new WriteHTML(), ext = "html" };
            }
        }
    }
}
