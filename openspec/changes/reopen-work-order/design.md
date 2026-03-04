## Context

The ChurchBulletin system uses a command-based state machine pattern for work order lifecycle management. Each state transition is a dedicated `StateCommandBase` subclass registered in `StateCommandList`. The `WorkOrderManage` Blazor page auto-renders a button for every command returned by `StateCommandList.GetValidStateCommands()` — no UI changes are needed to expose new transitions.

The existing `InProgressToCompleteCommand` sets `CompletedDate` as a side effect. The inverse transition (`CompleteToInProgressCommand`) must clear it.

Authorization is role/assignment based: each command's `UserCanExecute()` method checks whether the current user has permission. For the reopen transition, the same actor who completed the work order (the Assignee) is the appropriate actor to reopen it.

## Goals / Non-Goals

**Goals:**
- Allow an assignee to reopen a completed work order, transitioning it back to `InProgress`
- Clear `CompletedDate` when a work order is reopened
- Expose the transition in the existing Blazor UI as a "Reopen" button (automatic via `StateCommandList`)
- Expose the transition in the MCP server's `execute-work-order-command` tool
- Provide unit and acceptance test coverage matching the pattern of existing commands

**Non-Goals:**
- Allowing non-assignees to reopen work orders
- Adding a reason/notes field to the reopen action
- Database schema changes (CompletedDate is already a nullable column)
- Any UI customization beyond what the existing button-rendering infrastructure provides

## Decisions

### Decision 1: Actor is the Assignee (same as `InProgressToCompleteCommand`)

**Rationale:** The assignee is the person responsible for completing the work. It is natural for them to also be able to reopen it. The Creator role does not fulfill work orders and should not be able to change in-progress status decisions made by the assignee.

**Alternatives considered:**
- Allow Creators to reopen: Creates ambiguity about who owns the in-progress decision. Rejected.
- Allow any user with `CanFulfillWorkOrder`: Overly permissive; another employee should not reopen someone else's completed work. Rejected.

### Decision 2: Clear `CompletedDate` on reopen

**Rationale:** `CompletedDate` represents the time the work order was completed. After reopening, the work order is no longer complete, so the date is stale and misleading. Clearing it means the UI cannot display a completed timestamp for an in-progress order.

**Alternatives considered:**
- Retain `CompletedDate` as an audit marker: The field is semantically "when was this completed" — it has no meaning for an in-progress order. Clearing it preserves semantic correctness. Rejected.

### Decision 3: Use `"Reopen"` as the `TransitionVerbPresentTense` (button label)

**Rationale:** Short, descriptive, and consistent with domain language. The verb "Reopen" clearly communicates intent to end users.

**Alternatives considered:**
- `"Revoke Completion"`: Too technical. Rejected.
- `"Reset to In Progress"`: Too verbose for a button label. Rejected.

### Decision 4: No new MCP tool — update existing `execute-work-order-command`

**Rationale:** The existing `execute-work-order-command` tool accepts any valid command name. Adding `CompleteToInProgressCommand` and a friendly alias `"Reopen"` to the switch statement is sufficient and consistent with how other commands are exposed (e.g., `"Shelve"` is an alias for `InProgressToAssignedCommand`).

## Risks / Trade-offs

- **[Re-completion loop]** A work order can now cycle: InProgress → Complete → InProgress → Complete → … indefinitely. This is acceptable behavior; no loop limit is required at this stage.
- **[Audit trail]** Each `ChangeStatus()` call appends an audit event via `StateTransitionEvent`. Reopening is captured in the audit trail automatically.
- **[Data integrity]** Clearing `CompletedDate` is a soft change — it does not affect historical reports that may have already captured the date externally.
