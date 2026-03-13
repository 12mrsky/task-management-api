using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public int AssignedTo { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}