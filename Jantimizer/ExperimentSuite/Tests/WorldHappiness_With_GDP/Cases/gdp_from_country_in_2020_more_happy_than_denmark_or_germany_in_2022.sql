SELECT G1.gdp, C1.name, H2.happiness_score, G2.gdp, C2.name, H1.happiness_score FROM 
	((((world_happiness_2022 AS H1 JOIN world_happiness_2022 AS H2 ON 
	 	(H1.country = 'Denmark' OR H1.country = 'Germany') AND
	 	H1.happiness_score < H2.happiness_score)
	JOIN
	countries AS C1 ON C1.name = H2.country)
	JOIN
	countries AS C2 ON C2.name = H1.country)
	JOIN
	yearly_gdp AS G1 ON G1.country_code = C1.code AND G1.year = '2020')
	JOIN 
	yearly_gdp AS G2 ON G2.country_code = C2.code AND G2.year = '2020';