using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSys.Models;
using TicketingSys.Settings;

namespace TicketingSys.Utils
{
    public class DbSeeder
    {
        
    public static void Seed(ApplicationDbContext db, IConfiguration config, ILogger logger)
    {
        logger.LogWarning("ADMIN seed started");

        // admin user from env
        string userId = config["SEED_ADMIN_USERID"] ?? "NO ID IN ENV";
        string adminEmail = config["SEED_ADMIN_EMAIL"] ?? "ERROR";
        string adminFirstName = config["SEED_ADMIN_FIRSTNAME"] ?? "ERROR ";
        string adminLastName = config["SEED_ADMIN_LASTNAME"] ?? "User ";
        string adminFullName = $"{adminFirstName} {adminLastName}";

        logger.LogInformation("Seeding admin user with:");
        logger.LogInformation($"ADMIN Email: {adminEmail}");
        logger.LogInformation($"ADMIN Name: {adminFullName}");
        logger.LogInformation($"ADMIN ID: {userId}");


        // find out why there would be any in the first place
        var existing = db.Users.ToList();
        if (existing.Any())
        {           
            logger.LogWarning("REMOVING EXISTING USERS");
            db.Users.RemoveRange(existing); 
            db.SaveChanges();
        }


        if (!db.Users.Any()){
            logger.LogWarning("ADMIN INSERT STARTED");
            var adminUser = new User
                    {
                        userId = userId,
                        firstName = adminFirstName,
                        lastName = adminLastName,
                        fullName = adminFullName,
                        email = adminEmail,
                        IsAdmin = true
                    };

            db.Users.Add(adminUser);
            db.SaveChanges();
        }

    }
    }
}
