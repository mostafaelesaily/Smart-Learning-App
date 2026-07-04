using Smart_Learning_App.Data.Models;

namespace Smart_Learning_App.Repo_Uow.Base
{
    public interface IUow :  IDisposable
    {
        public IRepo<AppUser> User { get; }

        public IRepo<Course> Course { get; }

        public IRepo<Enrollments> Enrollments { get; }

        public IRepo<Lessons> Lessons { get; }
        public IRepo<Progress> Progress { get; }

        public IRepo<Topics> Topics { get; }

        public IRepo<Files> Files { get; }

        public IRepo<InstructorRequest> InstructorRequest { get; }

        Task<int> SaveChangesAsync();

        
    }
}
