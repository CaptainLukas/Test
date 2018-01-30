using System;
using System.Collections.Generic;
using System.Text;

namespace Textanalyse.Data.Repository
{
    public interface IRepository<T>
    {
        void SaveText(string text);
    }
}
