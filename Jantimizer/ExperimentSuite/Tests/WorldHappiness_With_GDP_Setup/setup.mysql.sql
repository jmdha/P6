CREATE DATABASE IF NOT EXISTS worldhappiness;
USE worldhappiness;

DROP TABLE IF EXISTS world_happiness_2022;

CREATE TABLE world_happiness_2022
(
    `rank`                   BIGINT,
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
