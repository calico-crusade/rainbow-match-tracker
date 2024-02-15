CREATE TABLE IF NOT EXISTS rainbow_league (
	id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
	
	name TEXT NOT NULL,
	display TEXT NOT NULL,
	url TEXT,
	image TEXT,
	
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	deleted_at TIMESTAMP,

	CONSTRAINT rainbow_league_unique UNIQUE(name, display)
);