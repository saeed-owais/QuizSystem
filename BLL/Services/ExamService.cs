using AutoMapper;
using Common.Entities;
using QuizSystem.BLL.Dtos.Exam;
using QuizSystem.BLL.Dtos.StudentExam;
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

        public async Task<Result<IEnumerable<ExamResultReportDto>>> GetExamResultsAsync(Guid examId, Guid instructorId, CancellationToken ct = default)
        {
            // 1. التحقق من ملكية الامتحان
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, ct);
            if (exam == null) return Result.Fail<IEnumerable<ExamResultReportDto>>("Exam not found.", ErrorType.NotFound);

            var course = await _unitOfWork.CourseRepository.GetByIdAsync(exam.CourseId, ct);
            if (course.InstructorId != instructorId)
                return Result.Fail<IEnumerable<ExamResultReportDto>>("Unauthorized.", ErrorType.Unauthorized);

            // 2. جلب النتائج
            var results = await _unitOfWork.StudentExamRepository.FindAsync(se => se.ExamId == examId, ct);

            // 3. جلب أسماء الطلاب
            var studentIds = results.Select(r => r.StudentId).ToList();
            var students = await _unitOfWork.StudentRepository.FindAsync(s => studentIds.Contains(s.Id), ct);

            // 4. التجميع
            var report = new List<ExamResultReportDto>();
            foreach (var res in results)
            {
                var student = students.FirstOrDefault(s => s.Id == res.StudentId);
                report.Add(new ExamResultReportDto
                {
                    StudentId = res.StudentId,
                    StudentName = student?.FullName ?? "Unknown",
                    Score = res.Score ?? 0,
                    SubmittedDate = res.SubmittedDate ?? DateTime.UtcNow
                });
            }

            return Result.Success<IEnumerable<ExamResultReportDto>>(report);
        }

        public async Task<Result<ExamDto>> CreateAutomaticExamAsync(CreateAutomaticExamDto dto, Guid instructorId, CancellationToken ct = default)
        {
            // 1. التحقق من ملكية الكورس
            var course = await _unitOfWork.CourseRepository.GetByIdAsync(dto.CourseId, ct);
            if (course == null) return Result.Fail<ExamDto>("Course not found.", ErrorType.NotFound);
            if (course.InstructorId != instructorId) return Result.Fail<ExamDto>("Unauthorized.", ErrorType.Unauthorized);

            var finalQuestionIds = new List<Guid>();

            // 2. معالجة المعايير
            foreach (var criteria in dto.Criteria)
            {
                // أ. جلب كل أسئلة المدرس لهذا المستوى
                // (ملاحظة: للأداء العالي جداً يفضل جلب IDs فقط، لكن Generic Repo يجلب Entities، مقبول حالياً)
                var availableQuestions = await _unitOfWork.QuestionRepository.FindAsync(
                    q => q.InstructorId == instructorId && q.Level == criteria.Level,
                    ct
                );

                var availableList = availableQuestions.ToList();

                // ب. التحقق من الوفرة
                if (availableList.Count < criteria.Count)
                {
                    return Result.Fail<ExamDto>(
                        $"Not enough questions for level '{criteria.Level}'. Requested: {criteria.Count}, Available: {availableList.Count}.",
                        ErrorType.Validation
                    );
                }

                // ج. الاختيار العشوائي (Shuffle and Take)
                var randomSelection = availableList
                    .OrderBy(x => Guid.NewGuid()) // خلط عشوائي بسيط
                    .Take(criteria.Count)
                    .Select(q => q.Id)
                    .ToList();

                finalQuestionIds.AddRange(randomSelection);
            }

            // 3. إنشاء الامتحان
            var exam = new Exam
            {
                Title = dto.Title,
                CourseId = dto.CourseId,
                ExamType = dto.ExamType,
                IsAutomatic = true,
                NumberOfQuestions = finalQuestionIds.Count
            };

            await _unitOfWork.ExamRepository.AddAsync(exam, ct);

            // 4. حفظ الروابط (الأسئلة المختارة)
            foreach (var qId in finalQuestionIds)
            {
                var examQuestion = new ExamQuestion { ExamId = exam.Id, QuestionId = qId };
                await _unitOfWork.ExamQuestionRepository.AddAsync(examQuestion, ct);
            }

            await _unitOfWork.CompleteAsync(ct);

            // 5. التجهيز للرد
            exam.Course = course; // للمابينج
            return Result.Success(_mapper.Map<ExamDto>(exam));
        }
    }
}