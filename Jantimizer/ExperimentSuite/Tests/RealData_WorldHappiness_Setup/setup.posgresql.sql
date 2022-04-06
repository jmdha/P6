CREATE SCHEMA IF NOT EXISTS worldhappiness;
SET search_path TO worldhappiness;

DROP TABLE IF EXISTS world_happiness_2022;

CREATE TABLE world_happiness_2022
(
    rank                     integer,
    country                  text,
    happiness_score          numeric,
    whisker_high             numeric,
    whisker_low              numeric,
    dystopia                 numeric,
    gdp_per_capita           numeric,
    social_support           numeric,
    life_expectancy          numeric,
    freedom_of_life_choices  numeric,
    generocity               numeric,
    perception_of_corruption numeric
);
