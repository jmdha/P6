CREATE SCHEMA IF NOT EXISTS census2013;
SET search_path TO census2013;

DROP TABLE IF EXISTS census;
CREATE TABLE census (
	age INT,
	workclass TEXT,
	education TEXT,
	education_num INT,
	marital_status TEXT,
	occupation TEXT,
	relationship TEXT,
	race TEXT,
	sex TEXT,
	capital_gain INT,
	capital_loss INT,
	hours_per_week INT,
	native_country TEXT
);