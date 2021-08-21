using DataLayer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLayer.Data.Seeding
{
    public class Seeding
    {
        private readonly ApplicationDbContext _context;
        public Seeding(ApplicationDbContext context)
        {
            _context = context;
        }
        public void SeedProjects()
        {
            var listProjects = new List<Project>();
            foreach (var item in SeedingData.SeedingData.Projects)
            {
                listProjects.Add(new Project() { Name = item });
            }

            _context.AddRange(listProjects);
            _context.SaveChanges();
        }

        #region UsersSeed
        public void SeedUsers()
        {
            var listUsers = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                Random rand = new Random();

                int indexFirstName = rand.Next(SeedingData.SeedingData.FirstNames.Length);
                rand = new Random();
                int indexLastName = rand.Next(SeedingData.SeedingData.LastName.Length);
                rand = new Random();
                int indexDomain = rand.Next(SeedingData.SeedingData.Domains.Length);
                listUsers.Add(new User { FirstName = SeedingData.SeedingData.FirstNames[indexFirstName], LastName = SeedingData.SeedingData.LastName[indexLastName], Email = SeedingData.SeedingData.LastName[indexLastName] + "@" + SeedingData.SeedingData.Domains[indexDomain] });
            }
            _context.AddRange(listUsers);
            _context.SaveChanges();
        }
        #endregion

        #region TimeLog
        public void SeedTimeLogs()
        {
            var listTimeLogs = new List<TimeLog>();
            var listProjects = _context.Projects.Select(x => x.Id).ToArray();

            Random r = new Random();
            int length = r.Next(1, 20); 
            var users = _context.Users.Select(x => x.Id).ToArray();
            var userWithProject = new List<UserProjectModel>();

            foreach (var userId in users)
            {
                var date = DateTime.Now.Date;
                for (int i = 0; i < length; i++)
                {
                    Random rand = new Random();
                    var hours = NextDouble(rand, 0.25, 8.00).ToString("0.00");
                    var projectId = GetRandom(listProjects);
                    if (!userWithProject.Any(u => u.UserId == userId && u.Date == date))
                    {
                        userWithProject.Add(new UserProjectModel { Date = date, UserId = userId, Hours = decimal.Parse(hours) });
                        listTimeLogs.Add(new TimeLog { Date = date, Hours = decimal.Parse(hours), ProjectId = projectId, UserId = userId });
                    }
                    else
                    {
                        var userHours = userWithProject.FirstOrDefault(u => u.UserId == userId && u.Date == date).Hours;
                        if (userHours < 7.75m)
                        {
                            while (userHours + decimal.Parse(hours) >= 8)
                            {
                                hours = NextDouble(rand, 0.25, 8.00).ToString("0.00");
                                userHours += decimal.Parse(hours);
                                if (userHours <= 8)
                                {
                                    break;
                                }
                                userHours = userWithProject.FirstOrDefault(u => u.UserId == userId && u.Date == date).Hours;
                            }
                        }
                        else
                        {
                            break;
                        }
                        listTimeLogs.Add(new TimeLog { Date = date, Hours = decimal.Parse(hours), ProjectId = projectId, UserId = userId });
                        userWithProject.FirstOrDefault(u => u.UserId == userId && u.Date == date).Hours += decimal.Parse(hours);
                    }
                    hours = userWithProject.FirstOrDefault(u => u.UserId == userId && u.Date == date).Hours.ToString();
                    if (double.Parse(hours)> 7.75)
                    {
                        date = date.AddDays(1);
                    }
                }
                length = r.Next(1, 20);
            }
            _context.TimeLogs.AttachRange(listTimeLogs);
            _context.SaveChanges();

        }
        #endregion

        private double NextDouble(Random rand, double minValue, double maxValue)
        {
            return rand.NextDouble() * (maxValue - minValue) + minValue;
        }
        public int GetRandom(int[] arr)
        {
            Random rand = new Random();
            int n = rand.Next(arr.Length);
            return arr[n];
        }

        public class UserProjectModel
        {
            public int UserId { get; set; }
            public DateTime Date { get; set; }
            public decimal Hours { get; set; }
        }
    }
}