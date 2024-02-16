import type { League, Team, Match, ExtendedMatch, PaginatedResult } from '~/models';

type RefNumber = Ref<number> | number;
type RefString = Ref<string> | string;

export const useRmtApi = () => {
    const { get } = useApiHelper();

    const DEFAULT_SIZE = 20;
    const url = (section: string, url?: string, version: string = 'v1') => {
        return `/${section}${url ? `/${url}` : ''}`;
    }

    return {
        v1: {
            leagues: {
                active: () => get<League[]>(url('leagues', 'active'), undefined, !process.client),
                all: (page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<League>>(url('leagues'), { page, size }, !process.client),
                get: (id: string) => get<League>(url('leagues', id), undefined, !process.client),
                matches: {
                    all: (id: string, page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('leagues', `${id}/matches`), { page, size }, !process.client),
                    active: (id: string) => get<Match[]>(url('leagues', `${id}/matches/active`), undefined, !process.client),
                    finished: (id: string, page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('leagues', `${id}/matches/finished`), { page, size }, !process.client),
                }
            },
            matches: {
                all: (page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('matches'), { page, size }, !process.client),
                get: (id: string) => get<ExtendedMatch>(url('matches', id), undefined, !process.client),
                range: (start: Date, end?: Date) => get<Match[]>(url('matches', 'range'), { start, end }, !process.client),
            },
            teams: {
                active: () => get<Team[]>(url('teams', 'active'), undefined, !process.client),
                all: (page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Team>>(url('teams'), { page, size }, !process.client),
                get: (id: string) => get<Team>(url('teams', id), undefined, !process.client),
                matches: (id: string, page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('teams', `${id}/matches`), { page, size }, !process.client),
            }
        }
    }
};
