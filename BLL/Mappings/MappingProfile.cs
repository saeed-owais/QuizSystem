using AutoMapper;
using BLL.Dtos.Course;
using Common.Entities;
using QuizSystem.BLL.Dtos.Exam;
using QuizSystem.BLL.Dtos.Question;
using QuizSystem.BLL.Dtos.Student;
using QuizSystem.BLL.Dtos.StudentExam;

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

            CreateMap<CreateExamDto, Exam>();
            CreateMap<Exam, ExamDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name));

            CreateMap<StudentCourse, StudentCourseDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Course.Instructor.FullName));

            CreateMap<Choice, StudentExamChoiceDto>();
            CreateMap<Question, StudentExamQuestionDto>();

            // 1. من StudentExam إلى StudentHistoryDto (للطالب)
            CreateMap<StudentExam, StudentHistoryDto>()
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Exam.Course.Name));

            // 2. من StudentExam إلى ExamResultReportDto (للمدرس)
            CreateMap<StudentExam, ExamResultReportDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName));
        }
    }
}