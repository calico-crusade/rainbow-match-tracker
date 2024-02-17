import type {
    Match, PaginatedResult,
    RefNumber,
    Team
} from '~/models';

type Matches = PaginatedResult<Match>;

export const useV1TeamApi = () => {
    const { get } = useApiHelper();
    const { pageSize } = useSettingsHelper();

    const url = (url?: string) => {
        return `/teams${url ? `/${url}` : ''}`;
    }

    function active() {
        return get<Team[]>(url('active'), undefined, !process.client);
    }

    function all(page: RefNumber, size?: RefNumber) {
        return get<PaginatedResult<Team>>(url(), {
            page,
            size: size ?? pageSize
        }, !process.client);
    }

    function one(id: string) {
        return get<Team>(url(id), undefined, !process.client);
    }

    function matches(id: string, page: RefNumber, size?: RefNumber) {
        return get<Matches>(url(`${id}/matches`), {
            page,
            size: size ?? pageSize
        }, !process.client);
    }

    return {
        active,
        all,
        get: one,
        matches
    }
}
