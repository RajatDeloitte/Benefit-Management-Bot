using System;
using System.Collections.Generic;
using System.Text;

namespace BMBOT.Entities
{
    public class FileCheckError
    {
        public string FilesCount { get; set; }

        public string RunDate { get; set; }

        public string Mode { get; set; }

        public List<FileObject> FileObjects { get; set; }

    }
}
