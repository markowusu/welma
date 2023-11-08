using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WelmaTransactions.Models
{
    public class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public Guid UserId { get; set; }

        // Navigation property for the related user
        public User User { get; set; }

        // Collection of transactions related to this wallet ---> This is because a wallet can have many transactions
        public ICollection<Transaction> Transactions { get; set; }

        public Wallet()
        {
            //TODO: What is there is a bonus that needs to be given on wallet creation what should happen. 
            // TODO: This function can take parameter then, instead of hard coding the value in.
    // Initialize properties with default values if necessary
            Balance = 0;
        }
    }
}


//TODO: FEATURE IMPLEMENTATION WILL HAVE TO CONSIDER A USER HAVING MUTLIPLE WALLETS. JUST LIKE THE IMPLEMENTATION OF WISE. 
// TODO: A use should be allowed to hold multiple wallets in multiple currencies in the system
