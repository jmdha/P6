SELECT 
		C1.id, C1.education, C1.capital_gain, C1.hours_per_week, C2.id, C2.education, C2.capital_gain, C2.hours_per_week
	FROM (
		census as C1 JOIN census as C2 ON 
			C1.education = '9th' AND 
			C1.capital_gain > C2.capital_gain AND
			C1.hours_per_week < C2.hours_per_week
	);