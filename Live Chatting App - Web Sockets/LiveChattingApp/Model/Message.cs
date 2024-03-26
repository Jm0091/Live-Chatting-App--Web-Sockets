using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChattingApp.Model
{
    /// <summary>
    /// Message model class created for storing message's details
    ///            - Creating separate model class for best practices for future expansion of model class
    /// </summary>
    public class Message
    {
        [Key]
        public Guid MsgId { get; set; }
        [Required]
        [StringLength(10)]
        public string UserName { get; set; }
        [Required]
        [StringLength(140)]
        public string Msg { get; set; }
        [Required]
        public DateTimeOffset MsgTime { get; set; }
    }
}
