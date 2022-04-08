SELECT 
		C1.id, C1.workclass, C1.capital_gain, C2.id, C2.workclass, C2.capital_gain
	FROM (
		census as C1 JOIN census as C2 ON 
			C1.workclass = 'Private' AND 
			C2.workclass = 'State-gov' AND
			C2.capital_gain > C1.capital_gain
	);