using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaParser
{
    public class MyStreamWriter
    {
        public delegate void Func();

        private string mFilename;

        public MyStreamWriter(string filename)
        {
            mFilename = filename;
            if (!File.Exists(mFilename))
            {
                using (StreamWriter sw = File.CreateText(mFilename))
                { }
            }
            else
            {
                using (StreamWriter outputFile = new StreamWriter(mFilename))
                { }
            }
        }

        public void AppendLine(string str)
        {
            using (StreamWriter outputFile = File.AppendText(mFilename))
            {
                outputFile.WriteLine(str);
            }
        }
    }
}
