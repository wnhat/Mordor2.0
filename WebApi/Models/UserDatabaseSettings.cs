using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public interface IUserDatabaseSettings
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }

        CollectionName UsersCollectionName { get; set; }

    }

    public class UserDatabaseSettings : IUserDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public CollectionName UsersCollectionName { get; set; } = null!;

    }
    public class CollectionName
    {
        public string User { get; set; }
        public string DefectCode { get; set; }
        public string ProductInfo { get; set; }

    }
}
