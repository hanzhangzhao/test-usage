Feature: Member Lifecycle

@TestId2
Scenario: 1 Member Enrolls in Health and Dental
    Given a certificate with certificate number 15
        And has policy number 12345
        And is under carrier equitable_life
        And is sponsored by client "Acme"
        And has division 001
        And the certificate has a family health option A at a rate of 99.0 CAD and a volume of 1 quantity
        And the certificate has a family dental option A at a rate of 100.0 CAD and a volume of 1 quantity

    When the member is enrolled
        And the books are closed for this month

    Then it is metered for this month
        And the family health option A rate is 99.0 CAD
        And the family health option A volume is 1 quantity
        And the family dental option A rate is 100.0 CAD
        And the family dental option A volume is 1 quantity

@TestId3
Scenario: 2 Member Benefit Terminate
    Given a certificate with certificate number 15
        And has policy number 12345
        And is under carrier equitable_life
        And is sponsored by client "Acme"
        And has division 001
        And the certificate has a family health option A at a rate of 99.0 CAD and a volume of 1 quantity
        And the certificate has a family dental option A at a rate of 100.0 CAD and a volume of 1 quantity

    When it is terminated
        And the books are closed for this month

    Then it is not metered for this month

@TestId4
Scenario: Member changes benefit period
    Then member changes benefit period

@TestId5
Scenario: Member hits reduction calc age
    Then member hits reduction calc age

@TestId6
Scenario: Member hits termination age
    Then member hits termination age

@TestId7
Scenario: Member has address change
    Then member has address change

@TestId8
Scenario: Member has salary increase
    Then member has salary increase

@TestId9
Scenario: Member has salary decrease
    Then member has salary decrease

@TestId10
Scenario: Member is approved for coverage above NEM
    Then member is approved for coverage above NEM
