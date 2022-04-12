CREATE SCHEMA IF NOT EXISTS covidfunding;
SET search_path TO covidfunding;

DROP TABLE IF EXISTS flows;
CREATE TABLE flows (
	reporting_org_id TEXT,
	reporting_org_name TEXT,
	reporting_org_type INT,
	provider_org_id TEXT,
	provider_org_name TEXT,
	provider_org_type INT,
	reciever_org_id TEXT,
	reciever_org_name TEXT,
	reciever_org_type INT,
	humanitarian BOOL,
	strict BOOL,
	transaction_direction TEXT,
	total_money BIGINT
);
COMMENT ON TABLE flows IS 'Data from https://data.world/hdx/e24de323-ed64-4c33-8eda-dace55d107b9';

DROP TABLE IF EXISTS transactions;
CREATE TABLE transactions (
	´month´ DATE,
	reporting_org_id TEXT,
	reporting_org_name TEXT,
	reporting_org_type INT,
	sector TEXT,
	recipent_country TEXT,
	humanitarian BOOL,
	strict BOOL,
	transaction_type TEXT,
	activity_id TEXT,
	net_money BIGINT,
	total_money BIGINT
);
COMMENT ON TABLE transactions IS 'Data from https://data.world/hdx/e24de323-ed64-4c33-8eda-dace55d107b9';