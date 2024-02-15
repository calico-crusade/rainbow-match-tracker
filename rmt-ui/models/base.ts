export interface Fts {
    id: string;
}

export interface DbObject {
    id: string;
    createdAt: Date;
    updatedAt: Date;
    deletedAt?: Date;
    deletedBy?: string;
}

export interface ApiResult {
    success: boolean;
    code: number;
    message?: string;
}

export interface ApiResultData<T> extends ApiResult {
    data: T;
}

export interface ErrorResult extends ApiResultData<string[]> { }

export interface PaginatedResult<T> {
    pages: number;
    count: number;
    results: T[];
}

export type LoadingStateTypes = 'initial' | 'loading' | 'not-found';
export type LoadingState<T> = LoadingStateTypes | T;
