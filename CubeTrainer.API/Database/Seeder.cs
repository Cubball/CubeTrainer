using CubeTrainer.API.Common.Helpers;
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
            new Case { Type = CaseType.OLL, Name = "OLL 1", DefaultSolution = "R' U' R' F R F' U R", },
            new Case { Type = CaseType.OLL, Name = "OLL 2", DefaultSolution = "F R U R' U' F", },
            new Case { Type = CaseType.PLL, Name = "PLL 1", DefaultSolution = "R2 U R' U R' U' R U' R2 U' D R' U R D'", },
            new Case { Type = CaseType.PLL, Name = "PLL 2", DefaultSolution = "R2 F R U R U' R' F' R U2 R' U2 R", },
        };
        context.Cases.AddRange(cases);
        await context.SaveChangesAsync();

        var algorithms = new[]
        {
            new Algorithm
            {
                CaseId = cases[0].Id,
                CreatorId = user.Id,
                Moves = "R U R' U' R' F R2 U' R' U' R U R' F'",
                IsPublic = true,
                UsersCount = 1,
            },
            new Algorithm
            {
                CaseId = cases[1].Id,
                CreatorId = user.Id,
                Moves = "R U R' U R U2 R'",
                IsPublic = false,
                UsersCount = 1,
            },
            new Algorithm
            {
                CaseId = cases[2].Id,
                CreatorId = user.Id,
                Moves = "R U R' U R U2 R'",
                IsPublic = true,
                UsersCount = 2,
                TotalRating = 5,
                UsersRatingsCount = 1,
            },
            new Algorithm
            {
                CaseId = cases[3].Id,
                CreatorId = user.Id,
                Moves = "R U R' U R U2 R'",
                IsPublic = true,
                UsersCount = 2,
            },
            new Algorithm
            {
                CaseId = cases[0].Id,
                CreatorId = otherUser.Id,
                Moves = "R U R' U' R' F R2 U' R' U' R U R' F'",
                IsPublic = false,
                UsersCount = 1,
            },
            new Algorithm
            {
                CaseId = cases[1].Id,
                CreatorId = otherUser.Id,
                Moves = "R U R' U R U2 R'",
                IsPublic = true,
                UsersCount = 2,
                TotalRating = 2,
                UsersRatingsCount = 1,
            },
            new Algorithm
            {
                CaseId = cases[2].Id,
                CreatorId = otherUser.Id,
                Moves = "R U R' U R U2 R'",
                IsPublic = true,
                UsersCount = 1,
            },
            new Algorithm
            {
                CaseId = cases[3].Id,
                CreatorId = otherUser.Id,
                Moves = "R U R' U R U2 R'",
                IsPublic = true,
                UsersCount = 1,
            },
        };
        context.Algorithms.AddRange(algorithms);
        await context.SaveChangesAsync();

        var userCases = new[]
        {
            new UserCase
            {
                UserId = user.Id,
                CaseId = cases[0].Id,
                SelectedAlgorithmId = algorithms[0].Id,
                Status = UserCaseStatus.InProgress,
            },
            new UserCase
            {
                UserId = user.Id,
                CaseId = cases[1].Id,
                SelectedAlgorithmId = algorithms[5].Id,
                Status = UserCaseStatus.Learned,
            },
            new UserCase
            {
                UserId = user.Id,
                CaseId = cases[2].Id,
                Status = UserCaseStatus.Learned,
            },
            new UserCase
            {
                UserId = otherUser.Id,
                CaseId = cases[0].Id,
                SelectedAlgorithmId = algorithms[2].Id,
                Status = UserCaseStatus.Learned,
            },
            new UserCase
            {
                UserId = otherUser.Id,
                CaseId = cases[1].Id,
                SelectedAlgorithmId = algorithms[3].Id,
                Status = UserCaseStatus.InProgress,
            },
        };
        context.UserCases.AddRange(userCases);
        await context.SaveChangesAsync();

        var algorithmStatistics = new[]
        {
            new AlgorithmStatistic
            {
                AlgorithmId = algorithms[0].Id,
                UserId = user.Id,
                TotalTimeSolvingInSeconds = 100,
                TimedSolvesCount = 10,
                UntimedSolvesCount = 5,
                BestTimeInSeconds = 5,
            },
            new AlgorithmStatistic
            {
                AlgorithmId = algorithms[1].Id,
                UserId = user.Id,
                TotalTimeSolvingInSeconds = 200,
                TimedSolvesCount = 20,
                UntimedSolvesCount = 10,
                BestTimeInSeconds = 6.9M,
            },
            new AlgorithmStatistic
            {
                AlgorithmId = algorithms[2].Id,
                UserId = otherUser.Id,
                TotalTimeSolvingInSeconds = 69,
                TimedSolvesCount = 10,
                UntimedSolvesCount = 0,
                BestTimeInSeconds = 4.20M,
            },
            new AlgorithmStatistic
            {
                AlgorithmId = algorithms[3].Id,
                UserId = otherUser.Id,
                TotalTimeSolvingInSeconds = 0,
                TimedSolvesCount = 0,
                UntimedSolvesCount = 1337,
            },
        };
        context.AlgorithmStatistics.AddRange(algorithmStatistics);
        await context.SaveChangesAsync();

        var ratings = new[]
        {
            new AlgorithmRating
            {
                AlgorithmId = algorithms[2].Id,
                UserId = otherUser.Id,
                Rating = 5,
            },
            new AlgorithmRating
            {
                AlgorithmId = algorithms[5].Id,
                UserId = user.Id,
                Rating = 2,
            },
        };
        context.AlgorithmRatings.AddRange(ratings);
        await context.SaveChangesAsync();

        var trainingPlans = new[]
        {
            new TrainingPlan
            {
                UserId = user.Id,
                Name = "Plan 1",
            },
            new TrainingPlan
            {
                UserId = user.Id,
                Name = "Plan 2",
            },
            new TrainingPlan
            {
                UserId = otherUser.Id,
                Name = "Plan 3",
            },
        };
        context.TrainingPlans.AddRange(trainingPlans);
        await context.SaveChangesAsync();

        var trainingPlanCases = new[]
        {
            new TrainingPlanCase
            {
                TrainingPlanId = trainingPlans[0].Id,
                CaseId = cases[0].Id,
                LastSolved = DateTimeHelpers.UtcNow.AddDays(-1),
                SolvesToLearnCount = 2,
                LastDifficultyRating = DifficultyRating.Normal,
            },
            new TrainingPlanCase
            {
                TrainingPlanId = trainingPlans[0].Id,
                CaseId = cases[3].Id,
                LastSolved = DateTimeHelpers.UtcNow.AddDays(-2),
                SolvesToLearnCount = 4,
                LastDifficultyRating = DifficultyRating.Hard,
            },
            new TrainingPlanCase
            {
                TrainingPlanId = trainingPlans[1].Id,
                CaseId = cases[1].Id,
                LastSolved = DateTimeHelpers.UtcNow.AddDays(-1),
                SolvesToLearnCount = 1,
                LastDifficultyRating = DifficultyRating.Easy,
            },
        };
        context.TrainingPlanCases.AddRange(trainingPlanCases);
        await context.SaveChangesAsync();
    }
}