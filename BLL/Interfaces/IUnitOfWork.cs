using Common.Entities;

namespace QuizSystem.BLL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Course> CourseRepository { get; }
        IGenericRepository<Exam> ExamRepository { get; }
        IGenericRepository<Question> QuestionRepository { get; }
        IGenericRepository<Student> StudentRepository { get; }
        IGenericRepository<Instructor> InstructorRepository { get; }
        IGenericRepository<StudentCourse> StudentCourseRepository { get; }
        IGenericRepository<StudentExam> StudentExamRepository { get; }
        IGenericRepository<Choice> ChoiceRepository { get; }
        IGenericRepository<ExamQuestion> ExamQuestionRepository { get; }

        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}