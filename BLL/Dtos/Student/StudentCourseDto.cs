namespace QuizSystem.BLL.Dtos.Student
{
    public class StudentCourseDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string InstructorName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}