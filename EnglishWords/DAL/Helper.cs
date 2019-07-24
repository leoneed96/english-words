using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishWords.DAL
{
    public static class DALHelper
    {
        public static IUnitOfWork CreateUnitOfWork => new UnitOfWork(new Data.DataContext());
    }
}
