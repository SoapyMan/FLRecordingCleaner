using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLRecordingCleaner
{
    public class FileInfo
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }

        // for debugging purposes
        public override string ToString()
        {
            return FileName;
        }
    }

    public class ProjectInfo
    {
        public ProjectInfo()
        {
            FolderWAVList = new List<FileInfo>();
            UsedWAVList = new List<string>();

            HasLoadErrors = false;
            PathError = false;
            FileError = false;
        }

        public string FileName { get; set; }
        public string ProjectPath { get; set; }
        public bool HasLoadErrors { get; set; }

        public bool PathError { get; set; }
        public bool FileError { get; set; }

        public List<FileInfo> FolderWAVList { get; set; }
        public List<string> UsedWAVList { get; set; }

        public IEnumerable<string> ProjectFiles { get; set; }
        public IEnumerable<FileInfo> UnusedWAVList { get; set; }

        // for debugging purposes
        public override string ToString()
        {
            return string.IsNullOrEmpty(FileName) ? ProjectPath : FileName;
        }
    }

    public class GatheringResult
    {
        public GatheringResult()
        {
            ProjectList = new List<ProjectInfo>();
        }

        public List<ProjectInfo> ProjectList { get; set; }
    }
}
