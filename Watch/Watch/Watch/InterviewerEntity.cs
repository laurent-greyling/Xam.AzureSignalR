using SQLite;

namespace Watch
{
    public class InterviewerEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string InterviewerId { get; set; }
    }
}
