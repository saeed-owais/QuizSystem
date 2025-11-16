using AutoMapper;
using BLL.Dtos.Course;
using Common.Entities;

namespace QuizSystem.BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.FullName));

            CreateMap<CreateCourseDto, Course>();

            CreateMap<UpdateCourseDto, Course>();
        }
    }
}