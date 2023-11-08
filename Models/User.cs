
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WelmaTransactions.Models

{
    public class User{

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public Guid Id { get; set; }

        [Required]
        public string Email {get; set;} = null!;

        public string Name {get; set;} = null!;

        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

        public string? ContactNumber {get; set;} 

        [JsonIgnore]
        public List<Transaction> Transactions { get; set; }

        [JsonIgnore]
         public ICollection<Wallet> Wallets { get; set; }


          public User()
        {
            // even though a suer does not have a transaction at the time of creation I want to initialize the list to ab empty list to hold all transactions later on
            Wallets = new List<Wallet>();
            Transactions = new List<Transaction>();
        }


        /*   
        Following the  Single Responsibility approach , there is not need to add business logic to the  Model as it will violate the principle and make it closed to just one logic of 
        performing  such business logic. Using a service implementation will be the best . Has it will ve more extensile to change to different methods of business logics.

        Separation of concern will not be adhered making the code  model have a business logic and data access properties 

        Example: what if we want to add a third party service later on how am I going to that? The only way is to change the model and do a migration on the database. This is not efficient and not maintainable.

        */




    }
}


// Caveats** ( Intentional) : I would say it is a matter of  finding the balance and knowing when  and where to take advantage of that.
// I added navigation properties to the wallet and transaction model. Just for a tradeoff in this project access data immediately without unnecessary queries  
// In a real world application; adding navigation properties is not the best practice as the database needs to be nomarlized and ensuring that we rather keep relations among  tables. 