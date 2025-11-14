using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.Course;

namespace QuizSystem.BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName));

            CreateMap<CreateCourseDto, Course>();
        }
    }
}