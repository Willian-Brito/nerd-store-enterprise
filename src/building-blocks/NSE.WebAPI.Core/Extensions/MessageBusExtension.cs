using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Bus;
using NSE.Core.DomainObjects;

namespace NSE.WebAPI.Core.Extensions;

public static class MessageBusExtension
{
    public static async Task PublishEvents<T>(this IMessageBus messageBus, T ctx) where T : DbContext
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Notifications)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        //var tasks = domainEvents
        //    .Select(async (domainEvent) => {
        //        await messageBus.PublishEvent(domainEvent);
        //    });
        // await Task.WhenAll(tasks);

        foreach (var task in domainEvents) await messageBus.PublishEvent(task);
    }
}