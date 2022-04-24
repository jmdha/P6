CREATE SCHEMA IF NOT EXISTS census2013;
SET search_path TO census2013;

DROP TABLE IF EXISTS census;
CREATE TABLE census (
	id SERIAL PRIMARY KEY,
	age BIGINT,
	workclass TEXT,
	education TEXT,
	education_num BIGINT,
	marital_status TEXT,
	occupation TEXT,
	relationship TEXT,
	race TEXT,
	sex TEXT,
	capital_gain BIGINT,
	capital_loss BIGINT,
	hours_per_week BIGINT,
	native_country TEXT
);
COMMENT ON TABLE census IS 'Data from https://github.com/sfu-db/AreCELearnedYet#dataset';