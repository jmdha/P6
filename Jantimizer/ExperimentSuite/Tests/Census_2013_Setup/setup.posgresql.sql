CREATE SCHEMA IF NOT EXISTS census2013;
SET search_path TO census2013;

DROP TABLE IF EXISTS census;
CREATE TABLE census (
	id SERIAL PRIMARY KEY,
	age BIGINT,
	workclass varchar(100),
	education varchar(100),
	education_num BIGINT,
	marital_status varchar(100),
	occupation varchar(100),
	relationship varchar(100),
	race varchar(100),
	sex varchar(100),
	capital_gain BIGINT,
	capital_loss BIGINT,
	hours_per_week BIGINT,
	native_country varchar(100)
);
COMMENT ON TABLE census IS 'Data from https://github.com/sfu-db/AreCELearnedYet#dataset';