//Written for The Accursed Crown of the Giant King. https://store.steampowered.com/app/1970780/
using System;
using System.IO;

namespace ACotGK_seng_Extractor
{
    static class Program
    {
        public static BinaryReader br;

        static void Main(string[] args)
        {
            using FileStream source = File.OpenRead(args[0]);
            br = new(source);
            int fileCount = br.ReadInt32();

            System.Collections.Generic.List<SUBFILE> fileTable = new();
            for (int i = 0; i < fileCount - 1; i++)
            {
                br.ReadInt32();
                string name = NullTerminatedString();
                if (br.BaseStream.Position < (i + 1) * 0x4C + 0x48)
                    br.BaseStream.Position = (i + 1) * 0x4C + 0x48;

                fileTable.Add(new SUBFILE
                {
                    name = name,
                    offset = br.ReadInt32(),
                    size = br.ReadInt32()
                });
            }
            foreach (SUBFILE sub in fileTable)
            {
                br.BaseStream.Position = sub.offset;
                Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + Path.GetDirectoryName(sub.name));
                using FileStream FS = File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]) + "//" + sub.name);
                BinaryWriter bw = new(FS);
                bw.Write(br.ReadBytes(sub.size));
                bw.Close();
            }
        }

        public struct SUBFILE
        {
            public string name;
            public int offset;
            public int size;
        }

        public static string NullTerminatedString()
        {
            char[] fileName = Array.Empty<char>();
            char readchar = (char)1;
            while (readchar > 0)
            {
                readchar = br.ReadChar();
                Array.Resize(ref fileName, fileName.Length + 1);
                fileName[^1] = readchar;
            }
            Array.Resize(ref fileName, fileName.Length - 1);
            string name = new(fileName);
            return name;
        }
    }
}
