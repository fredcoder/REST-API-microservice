using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Infrastructure.Entities
{
    public class ContactDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string MobileNumber { get; set; } = string.Empty;
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; } = string.Empty;
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
