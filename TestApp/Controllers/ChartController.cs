using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.ViewModels;

namespace TestApp.Controllers
{
    public class ChartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetChartData(DateTime? startDate = null, DateTime? endDate = null)
        {
            DateTime? firstDate = DateTime.MinValue;
            DateTime? lastDate = DateTime.MaxValue;

            if (startDate.HasValue)
            {
                firstDate = startDate;
            }
            if (endDate.HasValue)
            {
                lastDate = endDate;
            }

            var topUsers = _context.TimeLogs
                .Join(
                _context.Users,
                timeLog => timeLog.UserId,
                user => user.Id,
                (timeLog, user) => new { timeLog, user })
                                .Where(e => e.timeLog.Date >= firstDate && e.timeLog.Date <= lastDate)
                                .GroupBy(e => e.timeLog.UserId)
                                .OrderByDescending(g => g.Sum(e => e.timeLog.Hours))
                                .Take(10)
                                .Select(r => new UserProjectViewModel { UserId = r.Key, Hours = r.Sum(e => e.timeLog.Hours) })
                                .ToList();

            var userIds = topUsers.Select(r => r.UserId).ToList();

            var users = _context.Users.ToList().Where(item => userIds.Contains(item.Id)).ToArray();
            var allUsers = new List<UserProjectViewModel>();

            foreach (var user in users)
            {
                allUsers.Add(new UserProjectViewModel
                {
                    Hours = topUsers.FirstOrDefault(x => x.UserId == user.Id).Hours,
                    Name = user.FirstName + " " + user.LastName
                });
            }

            allUsers = allUsers.OrderByDescending(x => x.Hours).ToList();

            object[][] arrayForTable = new object[10][];

            for (int j = 0; j < 10; j++)
            {
                object[] sub = new object[2];

                sub[0] = allUsers[j].Name;

                sub[1] = allUsers[j].Hours;

                arrayForTable[j] = sub;
            }

            return Json(JsonConvert.SerializeObject(arrayForTable));
        }

        [HttpGet]
        public IActionResult GetInfoForUser(int id)
        {
            var projects = _context.TimeLogs
                .Join(
                _context.Projects,
                timeLog => timeLog.ProjectId,
                project => project.Id,
                (timeLog, project) => new { timeLog, project })
                .Where(x => x.timeLog.UserId == id)
                .GroupBy(e => e.timeLog.ProjectId)
                .Select(r => new ProjectSumHours { Id = r.Key, Hours = r.Sum(e => e.timeLog.Hours) });

            var allProjects = _context.Projects.ToList();

            var allPojectsForUser = new List<ProjectSumHours>();
            foreach (var item in projects)
            {
                allPojectsForUser.Add(new ProjectSumHours
                {
                    Name = allProjects.FirstOrDefault(x => x.Id == item.Id).Name,
                    Hours = item.Hours
                });
            }

            return Json(JsonConvert.SerializeObject(allPojectsForUser));
        }

    }
}
