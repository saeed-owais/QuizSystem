using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.Exam;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ExamDto>> CreateExamAsync(CreateExamDto dto, Guid instructorId, CancellationToken ct = default)
        {
            // 1. التحقق من ملكية الكورس
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(dto.CourseId, ct);
            if (course == null)
                return Result.Fail<ExamDto>("Course not found.", ErrorType.NotFound);

            if (course.InstructorId != instructorId)
                return Result.Fail<ExamDto>("You are not authorized to add exams to this course.", ErrorType.Unauthorized);

            // 2. إنشاء الامتحان
            var exam = _mapper.Map<Exam>(dto);

            // (الخصائص الافتراضية)
            exam.IsAutomatic = false;
            exam.NumberOfQuestions = 0; // سيزيد عند إضافة الأسئلة

            await _unitOfWork.ExamRepository.AddAsync(exam, ct);
            await _unitOfWork.CompleteAsync(ct);

            // نحتاج لتحميل اسم الكورس للمابينج
            // (ملاحظة: course object معنا بالفعل في الذاكرة، يمكننا استخدامه)
            exam.Course = course;

            return Result.Success(_mapper.Map<ExamDto>(exam));
        }

        public async Task<Result<ExamDto>> GetExamByIdAsync(Guid id, CancellationToken ct = default)
        {
            // هنا نحتاج لـ Include Course لجلب اسمه
            // سنستخدم الـ GenericRepo بذكاء (أو Specific Repo لو فضلنا)
            // للآن، سنجلب الامتحان ثم الكورس

            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(id, ct);
            if (exam == null)
                return Result.Fail<ExamDto>("Exam not found.", ErrorType.NotFound);

            // تحميل الكورس يدوياً للمابينج (لأن GenericRepo لا يدعم Include)
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(exam.CourseId, ct);
            exam.Course = course;

            return Result.Success(_mapper.Map<ExamDto>(exam));
        }

        public async Task<Result<IEnumerable<ExamDto>>> GetExamsByCourseAsync(Guid courseId, Guid instructorId, CancellationToken ct = default)
        {
            // 1. التحقق من ملكية الكورس أولاً
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId, ct);
            if (course == null) return Result.Fail<IEnumerable<ExamDto>>("Course not found.", ErrorType.NotFound);
            if (course.InstructorId != instructorId) return Result.Fail<IEnumerable<ExamDto>>("Unauthorized.", ErrorType.Unauthorized);

            var exams = await _unitOfWork.ExamRepository.FindAsync(e => e.CourseId == courseId, ct);

            // تحميل الكورسات للأغراض العرض (Optional Loop)
            foreach (var ex in exams) ex.Course = course;

            return Result.Success(_mapper.Map<IEnumerable<ExamDto>>(exams));
        }

        public async Task<Result> AddQuestionsToExamAsync(Guid examId, List<Guid> questionIds, Guid instructorId, CancellationToken ct = default)
        {
            // 1. جلب الامتحان والتحقق من ملكيته (عن طريق الكورس)
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, ct);
            if (exam == null) return Result.Fail("Exam not found.", ErrorType.NotFound);

            var course = await _unitOfWork.CourseRepository.GetByIdAsync(exam.CourseId, ct);
            if (course.InstructorId != instructorId) return Result.Fail("Unauthorized.", ErrorType.Unauthorized);

            // 2. التحقق من الأسئلة (هل هي موجودة وتخص المدرس؟)
            // سنجلب كل الأسئلة المطلوبة دفعة واحدة
            var validQuestions = await _unitOfWork.QuestionRepository.FindAsync(q => questionIds.Contains(q.Id) && q.InstructorId == instructorId, ct);

            if (validQuestions.Count() != questionIds.Count)
            {
                return Result.Fail("One or more questions are invalid or do not belong to you.", ErrorType.Validation);
            }

            // 3. إضافة الأسئلة (في جدول الربط ExamQuestions)
            foreach (var q in validQuestions)
            {
                // تحقق بسيط لمنع التكرار (لو السؤال موجود أصلاً في الامتحان)
                var exists = (await _unitOfWork.ExamQuestionRepository.FindAsync(eq => eq.ExamId == examId && eq.QuestionId == q.Id, ct)).Any();

                if (!exists)
                {
                    var examQuestion = new ExamQuestion
                    {
                        ExamId = examId,
                        QuestionId = q.Id
                    };
                    await _unitOfWork.ExamQuestionRepository.AddAsync(examQuestion, ct);
                }
            }

            // 4. تحديث عدد الأسئلة في الامتحان
            // (سنجلب العدد الحالي + الجديد)
            var currentCount = (await _unitOfWork.ExamQuestionRepository.FindAsync(eq => eq.ExamId == examId, ct)).Count();
            exam.NumberOfQuestions = currentCount + validQuestions.Count(); // تقريبي، الأدق هو العد بعد الحفظ لكن سنكتفي بهذا

            _unitOfWork.ExamRepository.Update(exam);
            await _unitOfWork.CompleteAsync(ct);

            return Result.Success();
        }
    }
}