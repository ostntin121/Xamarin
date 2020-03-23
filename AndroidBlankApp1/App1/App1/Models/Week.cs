using SQLite;

namespace App1.Models
{
    [Table("Weeks")]
    public class Week: IHasId<int>
    {
        public string Name { get; set; }
    }
}