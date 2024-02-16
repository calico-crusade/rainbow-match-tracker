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
                active: () => get<League[]>(url('leagues', 'active')),
                all: (page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<League>>(url('leagues'), { page, size }),
                get: (id: string) => get<League>(url('leagues', id)),
                matches: {
                    all: (id: string, page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('leagues', `${id}/matches`), { page, size }),
                    active: (id: string) => get<Match[]>(url('leagues', `${id}/matches/active`)),
                    finished: (id: string, page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('leagues', `${id}/matches/finished`), { page, size }),
                }
            },
            matches: {
                all: (page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('matches'), { page, size }),
                get: (id: string) => get<ExtendedMatch>(url('matches', id)),
                range: (start: Date, end?: Date) => get<Match[]>(url('matches', 'range'), { start, end }),
            },
            teams: {
                active: () => get<Team[]>(url('teams', 'active')),
                all: (page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Team>>(url('teams'), { page, size }),
                get: (id: string) => get<Team>(url('teams', id)),
                matches: (id: string, page: RefNumber, size: RefNumber = DEFAULT_SIZE) => get<PaginatedResult<Match>>(url('teams', `${id}/matches`), { page, size }),
            }
        }
    }
};
