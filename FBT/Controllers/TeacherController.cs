using FBT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using X.PagedList;
using System.Data.SqlClient;
using System.Globalization;

namespace FBT.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string TeacherID = username.Split('$')[0];
            ViewData["Avatar"] = TeacherID;
            Teacher? teacher = GetTeacherData(TeacherID);
            return View(teacher);

        }
        private Teacher? GetTeacherData(string teacherid)
        {
            using (var dbcontext = new FbtContext())
            {
                var teacher = dbcontext.Teachers
                .Include(s => s.TeacherNavigation)
                .FirstOrDefault(s => s.TeacherId == teacherid);
                return teacher;
            }
        }

        //----------------------------------------------------------------------Lấy danh sách lớp-----------------------------------------------------------------------------------------------
        //Lớp chủ nhiệm
        public ActionResult ViewClassList(string classId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string teacherId = username.Split('$')[0];
            ViewData["Avatar"] = teacherId;

            // Thay đổi ở đây để lấy thông tin lớp
            Class schoolClass = GetClassData(teacherId);
            // Đặt giá trị vào ViewBag
            ViewBag.TeacherId = teacherId;
            ViewBag.ClassId = classId;
            //   Console.WriteLine($"Class ID: {schoolClass?.ClassId}");
            //   Console.WriteLine($"Class Name: {schoolClass?.ClassName}");
            //   Console.WriteLine($"Number of Students: {schoolClass?.Students?.Count}");
            // Console.WriteLine($"Teacher ID: {schoolClass?.Teachers?.FirstOrDefault()?.TeacherId}");

            return View(schoolClass);
        }
        // lay danh sach lop chu nhiem 
        private Class GetClassData(string teacherId)
        {
            using (var dbContext = new FbtContext())
            {
                Console.WriteLine($"GetClassData - teacherId: {teacherId}");
                // Thay đổi Include để đảm bảo tất cả dữ liệu liên quan được tải lên
                var schoolClass = dbContext.Teachers
                    .Where(t => t.TeacherId == teacherId)
                    .SelectMany(t => t.Classes) // Chọn tất cả các lớp của giáo viên
                    .Include(c => c.Teachers).ThenInclude(c => c.TeacherNavigation)
                    .Include(c => c.Students).ThenInclude(c => c.StudentNavigation)
                    .FirstOrDefault();

                // Debug: In ra giá trị để kiểm tra
                //  Console.WriteLine($"TeacherId: {teacherId}");
                //  Console.WriteLine($"ClassId: {schoolClass?.ClassId}");

                // Kiểm tra Students
                if (schoolClass?.Students != null)
                {
                    foreach (var student in schoolClass.Students)
                    {
                        Console.WriteLine($"StudentId: {student.StudentId}");
                    }
                }

                return schoolClass;
            }
        }

        //lấy danh sách lớp giáo viên đó dạy
        public ActionResult ViewSubjectClasses()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string teacherId = username.Split('$')[0];
            ViewData["Avatar"] = teacherId;

            // Thực hiện truy vấn để lấy danh sách lớp mà giáo viên dạy môn chính
            var subjectClasses = GetSubjectClasses(teacherId);

            // Truyền danh sách lớp đến view
            return View(subjectClasses);
        }

        private List<Class> GetSubjectClasses(string teacherId)
        {
            using (var dbContext = new FbtContext())
            {
                Console.WriteLine($"GetSubjectClasses - teacherId: {teacherId}");
                // Lấy danh sách lớp mà giáo viên dạy
                var subjectClasses = dbContext.SubjectTeachers
                    .Where(st => st.TeacherId == teacherId)
                    .Select(st => st.Class)
                    .Distinct()
                    .Include(c => c.Students).ThenInclude(c => c.StudentNavigation)// Đảm bảo tải thông tin sinh viên
                    .ToList();

                var subjectName = dbContext.SubjectTeachers
               .Where(st => st.TeacherId == teacherId)
               .Select(st => st.Subject.SubjectName)
               .FirstOrDefault();
                // Debug: In ra giá trị để kiểm tra
                foreach (var schoolClass in subjectClasses)
                {
                    Console.WriteLine($"ClassId: {schoolClass?.ClassId}");
                    // Kiểm tra Students
                    if (schoolClass?.Students != null)
                    {
                        foreach (var student in schoolClass.Students)
                        {
                            Console.WriteLine($"StudentId: {student.StudentId}");
                        }
                    }
                }
                ViewBag.SubjectName = subjectName;
                return subjectClasses;
            }
        }

        public ActionResult ViewClassDetail(string classId)
        {
            // Kiểm tra nếu classId là null hoặc rỗng, chuyển hướng đến trang lỗi hoặc trang không tìm thấy
            if (string.IsNullOrEmpty(classId))
            {
                return RedirectToAction("Error"); // Thay "Error" bằng tên action hoặc trang bạn muốn chuyển hướng đến khi có lỗi
            }

            using (var dbContext = new FbtContext())
            {
                // Lấy thông tin chi tiết về lớp học dựa trên classId
                var schoolClass = dbContext.Classes
                    .Include(c => c.Students).ThenInclude(c => c.StudentNavigation)
                    .FirstOrDefault(c => c.ClassId == classId);

                // Kiểm tra xem lớp có tồn tại hay không
                if (schoolClass == null)
                {
                    return RedirectToAction("Error"); // Thay "Error" bằng tên action hoặc trang bạn muốn chuyển hướng đến khi có lỗi
                }

                // Truyền thông tin lớp đến view
                return View(schoolClass);
            }
        }




        // Thêm sinh viên vào danh sách lớp
        [HttpGet]
        public IActionResult AddStudent(string classId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string teacherId = username.Split('$')[0];
            ViewData["Avatar"] = teacherId;

            // Lấy thông tin lớp của giáo viên
            var schoolClass = GetClassData(teacherId);

            if (schoolClass == null)
            {
                // Nếu không tìm thấy lớp, thêm lỗi vào ModelState
                ViewData["StudentIdError"] = "Lớp không tồn tại.";
                return View();
            }
            ViewBag.ClassId = classId;
            // Lấy danh sách học sinh chưa thuộc lớp nào
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStudent(string teacherId, string studentId, string classId)
        {
            using (var dbContext = new FbtContext())
            {
                var existingStudent = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId);

                if (existingStudent != null)
                {
                    Console.WriteLine($"HttpPost - teacherId: {teacherId}");

                    var schoolClass = dbContext.Classes.Include(c => c.Students).FirstOrDefault(c => c.ClassId == classId);

                    if (schoolClass != null)
                    {
                        // Kiểm tra số lượng học sinh hiện tại và NumberOfStudent trong cùng một lệnh SQL
                        var currentNumberOfStudents = dbContext.Classes
                            .Where(c => c.ClassId == classId)
                            .Select(c => new
                            {
                                CurrentNumberOfStudents = c.Students.Count()
                            })
                            .FirstOrDefault();

                        // Kiểm tra nếu thêm học sinh vượt quá ngưỡng NumberOfStudent
                        if (currentNumberOfStudents != null && currentNumberOfStudents.CurrentNumberOfStudents < schoolClass.NumberOfStudent)
                        {
                            if (!schoolClass.Students.Any(s => s.StudentId == studentId))
                            {
                                var studentInOtherClass = dbContext.Classes
                                    .Where(c => c.ClassId != schoolClass.ClassId)
                                    .Any(c => c.Students.Any(s => s.StudentId == studentId));

                                if (!studentInOtherClass)
                                {
                                    schoolClass.Students.Add(existingStudent);
                                    dbContext.SaveChanges();
                                    return RedirectToAction("ViewClassList", new { teacherId });
                                }
                                else
                                {
                                    ViewData["StudentIdError"] = "Sinh viên này đã ở lớp khác.";
                                }
                            }
                            else
                            {
                                ViewData["StudentIdError"] = "Sinh viên này đã tồn tại trong lớp.";
                            }
                        }
                        else
                        {
                            ViewData["StudentIdError"] = "Lớp của bạn đã đầy không thể thêm học sinh.";
                        }
                    }
                    else
                    {
                        ViewData["StudentIdError"] = "Lớp không tồn tại.";
                    }
                }
                else
                {
                    ViewData["StudentIdError"] = "Học sinh này chưa đăng ký tài khoản.";
                }

                // Chuyển giá trị classId vào ViewData để tránh mất thông tin khi trả về view
                ViewData["ClassId"] = classId;

                return View();
            }
        }

        //Xóa học sinh  
        public ActionResult DeleteStudent(string teacherId, string studentId)
        {
            using (var dbContext = new FbtContext())
            {

                var schoolClass = dbContext.Teachers
                    .Where(t => t.TeacherId == teacherId)
                    .SelectMany(t => t.Classes) // Chọn tất cả các lớp của giáo viên
                    .Include(c => c.Teachers).ThenInclude(c => c.TeacherNavigation)
                    .Include(c => c.Students).ThenInclude(c => c.StudentNavigation)
                    .FirstOrDefault();

                if (schoolClass != null)
                {
                    var studentToRemove = schoolClass.Students.FirstOrDefault(s => s.StudentId == studentId);

                    if (studentToRemove != null)
                    {
                        schoolClass.Students.Remove(studentToRemove);
                        dbContext.SaveChanges();
                    }
                }


                // Redirect về trang danh sách lớp
                return RedirectToAction("ViewClassList", new { teacherId });

            }
        }

        //--------------------------------------------------------------------Chuc nang diem ---------------------------------------------------------------------------------------------------

        public ActionResult ViewScores(string classId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string teacherId = username.Split('$')[0];
            using (var dbContext = new FbtContext())
            {
                // Tìm giáo viên với ID đã cho
                var teacher = dbContext.Teachers
                    .Include(t => t.SubjectTeachers)
                    .ThenInclude(st => st.Subject)
                    .Include(t => t.Classes)
                    .FirstOrDefault(t => t.TeacherId == teacherId);

                if (teacher != null)
                {
                    // Lấy danh sách các SubjectId mà giáo viên đang dạy
                    var subjectIds = teacher.SubjectTeachers.Select(st => st.SubjectId).ToList();

                    // Lấy danh sách các lớp mà giáo viên đang dạy
                    var classIds = teacher.Classes.Select(c => c.ClassId).ToList();

                    // Nếu có tham số classId được truyền vào, sử dụng nó; ngược lại, lấy lớp đầu tiên của giáo viên
                    var selectedClassId = string.IsNullOrEmpty(classId) ? classIds.FirstOrDefault() : classId;

                    var selectedClassName = dbContext.Classes.Where(c => c.ClassId == selectedClassId).FirstOrDefault();

                    // Lấy tên của môn đầu tiên (hoặc kiểm tra nếu có môn nào đó)
                    var firstSubjectId = subjectIds.FirstOrDefault();
                    var firstSubjectName = dbContext.Subjects
                        .Where(s => s.SubjectId == firstSubjectId)
                        .Select(s => s.SubjectName)
                        .FirstOrDefault();

                    // Lấy tất cả các điểm của giáo viên cho lớp được chọn
                    var allScores = dbContext.Scores
                        .Include(s => s.Student).ThenInclude(s => s.StudentNavigation)
                        .Include(s => s.Subject)
                        .Where(s => s.Student.Classes.Any(c => c.ClassId == selectedClassId) && subjectIds.Contains(s.SubjectId))
                        .OrderBy(s => s.Semester)
                        .ToList();

                    // Lấy danh sách điểm cho học kỳ 1
                    var scoresForSemester1 = allScores.Where(s => s.Semester == 1).ToList();

                    // Lấy danh sách điểm cho học kỳ 2
                    var scoresForSemester2 = allScores.Where(s => s.Semester == 2).ToList();

                    ViewBag.SubjectName = firstSubjectName;
                    ViewData["Avatar"] = teacherId;
                    ViewBag.ClassName = selectedClassName.ClassName;
                    ViewBag.ClassId = selectedClassId;



                    // Trả về View chứa danh sách điểm cho học kỳ 1 và học kỳ 2
                    return View(Tuple.Create(scoresForSemester1, scoresForSemester2));
                }

                // Trả về thông báo lỗi nếu không tìm thấy giáo viên
                ViewData["Error"] = "Không tìm thấy thông tin giáo viên.";
                return View();
            }
        }






        // Trả về view để nhập điểm
        public IActionResult AddScore(string studentId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string teacherId = username.Split('$')[0];

            using (var dbContext = new FbtContext())
            {
                // Lấy danh sách các môn học mà giáo viên đang dạy
                var teacher = dbContext.Teachers
                    .Include(t => t.SubjectTeachers)
                    .ThenInclude(st => st.Subject)
                    .FirstOrDefault(t => t.TeacherId == teacherId);

                if (teacher != null)
                {
                    var subjectIds = teacher.SubjectTeachers.Select(st => st.SubjectId).ToList();
                    var subjects = dbContext.Subjects.Where(s => subjectIds.Contains(s.SubjectId)).ToList();
                    var studentdb = dbContext.PersonInformations.Where(s => s.Id == studentId).FirstOrDefault();

                    Console.WriteLine(studentdb.Fullname);
                    ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName");
                    ViewBag.TeacherId = teacherId;
                    ViewBag.StudentId = studentId;
                    ViewBag.StudentName = studentdb.Fullname;
                    var studentClass = dbContext.Students
                        .Where(s => s.StudentId == studentId)
                        .Select(s => s.Classes.FirstOrDefault() != null ? s.Classes.FirstOrDefault().ClassId : null)
                        .FirstOrDefault();

                    // Xây dựng URL cho trang ViewScores theo lớp
                    ViewBag.ViewScoresUrl = Url.Action("ViewScores", "Teacher", new { classId = studentClass });
                    return View();
                }

                ViewData["Error"] = "Không tìm thấy thông tin giáo viên.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddScore(string studentId, string subjectId, Score score, int semester)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string teacherId = username.Split('$')[0];

            using (var dbContext = new FbtContext())
            {
                try
                {
                    // Kiểm tra xem giáo viên có quyền thêm điểm cho môn học này không
                    var isAuthorized = dbContext.SubjectTeachers
                        .Any(st => st.TeacherId == teacherId && st.SubjectId == subjectId);

                    if (!isAuthorized)
                    {
                        ViewData["Error"] = "Bạn không có quyền thêm điểm cho môn học này.";
                        return View();
                    }

                    // Kiểm tra xem học sinh có tồn tại không
                    var studentExists = dbContext.Students.Any(s => s.StudentId == studentId);
                    if (!studentExists)
                    {
                        ViewData["Error"] = "Không tìm thấy học sinh.";
                        return View();
                    }

                    // Thiết lập giá trị semester từ trường ẩn
                    score.Semester = semester == 0 ? 1 : semester;

                    // Thêm điểm vào database
                    dbContext.Scores.Add(score);
                    dbContext.SaveChanges();

                    ViewData["Success"] = "Đã thêm điểm thành công.";

                    var studentClass = dbContext.Students
                        .Where(s => s.StudentId == studentId)
                        .Select(s => s.Classes.FirstOrDefault() != null ? s.Classes.FirstOrDefault().ClassId : null)
                        .FirstOrDefault();

                    // Xây dựng URL cho trang ViewScores theo lớp
                    string viewScoresUrl = Url.Action("ViewScores", "Teacher", new { classId = studentClass });

                    // Chuyển hướng về trang ViewScores theo lớp
                    return Redirect(viewScoresUrl);
                }
                catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2627)
                {
                    // 2627 is the error number for a unique key violation
                    ViewData["ErrorScore"] = "Sinh viên này đã có trong bảng điểm ở học kỳ này";
                }
                catch (Exception ex)
                {
                    // Handle other exceptions or rethrow if needed
                    ViewData["ErrorScore"] = "Có lỗi xảy ra khi thêm điểm: " + ex.Message;
                }

                return View();
            }
        }



        // Trả về view để cập nhật điểm
        public IActionResult UpdateScore(string studentId, string subjectId, int semester)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string teacherId = username.Split('$')[0];

            using (var dbContext = new FbtContext())
            {
                // Kiểm tra xem giáo viên có quyền cập nhật điểm cho môn học này không
                var isAuthorized = dbContext.SubjectTeachers
                    .Any(st => st.TeacherId == teacherId && st.SubjectId == subjectId);

                if (!isAuthorized)
                {
                    ViewData["Error"] = "Bạn không có quyền cập nhật điểm cho môn học này.";
                    return View();
                }

                // Kiểm tra xem học sinh có tồn tại không
                var studentExists = dbContext.Students.Any(s => s.StudentId == studentId);
                if (!studentExists)
                {
                    ViewData["Error"] = "Không tìm thấy học sinh.";
                    return View();
                }

                // Lấy điểm của học sinh cho môn học và kỳ học cần cập nhật
                var existingScore = dbContext.Scores
                    .FirstOrDefault(s => s.StudentId == studentId && s.SubjectId == subjectId && s.Semester == semester);

                if (existingScore == null)
                {
                    ViewData["Error"] = "Không tìm thấy điểm để cập nhật.";
                    return View();
                }

                // Truyền dữ liệu điểm cần cập nhật đến View để hiển thị trong form cập nhật
                ViewBag.ExistingScore = existingScore;

                // Lấy danh sách các môn học mà giáo viên đang dạy (giống như trong AddScore)
                var teacher = dbContext.Teachers
                    .Include(t => t.SubjectTeachers)
                    .ThenInclude(st => st.Subject)
                    .FirstOrDefault(t => t.TeacherId == teacherId);

                if (teacher != null)
                {
                    var subjectIds = teacher.SubjectTeachers.Select(st => st.SubjectId).ToList();
                    var subjects = dbContext.Subjects.Where(s => subjectIds.Contains(s.SubjectId)).ToList();
                    var studentdb = dbContext.PersonInformations.Where(s => s.Id == studentId).FirstOrDefault();
                    var studentClass = dbContext.Students
                        .Where(s => s.StudentId == studentId)
                        .Select(s => s.Classes.FirstOrDefault() != null ? s.Classes.FirstOrDefault().ClassId : null)
                        .FirstOrDefault();

                    // Xây dựng URL cho trang ViewScores theo lớp
                    ViewBag.ViewScoresUrl = Url.Action("ViewScores", "Teacher", new { classId = studentClass });
                    ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName");
                    ViewBag.TeacherId = teacherId;
                    ViewBag.StudentId = studentId;
                    ViewBag.StudentName = studentdb.Fullname;
                    ViewBag.ExistingScore1 = existingScore.Coefficient1;
                    ViewBag.ExistingScore2 = existingScore.Coefficient2;
                    ViewBag.ExistingScore3 = existingScore.Coefficient3;
                    return View("UpdateScore");
                }

                ViewData["Error"] = "Không tìm thấy thông tin giáo viên.";
                return View();
            }
        }

        // Hàm cập nhật điểm (HttpPost)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateScore(string studentId, string subjectId, Score score, int semester)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string teacherId = username.Split('$')[0];

            using (var dbContext = new FbtContext())
            {
                // Kiểm tra xem giáo viên có quyền cập nhật điểm cho môn học này không
                var isAuthorized = dbContext.SubjectTeachers
                    .Any(st => st.TeacherId == teacherId && st.SubjectId == subjectId);

                if (!isAuthorized)
                {
                    ViewData["Error"] = "Bạn không có quyền cập nhật điểm cho môn học này.";
                    return View();
                }

                // Kiểm tra xem học sinh có tồn tại không
                var studentExists = dbContext.Students.Any(s => s.StudentId == studentId);
                if (!studentExists)
                {
                    ViewData["Error"] = "Không tìm thấy học sinh.";
                    return View();
                }

                // Lấy điểm của học sinh cho môn học và kỳ học cần cập nhật
                var existingScore = dbContext.Scores
                    .FirstOrDefault(s => s.StudentId == studentId && s.SubjectId == subjectId && s.Semester == semester);

                if (existingScore == null)
                {
                    ViewData["Error"] = "Không tìm thấy điểm để cập nhật.";
                    return View();
                }

                // Cập nhật các trường điểm cần cập nhật
                existingScore.Coefficient1 = score.Coefficient1;
                existingScore.Coefficient2 = score.Coefficient2;
                existingScore.Coefficient3 = score.Coefficient3;

                // Lưu thay đổi vào database
                dbContext.SaveChanges();


                ViewData["Success"] = "Đã cập nhật điểm thành công.";
                var studentClass = dbContext.Students
                    .Where(s => s.StudentId == studentId)
                    .Select(s => s.Classes.FirstOrDefault() != null ? s.Classes.FirstOrDefault().ClassId : null)
                    .FirstOrDefault();

                // Xây dựng URL cho trang ViewScores theo lớp
                string viewScoresUrl = Url.Action("ViewScores", "Teacher", new { classId = studentClass });

                // Chuyển hướng về trang ViewScores theo lớp
                return Redirect(viewScoresUrl);
            }
        }



        public ActionResult DeleteScore(string studentId, int semester)
        {
            using (var dbContext = new FbtContext())
            {
                // Tìm điểm cần xóa
                var scoreToDelete = dbContext.Scores
                    .Where(s => s.StudentId == studentId && s.Semester == semester)
                    .FirstOrDefault();

                if (scoreToDelete != null)
                {
                    // Xóa điểm
                    dbContext.Scores.Remove(scoreToDelete);
                    dbContext.SaveChanges();

                    var studentClass = dbContext.Students
                        .Where(s => s.StudentId == studentId)
                        .Select(s => s.Classes.FirstOrDefault() != null ? s.Classes.FirstOrDefault().ClassId : null)
                        .FirstOrDefault();

                    // Xây dựng URL cho trang ViewScores theo lớp
                    string viewScoresUrl = Url.Action("ViewScores", "Teacher", new { classId = studentClass });

                    // Chuyển hướng về trang ViewScores theo lớp
                    return Redirect(viewScoresUrl);
                }

                // Trả về thông báo lỗi nếu không tìm thấy điểm
                ViewData["Error"] = "Không tìm thấy điểm.";
                return View("ViewScores");
            }
        }

        public ActionResult ViewClassTranscript(string classId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string teacherId = username.Split('$')[0];
            ViewData["Avatar"] = teacherId;

            // Thay đổi ở đây để lấy thông tin lớp
            Class schoolClass = GetClassData(teacherId);
            // Đặt giá trị vào ViewBag
            ViewBag.TeacherId = teacherId;
            ViewBag.ClassId = classId;
            //   Console.WriteLine($"Class ID: {schoolClass?.ClassId}");
            //   Console.WriteLine($"Class Name: {schoolClass?.ClassName}");
            //   Console.WriteLine($"Number of Students: {schoolClass?.Students?.Count}");
            // Console.WriteLine($"Teacher ID: {schoolClass?.Teachers?.FirstOrDefault()?.TeacherId}");

            return View(schoolClass);
        }




        //lịch dạy 
        public IActionResult TeacherTimeTable()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                var teacherID = username.Split('$')[0];
                var datetimeStart = DateTime.Now;
                var datetimeEnd = DateTime.Now;
                for (int i = 0; i < 8; i++)
                {
                    if (datetimeStart.DayOfWeek != DayOfWeek.Monday)
                    {
                        datetimeStart = datetimeStart.AddDays(-1);
                    }
                    if (datetimeEnd.DayOfWeek != DayOfWeek.Sunday)
                    {
                        datetimeEnd = datetimeEnd.AddDays(1);
                    }
                }
                using (var dbContext = new FbtContext())
                {
                    ViewData["TeacherName"] = dbContext.PersonInformations.Where(item => item.Teacher.TeacherId == teacherID).Select(item => item.Fullname).FirstOrDefault();
                    ViewData["Time"] = datetimeStart.ToString("dd/MM/yyyy") + " - " + datetimeEnd.ToString("dd/MM/yyyy");
                    var schedule = dbContext.Schedules.Where(item => item.Teacher.TeacherId == teacherID && item.DayOfWeek >= datetimeStart && item.DayOfWeek <= datetimeEnd)
                        .Include(s => s.Class.Grade.SchoolYear)
                        .Include(s => s.Teacher.TeacherNavigation)
                        .Include(s => s.Subject).ToList();
                    return View(schedule);
                }
            }
        }

        [HttpPost]
        public IActionResult TeacherTimeTable(string week)
        {
            var username = HttpContext.Session.GetString("Username");
            using (var dbContext = new FbtContext())
            {
                var teacherID = username.Split('$')[0];
                string[] time = week.Split("&&");
                DateTime weekBeginConvert;
                DateTime weekEndConvert;

                bool success1 = DateTime.TryParse(time[0], out weekBeginConvert);
                bool success2 = DateTime.TryParse(time[1], out weekEndConvert);
                ViewData["TeacherName"] = dbContext.PersonInformations.Where(item => item.Teacher.TeacherId == teacherID).Select(item => item.Fullname).FirstOrDefault();
                ViewData["Time"] = weekBeginConvert.ToString("dd/MM/yyyy") + " - " + weekEndConvert.ToString("dd/MM/yyyy");
                Console.WriteLine(week);
                Console.WriteLine(weekBeginConvert.ToString("dd/MM/yyyy") + "====" + weekBeginConvert.AddDays(6).ToString("dd/MM/yyyy"));
                if (success1 && success2)
                {
                    var scheduledb = dbContext.Schedules.Where(item => item.TeacherId == teacherID && item.DayOfWeek >= weekBeginConvert && item.DayOfWeek <= weekEndConvert)
                                .Include(s => s.Class.Grade.SchoolYear)
                                .Include(s => s.Teacher.TeacherNavigation)
                                .Include(s => s.Subject).ToList();
                    return View(scheduledb);

                }
            }
            return View();
        }


    }
}


