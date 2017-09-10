using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace kozitScript.stdlib
{
    /// <summary>
    /// Basic Input/Output System.
    /// </summary>
    public class BIOS : API
    {
        public override byte[] Interrupt(byte Current, byte[] Data)
        {

            switch (Current)
            {

                default:
                return Data;


            }

            
        }

        public byte[] GetBytes(string s)
        {
            byte[] ret = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
                ret[i] = (byte) s[i];
            return ret;
        }

        List<char> buffer;

        int loc;

        public BIOS()
        {
            buffer = new List<char>();
            loc = 0;
            ID = new byte[1] {0xFF};
        }

        public byte[] ReadFile(byte[] file)
        {
            return File.ReadAllBytes(Encoding.UTF8.GetString(file));
        }

        public void WriteFile(byte[] file, byte[] content)
        {
            File.WriteAllBytes(Encoding.UTF8.GetString(file),
                               content);
        }

        public void DeleteFile(byte[] file)
        {
            File.Delete(Encoding.UTF8.GetString(file));
        }

        public void DeleteFolder(byte[] folder)
        {
            Directory.Delete(Encoding.UTF8.GetString(folder));
        }

        public byte[] FolderExists(byte[] folder)
        {
            if (Directory.Exists(Encoding.UTF8.GetString(folder)))
            {
                return new byte[] { 0x1 };
            }
            return new byte[] { 0x0 };
        }

        public byte[] FileExists(byte[] file)
        {
            if (File.Exists(Encoding.UTF8.GetString(file)))
            {
                return new byte[] { 0x1 };
            }
            return new byte[] { 0x0 };
        }

        public void CreateFolder(byte[] folder)
        {
            Directory.CreateDirectory(Encoding.UTF8.GetString(folder));
        }

        public void AppendFile(byte[] file, byte[] content)
        {
            List<byte> c = new List<byte>();
            byte[] one = ReadFile(file);
            foreach (byte b in one)
                c.Add(b);
            foreach (byte b in content)
                c.Add(b);
            WriteFile(file, c.ToArray());
        }

        public void Exit(int i)
        {
            
        }

		public void Write(char c)
        {
            buffer.Add(c);
            loc++;
        }

        public byte[] ReadLine()
        {
            return GetBytes(Console.ReadLine());
        }

        public byte[] Read()
        {
            return BitConverter.GetBytes(Console.Read());
        }

        public byte[] ReadKey()
        {
            ConsoleKeyInfo x = Console.ReadKey();
            return BitConverter.GetBytes(x.KeyChar);
        }

        public void Write(byte[] ba, byte[] c)
        {
            if (c[0] == 1)
                Write(((int) ba[0]).ToString());
            else
                Write(Encoding.UTF8.GetString(ba));
        }

        public void Write(string s)
        {
            foreach (char c in s)
                Write(c);
        }

        public void Clear()
        {
            buffer.Clear();
        }
        
        public void Print()
        {
            Console.Write(buffer.ToArray());
            Clear();
        }

    }
}