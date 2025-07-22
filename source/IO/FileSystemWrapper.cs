using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebWacker.IO
{
    public interface IFileSystemWrapper
    {
        string[] GetFiles(string path, string searchPattern);

        string ReadAllText(string path);
    }

    public class FileSystemWrapper : IFileSystemWrapper
    {
        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
