using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.StudentExam;
using QuizSystem.BLL.Interfaces;
using QuizSystem.Common.Common;

namespace QuizSystem.BLL.Services
{
    public class StudentExamService : IStudentExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<StudentExamQuestionDto>>> StartExamAsync(Guid examId, Guid studentId, CancellationToken ct = default)
        {
            // 1. جلب الامتحان
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, ct);
            if (exam == null) return Result.Fail<List<StudentExamQuestionDto>>("Exam not found.", ErrorType.NotFound);

            // 2. التحقق: هل الطالب مسجل في الكورس؟
            // (نستخدم FindAsync للبحث في جدول StudentCourses)
            var isEnrolled = (await _unitOfWork.StudentCourseRepository.FindAsync(sc => sc.StudentId == studentId && sc.CourseId == exam.CourseId, ct)).Any();
            if (!isEnrolled) return Result.Fail<List<StudentExamQuestionDto>>("You are not enrolled in this course.", ErrorType.Unauthorized);

            // 3. التحقق: هل أخذ الامتحان من قبل؟ (لو كان Final مثلاً - سنفترض السماح بالتكرار الآن للتبسيط)

            // 4. جلب الأسئلة المرتبطة بالامتحان
            // (هنا التحدي: نحتاج الأسئلة + الاختيارات)
            // سنستخدم الطريقة اليدوية الموثوقة التي اتبعناها سابقاً:

            // أ. جلب الروابط (ExamQuestions)
            var examQuestionsLinks = await _unitOfWork.ExamQuestionRepository.FindAsync(eq => eq.ExamId == examId, ct);
            var questionIds = examQuestionsLinks.Select(eq => eq.QuestionId).ToList();

            if (!questionIds.Any()) return Result.Fail<List<StudentExamQuestionDto>>("This exam has no questions yet.", ErrorType.Failure);

            // ب. جلب الأسئلة (نحتاج Repository يدعم Includes أو نجلبهم يدوياً)
            // بما أننا نريد الاختيارات، سأستخدم خدعة التحميل اليدوي لضمان الأداء وعدم تغيير الـ Repo الآن
            var questions = await _unitOfWork.QuestionRepository.FindAsync(q => questionIds.Contains(q.Id), ct);

            // ج. جلب الاختيارات لكل هذه الأسئلة دفعة واحدة (Smart Query)
            var allChoices = await _unitOfWork.ChoiceRepository.FindAsync(c => questionIds.Contains(c.QuestionId), ct);

            // د. تجميع البيانات في الذاكرة (In-Memory Stitching)
            foreach (var q in questions)
            {
                q.Choices = allChoices.Where(c => c.QuestionId == q.Id).ToList();
            }

            // 5. التحويل لـ DTO الآمن (بدون IsCorrect)
            var dtos = _mapper.Map<List<StudentExamQuestionDto>>(questions);

            return Result.Success(dtos);
        }

        public async Task<Result<ExamResultDto>> SubmitExamAsync(SubmitExamDto dto, Guid studentId, CancellationToken ct = default)
        {
            // 1. جلب الامتحان للتحقق
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(dto.ExamId, ct);
            if (exam == null) return Result.Fail<ExamResultDto>("Exam not found.", ErrorType.NotFound);

            // 2. منطق التصحيح (Grading Logic)
            int correctCount = 0;

            // نجلب الإجابات الصحيحة من الداتابيز
            // (نبحث عن الاختيارات التي اختارها الطالب لنتأكد هل هي صحيحة أم لا)
            var selectedChoiceIds = dto.Answers.Select(a => a.ChoiceId).ToList();

            // نجلب الاختيارات المختارة من الداتابيز (لنتأكد من خاصية IsCorrect)
            var dbChoices = await _unitOfWork.ChoiceRepository.FindAsync(c => selectedChoiceIds.Contains(c.Id), ct);

            foreach (var answer in dto.Answers)
            {
                // نبحث عن الاختيار في الداتابيز
                var choice = dbChoices.FirstOrDefault(c => c.Id == answer.ChoiceId && c.QuestionId == answer.QuestionId);

                // إذا وجدنا الاختيار وكان صحيحاً
                if (choice != null && choice.IsCorrect)
                {
                    correctCount++;
                }
            }

            // 3. حساب النتيجة
            int totalQuestions = dto.Answers.Count; // أو نجيب العدد الحقيقي من الداتابيز للأمان
            // للأمان: نجلب العدد الحقيقي من ExamQuestions
            var realQuestionCount = (await _unitOfWork.ExamQuestionRepository.FindAsync(eq => eq.ExamId == dto.ExamId, ct)).Count();

            double scorePercentage = realQuestionCount > 0 ? ((double)correctCount / realQuestionCount) * 100 : 0;

            // 4. حفظ النتيجة (StudentExam)
            var studentExam = new StudentExam
            {
                StudentId = studentId,
                ExamId = dto.ExamId,
                Score = scorePercentage,
                SubmittedDate = DateTime.UtcNow
            };

            await _unitOfWork.StudentExamRepository.AddAsync(studentExam, ct);
            await _unitOfWork.CompleteAsync(ct);

            // 5. إرجاع النتيجة
            return Result.Success(new ExamResultDto
            {
                CorrectAnswers = correctCount,
                TotalQuestions = realQuestionCount,
                Score = scorePercentage
            });
        }
    }
}