## Why

Work orders can currently be completed (`InProgress → Complete`) but `Complete` is a terminal state with no way to revert. Assignees who discover additional work, realize a mistake was made during completion, or prematurely marked a work order done have no recourse in the system — they cannot recover without admin intervention.

Allowing the assignee to reopen a completed work order (transitioning it back to `InProgress`) eliminates this friction and matches how real-world maintenance workflows behave.

## What Changes

- New state command `CompleteToInProgressCommand` enabling the `Complete → InProgress` transition
- Registration of the new command in `StateCommandList` so it surfaces automatically in the UI as a "Reopen" button
- Side effect: clears `WorkOrder.CompletedDate` on reopen
- MCP `execute-work-order-command` tool updated to support `CompleteToInProgressCommand` and `Reopen` aliases
- Unit tests covering the new command's validity checks and state transition behavior
- Acceptance tests covering the full reopen workflow via the Blazor UI
- Updated architecture state diagram to reflect the new transition

## Capabilities

### New Capabilities

- `work-order-reopen`: Transition a completed work order back to `InProgress` status; only the assignee can execute this command; clears `CompletedDate` on execution

### Modified Capabilities

- `work-order-mcp-execute-command`: The `execute-work-order-command` MCP tool now accepts `CompleteToInProgressCommand` and `Reopen` as valid command names

## Impact

- **New file**: `src/Core/Model/StateCommands/CompleteToInProgressCommand.cs`
- **Modified**: `src/Core/Services/Impl/StateCommandList.cs` — added `CompleteToInProgressCommand` to `GetAllStateCommands()`
- **Modified**: `src/McpServer/Tools/WorkOrderTools.cs` — updated `execute-work-order-command` description and switch to support new command
- **Modified**: `src/AcceptanceTests/AcceptanceTestBase.cs` — added `ReopenExistingWorkOrder()` helper
- **New tests**: `src/UnitTests/Core/Model/StateCommands/CompleteToInProgressCommandTests.cs`
- **New tests**: `src/AcceptanceTests/WorkOrders/WorkOrderReopenTests.cs`
- **Updated arch doc**: `arch/arch-state-workorder.md`
- **No database schema changes** — `CompletedDate` is an existing nullable column; clearing it requires no migration
- **No new NuGet packages**
- **No UI changes** — the `WorkOrderManage` page auto-renders a button for every valid state command
