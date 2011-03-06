using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Recls;

namespace TeraDupeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var query1 = from file in FileSearcher.Search(@"I:\", "*.avi")
                         select file;

            var query2 = from file in FileSearcher.Search(@"K:\", "*.avi")
                         select file;

            var concat = query1.Concat(query2);

            var fileGroups =
                from file in concat
                group file by file.Size
                into groupedFiles
                where groupedFiles.Count() > 1 && CheckHash(groupedFiles)
                select groupedFiles;

            foreach (var fileGroup in fileGroups)
            {
                foreach (var file in fileGroup)
                {
                    Console.WriteLine(file.FileName + " : " + file.Size);
                }
            }
        }

        private static bool CheckHash(IGrouping<long, IEntry> groupedFiles)
        {
            foreach (var file in groupedFiles)
            {
                //if (!new FileInfo(file.Path).Exists) return 0;
                byte[] b;
                using (var br = new BinaryReader(File.OpenRead(file.Path)))
                {
                    b = br.ReadBytes(1000);
                    ulong hashFromBytes = GetHashFromBytes(b);
                }
            }

            return true;
        }

        private static ulong GetHashFromBytes(byte[] b)
        {
            ulong result = 0;
            if ((b != null) || (b.Length != 0))
            {
                unchecked
                {
                    result = 0xcbf29ce484222325u; //init prime
                    for (int i = 0; i < b.Length; i++)
                    {
                        result = ((result ^ b[i]) * 0x100000001b3); //xor with new and mul with prime
                    }
                }
            }
            return result;
        }

        //public static Int64 GetFileHash(string filename)
        //{
        //    if (!new FileInfo(filename).Exists) return 0;
        //    byte[] b;
        //    using (var br = new BinaryReader(File.OpenRead(filename)))
        //    {
        //        b = br.ReadBytes((int)br.BaseStream.Length);
        //    }
        //    return GetHashFromBytes(b);
        //}

    }
}
