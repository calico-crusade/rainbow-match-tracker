CREATE TABLE IF NOT EXISTS rainbow_match (
	id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
	
	hash TEXT NOT NULL UNIQUE,
	teams rainbow_match_team[] NOT NULL DEFAULT '{}',
	league_id UUID NOT NULL REFERENCES rainbow_league(id),
	status INT NOT NULL,
	start_time TIMESTAMP NOT NULL,
	best_of INT NOT NULL,
	last_batch_time TIMESTAMP NOT NULL,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	deleted_at TIMESTAMP
);