module CertificateUsage.Tests.Behaviour.Feature

open TickSpec.Xunit
open global.Xunit

let source =
    AssemblyStepDefinitionsSource(System.Reflection.Assembly.GetExecutingAssembly())

let scenarios resourceName =
    source.ScenariosFromEmbeddedResource resourceName |> MemberData.ofScenarios

[<Theory; MemberData("scenarios", "CertificateUsage.Tests.Behaviour.MemberLifecycle.MemberLifecycle.feature")>]
let MemberLifecycle (scenario: XunitSerializableScenario) = source.RunScenario(scenario)

[<Theory; MemberData("scenarios", "CertificateUsage.Tests.Behaviour.SpouseAndDependents.SpouseAndDependents.feature")>]
let SpouseAndDependents (scenario: XunitSerializableScenario) = source.RunScenario(scenario)

[<Theory; MemberData("scenarios", "CertificateUsage.Tests.Behaviour.CoordinationOfBenefits.CoordinationOfBenefits.feature")>]
let CoordinationOfBenefits (scenario: XunitSerializableScenario) = source.RunScenario(scenario)

[<Theory; MemberData("scenarios", "CertificateUsage.Tests.Behaviour.RateChanges.RateChanges.feature")>]
let RateChanges (scenario: XunitSerializableScenario) = source.RunScenario(scenario)
