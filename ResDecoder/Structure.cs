using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResDecoder
{
    public class Envirment
    {
        public static int NumberOfCharacter = 0;
    }

    public interface IResReader
    {
        void Read(FileStream fs);

        void Write(StringBuilder sb);
    }

    public abstract class ResReader : IResReader
    {
        public abstract void Read(FileStream fs);
        public abstract void Write(StringBuilder sb);

        private byte[] GetNumberOfBytes(FileStream fs, int count)
        {
            byte[] buff = new byte[count];
            fs.Read(buff, 0, count);
            return buff;
        }

        public byte GetByte(FileStream fs)
        {
            return GetNumberOfBytes(fs, 1)[0];
        }
        public ushort GetUshort(FileStream fs)
        {
            var buff = GetNumberOfBytes(fs, 2);
            return BitConverter.ToUInt16(buff, 0);
        }

        public UInt32 GetUint(FileStream fs)
        {
            var buff = GetNumberOfBytes(fs, 4);
            return BitConverter.ToUInt32(buff, 0);
        }

        public float GetFloat(FileStream fs)
        {
            var buff = GetNumberOfBytes(fs, 4);
            return BitConverter.ToSingle(buff, 0);
        }

        public string GetStringByLength(FileStream fs, int length)
        {
            var buff = GetNumberOfBytes(fs, length * 2);
            var ret = Encoding.Unicode.GetString(buff);
            ret = ret.Replace("\n", "\\n");
            return ret;
        }

        public string GetString(FileStream fs)
        {
            var length = GetUshort(fs);
            return GetStringByLength(fs, length);
        }

        public object GetBytes<T>(FileStream fs, T t)
        {
            Type type = typeof(T);
            if (type == typeof(uint))
            {
                var buff = GetNumberOfBytes(fs, 4);
                return BitConverter.ToUInt32(buff, 0);
            }

            { // else if type is ushort
                var buff = GetNumberOfBytes(fs, 2);
                return BitConverter.ToUInt16(buff, 0);
            }
        }
    }

    public class EntryCount : ResReader
    {
        public uint count = 0;

        override public void Read(FileStream fs)
        {
            count = GetUint(fs);
        }

        override public void Write(StringBuilder sb)
        { }
    }

    public class SimpleMulitString<T> : ResReader
    {
        T index;

        int arrayLength;
        string[] str;

        public SimpleMulitString(int length)
        {
            arrayLength = length;
            str = new string[arrayLength];
        }

        override public void Read(FileStream fs)
        {
            T type = default(T);
            index = (T)GetBytes(fs, type);
            for (int i = 0; i < arrayLength; i++)
            {
                str[i] = GetString(fs);
            }
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={index}");
            for (int i = 0; i < arrayLength; i++)
                sb.AppendLine(str[i]);

            // 換行
            sb.AppendLine("");
        }
    }

    public class SimpleSingleString<T> : SimpleMulitString<T>
    {
        public SimpleSingleString()
            :base(1)
        { }
    }

    public class TbAchievementScript : SimpleMulitString<uint>
    {
        public TbAchievementScript()
            : base(2)
        { }
    }

    public class TbAkashicParts : SimpleMulitString<ushort>
    {
        public TbAkashicParts()
            : base(2)
        { }
    }

    public class TbAppearance : ResReader
    {
        uint i1;
        byte b1;
        byte b2;
        ushort s1;
        ushort s2;
        string str1;

        override public void Read(FileStream fs)
        {
            i1 = GetUint(fs);
            b1 = GetByte(fs);
            b2 = GetByte(fs);
            s1 = GetUshort(fs);
            s2 = GetUshort(fs);
            str1 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i1}");
            sb.AppendLine($"NUM1={b1}");
            sb.AppendLine($"NUM2={b2}");
            sb.AppendLine($"NUM3={s1}");
            sb.AppendLine($"NUM4={s2}");
            sb.AppendLine($"NUM5={i1}");
            sb.AppendLine(str1);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbBooster : ResReader
    {
        ushort[] s = new ushort[12];
        byte[] b = new byte[18];
        float[] f = new float[9];
        uint i1;

        override public void Read(FileStream fs)
        {
            s[1] = GetUshort(fs);
            s[2] = GetUshort(fs);
            for(int j = 1; j <= 16; j++)
                b[j] = GetByte(fs);
            for (int j = 1; j <= 8; j++)
                f[j] = GetFloat(fs);
            for (int j = 3; j <= 11; j++)
                s[j] = GetUshort(fs);
            b[17] = GetByte(fs);
            i1 = GetUint(fs);
        }

        override public void Write(StringBuilder sb)
        {
            int k = 1;
            sb.AppendLine($"ID={s[1]}");
            sb.AppendLine($"NUM{k++}={s[2]}");
            for (int j = 1; j <= 16; j++)
                sb.AppendLine($"NUM{k++}={b[j]}");
            for (int j = 1; j <= 8; j++)
                sb.AppendLine($"NUM{k++}={f[j]}");
            for (int j = 3; j <= 11; j++)
                sb.AppendLine($"NUM{k++}={s[j]}");
            sb.AppendLine($"NUM{k++}={b[17]}");
            sb.AppendLine($"NUM{k++}={i1}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbBoosterScript : SimpleMulitString<ushort>
    {
        public TbBoosterScript()
            : base(3)
        { }
    }

    public class TbBuffScript : SimpleMulitString<ushort>
    {
        public TbBuffScript()
            : base(4)
        { }
    }

    public class TbCharacterParts : ResReader
    {
        ushort s1;
        ushort s2;
        byte b1;
        string str1;
        string str2;
        string str3;
        string str4;
        string str5;

        override public void Read(FileStream fs)
        {
            s1 = GetUshort(fs);
            s2 = GetUshort(fs);
            b1 = GetByte(fs);
            str1 = GetString(fs);
            str2 = GetString(fs);
            str3 = GetString(fs);
            str4 = GetString(fs);
            str5 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={s1}");
            sb.AppendLine($"NUM1={s2}");
            sb.AppendLine($"NUM2={b1}");
            sb.AppendLine(str1);
            sb.AppendLine(str2);
            sb.AppendLine(str3);
            sb.AppendLine(str4);
            sb.AppendLine(str5);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbChattingFilter : SimpleSingleString<uint>
    {
    }

    public class TbCinemaString : SimpleMulitString<ushort>
    {
        // 這個陣列長度是 角色總人數 + Mob(1個) + NPC(1個) 的長度
        // 台服沒有琪  所以長度是8
        // 美服有琪    所以長度是9
        public TbCinemaString()
            : base(Envirment.NumberOfCharacter + 2)
        {
        }
    }

    public class TbClassForm : SimpleSingleString<uint>
    {
    }

    public class TbTalkString : SimpleSingleString<ushort>
    {
    }

    public class TbClassSpeech : ResReader
    {
        uint[] i = new uint[44];

        override public void Read(FileStream fs)
        {
            int k = 1;
            for (int j = 1; j <= 8; j++)
                i[k++] = GetUint(fs);
            for (int j = 1; j <= 14; j++)
                i[k++] = GetByte(fs);
            for (int j = 9; j <= 15; j++)
                i[k++] = GetUint(fs);
            for (int j = 15; j <= 21; j++)
                i[k++] = GetByte(fs);
            for (int j = 16; j <= 22; j++)
                i[k++] = GetUint(fs);
        }

        override public void Write(StringBuilder sb)
        {
            int k = 1, l = 1;
            sb.AppendLine($"ID={i[1]}");
            //for (int j = 2; j <= 43; j++)
            //    sb.AppendLine($"NUM {k++}={i[j]}");
            for (int j = 2; j <= 43; j++)
                sb.AppendLine($"{i[j]}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbCutsceneString : SimpleSingleString<uint>
    {
    }

    public class TbLoadingImg : ResReader
    {
        ushort s1;
        ushort s2;
        string str1;
        override public void Read(FileStream fs)
        {
            s1 = GetUshort(fs);
            s2 = GetUshort(fs);
            str1 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={s1}");
            sb.AppendLine($"NUM1={s2}");
            sb.AppendLine(str1);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbMazeRewardGoldDirect : ResReader
    {
        uint i1;
        uint i2;
        uint i3;
        uint i4;
        string str1;
        string str2;
        string str3;
        string str4;

        override public void Read(FileStream fs)
        {
            i1 = GetUint(fs);
            i2 = GetUint(fs);
            i3 = GetUint(fs);
            i4 = GetUint(fs);
            str1 = GetString(fs);
            str2 = GetString(fs);
            str3 = GetString(fs);
            str4 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i1}");
            sb.AppendLine($"NUM1={i2}");
            sb.AppendLine($"NUM2={i3}");
            sb.AppendLine($"NUM3={i4}");
            sb.AppendLine(str1);
            sb.AppendLine(str2);
            sb.AppendLine(str3);
            sb.AppendLine(str4);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbMonsterScript : SimpleSingleString<uint>
    {
    }

    public class TbNameFilter : ResReader
    {
        uint i1;
        byte b1;
        string str1;

        override public void Read(FileStream fs)
        {
            i1 = GetUint(fs);
            b1 = GetByte(fs);
            str1 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i1}");
            sb.AppendLine($"NUM1={b1}");
            sb.AppendLine(str1);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbNPCScript : SimpleSingleString<uint>
    {
    }

    public class TbQuestScript : SimpleMulitString<uint>
    {
        public TbQuestScript()
            : base(5)
        { }
    }
    
    public class TbSpeech : ResReader
    {
        uint i1;
        byte b1;
        byte b2;
        byte b3;
        uint i2;
        uint i3;
        uint i4;
        string str1;
        byte b4;
        byte b5;
        byte b6;
        uint i5;
        uint i6;
        uint i7;
        string str2;

        override public void Read(FileStream fs)
        {
            i1 = GetUint(fs);
            b1 = GetByte(fs);
            b2 = GetByte(fs);
            b3 = GetByte(fs);
            i2 = GetUint(fs);
            i3 = GetUint(fs);
            i4 = GetUint(fs);
            str1 = GetString(fs);
            b4 = GetByte(fs);
            b5 = GetByte(fs);
            b6 = GetByte(fs);
            i5 = GetUint(fs);
            i6 = GetUint(fs);
            i7 = GetUint(fs);
            str2 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i1}");
            //sb.AppendLine($"NUM1={b1}");
            //sb.AppendLine($"NUM2={b2}");
            //sb.AppendLine($"NUM3={b3}");
            //sb.AppendLine($"NUM4={i2}");
            //sb.AppendLine($"NUM5={i3}");
            //sb.AppendLine($"NUM6={i4}");
            //sb.AppendLine(str1);
            //sb.AppendLine($"NUM7={b4}");
            //sb.AppendLine($"NUM8={b5}");
            //sb.AppendLine($"NUM9={b6}");
            //sb.AppendLine($"NUM10={i5}");
            //sb.AppendLine($"NUM11={i6}");
            //sb.AppendLine($"NUM12={i6}");
            //sb.AppendLine(str2);
            sb.AppendLine($"{b1}");
            sb.AppendLine($"{b2}");
            sb.AppendLine($"{b3}");
            sb.AppendLine($"{i2}");
            sb.AppendLine($"{i3}");
            sb.AppendLine($"{i4}");
            sb.AppendLine(str1);
            sb.AppendLine($"{b4}");
            sb.AppendLine($"{b5}");
            sb.AppendLine($"{b6}");
            sb.AppendLine($"{i5}");
            sb.AppendLine($"{i6}");
            sb.AppendLine($"{i6}");
            sb.AppendLine(str2);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbSpeechTag : ResReader
    {
        byte b1;
        string str1;
        byte b2;
        uint i1;
        uint i2;
        uint i3;
        uint i4;
        uint i5;
        uint i6;
        uint i7; // for Chii
        uint i8; // for Ephnel

        override public void Read(FileStream fs)
        {
            b1 = GetByte(fs);
            str1 = GetString(fs);
            b2 = GetByte(fs);
            i1 = GetUint(fs);
            i2 = GetUint(fs);
            i3 = GetUint(fs);
            i4 = GetUint(fs);
            i5 = GetUint(fs);
            i6 = GetUint(fs);
            i7 = GetUint(fs);
            i8 = GetUint(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={b1}");
            sb.AppendLine(str1);
            sb.AppendLine($"NUM1={b2}");
            sb.AppendLine($"NUM2={i1}");
            sb.AppendLine($"NUM4={i2}");
            sb.AppendLine($"NUM5={i3}");
            sb.AppendLine($"NUM6={i4}");
            sb.AppendLine($"NUM7={i5}");
            sb.AppendLine($"NUM8={i6}");
            sb.AppendLine($"NUM9={i7}");
            sb.AppendLine($"NUM10={i8}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbSpeechString : SimpleSingleString<uint>
    {
    }
        
    public class TbItemScript : ResReader
    {
        uint i1;
        string str1;
        string str2;
        string str3;
        string str4;
        string str5;
        string str6;
        byte b1;
        byte b2;
        byte b3;
        byte b4;
        byte b5;
        string str7;
        string str8;

        override public void Read(FileStream fs)
        {
            i1 = GetUint(fs);
            str1 = GetString(fs);
            str2 = GetString(fs);
            str3 = GetString(fs);
            str4 = GetString(fs);
            str5 = GetString(fs);
            str6 = GetString(fs);
            b1 = GetByte(fs);
            b2 = GetByte(fs);
            b3 = GetByte(fs);
            b4 = GetByte(fs);
            b5 = GetByte(fs);
            str7 = GetString(fs);
            str8 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i1}");
            sb.AppendLine(str1);
            sb.AppendLine(str2);
            sb.AppendLine(str3);
            sb.AppendLine(str4);
            sb.AppendLine(str5);
            sb.AppendLine(str6);
            sb.AppendLine(str7);
            sb.AppendLine(str8);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbSkill : ResReader
    {
        uint[] i = new uint[101];
        float[] f = new float[6];
        string[] str = new string[11];

        override public void Read(FileStream fs)
        {
            int k = 0, l = 0, m = 0;
            i[k++] = GetUint(fs);
            i[k++] = GetUshort(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUshort(fs);
            f[l++] = GetFloat(fs);
            f[l++] = GetFloat(fs);
            f[l++] = GetFloat(fs);
            f[l++] = GetFloat(fs);
            f[l++] = GetFloat(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUshort(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUshort(fs);
            i[k++] = GetUshort(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUshort(fs);
            i[k++] = GetByte(fs);
            str[m++] = GetString(fs);
            i[k++] = GetUshort(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetByte(fs);
            str[m++] = GetString(fs);
            str[m++] = GetString(fs);
            str[m++] = GetString(fs);
            str[m++] = GetString(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUint(fs);
            i[k++] = GetUshort(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetUshort(fs);
            str[m++] = GetString(fs);
            str[m++] = GetString(fs);
            str[m++] = GetString(fs);
            i[k++] = GetByte(fs);
            str[m++] = GetString(fs);
            str[m++] = GetString(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
            i[k++] = GetByte(fs);
        }

        override public void Write(StringBuilder sb)
        {
            int k = 0, l, m = 0, n = 1;
            sb.AppendLine($"ID={i[k++]}");
            for (; k < 20; k++)
                sb.AppendLine($"{i[k]}");
            for (l = 0; l < 5; l++)
                sb.AppendLine($"{f[l]}");
            for (; k < 50; k++)
                sb.AppendLine($"{i[k]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{i[k]}");
            sb.AppendLine($"{i[k]}");
            sb.AppendLine($"{i[k]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{str[m++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");
            sb.AppendLine($"{i[k++]}");

            //for (; k<20;k++)
            //    sb.AppendLine($"NUM{n++}={i[k]}");
            //for (l = 0; l < 5; l++)
            //    sb.AppendLine($"NUM{n++}={f[l]}");
            //for (; k < 50; k++)
            //    sb.AppendLine($"NUM{n++}={i[k]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"NUM{n++}={i[k]}");
            //sb.AppendLine($"NUM{n++}={i[k]}");
            //sb.AppendLine($"NUM{n++}={i[k]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"{str[m++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");
            //sb.AppendLine($"NUM{n++}={i[k++]}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbSkillScript : ResReader
    {
        ushort s1;
        string str1;
        string str2;
        string str3;
        string str4;
        uint i1;
        uint i2;
        string str5;
        string str6;

        override public void Read(FileStream fs)
        {
            s1 = GetUshort(fs);
            str1 = GetString(fs);
            str2 = GetString(fs);
            str3 = GetString(fs);
            str4 = GetString(fs);
            i1 = GetUint(fs);
            i2 = GetUint(fs);
            str5 = GetString(fs);
            str6 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={s1}");
            sb.AppendLine(str1);
            sb.AppendLine(str2);
            sb.AppendLine(str3);
            sb.AppendLine(str4);
            sb.AppendLine(str5);
            sb.AppendLine(str6);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbChattingCommand : ResReader
    {
        uint i1;
        byte b1;
        string [] str = new string[5];
        uint i2;

        override public void Read(FileStream fs)
        {
            i1 = GetUint(fs);
            b1 = GetByte(fs);
            str[0] = GetString(fs);
            str[1] = GetString(fs);
            str[2] = GetString(fs);
            str[3] = GetString(fs);
            str[4] = GetString(fs);
            i2 = GetUint(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i1}");
            //sb.AppendLine($"NUM1={b1}");
            //sb.AppendLine(str[0]);
            sb.AppendLine(str[1]);
            //sb.AppendLine(str[2]);
            //sb.AppendLine(str[3]);
            //sb.AppendLine(str[4]);
            //sb.AppendLine($"NUM2={i2}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbLeagueSkillScript : SimpleMulitString<ushort>
    {
        public TbLeagueSkillScript()
            : base(5)
        { }
    }

    public class TbShopString : SimpleSingleString<uint>
    {
    }
    
    public class TbSoulMetry : ResReader
    {
        ushort[] s = new ushort[19];
        uint[] i = new uint[7];
        byte[] b = new byte[4];
        string[] str = new string[5];

        override public void Read(FileStream fs)
        {
            for(int j = 0; j < 9; j++)
                s[j] = GetUshort(fs);
            for (int j = 0; j < 5; j++)
                i[j] = GetUint(fs);
            for (int j = 9; j < 14; j++)
                s[j] = GetUshort(fs);
            for (int j = 0; j < 5; j++)
                str[j] = GetString(fs);
            for (int j = 14; j < 19; j++)
                s[j] = GetUshort(fs);

            b[0] = GetByte(fs);
            b[1] = GetByte(fs);
            i[5] = GetUint(fs);
            i[6] = GetUint(fs);
            b[2] = GetByte(fs);
            b[3] = GetByte(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={s[0]}");
            for(int j = 0; j < 5; j++)
                sb.AppendLine(str[j]);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbSoulMetryString : SimpleSingleString<ushort>
    {
    }

    public class TbSystemMail : ResReader
    {
        byte b1;
        ushort s1;
        ushort s2;
        string str1;
        string str2;
        string str3;
        string str4;

        override public void Read(FileStream fs)
        {
            b1 = GetByte(fs);
            s1 = GetUshort(fs);
            s2 = GetUshort(fs);
            str1 = GetString(fs);
            str2 = GetString(fs);
            str3 = GetString(fs);
            str4 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={b1}");
            sb.AppendLine(str1);
            sb.AppendLine(str2);
            sb.AppendLine(str3);
            sb.AppendLine(str4);

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbTalk : ResReader
    {
        uint[] i = new uint[36];
        byte[] b = new byte[4];
        ushort s1;

        override public void Read(FileStream fs)
        {
            i[1] = GetUint(fs);
            b[1] = GetByte(fs);
            s1 = GetUshort(fs);
            i[2] = GetUint(fs);
            b[2] = GetByte(fs);
            i[3] = GetUint(fs);
            b[3] = GetByte(fs);
            for(int j = 4; j <= 35; j++)
                i[j] = GetUint(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i[1]}");
            sb.AppendLine($"NUM1={b[1]}");
            sb.AppendLine($"NUM2={s1}");
            sb.AppendLine($"NUM3={i[2]}");
            sb.AppendLine($"NUM4={b[2]}");
            sb.AppendLine($"NUM5={i[3]}");
            sb.AppendLine($"NUM6={b[3]}");
            for (int j = 4; j <= 35; j++)
                sb.AppendLine($"NUM{j}={i[j]}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbTalkList : ResReader
    {
        ushort[] s = new ushort[26];

        override public void Read(FileStream fs)
        {
            for(int j = 0; j < 26; j++)
                s[j] = GetUshort(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={s[0]}");
            for (int j = 1; j < 26; j++)
                sb.AppendLine($"NUM{j}={s[j]}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbTitleInfo : ResReader
    {
        uint[] i = new uint[18];
        float[] f = new float[5];

        override public void Read(FileStream fs)
        {
            int j = 0, k = 0;
            i[j++] = GetUint(fs);
            i[j++] = GetUint(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetUshort(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetUint(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetByte(fs);
            i[j++] = GetUshort(fs);
            i[j++] = GetUshort(fs);
            i[j++] = GetUshort(fs);
            i[j++] = GetUshort(fs);
            i[j++] = GetUshort(fs);
            f[k++] = GetFloat(fs);
            f[k++] = GetFloat(fs);
            f[k++] = GetFloat(fs);
            f[k++] = GetFloat(fs);
            f[k++] = GetFloat(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={i[0]}");
            int j;
            for (j = 1; j < 18; j++)
                sb.AppendLine($"{i[j]}");
            for (int k = 0; k < 5; k++, j++)
                sb.AppendLine($"{f[k]}");
            //for (j = 1; j < 18; j++)
            //    sb.AppendLine($"NUM{j}={i[j]}");
            //for (int k = 0; k < 5; k++, j++)
            //    sb.AppendLine($"NUM{j}={f[k]}");

            // 換行
            sb.AppendLine("");
        }
    }

    public class TbTitleString : SimpleMulitString<uint>
    {
        public TbTitleString()
            : base(3)
        { }
    }

    public class TbTooltipString : SimpleMulitString<uint>
    {
        public TbTooltipString()
            : base(2)
        { }
    }

    public class TbUIString : ResReader
    {
        uint s1;
        byte b1;
        string str1;

        override public void Read(FileStream fs)
        {
            s1 = GetUint(fs);
            b1 = GetByte(fs);
            str1 = GetString(fs);
        }

        override public void Write(StringBuilder sb)
        {
            sb.AppendLine($"ID={s1}");
            sb.AppendLine(str1);

            // 換行
            sb.AppendLine("");
        }
    }
}
