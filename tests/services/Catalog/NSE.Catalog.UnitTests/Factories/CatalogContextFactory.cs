using Microsoft.EntityFrameworkCore;
using Moq;
using NSE.Catalog.API.Data;
using NSE.Security.Identity.User;

namespace NSE.Catalog.UnitTests.Factories;

public class CatalogContextFactory
{
    public static CatalogContext Create()
    {
        var options = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        var userMock = new Mock<IAspNetUser>();
        userMock.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
        userMock.Setup(x => x.IsAuthenticated()).Returns(true);

        return new CatalogContext(options, userMock.Object);
    }
}