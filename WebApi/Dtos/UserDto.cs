using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dtos
{
    public class UserDto
    {

        public string Account { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Organization { get; set; }

    }
}
