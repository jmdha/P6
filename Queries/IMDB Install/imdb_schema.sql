CREATE TABLE IF NOT EXISTS name_basics
(
	    nconst             text,
	    primary_name       text,
	    birth_year         int,
	    death_year         int,
	    primary_profession text,
	    known_for_titles   text
);

CREATE TABLE IF NOT EXISTS ratings
(
	    tconst text,
	    average_rating numeric,
	    num_votes int
);

CREATE TABLE IF NOT EXISTS principals
(
	    tconst text,
	    ordering int,
	    nconst text,
	    category text,
	    job text,
	    characters text
);

CREATE TABLE IF NOT EXISTS episodes
(
	    tconst text,
	    parent_tconst text,
	    season_number int,
	    episode_number int
);

CREATE TABLE IF NOT EXISTS crews
(
	    tconst text,
	    directors text,
	    writers text
);

CREATE TABLE IF NOT EXISTS title_basics
(
	    tconst text,
	    title_type text,
	    primary_title text,
	    original_title text,
	    isAdult boolean,
	    startYear int,
	    endYear int,
	    runTimeMinutes int,
	    genres text
);

CREATE TABLE IF NOT EXISTS title_akas
(
	    id text,
	    ordering int,
	    title text,
	    region text,
	    language text,
	    types text,
	    attributes text,
	    is_original_title boolean
);

TRUNCATE name_basics;
TRUNCATE ratings;
TRUNCATE principals;
TRUNCATE episodes;
TRUNCATE crews;
TRUNCATE title_basics;
TRUNCATE title_akas;
