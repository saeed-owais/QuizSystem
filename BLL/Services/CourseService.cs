using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.Course;
using QuizSystem.BLL.Interfaces;

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

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto, Guid instructorId, CancellationToken cancellationToken = default)
        {
            var course = _mapper.Map<Course>(createCourseDto);
            course.InstructorId = instructorId;

            await _unitOfWork.CourseRepository.AddAsync(course, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return _mapper.Map<CourseDto>(course);
        }

        public async Task<CourseDto> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);
            return _mapper.Map<CourseDto>(course);
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            var courses = await _unitOfWork.CourseRepository.FindAsync(c => c.InstructorId == instructorId, cancellationToken);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }
    }
}