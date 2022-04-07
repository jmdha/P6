SELECT C.name FROM 
	((world_happiness_2022 AS H1 JOIN world_happiness_2022 AS H2 ON 
	 	H1.country = 'Denmark' AND 
	 	H1.happiness_score > H2.happiness_score AND H1.life_expectancy < H2.life_expectancy)
	JOIN
	countries AS C ON C.name = H2.country);