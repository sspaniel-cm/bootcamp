using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Model.StateCommands;

[TestFixture]
public class CompleteToAssignedCommandTests : StateCommandBaseTests
{
    [Test]
    public void ShouldNotBeValidInWrongStatus()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.InProgress;
        var employee = new Employee();
        order.Creator = employee;

        var command = new CompleteToAssignedCommand(order, employee);
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldNotBeValidWithWrongEmployee()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Complete;
        var employee = new Employee();
        order.Creator = employee;

        var command = new CompleteToAssignedCommand(order, new Employee());
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldNotBeValidForAssigneeWhoIsNotCreator()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Complete;
        var creator = new Employee();
        var assignee = new Employee();
        order.Creator = creator;
        order.Assignee = assignee;

        var command = new CompleteToAssignedCommand(order, assignee);
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldBeValidForCreator()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Complete;
        var employee = new Employee();
        order.Creator = employee;

        var command = new CompleteToAssignedCommand(order, employee);
        Assert.That(command.IsValid(), Is.True);
    }

    [Test]
    public void ShouldTransitionToAssigned()
    {
        var order = new WorkOrder();
        order.Number = "123";
        order.Status = WorkOrderStatus.Complete;
        order.CompletedDate = DateTime.UtcNow.AddDays(-1);
        var employee = new Employee();
        order.Creator = employee;

        var command = new CompleteToAssignedCommand(order, employee);
        command.Execute(new StateCommandContext());

        Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Assigned));
    }

    [Test]
    public void ShouldClearCompletedDate()
    {
        var order = new WorkOrder();
        order.Number = "123";
        order.Status = WorkOrderStatus.Complete;
        order.CompletedDate = DateTime.UtcNow.AddDays(-1);
        var employee = new Employee();
        order.Creator = employee;

        var command = new CompleteToAssignedCommand(order, employee);
        command.Execute(new StateCommandContext());

        Assert.That(order.CompletedDate, Is.Null);
    }

    [Test]
    public void ShouldSetAssignedDate()
    {
        var order = new WorkOrder();
        order.Number = "123";
        order.Status = WorkOrderStatus.Complete;
        var employee = new Employee();
        order.Creator = employee;

        var context = new StateCommandContext { CurrentDateTime = new DateTime(2026, 3, 4) };
        var command = new CompleteToAssignedCommand(order, employee);
        command.Execute(context);

        Assert.That(order.AssignedDate, Is.EqualTo(new DateTime(2026, 3, 4)));
    }

    protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
    {
        return new CompleteToAssignedCommand(order, employee);
    }
}
