using CarMarketplace;
using CarMarketplace.DAO;
using Microsoft.IdentityModel.Protocols;

class Program
{
    static async Task Main()
    {
        Service service = new Service();

        //1)
        //await service.GetData();

        //2)
        //What would you do if you had data that doesn’t change often
        //but it’s used pretty much all the time?

        //Would implement a caching mechanism to store this data in memory,
        //for example using the interfaceIMemoryCache.


        //Would it make a difference if you have more than one instance of your application?
        //Yes, because each instance has its own cache, and in case there is a change,
        //all instances should see it. In that case, I would use distributed cache like Redis

        //3)
        //await service.UpdateCustomersBalanceByInvoicesAsync(new List<Invoice>());

        //4)
        //await service.GetOrders(DateTime.Now, DateTime.Now, new List<int>(), new List<int>(), true);

        //5)
        //var basePath = AppContext.BaseDirectory;
        //var folderPath = Path.Combine(basePath, "CSFiles");
        //await service.ProcessCsFiles(folderPath);
    }

}
