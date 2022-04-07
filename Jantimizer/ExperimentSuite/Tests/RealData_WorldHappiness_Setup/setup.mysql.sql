CREATE DATABASE IF NOT EXISTS worldhappiness;
USE worldhappiness;

DROP TABLE IF EXISTS world_happiness_2022;

CREATE TABLE world_happiness_2022
(
    `rank`                   int,
    country                  varchar(100),
    happiness_score          double,
    whisker_high             double,
    whisker_low              double,
    dystopia                 double,
    gdp_per_capita           double,
    social_support           double,
    life_expectancy          double,
    freedom_of_life_choices  double,
    generocity               double,
    perception_of_corruption double
)
COMMENT='Data from https://www.kaggle.com/datasets/mathurinache/world-happiness-report-2022';


DROP TABLE IF EXISTS countries;
CREATE TABLE countries
(
    code varchar(100) not null
        primary key,
    name varchar(100)
)
COMMENT='Partitioned data from https://data.worldbank.org/indicator/Ny.Gdp.Mktp.Cd';

DROP TABLE IF EXISTS yearly_gdp;
CREATE TABLE yearly_gdp
(
    country_code varchar(4),
    year         integer,
    gdp          double,
    constraint yearly_gdp_pk
        unique (country_code, year)
)
COMMENT='Partitioned data from https://data.worldbank.org/indicator/Ny.Gdp.Mktp.Cd';

