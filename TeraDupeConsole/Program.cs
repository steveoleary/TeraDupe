using System;
using System.Collections.Generic;
using System.Linq;
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
                where groupedFiles.Count() > 1
                select groupedFiles;

            foreach (var fileGroup in fileGroups)
            {
                foreach (var file in fileGroup)
                {
                    Console.WriteLine(file.FileName + " : " + file.Size);
                }
            }
        }
    }
}
