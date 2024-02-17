import type {
    Match, PaginatedResult,
    RefNumber,
    ExtendedMatch,
    MatchSearch
} from '~/models';

type Matches = PaginatedResult<Match>;

export const useV1MatchApi = () => {
    const { get, post } = useApiHelper();
    const { pageSize } = useSettingsHelper();

    const url = (url?: string) => {
        return `/matches${url ? `/${url}` : ''}`;
    }

    function all(page: RefNumber, size?: RefNumber) {
        return get<Matches>(url(), {
            page,
            size: size ?? pageSize
        }, !process.client);
    }

    function one(id: string) {
        return get<ExtendedMatch>(url(id), undefined, !process.client);
    }

    function range(start: Date, end?: Date) {
        return get<Match[]>(url('range'), { start, end }, !process.client);
    }

    function search(req: MatchSearch) {
        return post<Match[]>(url('search'), req, undefined, !process.client);
    }

    return {
        all,
        get: one,
        range,
        search
    }
}
