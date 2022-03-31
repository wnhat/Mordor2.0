using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass
{
    public class VersionCheckClass : IEquatable<VersionCheckClass>
    {
        public int VersionNumber;     // 版本迭代号，当message存在变动时请及时更新该数字，服务器将会校验该版本；
        public string UpdateTime;          // 程序上次更新的时间 example: 202100807

        public VersionCheckClass(int firstVersionNumber, string updateTime)
        {
            VersionNumber = firstVersionNumber;
            UpdateTime = updateTime;
        }
        public VersionCheckClass() { }
        public override bool Equals(object obj)
        {
            return Equals(obj as VersionCheckClass);
        }

        public bool Equals(VersionCheckClass other)
        {
            return other != null &&
                   VersionNumber == other.VersionNumber &&
                   UpdateTime == other.UpdateTime;
        }

        public override int GetHashCode()
        {
            int hashCode = -427018499;
            hashCode = hashCode * -1521134295 + VersionNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UpdateTime);
            return hashCode;
        }

        public static bool operator ==(VersionCheckClass left, VersionCheckClass right)
        {
            return EqualityComparer<VersionCheckClass>.Default.Equals(left, right);
        }

        public static bool operator !=(VersionCheckClass left, VersionCheckClass right)
        {
            return !(left == right);
        }
        public bool CheckVersion(VersionCheckClass other)
        {
            if (other.VersionNumber == this.VersionNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static class StaticVersion
    {
        public static VersionCheckClass Version;
        static StaticVersion()
        {
            int firstVersionNumber = 1;
            string updateTime = "20210808";
            Version = new VersionCheckClass(firstVersionNumber, updateTime);
        }
    }

    [System.Serializable]
    public class VersionException : System.Exception
    {
        public VersionException() { }
        public VersionException(string message) : base(message) { }
        public VersionException(string message, System.Exception inner) : base(message, inner) { }
        protected VersionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
