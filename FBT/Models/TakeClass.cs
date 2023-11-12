namespace FBT.Models
{
    public partial class TakeClass
    {
        public string ClassId { get; set; } = null!;
        public string StudentId { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
