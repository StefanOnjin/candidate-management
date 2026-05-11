using CandidateManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Skills.AnyAsync() || await context.Candidates.AnyAsync())
                return;

            var skills = new List<Skill>
            {
                new Skill { SkillName = "C# programming" },
                new Skill { SkillName = "Java programming" },
                new Skill { SkillName = "Database design" },
                new Skill { SkillName = "REST API development" },
                new Skill { SkillName = "English language" },
                new Skill { SkillName = "German language" },
                new Skill { SkillName = "Russian language" }
            };

            await context.Skills.AddRangeAsync(skills);
            await context.SaveChangesAsync();

            var csharp = skills.First(s => s.SkillName == "C# programming");
            var java = skills.First(s => s.SkillName == "Java programming");
            var database = skills.First(s => s.SkillName == "Database design");
            var restApi = skills.First(s => s.SkillName == "REST API development");
            var english = skills.First(s => s.SkillName == "English language");
            var german = skills.First(s => s.SkillName == "German language");

            var candidates = new List<Candidate>
            {
                new Candidate
                {
                    FullName = "Stefan Onjin",
                    DateOfBirth = new DateOnly(2004, 1, 19),
                    ContactNumber = "+381611111111",
                    EmailAddress = "stefan@gmail.com",
                    CandidateSkills = new List<CandidateSkill>
                    {
                        new CandidateSkill { SkillId = csharp.Id },
                        new CandidateSkill { SkillId = database.Id },
                        new CandidateSkill { SkillId = english.Id }
                    }
                },
                new Candidate
                {
                    FullName = "Dragan Draganovic",
                    DateOfBirth = new DateOnly(1996, 10, 10),
                    ContactNumber = "+381644444444",
                    EmailAddress = "dragan.draganovic@gmail.com",
                    CandidateSkills = new List<CandidateSkill>
                    {
                        new CandidateSkill { SkillId = java.Id },
                        new CandidateSkill { SkillId = restApi.Id },
                        new CandidateSkill { SkillId = german.Id }
                    }
                },
                new Candidate
                {
                    FullName = "Petar Petrovic",
                    DateOfBirth = new DateOnly(2000, 1, 18),
                    ContactNumber = "+381600000000",
                    EmailAddress = "pera@example.com",
                    CandidateSkills = new List<CandidateSkill>
                    {
                        new CandidateSkill { SkillId = csharp.Id },
                        new CandidateSkill { SkillId = restApi.Id },
                        new CandidateSkill { SkillId = english.Id }
                    }
                }
            };

            await context.Candidates.AddRangeAsync(candidates);
            await context.SaveChangesAsync();
        }
    }
}