using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App1.Models;

namespace App1.Data
{
    public class Repository<T> where T: IHasId<int>, new()
    {
        private DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }
        
        public IEnumerable<T> GetItems()
        {
            var items = _context.Database.Table<T>().ToList();
            return items;
        }
        
        public T GetItem(int id)
        {
            return _context.Database.Get<T>(id);
        }
        
        public int DeleteItem(int id)
        {
            return _context.Database.Delete<T>(id);
        }
        
        public int SaveItem(T item)
        {
            if (item.Id != 0)
            {
                _context.Database.Update(item);
                return item.Id;
            }
            else
            {
                return _context.Database.Insert(item);
            }
        }
    }
}