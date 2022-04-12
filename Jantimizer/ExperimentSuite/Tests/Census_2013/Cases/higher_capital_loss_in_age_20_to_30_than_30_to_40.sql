SELECT C1.id, C1.age, C2.id, C2.age FROM (
	census as C1 JOIN census as C2 ON
		C1.age >= 20 AND 
		C1.age <= 30 AND
		C2.age > 30 AND 
		C2.age <= 40 AND
		C1.capital_loss > C2.capital_loss
);