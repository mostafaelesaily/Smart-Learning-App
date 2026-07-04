using Smart_Learning_App.Data;
using Smart_Learning_App.Data.Models;
using Smart_Learning_App.Repo_Uow.Base;

namespace Smart_Learning_App.Repo_Uow.Main
{
    public class MainUow : IUow
    {
        private readonly AppDbContext _Context;
        public MainUow(AppDbContext context)
        {
            _Context = context;
            User = new MainRepo<AppUser>(_Context);
            Course = new MainRepo<Course>(_Context);
            Enrollments = new MainRepo<Enrollments>(_Context);
            Lessons = new MainRepo<Lessons>(_Context);
            Progress = new MainRepo<Progress>(_Context);
            Topics = new MainRepo<Topics>(_Context);
            Files = new MainRepo<Files>(_Context);
            InstructorRequest = new MainRepo<InstructorRequest>(_Context);
         }

        public IRepo<AppUser> User { get; private set; }
        public IRepo<Course> Course { get; private set; }
        public IRepo<Enrollments> Enrollments { get; set; }
        public IRepo<Lessons> Lessons { get; private set; }
        public IRepo<Progress> Progress { get; private set; }
        public IRepo<Topics> Topics { get; private set; }

        public IRepo<Files> Files { get; private set; }

        public IRepo<InstructorRequest> InstructorRequest { get; private set; }

        public void Dispose()
        {
            _Context.Dispose();
        }

        public Task<int> SaveChangesAsync()
        {
            return _Context.SaveChangesAsync();
        }
    }
}
