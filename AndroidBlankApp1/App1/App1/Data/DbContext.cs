using System.Threading.Tasks;
using App1.Models;
using SQLite;
using Task = App1.Models.Task;

namespace App1.Data
{
    public class DbContext
    {
        private SQLiteConnection database;
        //private SQLiteAsyncConnection asyncDatabase;
        public SQLiteConnection Database => database;
        
        public Repository<UserData> UserData;
        public Repository<Week> Weeks;
        public Repository<DailyPlan> DailyPlans;
        public Repository<Task> Tasks;

        public DbContext(string databasePath)
        {
            //asyncDatabase = new SQLiteAsyncConnection(databasePath);
            database = new SQLiteConnection(databasePath);
            Init();
        }

        public void Init()
        {
            database.CreateTable<UserData>();
            database.CreateTable<Week>();
            database.CreateTable<DailyPlan>();
            database.CreateTable<Task>();
            
            UserData = new Repository<UserData>(this);
            Weeks = new Repository<Week>(this);
            DailyPlans = new Repository<DailyPlan>(this);
            Tasks = new Repository<Task>(this);
        }

        public void CreateTable<T>() where T: IHasId<int>, new()
        {
            database.CreateTable<T>();
        }
        
        
    }
}