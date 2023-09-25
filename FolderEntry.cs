using System;
using System.Collections.Generic;

namespace folder_cacher_net
{
    [Serializable]
    public class FolderEntry
    {
        public FolderEntry()
        { }

        public FolderEntry(string _DirectoryPath, float _Percent, UInt32 _Worker)
        {
            DirectoryPath = _DirectoryPath;
            Percent = _Percent;
            Worker = _Worker;
        }

        public string DirectoryPath;
        public float Percent;
        public UInt32 Worker;
    }

    [Serializable]
    public class FConfig
    {
        public FConfig()
        {
            List = new List<FolderEntry>();
        }

        public List<FolderEntry> List;
    }

    public class FolderEntryComparer : IComparer<FolderEntry>
    {
        public int Compare(FolderEntry A, FolderEntry B)
        {
            if (A.DirectoryPath == null || B.DirectoryPath == null)
                return ((A.DirectoryPath == null) ? 0 : 1) - ((B.DirectoryPath == null) ? 0 : 1);
            return A.DirectoryPath.CompareTo(B.DirectoryPath);
        }
    }
}