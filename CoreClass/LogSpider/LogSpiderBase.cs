using CoreClass.Model;
using System;
using System.IO;

namespace CoreClass.LogSpider
{
    /// <summary>
    /// 提供对单一持续增长的文件进行数据读取的功能，能够记录上次读写的位置，并从该位置开始继续读取并返回字符串(byte[] => UTF-8 string);
    /// </summary>
    public class LogSpiderBase
    {
        DateTime InstanceCreateTime = DateTime.Now;        //该实例创建的时间（不是所给路径文件创建时间）
        public long ReadCurserPosition = 0;

        public string FilePath;
        public DateTime lastAccessTime = DateTime.MinValue;

        //public bool Readable {get {return lastAccessTime != Directory.GetLastWriteTime(FilePath); } }

        public LogSpiderBase(string filePath)
        {
            FilePath = filePath;
        }
        public string StartSpider()
        {
            try
            {
                var newdate = Directory.GetLastWriteTime(FilePath);
                if (lastAccessTime != newdate)
                {
                    var data = ReadFile();
                    lastAccessTime = newdate;
                    return data;
                }
                else
                {
                    return null;
                }
            }
            catch (FileNotFoundException e)
            {
                return null;
            }
            catch (DirectoryNotFoundException e)
            {
                return null;
            }
            catch (Exception)
            {
                throw;
                //return null;
            }
        }
        public string ReadFile()
        {
            try
            {
                FileStream file = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //set the filestream curser to the postition where lasttime readed;
                file.Position = ReadCurserPosition;
                long nowlenth = file.Length;
                byte[] bytearray = new byte[nowlenth - ReadCurserPosition];

                // 一般文件长度不会大于 max int,但当文件巨大时会产生读取的错误；
                file.Read(bytearray, 0, (int)(nowlenth - ReadCurserPosition));
                string result = System.Text.Encoding.UTF8.GetString(bytearray);
                ReadCurserPosition = nowlenth;
                file.Close();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    } 
}