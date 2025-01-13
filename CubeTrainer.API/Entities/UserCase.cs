namespace CubeTrainer.API.Entities;

internal class UserCase
{
    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public Guid CaseId { get; set; }

    public Case Case { get; set; } = default!;

    public Guid? SelectedAlgorithmId { get; set; }

    public Algorithm? SelectedAlgorithm { get; set; }

    public UserCaseStatus Status { get; set; }
}