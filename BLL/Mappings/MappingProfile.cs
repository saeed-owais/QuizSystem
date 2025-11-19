using AutoMapper;
using BLL.Dtos.Course;
using Common.Entities;
using QuizSystem.BLL.Dtos.Question;

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

            CreateMap<CreateChoiceDto, Choice>();
            CreateMap<Choice, ChoiceDto>();

            CreateMap<CreateQuestionDto, Question>();
            CreateMap<Question, QuestionDto>();

            CreateMap<UpdateQuestionDto, Question>();
        }
    }
}