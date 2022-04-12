CREATE DATABASE IF NOT EXISTS covidfunding;
USE covidfunding;

SET GLOBAL max_allowed_packet=1073741824;

DROP TABLE IF EXISTS flows;
CREATE TABLE flows (
	reporting_org_id varchar(1000),
	reporting_org_name varchar(1000),
	reporting_org_type INT,
	provider_org_id varchar(1000),
	provider_org_name varchar(1000),
	provider_org_type INT,
	reciever_org_id varchar(1000),
	reciever_org_name varchar(1000),
	reciever_org_type INT,
	humanitarian BOOL,
	strict BOOL,
	transaction_direction varchar(1000),
	total_money BIGINT
) COMMENT='Data from https://data.world/hdx/e24de323-ed64-4c33-8eda-dace55d107b9';

DROP TABLE IF EXISTS transactions;
CREATE TABLE transactions (
	´month´ DATE,
	reporting_org_id varchar(1000),
	reporting_org_name varchar(1000),
	reporting_org_type INT,
	sector varchar(1000),
	recipent_country varchar(1000),
	humanitarian BOOL,
	strict BOOL,
	transaction_type varchar(1000),
	activity_id varchar(1000),
	net_money BIGINT,
	total_money BIGINT
) COMMENT='Data from https://data.world/hdx/e24de323-ed64-4c33-8eda-dace55d107b9';