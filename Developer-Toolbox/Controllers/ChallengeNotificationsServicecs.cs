﻿using Developer_Toolbox.Data;
using Developer_Toolbox.Models;
using System.Security.Policy;
namespace Developer_Toolbox.Controllers
{
    public class ChallengeNotificationService
    {
        private readonly ApplicationDbContext _context;

        public ChallengeNotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CheckActiveChallengesAndSendNotifications()
        {
            // Obține provocările active din baza de date
            var challenges = _context.WeeklyChallenges
                .ToList();

            foreach (var challenge in challenges)
            {
                var users = _context.Users.ToList();

                foreach (var user in users)
                {
                    // Notificare la începutul provocării
                    var startNotificationExists = _context.Notifications.Any(n =>
                        (n.UserId == user.Id ||
                        n.UserId == null) &&
                        n.Link == "/WeeklyChallenges/Show/" + challenge.Id &&
                        n.Message == $"Challenge {challenge.Title} has started!");

                    if (!startNotificationExists && challenge.StartDate <= DateTime.Now && challenge.EndDate >= DateTime.Now)
                    {
                        var startNotification = new Notification
                        {
                            UserId = user.Id,
                            Message = $"Challenge {challenge.Title} has started!",
                            Link = "/WeeklyChallenges/Show/" + challenge.Id,
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };

                        _context.Notifications.Add(startNotification);
                    }

                    // Notificare spre sfârșitul provocării
                    var endingSoonNotificationExists = _context.Notifications.Any(n =>
                        n.UserId == user.Id &&
                        n.Link == "/WeeklyChallenges/Show/" + challenge.Id &&
                        n.Message == $"Challenge {challenge.Title} is ending soon!");

                    if (!endingSoonNotificationExists && (DateTime.Now > challenge.StartDate && DateTime.Now <= challenge.EndDate ||
                                                         DateTime.Now == challenge.StartDate && DateTime.Now.Hour < challenge.EndDate.Hour))
                    {
                        var endingSoonNotification = new Notification
                        {
                            UserId = user.Id,
                            Message = $"Challenge {challenge.Title} is ending soon!",
                            Link = "/WeeklyChallenges/Show/" + challenge.Id,
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };

                        _context.Notifications.Add(endingSoonNotification);
                    }

                    // Notificare la sfârșitul provocării
                    var endNotificationExists = _context.Notifications.Any(n =>
                        n.UserId == user.Id &&
                        n.Link == "/WeeklyChallenges/Show/" + challenge.Id &&
                        n.Message == $"Challenge {challenge.Title} has ended!");

                    if (!endNotificationExists && DateTime.Now > challenge.EndDate.AddDays(1))
                    {
                        var endNotification = new Notification
                        {
                            UserId = user.Id,
                            Message = $"Challenge {challenge.Title} has ended!",
                            Link = "/WeeklyChallenges/Show/" + challenge.Id,
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };

                        _context.Notifications.Add(endNotification);
                    }
                }
            }

            _context.SaveChanges();
        }

    }

}
