using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Element
{
    public class FileContainer
    {
        public string Name;
        public byte[] Data;
        public bool ReadComplete
        {
            get
            {
                return Data != null;
            }
        }
        public FileContainer(string name,byte[] data)
        {
            this.Name = name;
            this.Data = data;
        }
        public static FileContainer CreatFileContainer(string filepath)
        {
            try
            {
                MemoryStream buffer = new MemoryStream();
                FileInfo Newfile = new FileInfo(filepath);
                Newfile.OpenRead().CopyTo(buffer);
                return new FileContainer(Path.GetFileName(filepath), buffer.ToArray());
            }
            catch
            {
                string errorstring = String.Format("file Read Error,path:{0}", filepath);
                throw new FileContainerException(errorstring);
            }
        }
        public void SaveFileInDisk(string savePath)
        {
            // TODO：use Async Process and handle the error;
            FileInfo newsavefile = new FileInfo(Path.Combine(savePath, Name));
            FileStream writestream = newsavefile.OpenWrite();
            FileFromMemory.CopyTo(writestream);
            writestream.Close();
        }
        public MemoryStream FileFromMemory
        {
            get
            {
                return new MemoryStream(Data);
            }
        }
    }
    /// <summary>
    /// copy the giving path dir(and it`s subdir) to local memory;
    /// </summary>
    public class DirContainer
    {
        public string Name;
        public DateTime CreateTime;
        public FileContainer[] FileContainerArray = null;
        public DirContainer[] DirContainerArray = null;
        public string[] FileNameList
        {
            get
            {
                List<string> filenamelist = new List<string>();
                if (FileContainerArray != null)
                {
                    foreach (var item in FileContainerArray)
                    {
                        filenamelist.Add(item.Name);
                    }
                }
                if (DirContainerArray != null)
                {
                    foreach (var dir in DirContainerArray)
                    {
                        foreach (var item in dir.FileNameList)
                        {
                            filenamelist.Add(item);
                        }
                    }
                }
                return filenamelist.ToArray();
            }
        }
        public DirContainer(string dirPath)
        {
            DirectoryInfo DirInfo = new DirectoryInfo(dirPath);
            if (!DirInfo.Exists)
            {
                string errorstring = String.Format("Directory not exist, path:{0}", dirPath);
                throw new FileContainerException(errorstring);
            }
            this.Name = Path.GetFileName(dirPath);
            this.CreateTime = DirInfo.CreationTime;
            InitialFile(dirPath);
            InitialDir(dirPath);
        }
        void InitialFile(string path)
        {
            string[] filenamearray = Directory.GetFiles(path);
            if (filenamearray.Count() > 0)
            {
                FileContainerArray = new FileContainer[filenamearray.Count()];
                for (int i = 0; i < FileContainerArray.Count(); i++)
                {
                    FileContainerArray[i] = FileContainer.CreatFileContainer(filenamearray[i]);
                }
            }
        }
        void InitialDir(string path)
        {
            string[] dirarray = Directory.GetDirectories(path);
            if (dirarray.Count() > 0)
            {
                DirContainerArray = new DirContainer[dirarray.Count()];
                for (int i = 0; i < dirarray.Count(); i++)
                {
                    DirContainerArray[i] = new DirContainer(dirarray[i]);
                }
            }
        }
        public void SaveDirInDisk(string savePath)
        {
            DirectoryInfo savetarget = new DirectoryInfo(savePath);
            DirectoryInfo subDir = savetarget.CreateSubdirectory(Name);
            if (FileContainerArray != null)
            {
                foreach (var file in FileContainerArray)
                {
                    file.SaveFileInDisk(subDir.FullName);
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var Dir in DirContainerArray)
                {
                    Dir.SaveDirInDisk(subDir.FullName);
                }
            }
        }
        public MemoryStream GetFileFromMemory(string fileName)
        {
            if (FileContainerArray != null)
            {
                foreach (var file in FileContainerArray)
                {
                    if (file.Name == fileName)
                    {
                        return file.FileFromMemory;
                    }
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var Dir in DirContainerArray)
                {
                    var returnvalue = Dir.GetFileFromMemory(fileName);
                    if (returnvalue != null)
                    {
                        return returnvalue;
                    }
                }
            }
            return null;
        }
        public FileContainer GetFileContainer(string fileName)
        {
            if (FileContainerArray != null)
            {
                foreach (var file in FileContainerArray)
                {
                    if (file.Name == fileName)
                    {
                        return file;
                    }
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var Dir in DirContainerArray)
                {
                    var returnvalue = Dir.GetFileContainer(fileName);
                    if (returnvalue != null)
                    {
                        return returnvalue;
                    }
                }
            }
            return null;
        }
        public DirContainer GetDirContainer(string DirName)
        {
            if (DirContainerArray == null)
            {
                return null;
            }
            foreach (var item in DirContainerArray)
            {
                if (item.Name == DirName)
                {
                    return item;
                }
            }
            if (DirContainerArray != null)
            {
                foreach (var Dir in DirContainerArray)
                {
                    var returnvalue = Dir.GetDirContainer(DirName);
                    if (returnvalue != null)
                    {
                        return returnvalue;
                    }
                }
            }
            return null;
        }
        public bool Contains(string filename)
        {
            if (FileNameList.Contains(filename))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Contains(string[] filename)
        {
            string[] filenamelist = this.FileNameList;
            foreach (var item in filename)
            {
                if (filenamelist.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class FileContainerException : ApplicationException
    {
        public FileContainerException(string message) : base(message)
        {

        }
    }

}
