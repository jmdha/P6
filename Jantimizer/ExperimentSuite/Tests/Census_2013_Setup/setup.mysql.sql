CREATE DATABASE IF NOT EXISTS census2013;
USE census2013;

SET GLOBAL max_allowed_packet=1073741824;

DROP TABLE IF EXISTS census;
CREATE TABLE census (
	age INT,
	workclass varchar(100),
	education varchar(100),
	education_num INT,
	marital_status varchar(100),
	occupation varchar(100),
	relationship varchar(100),
	race varchar(100),
	sex varchar(100),
	capital_gain INT,
	capital_loss INT,
	hours_per_week INT,
	native_country varchar(100)
);