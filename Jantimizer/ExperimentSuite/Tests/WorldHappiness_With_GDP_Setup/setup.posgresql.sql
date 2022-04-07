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
COMMENT ON TABLE world_happiness_2022 IS 'Data from https://www.kaggle.com/datasets/mathurinache/world-happiness-report-2022';


DROP TABLE IF EXISTS countries;
CREATE TABLE countries
(
    code text not null
        primary key,
    name text
);
COMMENT ON TABLE countries IS 'Partitioned data from https://data.worldbank.org/indicator/Ny.Gdp.Mktp.Cd';

DROP TABLE IF EXISTS yearly_gdp;
CREATE TABLE yearly_gdp
(
    country_code text,
    year         integer,
    gdp          numeric,
    constraint yearly_gdp_pk
        unique (country_code, year)
);
COMMENT ON TABLE yearly_gdp IS 'Partitioned data from https://data.worldbank.org/indicator/Ny.Gdp.Mktp.Cd';

