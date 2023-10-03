create type retro_cert_update_type as enum ('enrollment', 'termination', 'update');

create table retroactive_certificate_update
(
    id                 uuid primary key       not null default gen_random_uuid(),
    type               retro_cert_update_type not null,
    certificate_number varchar(50)            not null,
    carrier_name       varchar(50)            not null,
    client_name        varchar(256)           not null,
    policy_number      varchar(50)            not null,
    product_line       varchar(256)           not null,
    coverage           varchar(32),
    option             varchar(24)            not null,
    backdate           timestamp              not null,
    update_date        timestamp              not null,
    created_at         timestamp              not null default current_timestamp
);

create index idx__retroactive_certificate_update__backdate on retroactive_certificate_update (backdate);
create index idx__retroactive_certificate_update__update_date on retroactive_certificate_update (update_date);
