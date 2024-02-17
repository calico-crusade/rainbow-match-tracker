export const useRmtApi = () => {
    const leagues = useV1LeagueApi();
    const matches = useV1MatchApi();
    const teams = useV1TeamApi();

    return {
        v1: {
            leagues,
            matches,
            teams
        }
    }
};
