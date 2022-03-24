using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Helpers
{
    public static class IOHelper
    {
        public static void CreateDirIfNotExist(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static void CreateDirIfNotExist(string baseDir, string dir)
        {
            string newPath = Path.Join(baseDir, dir);
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
        }

        public static DirectoryInfo GetDirectory(string dir) => GetDirectory(dir, "");
        public static DirectoryInfo GetDirectory(string baseDir, string dir) => new DirectoryInfo(Path.Join(baseDir, dir));
        public static DirectoryInfo GetDirectory(DirectoryInfo baseDir, string dir) => GetDirectory(baseDir.FullName, dir);

        public static FileInfo GetFile(string path) => new FileInfo(path);
        public static FileInfo GetFile(string baseDir, string fileName) => new FileInfo(Path.Join(baseDir, fileName));
        public static FileInfo GetFile(DirectoryInfo baseDir, string fileName) => GetFile(baseDir.FullName, fileName);

        public static FileInfo? GetFileVariantOrNull(DirectoryInfo dir, string name, string type, string ext)
        {
            var specific = new FileInfo(Path.Combine(dir.FullName, $"{name}.{type}.{ext}"));

            if (specific.Exists)
                return specific;

            var generic = new FileInfo(Path.Combine(dir.FullName, $"{name}.{ext}"));

            if (generic.Exists)
                return generic;

            return null;
        }

        public static FileInfo GetFileVariant(DirectoryInfo dir, string name, string type, string ext)
        {
            var result = GetFileVariantOrNull(dir, name, type, ext);
            if (result != null)
                return result;
            throw new IOException($"Could not find a variant of file `{dir.FullName}{name}.{type}.{ext}`");
        }

        public static FileInfo GetFileVariantOrNone(DirectoryInfo dir, string name, string type, string ext)
        {
            var result = GetFileVariantOrNull(dir, name, type, ext);
            if (result != null)
                return result;
            return new FileInfo("None");
        }

        public static List<string> GetInvariantsInDir(DirectoryInfo dir)
        {
            // Every filename, until first '.', unique
            return dir.GetFiles()
                .Select(x => x.Name.Split('.')[0])
                .GroupBy(x => x)
                .Select(x => x.First())
                .ToList();
        }
    }
}
