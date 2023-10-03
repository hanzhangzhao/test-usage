module Usage.CertificateUsage

open System
open Models
open Dto.Events.Dto
open Usage.Dto.Events.MemberEnrolled
open Usage.Models

let private mapPlanSelections (selections: PlanSelectionDto list) =
    selections
    |> List.map (fun p ->
        { ProductLine = p.LineGroup
          Coverage = None
          Option = p.Selection |> Option.defaultValue "X"
          RatePer = 1.0M
          Volume= { Amount = 1.0M; Unit = "CAD" } })
    
let private fromEnrollment (eventDto: MemberEnrollmentDto) =
    { CertificateNumber = eventDto.CertificateNumber
      State = Covered
      Carrier = eventDto.Carrier
      ClientName = "empty for now"
      PolicyNumber = eventDto.PolicyNumber
      Effective = eventDto.BenefitsStartDate
      ProductSelections = mapPlanSelections eventDto.PlanSelections
      History = [ ] }
    
let private historyFromCert (cert: Certificate) : CertificateHistory =
    { State = cert.State
      Effective = cert.Effective
      PolicyNumber = cert.PolicyNumber
      ProductSelections = cert.ProductSelections }
    
let private appendHistory (cert: Certificate) =
    { cert with History = (historyFromCert cert) :: cert.History }

let nextCertificateState (certificate: Certificate option) (memberEvent: MemberEventDto) =
    match certificate with
    | None ->
        match memberEvent with
        | MemberEnrollmentDto eventDto -> eventDto |> fromEnrollment |> Some
        | _ -> None
    | Some cert ->
        match memberEvent with
        | MemberEnrollmentDto memberEnrollmentDto ->
            // Status doesn't change, maybe some dates and selections?
            { cert with
                State = Covered
                Carrier = memberEnrollmentDto.Carrier
                PolicyNumber = memberEnrollmentDto.PolicyNumber
                Effective = memberEnrollmentDto.BenefitsStartDate
                ProductSelections = mapPlanSelections memberEnrollmentDto.PlanSelections
                History = (historyFromCert cert) :: cert.History }
            |> Some
        | MemberDependentAddedDto memberDependentAddedDto ->
            // coverage (single to family) could change
            // TODO: Add plans selections and coverage to this event
            cert |> appendHistory |> Some
        | MemberSpouseAddedDto memberSpouseAddedDto ->
            // Coverage (single to family/couple) could change
            // TODO: Add plans selection and coverage to this event
            cert |> appendHistory |> Some
        | MemberEmploymentUpdatedDto memberEmploymentUpdatedDto ->
            // Volume could have changed
            // TODO: Add plans selections to this event
            cert |> appendHistory |> Some
        | MemberIncomeUpdatedDto memberIncomeUpdatedDto ->
            // Volume could have changed
            // TODO: Add plans selections to this event
            cert |> appendHistory |> Some
        | MemberTerminatedDto memberTerminatedDto ->
            // Member is excluded from coverage, at the effective date
            { cert with
                State = Excluded
                Effective = memberTerminatedDto.BenefitsEndedDate |> Option.defaultValue DateTime.Now
                History = (historyFromCert cert) :: cert.History }
            |> Some
        | MemberCancelledDto memberCancelledDto ->
            // Member is excluded from coverage, at the effective date
            // TODO: How does cancel and terminate differ for billing?
            { cert with
                State = Excluded
                Effective = memberCancelledDto.BenefitsEndedDate |> Option.defaultValue DateTime.Now
                History = (historyFromCert cert) :: cert.History }
            |> Some
        | DependentTerminatedDto dependentTerminatedDto ->
            // Coverage (family/couple to single) could change
            // TODO: Add plans selection and coverage to this event
            cert |> appendHistory |> Some
        | SpouseTerminatedDto spouseTerminatedDto ->
            // Coverage (family/couple to single) could change
            // TODO: Add plans selection and coverage to this event
            cert |> appendHistory |> Some
        | MemberCobUpdatedDto memberCobUpdatedDto ->
            // Member could have waived some benefits?
            // TODO: Add plans selections and coverage to this event
            cert |> appendHistory |> Some
        | DependentCobUpdatedDto dependentCobUpdatedDto ->
            // Not sure if this effects coverage
            cert |> appendHistory |> Some
        | SpouseCobUpdatedDto spouseCobUpdatedDto ->
            // Not sure if this effects coverage
            cert |> appendHistory |> Some
        | MemberProfileUpdatedDto _ -> cert |> Some
        
        | MemberTaxProvinceUpdatedDto _ ->
            // Not sure if we care about this
            cert |> appendHistory |> Some
        | SpouseProfileUpdatedDto _ -> cert |> Some
        | DependentProfileUpdatedDto _ -> cert |> Some
        | SpouseCohabUpdatedDto spouseCohabUpdatedDto ->
            // Not sure if we care about this
            cert |> appendHistory |> Some
        | DependentDisabilityUpdatedDto _ -> cert |> Some
        | DependentPostSecondaryEducationUpdatedDto _ -> cert |> Some
        | MemberBenefitClassTransferredDto memberBenefitClassTransferredDto ->
            // TODO: Add plans and selections
            // Does the same cert get transferred? What if it's a new carrier?
            { cert with
                PolicyNumber = memberBenefitClassTransferredDto.To.Certificate.PolicyNumber.Number
                Carrier = memberBenefitClassTransferredDto.To.ProductConfiguration.Carrier
                Effective = memberBenefitClassTransferredDto.To.BenefitsStartDate |> Option.defaultValue DateTime.Now
                History = (historyFromCert cert) :: cert.History }
            |> Some
        | MemberReinstatementConfirmedDto memberEnrollmentDto ->
            { cert with
                Carrier = memberEnrollmentDto.Carrier
                State = Covered
                ProductSelections = memberEnrollmentDto.PlanSelections |> mapPlanSelections
                Effective = memberEnrollmentDto.BenefitsStartDate
                History = (historyFromCert cert) :: cert.History }
            |> Some