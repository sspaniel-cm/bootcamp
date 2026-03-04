using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.UI.Shared.Pages;

namespace ClearMeasure.Bootcamp.AcceptanceTests.WorkOrders;

public class WorkOrderUnassignTests : AcceptanceTestBase
{
    [Test, Retry(2)]
    public async Task ShouldAssignThenUnassignWorkOrder()
    {
        await LoginAsCurrentUser();

        var order = await CreateAndSaveNewWorkOrder();
        order = await ClickWorkOrderNumberFromSearchPage(order);
        order = await AssignExistingWorkOrder(order, CurrentUser.UserName);
        order = await ClickWorkOrderNumberFromSearchPage(order);

        await Click(nameof(WorkOrderManage.Elements.CommandButton) + AssignedToDraftCommand.Name);
        order = await ClickWorkOrderNumberFromSearchPage(order);

        await Expect(Page.GetByTestId(nameof(WorkOrderManage.Elements.Status))).ToHaveTextAsync(WorkOrderStatus.Draft.FriendlyName);
    }
}
