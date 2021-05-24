using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caro2021.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }


        public Room Room { get; set; }
        public Guid RoomId { get; set; }


        public User User { get; set; }
        public Guid UserId { get; set; }

    }
}
