namespace Smart_Learning_App.Data.Models
{
    public class Lessons
    {
        public int Id { get; set; }

        public int courseId { get; set; }

        public string Title { get; set; }   
        
        public int OrderIndex { get; set; }

        public Course Course { get; set; }

        public ICollection<Topics> Topics { get; set; } = new List<Topics>();

        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();

        public ICollection<Files> Files { get; set; } = new List<Files>();

    }
}
