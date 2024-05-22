using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace FinanceFolio.Models
{
    [CollectionName("Users")]
    public class ApplicationUser:MongoIdentityUser<Guid>
    {
    }
}
