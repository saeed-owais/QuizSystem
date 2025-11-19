using AutoMapper;
using BLL.Dtos.Course;
using Common.Entities;
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

            var courseDto = _mapper.Map<CourseDto>(course);
            return Result.Success(courseDto);
        }

        public async Task<Result<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            //var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);
            var course = await _unitOfWork.CourseRepository.GetCourseWithInstructorAsync(courseId, cancellationToken);

            if (course == null)
            {
                return Result.Fail<CourseDto>($"Course with Id '{courseId}' was not found.", ErrorType.NotFound);
            }

            return Result.Success(_mapper.Map<CourseDto>(course));
        }

        public async Task<Result<IEnumerable<CourseDto>>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            var courses = await _unitOfWork.CourseRepository.FindAsync(c => c.InstructorId == instructorId, cancellationToken);

            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return Result.Success(courseDtos);
        }

        public async Task<Result> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateCourseDto, Guid instructorId, CancellationToken cancellationToken = default)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);

            if (course == null)
            {
                return Result.Fail($"Course with Id '{courseId}' was not found.", ErrorType.NotFound);
            }

            if (course.InstructorId != instructorId)
            {
                return Result.Fail("You are not authorized to update this course.", ErrorType.Unauthorized);
            }

            _mapper.Map(updateCourseDto, course);
            _unitOfWork.CourseRepository.Update(course);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteCourseAsync(Guid courseId, Guid instructorId, CancellationToken cancellationToken = default)
        {
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, cancellationToken);

            if (course == null)
            {
                return Result.Fail($"Course with Id '{courseId}' was not found.", ErrorType.NotFound);
            }

            if (course.InstructorId != instructorId)
            {
                return Result.Fail("You are not authorized to delete this course.", ErrorType.Unauthorized);
            }

            _unitOfWork.CourseRepository.Delete(course);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}