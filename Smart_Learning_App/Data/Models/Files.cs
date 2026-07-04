namespace Smart_Learning_App.Data.Models
{
    public class Files
    {
        public int id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; }

        public int LessonId { get; set; }

        public Lessons lessons { get; set; }  

    }
}
