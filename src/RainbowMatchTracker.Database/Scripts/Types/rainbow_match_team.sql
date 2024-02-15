DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1
		FROM pg_type
		WHERE typname = 'rainbow_match_team'
	) THEN
		CREATE TYPE rainbow_match_team AS (
			id UUID,
			score INT
		);
	END IF;

END$$;