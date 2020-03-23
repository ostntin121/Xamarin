using SQLite;

namespace App1.Models
{
    [Table("UserData")]
    public class UserData: IHasId<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}