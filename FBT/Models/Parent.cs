namespace FBT.Models;

public partial class Parent
{
    public string ParentId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string? Job { get; set; }

    public virtual PersonInformation ParentNavigation { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
