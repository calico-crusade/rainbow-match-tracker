import type {
    League, Team,
    Match, PaginatedResult,
    RefNumber
} from '~/models';

type Matches = PaginatedResult<Match>;

export const useV1LeagueApi = () => {
    const { get } = useApiHelper();
    const { pageSize } = useSettingsHelper();

    const url = (url?: string) => {
        return `/leagues${url ? `/${url}` : ''}`;
    }

    function active() {
        return get<League[]>(url('active'), undefined, !process.client);
    }

    function all(page: RefNumber, size?: RefNumber) {
        return get<PaginatedResult<League>>(url(), {
            page,
            size: size ?? pageSize
        }, !process.client);
    }

    function one(id: string) {
        return get<League>(url(id), undefined, !process.client);
    }

    function matchesAll(id: string, page: RefNumber, size?: RefNumber) {
        return get<Matches>(url(`${id}/matches`), {
            page,
            size: size ?? pageSize
        }, !process.client);
    }

    function matchesActive(id: string) {
        return get<Match[]>(url(`${id}/matches/active`), undefined, !process.client);
    }

    function matchesFinished(id: string, page: RefNumber, size?: RefNumber) {
        return get<Matches>(url(`${id}/matches/finished`), {
            page,
            size: size ?? pageSize
        }, !process.client);
    }

    function teams(id: string) {
        return get<Team[]>(url(`${id}/teams`), undefined, !process.client);
    }

    return {
        active,
        all,
        get: one,
        teams,
        matches: {
            all: matchesAll,
            active: matchesActive,
            finished: matchesFinished
        }
    }
}
