## 1. Domain Command

- [x] 1.1 Create `src/Core/Model/StateCommands/CompleteToInProgressCommand.cs`
  - Inherits `StateCommandBase`
  - `GetBeginStatus()` returns `WorkOrderStatus.Complete`
  - `GetEndStatus()` returns `WorkOrderStatus.InProgress`
  - `UserCanExecute()` checks `currentUser == WorkOrder.Assignee`
  - `TransitionVerbPresentTense` = `"Reopen"`, `TransitionVerbPastTense` = `"Reopened"`
  - `Execute()` clears `WorkOrder.CompletedDate = null` before calling `base.Execute()`

## 2. Registration

- [x] 2.1 Register `CompleteToInProgressCommand` in `src/Core/Services/Impl/StateCommandList.cs` `GetAllStateCommands()`

## 3. MCP Server

- [x] 3.1 Update `execute-work-order-command` tool description in `src/McpServer/Tools/WorkOrderTools.cs` to list `CompleteToInProgressCommand` and `Reopen`
- [x] 3.2 Add `"CompleteToInProgressCommand"` and `"Reopen"` cases to the command switch in `ExecuteWorkOrderCommand`

## 4. Acceptance Test Infrastructure

- [x] 4.1 Add `ReopenExistingWorkOrder()` helper to `src/AcceptanceTests/AcceptanceTestBase.cs`

## 5. Unit Tests

- [x] 5.1 Create `src/UnitTests/Core/Model/StateCommands/CompleteToInProgressCommandTests.cs`
  - `ShouldNotBeValidInWrongStatus` — fails when work order is not in `Complete` status
  - `ShouldNotBeValidWithWrongEmployee` — fails when current user is not the assignee
  - `ShouldBeValid` — passes when work order is `Complete` and current user is assignee
  - `ShouldTransitionStateProperly` — transitions to `InProgress` and clears `CompletedDate`

## 6. Acceptance Tests

- [x] 6.1 Create `src/AcceptanceTests/WorkOrders/WorkOrderReopenTests.cs`
  - `ShouldReopenWorkOrder` — full workflow (create → assign → begin → complete → reopen); asserts status is `InProgress` in DB and `CompletedDate` is null
  - `ReopenedWorkOrderShouldBeEditable` — asserts Title and Description fields are enabled after reopen

## 7. Architecture Documentation

- [x] 7.1 Update `arch/arch-state-workorder.md` — add `Complete --> InProgress : CompleteToInProgressCommand` to the state diagram
