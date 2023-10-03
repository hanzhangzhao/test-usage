alter table carrier_rates add column product_line varchar(50);

drop index idx_carrier_rates_carrier_policy_number_option_coverage;
create index idx_carrier_rates_carrier_policy_product_line_option_coverage on carrier_rates (carrier, policy_number, product_line, "option", coverage);
