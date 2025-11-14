using Common.Entities;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Entities;
using QuizSystem.DAL.Data;

namespace QuizSystem.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<Course> CourseRepository { get; private set; }
        public IGenericRepository<Exam> ExamRepository { get; private set; }
        public IGenericRepository<Question> QuestionRepository { get; private set; }
        public IGenericRepository<Student> StudentRepository { get; private set; }
        public IGenericRepository<Instructor> InstructorRepository { get; private set; }
        public IGenericRepository<StudentCourse> StudentCourseRepository { get; private set; }
        public IGenericRepository<StudentExam> StudentExamRepository { get; private set; }
        public IGenericRepository<Choice> ChoiceRepository { get; private set; }
        public IGenericRepository<ExamQuestion> ExamQuestionRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CourseRepository = new GenericRepository<Course>(_context);
            ExamRepository = new GenericRepository<Exam>(_context);
            QuestionRepository = new GenericRepository<Question>(_context);
            StudentRepository = new GenericRepository<Student>(_context);
            InstructorRepository = new GenericRepository<Instructor>(_context);
            StudentCourseRepository = new GenericRepository<StudentCourse>(_context);
            StudentExamRepository = new GenericRepository<StudentExam>(_context);
            ChoiceRepository = new GenericRepository<Choice>(_context);
            ExamQuestionRepository = new GenericRepository<ExamQuestion>(_context);
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}