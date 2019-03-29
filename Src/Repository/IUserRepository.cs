using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
   public interface IUserRepository:IRepository
    {
        bool Login(string username, string password);
    }
}
