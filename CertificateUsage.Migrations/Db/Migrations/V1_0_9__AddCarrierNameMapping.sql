create table carrier_alias_mapping
(
    id           uuid primary key not null default gen_random_uuid(),
    alias        varchar(256)     not null,
    carrier_name varchar(256)     not null
);

create index idx__carrier_alias_mapping__name on carrier_alias_mapping (alias);

insert into carrier_alias_mapping (alias, carrier_name)
values ('ayacare', 'ayacare'),
       ('aya care', 'ayacare'),
       ('ayapayments', 'ayacare'),
       ('canadalife', 'canadalife'),
       ('cl_157145', 'canadalife'),
       ('great_west_life', 'canadalife'),
       ('gwlomega', 'canadalife'),
       ('fenchurch', 'fenchurch'),
       ('Fenchurch', 'fenchurch'),
       ('rippling', 'bob');
