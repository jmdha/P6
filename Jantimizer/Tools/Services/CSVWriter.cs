using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;

namespace Tools.Services
{
    public class CSVWriter
    {
        private string Path;
        private string DirectoryName;
        private string FileName;

        public CSVWriter(string directory, string fileName)
        {
            DirectoryName = directory;
            FileName = fileName;
            Path = directory + "/" + fileName;
        }

        public void Write<T, ClassMap>(IEnumerable<T> data, bool useMap = false)
            where ClassMap : CsvHelper.Configuration.ClassMap {
            Directory.CreateDirectory(DirectoryName);
            FileStream stream;
            CsvConfiguration config;
            if (File.Exists(Path))
            {
                stream = File.Open(Path, FileMode.Append);
                config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    // Don't write the header again.
                    HasHeaderRecord = false,
                };

            } else
            {
                stream = File.Open(Path, FileMode.Create);
                config = new CsvConfiguration(CultureInfo.InvariantCulture);
            }
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                if (useMap)
                    csv.Context.RegisterClassMap<ClassMap>();
                csv.WriteRecords(data);
                writer.Flush();
            }
        }
    }
}
