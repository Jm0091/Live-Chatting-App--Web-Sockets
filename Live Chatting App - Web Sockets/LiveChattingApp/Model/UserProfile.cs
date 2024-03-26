using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChattingApp.Model
{
    /// <summary>
    /// UserProfile model class created for storing user's value
    ///            - Creating separate model class for best practices for future expansion of model class
    /// </summary>
    public class UserProfile
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        [StringLength(10)]
        public string UserName { get; set; }
        [Required]
        public DateTimeOffset UserCreationTime { get; set; }
    }
}
