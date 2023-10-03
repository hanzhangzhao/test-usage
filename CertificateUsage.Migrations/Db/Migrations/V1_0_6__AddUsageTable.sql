create table certificate_usage
(
    id                 uuid primary key not null default gen_random_uuid(),
    certificate_number varchar(50)          not null,
    carrier_name       varchar(256)         not null,
    client_name        varchar(256)         not null,
    policy_number      varchar(50)		    not null,
    scb_policy_number  varchar(50)			not null,
    benefit_start_date timestamp            not null,
    benefit_end_date   timestamp,
    division	       varchar(256),
    product_line	   varchar(256)		    not null,
    product_line_group varchar(256),
    coverage		   varchar(32),
    option			   varchar(24)		    not null,
    rate_per		   decimal,
    volume_amount      decimal				not null,
    volume_unit        varchar(24)		    not null,
    carrier_rate       decimal              not null,
    tax_rate           decimal,
    tax_province       varchar(32)			not null,
    month              int                  not null,
    year               int                  not null,
    created            timestamp            null default current_timestamp
);

create index idx_certificate_usage__year__month on certificate_usage (year, month);
create unique index idx_certificate_usage_by_certificate_id on certificate_usage (certificate_number, carrier_name, policy_number, client_name, product_line, coverage, option, month, year);
