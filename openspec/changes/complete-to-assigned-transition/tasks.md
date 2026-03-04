## 1. Complete-to-Assigned Transition

- [x] 1.1 Create `CompleteToAssignedCommand.cs` in `src/Core/Model/StateCommands/`
- [x] 1.2 Register `CompleteToAssignedCommand` in `StateCommandList.GetAllStateCommands()`
- [x] 1.3 Update `WorkOrder.CanReassign()` to return `true` for `Complete` status
- [x] 1.4 Update `StateCommandListTests.ShouldReturnAllStateCommandsInCorrectOrder` to expect 7 commands

## 2. Tests

- [x] 2.1 Add `CompleteToAssignedCommandTests.cs` unit tests (valid/invalid scenarios, state transitions, date side effects)
- [x] 2.2 Add `StateCommandHandlerForReassignTests.cs` integration test (full DB roundtrip)

## 3. Documentation

- [x] 3.1 Update `arch/arch-state-workorder.md` to include `Complete --> Assigned` transition in the Mermaid diagram
- [x] 3.2 Create `openspec/changes/complete-to-assigned-transition/` spec folder with proposal, design, tasks, and spec
