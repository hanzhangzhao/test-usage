const dig = (pth, obj) => pth.reduce((xs, x) => (xs && xs[x]) ? xs[x] : null, obj);

const snapshotEvt = (memberId, event) => ({
  memberId,
  reason: event.eventType,
  correlationId: event.eventId,
});

const emitSnapshotRequest = (memberId, effectiveDate, event, emitFn) => {
  const date = new Date(effectiveDate);

  emitFn(
    `snapshot_queue-${date.toISOString().substring(0,10)}`,
    'SnapShotRequested',
    snapshotEvt(memberId, event)
  );
};

const forEach = (fun, arr) => arr.forEach(fun);

fromStream('members')
  .when({
    MemberEnrollmentConfirmationScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'benefits_start_date'], event),
      event,
      emit
    ),
    MemberReinstatementConfirmationScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'benefits_start_date'], event),
      event,
      emit
    ),
    MemberBenefitStartScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'benefits_start_date'], event),
      event,
      emit
    ),
    MemberBenefitEndScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'benefits_ended_date'], event),
      event,
      emit
    ),
    MemberBenefitClassTransferScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'to', 'benefits_start_date'], event),
      event,
      emit
    ),
    MemberCobUpdateScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'coverage_coordination', 'effective_date'], event),
      event,
      emit
    ),
    SpouseCobUpdateScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'spouse', 'coverage_coordination', 'effective_date'], event),
      event,
      emit
    ),
    DependentCobUpdateScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'dependent', 'coverage_coordination', 'effective_date'], event),
      event,
      emit
    ),
    MemberSpouseAddScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'effective_start_date'], event),
      event,
      emit
    ),
    EnrolledSpouseRemoveScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'effective_end_date'], event),
      event,
      emit
    ),
    MemberDependentAddScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'effective_start_date'], event),
      event,
      emit
    ),
    EnrolledDependentRemoveScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'effective_end_date'], event),
      event,
      emit
    ),
    EnrolledMemberTaxProvinceUpdateScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'employment', 'income_effective_date'], event),
      event,
      emit
    ),
    EnrolledMemberIncomeUpdateScheduled: (_s, event) => emitSnapshotRequest(
      dig(['data', 'member_id'], event),
      dig(['data', 'employment', 'income_effective_date'], event),
      event,
      emit
    ),
  });
