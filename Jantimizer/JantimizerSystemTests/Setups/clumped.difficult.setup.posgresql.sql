﻿-- Setup schema
CREATE SCHEMA IF NOT EXISTS systemtests_clumped_difficult;
SET search_path TO systemtests_clumped_difficult;

TRUNCATE TABLE A RESTART IDENTITY;
TRUNCATE TABLE B RESTART IDENTITY;
TRUNCATE TABLE C RESTART IDENTITY;
TRUNCATE TABLE D RESTART IDENTITY;

DROP TABLE IF EXISTS a;
DROP TABLE IF EXISTS b;
DROP TABLE IF EXISTS c;
DROP TABLE IF EXISTS d;

CREATE TABLE a (
	S SERIAL PRIMARY KEY,
    V BIGINT NOT NULL
);
CREATE TABLE b (
	S SERIAL PRIMARY KEY,
    V BIGINT NOT NULL
);
CREATE TABLE c (
	S SERIAL PRIMARY KEY,
    V BIGINT NOT NULL
);
CREATE TABLE d (
	S SERIAL PRIMARY KEY,
    V BIGINT NOT NULL
);

-- Analyse tables
ANALYZE;

-- Insert data
DROP PROCEDURE IF EXISTS LoadTestData_A;
CREATE PROCEDURE LoadTestData_A()
language plpgsql    
AS $$
DECLARE
	v_max INT := 10;
	v_counter INT := 0;
BEGIN
	WHILE v_counter < v_max LOOP
		INSERT INTO a (v) VALUES ( sin(v_counter) * 100 );
        v_counter := v_counter+1;
	END LOOP;
	RETURN;
END;$$;

DROP PROCEDURE IF EXISTS LoadTestData_B;
CREATE PROCEDURE LoadTestData_B()
language plpgsql    
AS $$
DECLARE
	v_max INT := 50;
	v_counter INT := 0;
BEGIN
	WHILE v_counter < v_max LOOP
		INSERT INTO b (v) VALUES ( sin(v_counter) * 100 );
        v_counter := v_counter+1;
	END LOOP;
	RETURN;
END;$$;

DROP PROCEDURE IF EXISTS LoadTestData_C;
CREATE PROCEDURE LoadTestData_C()
language plpgsql    
AS $$
DECLARE
	v_max INT := 100;
	v_counter INT := 0;
BEGIN
	WHILE v_counter < v_max LOOP
		INSERT INTO c (v) VALUES ( sin(v_counter) * 100 );
        v_counter := v_counter+1;
	END LOOP;
	RETURN;
END;$$;

DROP PROCEDURE IF EXISTS LoadTestData_D;
CREATE PROCEDURE LoadTestData_D()
language plpgsql    
AS $$
DECLARE
	v_max INT := 500;
	v_counter INT := 0;
BEGIN
	WHILE v_counter < v_max LOOP
		INSERT INTO d (v) VALUES ( sin(v_counter) * 100 );
        v_counter := v_counter+1;
	END LOOP;
	RETURN;
END;$$;

CALL LoadTestData_A();
CALL LoadTestData_B();
CALL LoadTestData_C();
CALL LoadTestData_D();
