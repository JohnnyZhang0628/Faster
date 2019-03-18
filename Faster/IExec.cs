using System;
using System.Collections.Generic;
using System.Text;

namespace Faster
{
   public interface IExec
    {
        IEnumerable<T> Query<T>();

        bool Add<T>(T model);

        bool Delete();

        bool Update<T>(T model);
    }
}
