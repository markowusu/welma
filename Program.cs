using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Welma.Core.Database;
using Welma.Core.Database.DTO;
using WelmaTransactions.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<DataContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Handle circular references
    NullValueHandling = NullValueHandling.Ignore // Ignore null values
};


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

var app = builder.Build();


const int MaxTokens = 1000; // Define the maximum number of tokens to store

string[] processedTokens = new string[MaxTokens]; // Array to store processed tokens
int tokenIndex = 0;


DotNetEnv.Env.Load(); 


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/CreateUser", async (UserDto userDto , DataContext db ) => {

    if (userDto is null ){
        return Results.BadRequest("Invalid user data");
    }

    var user = new User
    {
        Email = userDto.Email,
        Name = userDto.Name,
        ContactNumber = userDto.ContactNumber,
        CreatedAt = DateTime.UtcNow
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/create/{user.Id}", user);
   
});

app.MapGet("/GetAllUsers", async (DataContext db) =>  
     await db.Users.ToListAsync());

//  performing an update operation 
app.MapPut("/UpdateUser", async (Guid id , UserUpdateDto userInput, DataContext db) =>{

    var user = await db.Users.FindAsync(id);

    if (user is null ){
        return Results.NotFound();
    }

    user.Email = userInput.Email;
    user.Name = userInput.Name;
    user.ContactNumber = userInput.ContactNumber;

    await db.SaveChangesAsync();

    return Results.NoContent();

}
);


app.MapPost("/CreateTransaction", async (Transaction transaction, DataContext db) =>
{
    db.Transactions.Add(transaction);
    await db.SaveChangesAsync();
    return Results.Created($"/CreateTransaction/{transaction.Id}", transaction);
});


app.MapGet("/GetAllTransactions", async (DataContext db) =>
{
    var transactions = await db.Transactions.ToListAsync();
    return Results.Ok(transactions);
});


app.MapPut("/UpdateTransaction", async (Guid id, Transaction userInput, DataContext db) =>
{
    var transaction = await db.Transactions.FindAsync(id);

    if (transaction is null)
    {
        return Results.NotFound();
    }

    transaction.OrderId = userInput.OrderId;
    transaction.Name = userInput.Name;

    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.MapGet("/GetAllWallets", async (DataContext db) =>
{
    var wallets = await db.Wallets.ToListAsync();
    return Results.Ok(wallets);
});

app.MapPost("/CreateWallet", async (WalletCreationDto walletDto, DataContext db) =>
{   
    if (walletDto is null){
        return Results.BadRequest("Invalid wallet data");
    }

    var user = await db.Users.FindAsync(walletDto.UserId);

    if (user is null ){
        return Results.NotFound("User not found");
    }


     // Check if the user already has a wallet
    var existingWallet = await db.Wallets.FirstOrDefaultAsync(w => w.UserId == user.Id);

    if (existingWallet != null)
    {
        // User already has a wallet, return a conflict response or handle as desired
        return Results.Conflict("User already has a wallet");
    }

    
    var wallet = new Wallet
    {
        UserId = walletDto.UserId,
        User = user
    };

    db.Wallets.Add(wallet);

    await db.SaveChangesAsync();

    // var jsonString = JsonConvert.SerializeObject(wallet, Formatting.None);   // Used to resolve the circular reference problem

    var walletDtoResponse = new WalletDtoResponse
    {
        Balance = wallet.Balance,
        UserId = wallet.UserId,
        Id = wallet.Id,
        
    };


    return Results.Created($"/CreateWallet/{wallet.Id}",walletDtoResponse);
});


app.MapPut("/TopUpWallet", async (Guid id, WalletTopUpDto walletTopDto, DataContext db, string requestToken) =>
{

    // Edge case ==> ensure idempotence 
    

    // request Token will come from the FrontEnd

    // making sure  a user cannot make double top request with the same token id generated for that specific token. 

    // this might happen when a user loose connection and the transaction has been but the was an out of service interaction 

    // creating a request toke for each request and caching it or storing it a distributed cache system will hep us check against multiple request.

    // TODO: Advance resolution is create a timeout for retry when there is an error in delivering a service. Especially in microservice architectures 
    //TODO: Implement this using a  redis cache. and set TTL for all the tokens so prevent accumulation in the DB the token.
    
    if (!ArrayContains(processedTokens, tokenIndex, requestToken)){

    var wallet = await db.Wallets.FindAsync(id);

    if (walletTopDto is null)
    {
        return Results.NotFound();
    }

    wallet!.Balance += walletTopDto.AdditionalBalance;

     if (tokenIndex < MaxTokens) {
            processedTokens[tokenIndex] = requestToken;
            tokenIndex++;
     }

    await db.SaveChangesAsync();

    return Results.Ok($"You have successfully top up your wallet with {walletTopDto.AdditionalBalance}");
     }
     else
     {
             return Results.NoContent();
     }
    
});

// This is not efficient but it used to explain the concept of idempotence when it comes to transactions that a crucial and can lead to loss of money or put the companies fame on the line. 
// This is an O(n) time complexity that can take log to find, hash table can be used for this implementation to as well. 
bool ArrayContains(string[] array, int length, string token) {
    for (int i = 0; i < length; i++) {
        if (array[i] == token) {
            return true;
        }
    }
    return false;
}

app.Run();
