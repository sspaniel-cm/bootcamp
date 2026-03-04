using ClearMeasure.Bootcamp.Core.Queries;
using ClearMeasure.Bootcamp.UI.Shared.Pages;

namespace ClearMeasure.Bootcamp.AcceptanceTests.WorkOrders;

public class WorkOrderReassignTests : AcceptanceTestBase
{
    [Test, Retry(2)]
    public async Task ShouldReassignWorkOrderFromCompleteToAssigned()
    {
        await LoginAsCurrentUser();

        var order = await CreateAndSaveNewWorkOrder();
        order = await ClickWorkOrderNumberFromSearchPage(order);
        order = await AssignExistingWorkOrder(order, CurrentUser.UserName);
        order = await ClickWorkOrderNumberFromSearchPage(order);
        order = await BeginExistingWorkOrder(order);
        order = await ClickWorkOrderNumberFromSearchPage(order);
        order = await CompleteExistingWorkOrder(order);
        order = await ClickWorkOrderNumberFromSearchPage(order);

        order = await ReassignExistingWorkOrder(order, CurrentUser.UserName);
        order = await ClickWorkOrderNumberFromSearchPage(order);

        var rehyratedOrder = await Bus.Send(new WorkOrderByNumberQuery(order.Number!)) ??
                             throw new InvalidOperationException();
        rehyratedOrder.Status.ShouldBe(WorkOrderStatus.Assigned);
        rehyratedOrder.CompletedDate.ShouldBeNull();

        await Expect(Page.GetByTestId(nameof(WorkOrderManage.Elements.Status)))
            .ToHaveTextAsync(WorkOrderStatus.Assigned.FriendlyName);
    }
}
