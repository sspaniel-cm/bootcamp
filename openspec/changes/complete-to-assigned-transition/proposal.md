## Why

Work orders can become complete but still require further action — for example, when quality issues are discovered, follow-up tasks arise, or the initial completion was in error. Currently, `Complete` is a terminal state with no outbound transitions. This change adds a `Reassign` transition so the creator can move a completed work order back to `Assigned`, optionally to a new assignee, and restart the work cycle.

## What Changes

- New state transition: `Complete` → `Assigned` via `CompleteToAssignedCommand` / verb `"Reassign"`
- Only the work order's **creator** may execute this transition
- Side effects: clears `CompletedDate`, sets `AssignedDate` to current date/time, allows picking a new (or same) assignee
- `WorkOrder.CanReassign()` extended to return `true` when status is `Complete`, enabling the assignee picker in the UI
- No schema changes — all fields (`CompletedDate`, `AssignedDate`, `Assignee`) already exist
- `CompleteToAssigned` command registered in `StateCommandList`; the existing UI button-rendering logic auto-discovers it

## Capabilities

### New Capabilities

- `complete-to-assigned-transition`: Work order creator can reassign a completed work order back to assigned status

### Modified Capabilities

- `work-order-can-reassign`: `WorkOrder.CanReassign()` now returns `true` for both `Draft` and `Complete` status, enabling the assignee picker when reassigning a completed work order
