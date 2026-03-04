## Context

The ChurchBulletin work order state machine uses a command-based pattern. Each state transition is a record implementing `IStateCommand`, registered in `StateCommandList`. The existing `Complete` state had no outbound transitions — it was terminal.

The `WorkOrderManage` Blazor page auto-renders action buttons for each valid `IStateCommand` returned by `StateCommandList.GetValidStateCommands()`. The assignee picker is enabled/disabled based on `WorkOrder.CanReassign()`.

## Goals / Non-Goals

**Goals:**
- Allow work order creators to reassign a completed work order back to Assigned status
- Support changing the assignee as part of the reassignment
- Clear `CompletedDate` and set a new `AssignedDate` on transition
- Surface the action automatically through the existing UI button rendering — no new UI components
- Gate the transition by creator role only

**Non-Goals:**
- Allowing the assignee or other roles to trigger this transition
- Creating a new "Rejected" or intermediate status
- Any schema migrations (all required fields already exist)
- Changing how other transitions work

## Decisions

### Decision 1: Gate by `WorkOrder.Creator` (not `Assignee`)

**Rationale:** The issue requires creator-only authorization, matching the pattern used by `DraftToAssignedCommand`. The assignee is the fulfiller; the creator is the owner who decides whether work is truly done.

### Decision 2: Extend `WorkOrder.CanReassign()` to include `Complete` status

**Rationale:** The Blazor `WorkOrderManage.razor` disables the assignee picker using `disabled="@(!WorkOrder.CanReassign())"`. Rather than adding special-case UI logic, extending `CanReassign()` keeps the condition in the domain model where it belongs.

**Alternatives considered:**
- Adding a new property `CanReassignFromComplete` to `WorkOrder`: Creates duplication and leaks presentation logic into two separate domain methods.

### Decision 3: Register `CompleteToAssignedCommand` at the end of `StateCommandList`

**Rationale:** Ordering is tested explicitly in `StateCommandListTests`. Appending maintains the existing order and avoids disrupting test index assertions.

### Decision 4: No new UI required

**Rationale:** `WorkOrderManage.razor` iterates `ValidCommands` and renders one button per command using `TransitionVerbPresentTense` as the label. `"Reassign"` appears automatically when the command is valid.

## Risks / Trade-offs

- **[CanReassign change]** Extending `CanReassign()` to include `Complete` may affect other callers that use it for different purposes. → Mitigation: Searched the codebase; `CanReassign()` is only used in `WorkOrderManage.razor` for the assignee picker disable binding. The change is safe.
- **[No acceptance test]** No Playwright acceptance test was added in this change. → Mitigation: The unit and integration tests cover the command lifecycle. Acceptance tests can be added in a follow-on if needed.
