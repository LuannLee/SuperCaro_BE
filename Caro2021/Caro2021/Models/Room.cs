using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caro2021.Models
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        // InActive = 0 ; Active = 1 ; InProcessing = 2
        public int Status { get; set; }



        public List<Message> Messages { get; set; }

    }
}
