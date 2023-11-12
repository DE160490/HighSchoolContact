namespace FBT.Models
{
    public partial class HomeroomTeacher
    {
        public string TeacherId { get; set; } = null!;
        public string ClassId { get; set; } = null!;
        public virtual Teacher Teacher { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
    }
}
