CREATE TABLE IF NOT EXISTS rainbow_team (
	id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
	
	name TEXT NOT NULL,
	code TEXT NOT NULL,
	url TEXT,
	image TEXT,
	
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	deleted_at TIMESTAMP,

	CONSTRAINT rainbow_team_unique UNIQUE(name, code)
);