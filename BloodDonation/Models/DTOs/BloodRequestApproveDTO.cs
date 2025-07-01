using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class BloodRequestApproveDTO
    {
        [Required]
        public Guid RequestId { get; set; }
    }
}