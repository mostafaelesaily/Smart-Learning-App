namespace Smart_Learning_App.Data.Models
{
    public class Topics
    {
        public int  Id { get; set; }

        public int LessonId { get; set; }   

        public string Title { get; set; }

        public string? Description { get; set; }

        public Lessons lessons { get; set; }
                
    }
}
