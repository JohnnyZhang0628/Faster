using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Service
{
    public class UserService : BaseService, IUserRepository
    {
       
        public bool Login(string username, string password)
        {
            return true;
        }
    }
}
