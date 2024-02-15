DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1
		FROM pg_type
		WHERE typname = 'rainbow_league_data'
	) THEN
		CREATE TYPE rainbow_league_data AS (
			name TEXT,
			display TEXT,
			url TEXT,
			image TEXT
		);
	END IF;

END$$;