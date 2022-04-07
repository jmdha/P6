DROP PROCEDURE IF EXISTS LoadTestData_A;
CREATE PROCEDURE LoadTestData_A()
language plpgsql    
AS $$
DECLARE
	v_max INT := 10;
	v_counter INT := 0;
BEGIN
	WHILE v_counter < v_max LOOP
		INSERT INTO a (v) VALUES ( 10 );
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
		INSERT INTO b (v) VALUES ( 100 );
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
		INSERT INTO c (v) VALUES ( 1000 );
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
		INSERT INTO d (v) VALUES ( 10000 );
        v_counter := v_counter+1;
	END LOOP;
	RETURN;
END;$$;

CALL LoadTestData_A();
CALL LoadTestData_B();
CALL LoadTestData_C();
CALL LoadTestData_D();
