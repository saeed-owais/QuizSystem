using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.Course;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CourseDto>> CreateCourseAsync(CreateCourseDto createCourseDto, Guid instructorId, CancellationToken cancellationToken = default)
        {
            var course = _mapper.Map<Course>(createCourseDto);
            course.InstructorId = instructorId;

            await _unitOfWork.CourseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            // سنفترض هنا أن المابينج سيعمل (سنحتاج لتحميل المدرس يدوياً لو أردنا الاسم)
            var courseDto = _mapper.Map<CourseDto>(course);
            return Result.Success(courseDto);
        }

        public async Task<Result<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);

            if (course == null)
            {
                return Result.Fail<CourseDto>($"Course with Id '{courseId}' was not found.");
            }

            return Result.Success(_mapper.Map<CourseDto>(course));
        }

        public async Task<Result<IEnumerable<CourseDto>>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            var courses = await _unitOfWork.CourseRepository.FindAsync(c => c.InstructorId == instructorId, cancellationToken);

            // هنا، الفشل غير متوقع. القائمة الفارغة تعتبر "نجاح"
            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return Result.Success(courseDtos);
        }
    }
}