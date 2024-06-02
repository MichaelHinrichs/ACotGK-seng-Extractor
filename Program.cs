//Written for The Accursed Crown of the Giant King. https://store.steampowered.com/app/1970780
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
                fileTable.Add(new SUBFILE
                {
                    name = new string(br.ReadChars(64)).TrimEnd('\0'),
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
    }
}
