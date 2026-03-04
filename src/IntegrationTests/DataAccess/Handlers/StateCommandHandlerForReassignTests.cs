using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.DataAccess.Handlers;
using ClearMeasure.Bootcamp.UnitTests.Core.Queries;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess.Handlers;

public class StateCommandHandlerForReassignTests : IntegratedTestBase
{
    [Test]
    public async Task ShouldReassignWorkOrderFromCompleteToAssigned()
    {
        new DatabaseTests().Clean();

        var o = Faker<WorkOrder>();
        o.Id = Guid.Empty;
        var creator = Faker<Employee>();
        var originalAssignee = Faker<Employee>();
        var newAssignee = Faker<Employee>();
        o.Creator = creator;
        o.Assignee = originalAssignee;
        o.Status = WorkOrderStatus.Complete;
        o.CompletedDate = DateTime.UtcNow.AddDays(-1);
        await using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(creator);
            context.Add(originalAssignee);
            context.Add(newAssignee);
            context.Add(o);
            await context.SaveChangesAsync();
        }

        o.Assignee = newAssignee;
        var command = new CompleteToAssignedCommand(o, creator);
        var remotedCommand = RemotableRequestTests.SimulateRemoteObject(command);

        var handler = TestHost.GetRequiredService<StateCommandHandler>();
        var result = await handler.Handle(remotedCommand);

        var context3 = TestHost.GetRequiredService<DbContext>();
        var order = context3.Find<WorkOrder>(result.WorkOrder.Id) ?? throw new InvalidOperationException();
        order.Status.ShouldBe(WorkOrderStatus.Assigned);
        order.CompletedDate.ShouldBeNull();
        order.AssignedDate.ShouldBe(TestHost.TestTime.DateTime);
        order.Assignee.ShouldBe(newAssignee);
        order.Creator.ShouldBe(creator);
    }
}
