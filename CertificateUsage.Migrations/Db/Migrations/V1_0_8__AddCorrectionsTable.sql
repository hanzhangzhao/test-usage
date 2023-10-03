create type certificate_usage_type as enum ('charge', 'reversal', 'correction');

drop index idx_certificate_usage__year__month;
drop index idx_certificate_usage_by_certificate_id;
alter table certificate_usage rename to certificate_usage_0;

create table certificate_usage
(
    id                  uuid primary key       not null default gen_random_uuid(),
    causation_id        uuid,
    correlated_usage_id uuid,
    usage_type          certificate_usage_type not null,
    carrier_name        varchar(256)           not null,
    billing_date        timestamp              not null,
    date_incurred       timestamp              not null,
    certificate_number  varchar(50)            not null,
    client_name         varchar(256)           not null,
    policy_number       varchar(50)            not null,
    scb_policy_number   varchar(50)            not null,
    benefit_start_date  timestamp              not null,
    benefit_end_date    timestamp,
    division            varchar(256),
    product_line        varchar(256)           not null,
    product_line_group  varchar(256),
    coverage            varchar(32),
    option              varchar(24)            not null,
    rate_per            decimal,
    volume_amount       decimal                not null,
    volume_unit         varchar(24)            not null,
    carrier_rate        decimal                not null,
    tax_rate            decimal,
    tax_province        varchar(32)            not null,
    created             timestamp null default current_timestamp
);

create index idx_certificate_billing_date on certificate_usage (billing_date);

insert into certificate_usage
select id,
       null     as causation_id,
       null     as correlated_usage_id,
       'charge' as usage_type,
       carrier_name,
       make_timestamp(year, month, 1, 0, 0, 0),
       make_timestamp(year, month, 1, 0, 0, 0),
       certificate_number,
       client_name,
       policy_number,
       scb_policy_number,
       benefit_start_date,
       benefit_end_date,
       division,
       product_line,
       product_line_group,
       coverage,
option,
    rate_per,
    volume_amount,
    volume_unit,
    carrier_rate,
    tax_rate,
    tax_province,
    created
from certificate_usage_0;

drop table certificate_usage_0;
