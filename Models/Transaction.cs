using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WelmaTransactions.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string OrderId { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime Timestamp { get; set; }

         public Guid UserId { get; set; }

    // Navigation property
        public User User { get; set; }

        public Transaction()
        {
            // this is a helper function to initialize the timestamp
            
            Timestamp = DateTime.Now;
        }
    }
}
