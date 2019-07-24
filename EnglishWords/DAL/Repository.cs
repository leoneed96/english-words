using EnglishWords.Data;
using EnglishWords.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EnglishWords.DAL
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> All();

        T Create(T t);
        int Delete(T t);
        int Delete(Expression<Func<T, bool>> predicate);
        int Update(T t);

        void Attach(T t);
        void Detach(T t);
        void SetFromObject<TObject>(int id, TObject instance);
    }
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        private DbSet<T> DbSet => _context.Set<T>();

        public Repository(DataContext context)
        {
            _context = context;
        }

        public IQueryable<T> All()
        {
            return DbSet.AsQueryable<T>();
        }

        public void Attach(T t)
        {
            if (!DbSet.Local.Any(x => x.ID == t.ID))
                DbSet.Attach(t);
        }


        public void SetFromObject<TObject>(int id, TObject instance)
        {
            var obj = All().Where(x => x.ID == id).SingleOrDefault();
            if (obj == null)
                throw new Exception("Object not found or it is dublicate");
            Attach(obj);
            _context.Entry(obj).CurrentValues.SetValues(instance);
        }

        public T Create(T t)
        {
            var ret = DbSet.Add(t);
            return ret;
        }

        public int Delete(T t)
        {
            if (!DbSet.Local.Any(x => x.ID == t.ID))
                DbSet.Attach(t);
            DbSet.Remove(t);
            return 0;
        }

        public virtual int Update(T t)
        {
            if (_context.Entry(t).State != EntityState.Detached) return 0;

            DbSet.Attach(t);
            _context.Entry(t).State = EntityState.Modified;

            return 0;
        }

        public int Delete(Expression<Func<T, bool>> predicate)
        {
            var toDel = All().Where(predicate).ToArray();
            foreach (var item in toDel)
            {
                DbSet.Remove(item);
            }
            return toDel.Length;
        }

        public void Detach(T t)
        {
            _context.Entry(t).State = EntityState.Detached;
        }
    }
}
