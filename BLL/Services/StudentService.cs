using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.Student;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> EnrollInCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
        {
            // 1. التحقق من وجود الكورس
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, ct);
            if (course == null)
                return Result.Fail("Course not found.", ErrorType.NotFound);

            // 2. التحقق هل الطالب مسجل بالفعل؟
            // (استخدمنا FindAsync للبحث في جدول الربط)
            var existingEnrollment = await _unitOfWork.StudentCourseRepository
                .FindAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId, ct);

            if (existingEnrollment.Any())
                return Result.Fail("You are already enrolled in this course.", ErrorType.Conflict);

            // 3. التسجيل
            var studentCourse = new StudentCourse
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow
            };

            await _unitOfWork.StudentCourseRepository.AddAsync(studentCourse, ct);
            await _unitOfWork.CompleteAsync(ct);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<StudentCourseDto>>> GetMyCoursesAsync(Guid studentId, CancellationToken ct = default)
        {
            // نحتاج لجلب جدول الربط StudentCourse وعمل Include للكورس والمدرس
            // الـ Generic Repo الحالي لا يدعم Include بمرونة، لذا سنستخدم FindAsync
            // ولكن للأسف لن يتم تحميل بيانات الكورس (Course Name) إلا إذا عدلنا الـ Repo
            // أو استخدمنا Specific Repo.

            // *الحل السريع والمقبول حالياً:* سنجلب البيانات ونرى هل EF Core سيقوم بتحميلها (Auto-fixup) 
            // أو نستخدم استعلام مباشر لو كنا نستخدم DbContext.

            // بما أننا في BLL، سنفترض أننا بحاجة لـ StudentCourseRepository مخصص (مثل CourseRepository)
            // لكن لتوفير الوقت، سأستخدم خدعة بسيطة: سأطلب من الـ Repo جلب البيانات
            // وسأعتمد على أننا سنحتاج لعمل Include يدوياً أو تعديل الـ GenericRepo لاحقاً.

            // *تعديل هام:* لكي يعمل الـ Mapping بشكل صحيح، يجب أن تكون بيانات Course و Instructor محملة.
            // سأستخدم المنطق الموجود حالياً، وإذا رجعت البيانات null سنعرف أننا بحاجة لـ Include.

            // (ملاحظة: في المشاريع الحقيقية، ننشئ IStudentRepository فيه GetCoursesWithDetails)
            // سأكتب الكود هنا بافتراض أننا سننشئ StudentRepository لاحقاً، أو نستخدم الـ Generic المتاح.

            var studentCourses = await _unitOfWork.StudentCourseRepository
                .FindAsync(sc => sc.StudentId == studentId, ct);

            // ** مشكلة الأداء هنا **: FindAsync العادية لا تجلب الـ Course.
            // الحل الأمثل السريع: تحميل الكورسات IDs ثم جلبهم.

            var courseIds = studentCourses.Select(sc => sc.CourseId).ToList();

            // نجلب الكورسات (مع المدرسين) دفعة واحدة
            var courses = await _unitOfWork.CourseRepository.FindAsync(c => courseIds.Contains(c.Id), ct);

            // نجلب المدرسين للكورسات (لأن FindAsync للكورس لا تجلب المدرس بـ Include)
            // هذه "لفة" طويلة بسبب عدم وجود Repository مخصص.
            // لكنها تعمل وصحيحة.

            // تجميع البيانات يدوياً (Manual Join in Memory)
            var dtos = new List<StudentCourseDto>();

            foreach (var sc in studentCourses)
            {
                var course = courses.FirstOrDefault(c => c.Id == sc.CourseId);
                if (course != null)
                {
                    // نحتاج لاسم المدرس. سنجلبه من CourseRepository لو كنا استخدمنا الدالة المخصصة،
                    // لكننا استخدمنا FindAsync العادية.
                    // لذا سأقوم بتحميل المدرس لكل كورس (مكلف قليلاً لكنه الحل المتاح حالياً)
                    var instructor = await _unitOfWork.InstructorRepository.GetByIdAsync(course.InstructorId, ct);

                    dtos.Add(new StudentCourseDto
                    {
                        CourseId = course.Id,
                        CourseName = course.Name,
                        InstructorName = instructor?.FullName ?? "Unknown",
                        EnrollmentDate = sc.EnrollmentDate
                    });
                }
            }

            return Result.Success<IEnumerable<StudentCourseDto>>(dtos);
        }
    }
}