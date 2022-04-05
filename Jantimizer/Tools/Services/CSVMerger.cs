using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Services
{
    public static class CSVMerger
    {
        // Recursivly merges all csvs in the same directory
        // Assumes same header for all of them
        public static void Merge<T, ClassMap>(string rootPath, string fileName)
            where ClassMap : CsvHelper.Configuration.ClassMap
        {
            CSVWriter writer = new CSVWriter(rootPath, fileName);
            var csvFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

            foreach (var csvFile in csvFiles)
            {
                using (var reader = new StreamReader(csvFile)) {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<ClassMap>();
                        var records = csv.GetRecords<T>();
                        writer.Write<T, ClassMap>(records, true);
                    }
                }
            }
        }
    }
}
