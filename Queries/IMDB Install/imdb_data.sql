copy name_basics from E'C:/Users/Public/Data/name.basics.tsv/data.tsv' with (format 'csv', delimiter E'\t', null '\N', header true, quote E'\b');

copy ratings from E'C:/Users/Public/Data/title.ratings.tsv/data.tsv' with (format 'csv', delimiter E'\t', null '\N', header true, quote E'\b');

copy principals from E'C:/Users/Public/Data/title.principals.tsv/data.tsv' with (format 'csv', delimiter E'\t', null '\N', header true, quote E'\b');

copy episodes from E'C:/Users/Public/Data/title.episode.tsv/data.tsv' with (format 'csv', delimiter E'\t', null '\N', header true, quote E'\b');

copy crews from E'C:/Users/Public/Data/title.crew.tsv/data.tsv' with (format 'csv', delimiter E'\t', null '\N', header true, quote E'\b');

copy title_basics from E'C:/Users/Public/Data/title.basics.tsv/data.tsv' with (format 'csv', delimiter E'\t', null '\N', header true, quote E'\b');

copy title_akas from E'C:/Users/Public/Data/title.akas.tsv/data.tsv' with (format 'csv', delimiter E'\t', null '\N', header true, quote E'\b');
