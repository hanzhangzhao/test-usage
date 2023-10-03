create type certificate_status as enum ('active', 'terminated');

create table certificate
(
    id                 uuid primary key not null default gen_random_uuid(),
    certificate_number varchar(50)          not null,
    carrier            varchar(50)          not null,
    policy_number      varchar(50)          not null,
    client_name        varchar(256)         not null,
    status             certificate_status   not null,
    certificate        jsonb                not null,
    updated            timestamp            not null default current_timestamp,
    created            timestamp            not null default current_timestamp
);

create unique index idx_certificate_by_certificate_id on certificate (certificate_number, carrier, policy_number, client_name);
create index idx_certificate_by_certificate_id_and_status on certificate (certificate_number, carrier, policy_number, client_name, status);
