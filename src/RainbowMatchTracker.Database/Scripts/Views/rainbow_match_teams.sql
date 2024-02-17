CREATE OR REPLACE VIEW rainbow_match_teams AS
WITH extracted AS (
	SELECT
		DISTINCT
		(unnest(teams)).id as team_id,
		(unnest(teams)).score as team_score,
		league_id,
		id,
		start_time,
		hash,
		best_of,
		last_batch_time,
		status
	FROM rainbow_match
	WHERE deleted_at IS NULL
)
SELECT
	e.*,
	t.name as team_name,
	t.code as team_code,
	t.image as team_image,
	t.url as team_url
FROM extracted e
JOIN rainbow_team t ON e.team_id = t.id
WHERE t.deleted_at IS NULL
ORDER BY e.start_time DESC;