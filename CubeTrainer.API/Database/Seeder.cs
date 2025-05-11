using CubeTrainer.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Database;

internal static class Seeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager)
    {
        await context.Database.MigrateAsync();
        if (context.Users.Any())
        {
            return;
        }

        var user = new User
        {
            UserName = "test@test.com",
            Email = "test@test.com",
        };
        await userManager.CreateAsync(user, "password");
        var otherUser = new User
        {
            UserName = "other@test.com",
            Email = "other@test.com",
        };
        await userManager.CreateAsync(otherUser, "password");

        var cases = new[]
        {
            new Case { Type = CaseType.OLL, Name = "OLL 1", DefaultSolution = "R U2 R2 F R F' U2 R' F R F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 2", DefaultSolution = "L F L' U2 R U2 R' U2 L F' L'", },
            new Case { Type = CaseType.OLL, Name = "OLL 3", DefaultSolution = "R' F2 R2 U2 R' F R U2 R2 F2 R", },
            new Case { Type = CaseType.OLL, Name = "OLL 4", DefaultSolution = "F U R U' R' F' U' F R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 5", DefaultSolution = "B' R2 F R F' R B", },
            new Case { Type = CaseType.OLL, Name = "OLL 6", DefaultSolution = "F U' R2 D R' U' R D' R2 U F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 7", DefaultSolution = "L' U2 L U2 L F' L' F", },
            new Case { Type = CaseType.OLL, Name = "OLL 8", DefaultSolution = "R U2 R' U2 R' F R F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 9", DefaultSolution = "R' U' R U' R' U R' F R F' U R", },
            new Case { Type = CaseType.OLL, Name = "OLL 10", DefaultSolution = "R U R' U R' F R F' R U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 11", DefaultSolution = "F' L' U' L U F U F R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 12", DefaultSolution = "F R U R' U' F' U F R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 13", DefaultSolution = "F U R U2 R' U' R U R' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 14", DefaultSolution = "R' F R U R' F' R F U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 15", DefaultSolution = "R' F' R L' U' L U R' F R", },
            new Case { Type = CaseType.OLL, Name = "OLL 16", DefaultSolution = "R' F R U R' U' F' R U' R' U2 R", },
            new Case { Type = CaseType.OLL, Name = "OLL 17", DefaultSolution = "R U R' U R' F R F' U2 R' F R F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 18", DefaultSolution = "F R' F' R U R U' R' U F R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 19", DefaultSolution = "R' U2 F R U R' U' F2 U2 F R", },
            new Case { Type = CaseType.OLL, Name = "OLL 20", DefaultSolution = "R U R' U' R' F R F' R U2 R2 F R F' R U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 21", DefaultSolution = "R U R' U R U' R' U R U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 22", DefaultSolution = "R U2 R2 U' R2 U' R2 U2 R", },
            new Case { Type = CaseType.OLL, Name = "OLL 23", DefaultSolution = "R2 D R' U2 R D' R' U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 24", DefaultSolution = "R U R D R' U' R D' R2", },
            new Case { Type = CaseType.OLL, Name = "OLL 25", DefaultSolution = "R U2 R D R' U2 R D' R2", },
            new Case { Type = CaseType.OLL, Name = "OLL 26", DefaultSolution = "R U2 R' U' R U' R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 27", DefaultSolution = "R U R' U R U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 28", DefaultSolution = "F R U R' U' F2 L' U' L U F", },
            new Case { Type = CaseType.OLL, Name = "OLL 29", DefaultSolution = "R U2 R2 F R F' R U' R' U R U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 30", DefaultSolution = "F R' F R2 U' R' U' R U R' F2", },
            new Case { Type = CaseType.OLL, Name = "OLL 31", DefaultSolution = "R' U' F U R U' R' F' R", },
            new Case { Type = CaseType.OLL, Name = "OLL 32", DefaultSolution = "R U B' U' R' U R B R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 33", DefaultSolution = "R U R' U' R' F R F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 34", DefaultSolution = "R U R' U' B' R' F R F' B", },
            new Case { Type = CaseType.OLL, Name = "OLL 35", DefaultSolution = "R U2 R2 F R F' R U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 36", DefaultSolution = "R U R2 F' U' F U R2 U2 R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 37", DefaultSolution = "F R' F' R U R U' R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 38", DefaultSolution = "R U R' U R U' R' U' R' F R F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 39", DefaultSolution = "L F' L' U' L U F U' L'", },
            new Case { Type = CaseType.OLL, Name = "OLL 40", DefaultSolution = "R U R' F' U' F R U R' U' R U' R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 41", DefaultSolution = "R U R' U R U2 R' F R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 42", DefaultSolution = "R' U' R U' R' U2 R F R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 43", DefaultSolution = "R' U' F' U F R", },
            new Case { Type = CaseType.OLL, Name = "OLL 44", DefaultSolution = "F U R U' R' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 45", DefaultSolution = "F R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 46", DefaultSolution = "R' U' R' F R F' U R", },
            new Case { Type = CaseType.OLL, Name = "OLL 47", DefaultSolution = "F' L' U' L U L' U' L U F", },
            new Case { Type = CaseType.OLL, Name = "OLL 48", DefaultSolution = "F R U R' U' R U R' U' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 49", DefaultSolution = "R B' R2 F R2 B R2 F' R", },
            new Case { Type = CaseType.OLL, Name = "OLL 50", DefaultSolution = "R' F R2 B' R2 F' R2 B R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 51", DefaultSolution = "F U R U' R' U R U' R' F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 52", DefaultSolution = "R U R' U R U' B U' B' R'", },
            new Case { Type = CaseType.OLL, Name = "OLL 53", DefaultSolution = "F R U R' U' F' R U R' U' R' F R F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 54", DefaultSolution = "F' L' U' L U F U2 R U R' U' R' F R F'", },
            new Case { Type = CaseType.OLL, Name = "OLL 55", DefaultSolution = "R' F U R U' R2 F' R2 U R' U' R", },
            new Case { Type = CaseType.OLL, Name = "OLL 56", DefaultSolution = "R' F' R U' L' U L U' L' U L R' F R", },
            new Case { Type = CaseType.PLL, Name = "Aa Perm", DefaultSolution = "R' F R' B2 R F' R' B2 R2", },
            new Case { Type = CaseType.PLL, Name = "Ab Perm", DefaultSolution = "R' B' R U' R D R' U R D' R2 B R", },
            new Case { Type = CaseType.PLL, Name = "E Perm", DefaultSolution = "R' U' R' D' R U' R' D R U R' D' R U R' D R2", },
            new Case { Type = CaseType.PLL, Name = "F Perm", DefaultSolution = "R' U R U' R2 F' U' F U R F R' F' R2", },
            new Case { Type = CaseType.PLL, Name = "Ga Perm", DefaultSolution = "R2 U R' U R' U' R U' R2 D U' R' U R D'", },
            new Case { Type = CaseType.PLL, Name = "Gb Perm", DefaultSolution = "R' U' R U D' R2 U R' U R U' R U' R2 D", },
            new Case { Type = CaseType.PLL, Name = "Gc Perm", DefaultSolution = "R2 U' R U' R U R' U R2 D' U R U' R' D", },
            new Case { Type = CaseType.PLL, Name = "Gd Perm", DefaultSolution = "R U R' U' D R2 U' R U' R' U R' U R2 D'", },
            new Case { Type = CaseType.PLL, Name = "H Perm", DefaultSolution = "R2 U2 R U2 R2 U2 R2 U2 R U2 R2", },
            new Case { Type = CaseType.PLL, Name = "Ja Perm", DefaultSolution = "R U' L' U R' U2 L U' L' U2 L", },
            new Case { Type = CaseType.PLL, Name = "Jb Perm", DefaultSolution = "R U R' F' R U R' U' R' F R2 U' R'", },
            new Case { Type = CaseType.PLL, Name = "Na Perm", DefaultSolution = "F' R U R' U' R' F R2 F U' R' U' R U F' R'", },
            new Case { Type = CaseType.PLL, Name = "Nb Perm", DefaultSolution = "R' U R U' R' F' U' F R U R' F R' F' R U' R", },
            new Case { Type = CaseType.PLL, Name = "Ra Perm", DefaultSolution = "L U2 L' U2 L F' L' U' L U L F L2", },
            new Case { Type = CaseType.PLL, Name = "Rb Perm", DefaultSolution = "R' U2 R U2 R' F R U R' U' R' F' R2", },
            new Case { Type = CaseType.PLL, Name = "T Perm", DefaultSolution = "R U R' U' R' F R2 U' R' U' R U R' F'", },
            new Case { Type = CaseType.PLL, Name = "Ua Perm", DefaultSolution = "R U R' U R' U' R2 U' R' U R' U R", },
            new Case { Type = CaseType.PLL, Name = "Ub Perm", DefaultSolution = "R2 U R U R' U' R' U' R' U R'", },
            new Case { Type = CaseType.PLL, Name = "V Perm", DefaultSolution = "R' U R' U' R D' R' D R' U D' R2 U' R2 D R2", },
            new Case { Type = CaseType.PLL, Name = "Y Perm", DefaultSolution = "F R U' R' U' R U R' F' R U R' U' R' F R F'", },
            new Case { Type = CaseType.PLL, Name = "Z Perm", DefaultSolution = "R' U' R U' R U R U' R' U R U R2 U' R'", },
        };
        context.Cases.AddRange(cases);
        await context.SaveChangesAsync();
    }
}