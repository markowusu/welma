using WelmaTransactions.Models;

namespace Welma.Core.Database.DTO{


    public class WalletCreationDto
{
    public Guid UserId { get; set; }
}

public class WalletTopUpDto
{ 
    public decimal AdditionalBalance { get; set; }
}



public class WalletDtoResponse{
     public decimal Balance { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set;}

    public  Guid Id { get; set; }
}



}