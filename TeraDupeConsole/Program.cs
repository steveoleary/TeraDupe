using System;
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
                group file by new { file.Size, hash = GetHash(file) }
                into groupedFiles
                where groupedFiles.Count() > 1
                select groupedFiles;

            foreach (var fileGroup in fileGroups)
            {
                foreach (var file in fileGroup)
                {
                    Console.WriteLine(file.FileName + " : " + file.Size);
                }
            }

            Console.ReadLine();
        }

        private static ulong GetHash(IEntry file)
        {
            ulong hashFromBytes;

            using (BinaryReader b = new BinaryReader(File.Open(file.Path, FileMode.Open)))
            {
                int length = (int)b.BaseStream.Length;
                int pos = length / 2;
                //TODO: If length is less than 2000 bytes read in entire file
                b.BaseStream.Seek(pos, SeekOrigin.Begin);
                hashFromBytes = GetHashFromBytes(b.ReadBytes(2000));
            }

            return hashFromBytes;
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
    }
}
