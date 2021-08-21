using DataLayer;
using DataLayer.Data.Models;
using DataLayer.Data.Seeding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.ViewModels;

namespace TestApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public PagedResult<UserViewModel> Users { get; set; }
        public void OnGet(int p = 1)
        {
            Users = _context.Users.
                Select(u => new UserViewModel
                {
                    Email = u.Email,
                    Name = u.FirstName + " " + u.LastName,
                    Id = u.Id,
                }).ToList().GetPaged(p, 10);
        }

        public void OnPost()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Projects.RemoveRange(_context.Projects);
            _context.TimeLogs.RemoveRange(_context.TimeLogs);
            _context.SaveChanges();
            var seedingObject = new Seeding(_context);
            seedingObject.SeedProjects();
            seedingObject.SeedUsers();
            seedingObject.SeedTimeLogs();
            Users = _context.Users.
                Select(u => new UserViewModel
                {
                    Email = u.Email,
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName
                })
                .ToList()
                .GetPaged(1, 10);
        }
    }
}
