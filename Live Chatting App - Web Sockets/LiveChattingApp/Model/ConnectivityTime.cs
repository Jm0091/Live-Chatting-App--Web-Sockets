using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChattingApp.Model
{
    /// <summary>
    /// ConnectivitTime model class created for storing user connection & dicoonection's time
    ///            - Creating separate model class for best practices for future expansion of model class
    /// </summary>
    public class ConnectivityTime
    {
        [Key]
        public Guid TimeId { get; set; }
        [Required]
        [StringLength(10)]
        public string UserName { get; set; }
        [Required]
        public DateTimeOffset ConnectingTime { get; set; }
        [Required]
        public DateTimeOffset DisconnectingTime { get; set; }
    }
}
