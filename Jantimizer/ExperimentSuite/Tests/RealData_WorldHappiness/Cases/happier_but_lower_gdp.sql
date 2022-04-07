SELECT
       happiness_poorer.country AS poorer_name,
       happiness_poorer.happiness_score AS poorer_happy,
       gdp_less.gdp AS poorer_gdp,
       happiness_richer.country AS richer_name,
       happiness_richer.happiness_score AS richer_happy,
       gdp_more.gdp AS richer_gdp
    FROM yearly_gdp gdp_more
    JOIN yearly_gdp gdp_less
        ON gdp_less.gdp < gdp_more.gdp AND
           gdp_less.year = gdp_more.year
    JOIN countries country_poorer ON
        country_poorer.code = gdp_less.country_code
    JOIN countries country_richer ON
        country_richer.code = gdp_more.country_code
    JOIN world_happiness_2022 happiness_poorer ON
        happiness_poorer.country = country_poorer.name
    JOIN world_happiness_2022 happiness_richer ON
        happiness_richer.country = country_richer.name
    WHERE
          gdp_less.year = 2020 AND
          happiness_poorer.happiness_score > happiness_richer.happiness_score;