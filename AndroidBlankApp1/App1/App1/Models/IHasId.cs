using SQLite;

namespace App1.Models
{
    public class IHasId<T>
    {
        [PrimaryKey, AutoIncrement] public T Id { get; set; }

        public IHasId()
        {

        }
    }
}