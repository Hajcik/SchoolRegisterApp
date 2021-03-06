using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolRegister.BLL.Entities;
using SchoolRegister.Services.Interfaces;
using SchoolRegister.ViewModels.DTOs;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SchoolRegister.Web.Controllers
{

    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ISubjectService _subjectService;
        private readonly IGradeService _gradeService;
        private readonly ITeacherService _teacherService;
        private readonly UserManager<User> _userManager;
        public TeacherController(IStudentService studentService, ISubjectService subjectService, UserManager<User> userManager, IGradeService gradeService, ITeacherService teacherService)
        {
            _studentService = studentService;
            _subjectService = subjectService;
            _userManager = userManager;
            _gradeService = gradeService;
            _teacherService = teacherService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddGrade(int? studentId = null)
        {
            var teacher = _userManager.GetUserAsync(User).Result;
            var students = _studentService.GetStudents(s => s.Group.SubjectGroups.Any(sg => sg.Subject.TeacherId == teacher.Id));
            var subjects = _subjectService.GetSubjects(t => t.TeacherId == teacher.Id);
            ViewBag.StudentsList = new SelectList(students.Select(t => new
            {
                Text = $"{t.FirstName} {t.LastName}",
                Value = t.Id,
                Selected = t.Id == studentId

            }), "Value", "Text");
            ViewBag.SubjectList = new SelectList(subjects.Select(s => new
            {
                Text = s.Name,
                Value = s.Id
            }), "Value", "Text");
            ViewBag.GradeScale = new SelectList(Enum.GetValues(typeof(GradeScale)).Cast<GradeScale>().Select(x => new
            {
                Text = x.ToString(),
                Value = ((int)x).ToString()
            }), "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddGrade(AddGradeToStudentDto addGradeToStudentDto)
        {
            if (ModelState.IsValid)
            {
                var teacher = _userManager.GetUserAsync(User).Result;
                addGradeToStudentDto.TeacherId = teacher.Id;
                _gradeService.AddGradeToStudent(addGradeToStudentDto);
                return RedirectToAction("Details", "Student", new { studentId = addGradeToStudentDto.StudentId });
            }
            return View();
        }

        public IActionResult SendEmailToParent(int studentId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            @ViewBag.SenderId = user.Id;

            var studenciVm = _studentService.GetStudents();
            ViewBag.StudentsList = new SelectList(studenciVm.Select(t => new
            {
                Text = $"{t.FirstName} {t.LastName}",
                Value = t.Id
            })
                , "Value", "Text");

            return View(new SendEmailToParentDto() { StudentId = studentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendEmailToParent(SendEmailToParentDto sendEmailToParentDto)
        {
            var teacher = _userManager.GetUserAsync(User).Result;
            sendEmailToParentDto.SenderId = teacher.Id;
            if (_teacherService.SendEmailToParent(sendEmailToParentDto))
            {
                return RedirectToAction("Index", "Student");
            }
            return View("Error");
        }
    }
}