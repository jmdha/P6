SELECT G.gdp, C.name FROM 
	((world_happiness_2022 AS H1 JOIN world_happiness_2022 AS H2 ON 
	 	H1.country = 'Denmark' AND 
	 	H1.happiness_score > H2.happiness_score)
	JOIN
	countries AS C ON C.name = H2.country)
	JOIN
	yearly_gdp AS G ON G.country_code = C.code AND G.year = '2020';