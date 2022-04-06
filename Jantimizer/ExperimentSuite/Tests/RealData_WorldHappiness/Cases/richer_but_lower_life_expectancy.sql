SELECT c1.country, c2.country
    FROM world_happiness_2022 c1, world_happiness_2022 c2
    WHERE
          c1.gdp_per_capita > c2.gdp_per_capita AND
          c1.life_expectancy < c2.life_expectancy