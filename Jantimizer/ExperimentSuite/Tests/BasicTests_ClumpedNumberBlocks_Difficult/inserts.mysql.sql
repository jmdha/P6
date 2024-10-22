﻿DROP PROCEDURE IF EXISTS LoadTestData_A;
CREATE PROCEDURE LoadTestData_A()
BEGIN
	DECLARE v_max INT UNSIGNED DEFAULT 10;
	DECLARE v_counter INT UNSIGNED DEFAULT 0;

	START TRANSACTION;
	WHILE v_counter < v_max DO
		INSERT INTO a (v) VALUES ( sin(v_counter) * 100 );
        SET v_counter=v_counter+1;
	END WHILE;
	COMMIT;
END;

DROP PROCEDURE IF EXISTS LoadTestData_B;
CREATE PROCEDURE LoadTestData_B()
BEGIN
	DECLARE v_max INT UNSIGNED DEFAULT 50;
	DECLARE v_counter INT UNSIGNED DEFAULT 0;

	START TRANSACTION;
	WHILE v_counter < v_max DO
		INSERT INTO b (V) VALUES ( sin(v_counter) * 100 );
        SET v_counter=v_counter+1;
	END WHILE;
	COMMIT;
END;

DROP PROCEDURE IF EXISTS LoadTestData_C;
CREATE PROCEDURE LoadTestData_C()
BEGIN
	DECLARE v_max INT UNSIGNED DEFAULT 100;
	DECLARE v_counter INT UNSIGNED DEFAULT 0;

	START TRANSACTION;
	WHILE v_counter < v_max DO
		INSERT INTO c (V) VALUES ( sin(v_counter) * 100 );
        SET v_counter=v_counter+1;
	END WHILE;
	COMMIT;
END;

DROP PROCEDURE IF EXISTS LoadTestData_D;
CREATE PROCEDURE LoadTestData_D()
BEGIN
	DECLARE v_max INT UNSIGNED DEFAULT 500;
	DECLARE v_counter INT UNSIGNED DEFAULT 0;

	START TRANSACTION;
	WHILE v_counter < v_max DO
		INSERT INTO d (V) VALUES ( sin(v_counter) * 100 );
        SET v_counter=v_counter+1;
	END WHILE;
	COMMIT;
END;

CALL LoadTestData_A();
CALL LoadTestData_B();
CALL LoadTestData_C();
CALL LoadTestData_D();
