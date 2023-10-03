create schema if not exists certificate_usage;

create type coverage_type as enum ('covered', 'excluded');

create table certificate_usage_changes
(
    id                 uuid primary key not null default gen_random_uuid(),
    certificate_number varchar(256)     not null,
    carrier            varchar(50)      not null,
    policy_number      varchar(50)      not null,
    effective          timestamp        not null,
    type               coverage_type    not null,
    coverage_data      jsonb            not null,
    event_meta         jsonb            not null,
    created            timestamp        not null
);

create index idx_certificate_usage_policy_number on certificate_usage_changes (certificate_number);
create index idx_certificate_usage_carrier on certificate_usage_changes (carrier);
create index idx_certificate_usage_certificate_number on certificate_usage_changes (certificate_number);
create index idx_certificate_usage_effective on certificate_usage_changes (effective);
create index idx_certificate_usage_type on certificate_usage_changes (type);
