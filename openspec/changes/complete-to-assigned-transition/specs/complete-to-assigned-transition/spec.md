## ADDED Requirements

### Requirement: Creator can reassign a completed work order
The system SHALL allow the creator of a work order to transition it from `Complete` status back to `Assigned` status by executing the `CompleteToAssignedCommand` with the verb `"Reassign"`.

#### Scenario: Creator triggers Reassign on a completed work order
- **GIVEN** a work order in `Complete` status
- **AND** the current user is the work order's creator
- **WHEN** the `CompleteToAssignedCommand` is executed
- **THEN** the work order status transitions to `Assigned`
- **AND** `CompletedDate` is cleared (null)
- **AND** `AssignedDate` is set to the current date/time

#### Scenario: Non-creator cannot reassign a completed work order
- **GIVEN** a work order in `Complete` status
- **AND** the current user is NOT the work order's creator
- **WHEN** `CompleteToAssignedCommand.IsValid()` is evaluated
- **THEN** it returns `false`

#### Scenario: Creator cannot reassign from a non-Complete status
- **GIVEN** a work order NOT in `Complete` status
- **AND** the current user is the work order's creator
- **WHEN** `CompleteToAssignedCommand.IsValid()` is evaluated
- **THEN** it returns `false`

## MODIFIED Requirements

### Requirement: WorkOrder.CanReassign() returns true for Complete status
The `WorkOrder.CanReassign()` method SHALL return `true` when the work order status is `Draft` OR `Complete`, enabling the assignee picker in the UI during reassignment from either state.

#### Scenario: CanReassign returns true for Complete status
- **GIVEN** a work order in `Complete` status
- **WHEN** `WorkOrder.CanReassign()` is called
- **THEN** it returns `true`

#### Scenario: CanReassign still returns true for Draft status
- **GIVEN** a work order in `Draft` status
- **WHEN** `WorkOrder.CanReassign()` is called
- **THEN** it returns `true`

#### Scenario: CanReassign returns false for non-Draft, non-Complete statuses
- **GIVEN** a work order in `Assigned`, `InProgress`, or `Cancelled` status
- **WHEN** `WorkOrder.CanReassign()` is called
- **THEN** it returns `false`

### Requirement: Reassign action appears in UI for eligible creators
The `WorkOrderManage` page SHALL display a `"Reassign"` action button for work orders in `Complete` status when the current user is the creator, via the existing `ValidCommands` rendering mechanism.

#### Scenario: Reassign button visible to creator on complete work order
- **GIVEN** a work order in `Complete` status
- **AND** the current user is the creator
- **WHEN** the WorkOrderManage page loads
- **THEN** a button with label `"Reassign"` is rendered

#### Scenario: Reassign button not visible to non-creator
- **GIVEN** a work order in `Complete` status
- **AND** the current user is NOT the creator
- **WHEN** the WorkOrderManage page loads
- **THEN** no `"Reassign"` button is rendered
