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
            new Case { Type = CaseType.OLL, Name = "OLL 57", DefaultSolution = "L' R U R' U' L R' F R F'", },
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

        var createdAt = new DateTime(2025, 1, 1);
        var algorithms = new[]
        {
            new Algorithm { Case = cases[0], IsPublic = true, Moves = "R U2 R2 F R F' U2 R' F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[1], IsPublic = true, Moves = "y' F R U R' U' F' f R U R' U' f'", CreatedAt = createdAt },
            new Algorithm { Case = cases[2], IsPublic = true, Moves = "y2 f R U R' U' f' U' F R U R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[3], IsPublic = true, Moves = "y2 f R U R' U' f' U F R U R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[4], IsPublic = true, Moves = "y' r' U2 R U R' U r", CreatedAt = createdAt },
            new Algorithm { Case = cases[5], IsPublic = true, Moves = "r U2 R' U' R U' r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[6], IsPublic = true, Moves = "r U R' U R U2 r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[7], IsPublic = true, Moves = "y2 r' U' R U' R' U2 r", CreatedAt = createdAt },
            new Algorithm { Case = cases[8], IsPublic = true, Moves = "y' R U R' U' R' F R2 U R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[9], IsPublic = true, Moves = "R U R' U R' F R F' R U2 R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[10], IsPublic = true, Moves = "y' M R U R' U R U2 R' U M'", CreatedAt = createdAt },
            new Algorithm { Case = cases[11], IsPublic = true, Moves = "y' M' R' U' R U' R' U2 R U' M", CreatedAt = createdAt },
            new Algorithm { Case = cases[12], IsPublic = true, Moves = "r U' r' U' r U r' F' U F", CreatedAt = createdAt },
            new Algorithm { Case = cases[13], IsPublic = true, Moves = "R' F R U R' F' R F U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[14], IsPublic = true, Moves = "y2 r' U' r R' U' R U r' U r", CreatedAt = createdAt },
            new Algorithm { Case = cases[15], IsPublic = true, Moves = "y2 r U r' R U R' U' r U' r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[16], IsPublic = true, Moves = "R U R' U R' F R F' U2 R' F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[17], IsPublic = true, Moves = "y R U2 R2 F R F' U2 M' U R U' r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[18], IsPublic = true, Moves = "M U R U R' U' M' R' F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[19], IsPublic = true, Moves = "r U R' U' M2 U R U' R' U' M'", CreatedAt = createdAt },
            new Algorithm { Case = cases[20], IsPublic = true, Moves = "R U R' U R U' R' U R U2 R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[21], IsPublic = true, Moves = "R U2 R2' U' R2 U' R2' U2 R", CreatedAt = createdAt },
            new Algorithm { Case = cases[22], IsPublic = true, Moves = "R2 D R' U2 R D' R' U2 R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[23], IsPublic = true, Moves = "y' r U R' U' r' F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[24], IsPublic = true, Moves = "y F' r U R' U' r' F R", CreatedAt = createdAt },
            new Algorithm { Case = cases[25], IsPublic = true, Moves = "R U2 R' U' R U' R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[26], IsPublic = true, Moves = "R U R' U R U2 R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[27], IsPublic = true, Moves = "r U R' U' M U R U' R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[28], IsPublic = true, Moves = "y2 R U R' U' R U' R' F' U' F R U R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[29], IsPublic = true, Moves = "F U R U2 R' U' R U2 R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[30], IsPublic = true, Moves = "R' U' F U R U' R' F' R", CreatedAt = createdAt },
            new Algorithm { Case = cases[31], IsPublic = true, Moves = "S R U R' U' R' F R f'", CreatedAt = createdAt },
            new Algorithm { Case = cases[32], IsPublic = true, Moves = "R U R' U' R' F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[33], IsPublic = true, Moves = "R U R2 U' R' F R U R U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[34], IsPublic = true, Moves = "R U2 R2 F R F' R U2 R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[35], IsPublic = true, Moves = "y L' U' L U' L' U L U L F' L' F", CreatedAt = createdAt },
            new Algorithm { Case = cases[36], IsPublic = true, Moves = "F R U' R' U' R U R' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[37], IsPublic = true, Moves = "R U R' U R U' R' U' R' F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[38], IsPublic = true, Moves = "L F' L' U' L U F U' L'", CreatedAt = createdAt },
            new Algorithm { Case = cases[39], IsPublic = true, Moves = "R' F R U R' U' F' U R", CreatedAt = createdAt },
            new Algorithm { Case = cases[40], IsPublic = true, Moves = "R U R' U R U2 R' F R U R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[41], IsPublic = true, Moves = "R' U' R U' R' U2 R F R U R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[42], IsPublic = true, Moves = "R' U' F' U F R", CreatedAt = createdAt },
            new Algorithm { Case = cases[43], IsPublic = true, Moves = "y2 f R U R' U' f'", CreatedAt = createdAt },
            new Algorithm { Case = cases[44], IsPublic = true, Moves = "F R U R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[45], IsPublic = true, Moves = "R' U' R' F R F' U R", CreatedAt = createdAt },
            new Algorithm { Case = cases[46], IsPublic = true, Moves = "F' L' U' L U L' U' L U F", CreatedAt = createdAt },
            new Algorithm { Case = cases[47], IsPublic = true, Moves = "F R U R' U' R U R' U' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[48], IsPublic = true, Moves = "y2 r U' r2 U r2 U r2 U' r", CreatedAt = createdAt },
            new Algorithm { Case = cases[49], IsPublic = true, Moves = "y2 r' U r2 U' r2 U' r2 U r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[50], IsPublic = true, Moves = "y2 f R U R' U' R U R' U' f'", CreatedAt = createdAt },
            new Algorithm { Case = cases[51], IsPublic = true, Moves = "y2 R' F' U' F U' R U R' U R", CreatedAt = createdAt },
            new Algorithm { Case = cases[52], IsPublic = true, Moves = "y2 r' U' R U' R' U R U' R' U2 r", CreatedAt = createdAt },
            new Algorithm { Case = cases[53], IsPublic = true, Moves = "r U R' U R U' R' U R U2 r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[54], IsPublic = true, Moves = "y R U2 R2 U' R U' R' U2 F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[55], IsPublic = true, Moves = "r U r' U R U' R' U R U' R' r U' r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[56], IsPublic = true, Moves = "R U R' U' M' U R U' r'", CreatedAt = createdAt },
            new Algorithm { Case = cases[57], IsPublic = true, Moves = "x R' U R' D2 R U' R' D2 R2 x'", CreatedAt = createdAt },
            new Algorithm { Case = cases[58], IsPublic = true, Moves = "x R2 D2 R U R' D2 R U' R x'", CreatedAt = createdAt },
            new Algorithm { Case = cases[59], IsPublic = true, Moves = "x' R U' R' D R U R' D' R U R' D R U' R' D' x", CreatedAt = createdAt },
            new Algorithm { Case = cases[60], IsPublic = true, Moves = "y R' U' F' R U R' U' R' F R2 U' R' U' R U R' U R", CreatedAt = createdAt },
            new Algorithm { Case = cases[61], IsPublic = true, Moves = "R2 U R' U R' U' R U' R2 D U' R' U R D'", CreatedAt = createdAt },
            new Algorithm { Case = cases[62], IsPublic = true, Moves = "R' U' R U D' R2 U R' U R U' R U' R2 D", CreatedAt = createdAt },
            new Algorithm { Case = cases[63], IsPublic = true, Moves = "R2 U' R U' R U R' U R2 D' U R U' R' D", CreatedAt = createdAt },
            new Algorithm { Case = cases[64], IsPublic = true, Moves = "R U R' U' D R2 U' R U' R' U R' U R2 D'", CreatedAt = createdAt },
            new Algorithm { Case = cases[65], IsPublic = true, Moves = "M2 U' M2 U2 M2 U' M2", CreatedAt = createdAt },
            new Algorithm { Case = cases[66], IsPublic = true, Moves = "y R' U L' U2 R U' R' U2 R L", CreatedAt = createdAt },
            new Algorithm { Case = cases[67], IsPublic = true, Moves = "R U R' F' R U R' U' R' F R2 U' R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[68], IsPublic = true, Moves = "R U R' U R U R' F' R U R' U' R' F R2 U' R' U2 R U' R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[69], IsPublic = true, Moves = "R' U R U' R' F' U' F R U R' F R' F' R U' R", CreatedAt = createdAt },
            new Algorithm { Case = cases[70], IsPublic = true, Moves = "y R U' R' U' R U R D R' U' R D' R' U2 R'", CreatedAt = createdAt },
            new Algorithm { Case = cases[71], IsPublic = true, Moves = "R' U2 R U2 R' F R U R' U' R' F' R2", CreatedAt = createdAt },
            new Algorithm { Case = cases[72], IsPublic = true, Moves = "R U R' U' R' F R2 U' R' U' R U R' F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[73], IsPublic = true, Moves = "y2 M2 U M U2 M' U M2", CreatedAt = createdAt },
            new Algorithm { Case = cases[74], IsPublic = true, Moves = "M2 U' M U2 M' U' M2", CreatedAt = createdAt },
            new Algorithm { Case = cases[75], IsPublic = true, Moves = "R' U R' U' R D' R' D R' U D' R2 U' R2 D R2", CreatedAt = createdAt },
            new Algorithm { Case = cases[76], IsPublic = true, Moves = "F R U' R' U' R U R' F' R U R' U' R' F R F'", CreatedAt = createdAt },
            new Algorithm { Case = cases[77], IsPublic = true, Moves = "y M2 U M2 U M' U2 M2 U2 M'", CreatedAt = createdAt },
        };
        context.Algorithms.AddRange(algorithms);
        await context.SaveChangesAsync();
    }
}