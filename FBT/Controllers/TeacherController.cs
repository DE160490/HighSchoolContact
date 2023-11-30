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
            Teacher teacher = GetTeacherData(TeacherID);
            return View(teacher);

        }
        private Teacher GetTeacherData(string teacherid)
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
                    .Include(c => c.Students).ThenInclude(c=>c.StudentNavigation)// Đảm bảo tải thông tin sinh viên
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

        //lịch dạy 
        public ActionResult TeacherTimeTable(string teacherId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                return RedirectToAction("Login", "Login");
            }

            teacherId = username.Split('$')[0];
            ViewData["Avatar"] = teacherId;
            using (var dbContext = new FbtContext())
            {
                var teacher = dbContext.Teachers
                    .Include(t => t.Classes)
                    .Include(t => t.TeacherNavigation)
                    .FirstOrDefault(t => t.TeacherId == teacherId);

                if (teacher != null)
                {
                    var classId = teacher.Classes.FirstOrDefault()?.ClassId;

                    var now = DateTime.Now;
                    DateTime weekBegins = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday).Date;
                    DateTime weekEnds = weekBegins.AddDays(7).AddSeconds(-1);

                    var schedules = dbContext.Schedules
                        .Where(s => s.ClassId == classId && s.WeekBegins >= weekBegins && s.WeekEnds <= weekEnds)
                        .Include(s => s.Teacher.TeacherNavigation)
                        .Include(s => s.Subject)
                        .ToList();

                    ViewBag.TeacherName = teacher.TeacherNavigation.Fullname;
                    ViewBag.WeekBegins = weekBegins;
                    ViewBag.WeekEnds = weekEnds;
                    ViewBag.TeacherId = teacherId;
                    return View(schedules);
                }

                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult TeacherTimeTable(string teacherId, string weekBegins, string weekEnds)
        {
            DateTime? weekBeginsDate = null;
            DateTime? weekEndsDate = null;

            if (DateTime.TryParseExact(weekBegins, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedWeekBegins))
            {
                weekBeginsDate = parsedWeekBegins;
            }

            if (DateTime.TryParseExact(weekEnds, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedWeekEnds))
            {
                weekEndsDate = parsedWeekEnds;
            }

            using (var dbContext = new FbtContext())
            {
                var teacher = dbContext.Teachers
                    .Include(t => t.Classes)
                    .Include(t => t.TeacherNavigation)
                    .FirstOrDefault(t => t.TeacherId == teacherId);

                if (teacher != null)
                {
                    var classId = teacher.Classes.FirstOrDefault()?.ClassId;

                    if (classId != null)
                    {
                        var schedules = dbContext.Schedules
                            .Where(s => s.ClassId == classId && s.WeekBegins >= weekBeginsDate && s.WeekEnds <= weekEndsDate)
                            .Include(s => s.Teacher.TeacherNavigation)
                            .Include(s => s.Subject)
                            .ToList();

                        ViewBag.TeacherName = teacher.TeacherNavigation.Fullname;
                        ViewBag.WeekBegins = weekBeginsDate;
                        ViewBag.WeekEnds = weekEndsDate;
                        ViewBag.TeacherId = teacherId;
                        return View(schedules);
                    }
                }

                return View("Error");
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
                    ViewBag.ClassId   = selectedClassId;
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
                    // Chuyển hướng về trang ViewScores của kỳ học tương ứng
                    return RedirectToAction("ViewScores");
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
                    ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName");
                    ViewBag.TeacherId = teacherId;
                    ViewBag.StudentId = studentId;
                    ViewBag.StudentName = studentdb.Fullname;

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
                // Chuyển hướng về trang ViewScores của kỳ học tương ứng
                return RedirectToAction("ViewScores");
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

                    // Chuyển hướng hoặc trả về thông báo thành công
                    return RedirectToAction("ViewScores");
                }

                // Trả về thông báo lỗi nếu không tìm thấy điểm
                ViewData["Error"] = "Không tìm thấy điểm.";
                return View("ViewScores");
            }
        }







    }
}



