create table carrier_rates
(
    id                 uuid primary key not null default gen_random_uuid(),
    carrier            varchar(50)      not null,
    policy_number      varchar(50)      not null,
    option             varchar(50)      not null,
    coverage           varchar(50)      null,
    effective          timestamp        not null,
    rate_data          jsonb            not null,
    event_meta         jsonb            not null,
    created            timestamp        not null
);

create index idx_carrier_rates_carrier_policy_number_option_coverage on carrier_rates (carrier, policy_number, "option", coverage);
create index idx_carrier_rates_effective on carrier_rates (effective);
