using System.ComponentModel.DataAnnotations;
namespace connectingToDBTESTING.Models
{
    public class Message : BaseEntity
    {
        [Required(ErrorMessage = "Please enter anything.")]
        [StringLength(225, ErrorMessage = "Message must be between 3 and 225 characters", MinimumLength = 3)]
        [Display(Name = "Message:")]
        public string Text { get; set; }

    }
}