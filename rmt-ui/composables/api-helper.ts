import type { AsyncData, _AsyncData } from "./../node_modules/nuxt/dist/app/composables/asyncData.d.ts";
import filesaver from 'file-saver';
import type { ApiResultData, ErrorResult } from "~/models";
import { FetchError } from 'ofetch';

export type Params = { [key: string]: any; };

export const useApiHelper = () => {
    const { apiUrl, token } = useSettingsHelper();

    const wrapUrl = (url: string) => {
        if (url.toLowerCase().indexOf('https://') !== -1 ||
            url.toLowerCase().indexOf('http://') !== -1) return url;

        if (url.startsWith('/')) url = url.substring(1);
        if (url.endsWith('?')) url = url.substring(0, url.length - 1);

        return `${apiUrl}/${url}`;
    };

    const headers = () => {
        const t = token.value;
        if (!t) return undefined;
        return { 'Authorization': `Bearer ${t}` };
    };

    const doReq = <T>(url: string, opts: any, lazy: boolean) => {
        return lazy
            ? useLazyFetch<ApiResultData<T>, FetchError<ErrorResult>>(wrapUrl(url), opts)
            : useFetch<ApiResultData<T>, FetchError<ErrorResult>>(wrapUrl(url), opts);
    }

    function get<T>(url: string, params?: Params, lazy: boolean = false, key: string = url) {
        return doReq<T>(url, {
            params,
            key,
            headers: headers()
        }, lazy);
    }

    function post<T>(url: string, body?: any, params?: Params, lazy: boolean = false, key?: string) {
        return doReq<T>(url, <any>{
            params,
            headers: headers(),
            body,
            method: 'post',
            key,
        }, lazy);
    }

    function put<T>(url: string, body?: any, params?: Params, lazy: boolean = false) {
        return doReq<T>(url, <any>{
            params,
            headers: headers(),
            body,
            method: 'PUT'
        }, lazy);
    }

    function deletefn<T>(url: string, body?: any, params?: Params, lazy: boolean = false) {
        return doReq<T>(url, <any>{
            params,
            headers: headers(),
            method: 'DELETE',
            body
        }, lazy);
    }

    async function download(url: string, name?: string, req?: RequestInit) {
        const uri = wrapUrl(url);
        const t = await fetch(uri, req);
        const t_1 = await t.blob();
        filesaver.saveAs(t_1, name);
    }

    function onFinish<T>(request: AsyncData<T, any>, fn: (item?: T) => void, err?: (item: any) => void) {
        const { pending, data, error } = request;

        const resolve = () => {
            if (pending.value) return;

            if (error.value && err) {
                err(error);
                return;
            }

            fn(data.value ?? undefined);
        }

        resolve();

        watch(() => pending.value, () => resolve(), { immediate: true });
    }

    function toPromise<T>(request: AsyncData<T, any>, noError: boolean = false): Promise<T | undefined> {
        return new Promise((resolve, reject) => {
            onFinish(request, (d) => resolve(d), (err) => noError ? resolve(undefined) : reject(err));
        });
    }

    async function toPromiseApi<T>(request: AsyncData<ApiResultData<T> | null, any>, noError: boolean = false): Promise<T | undefined> {
        const value = await toPromise(request, noError);
        return value?.data;
    }

    const unwrap = <T>(result: _AsyncData<ApiResultData<T> | null, FetchError<ErrorResult> | null>) => {
        const { data, error } = result;

        const resData = computed(() => {
            if (data.value && !data.value.success) return undefined;
            return data.value?.data;
        });
        const resError = computed(() => {
            if (data.value && !data.value.success) return data.value;
            return error.value?.data;
        });

        const success = computed(() => data.value?.success ?? false);

        return {
            ...result,
            result: resData,
            error: resError,
            originalError: error,
            success
        }
    };

    const check = <T>(result: _AsyncData<ApiResultData<T> | null, FetchError<ErrorResult> | null>, context?: string) => {
        const { data, error } = result;

        if (!data.value || !data.value.success || error.value) {
            const first = (error.value?.data?.data ?? []).join(', ');
            const msg = first || data.value?.message || error.value?.data?.message || error.value?.message || 'Unknown error';
            console.error('API error', {
                context,
                data: data.value,
                error: error.value,
                result: {...result},
                msg
            })
            return { error: msg, data: undefined, rawError: error.value };
        }

        return { error: undefined, data: data.value.data, rawError: error.value };
    }

    // function upload<T>(url: string, file: File) {
    //     type Progress = {
    //         state: 'loading' | 'error' | 'success' | 'processing';
    //         progress: {
    //             percent: number;
    //             total: number;
    //             loaded: number;
    //         };
    //         result?: ApiResultData<T>;
    //         error?: ErrorResult;
    //     }

    //     const formData = new FormData();
    //     formData.append('files', file, file.name);

    //     const subject = new Subject<Progress>();

    //     const uri = wrapUrl(url);
    //     const xhr = new XMLHttpRequest();
    //     xhr.open('POST', uri, true);
    //     if (token.value) xhr.setRequestHeader('Authorization', `Bearer ${token.value}`);
    //     xhr.upload.onprogress = (e) => {
    //         if (!e.lengthComputable) return;

    //         const prec = Math.round((e.loaded / e.total) * 100);

    //         subject.next({
    //             state: prec === 100 ? 'processing' : 'loading',
    //             progress: {
    //                 total: e.total,
    //                 loaded: e.loaded,
    //                 percent: prec
    //             }
    //         });
    //     };
    //     xhr.onreadystatechange = () => {
    //         if (xhr.readyState !== 4) return;

    //         const success = xhr.status >= 200 && xhr.status < 300;
    //         const res = JSON.parse(xhr.response);
    //         subject.next({
    //             state: success ? 'success' : 'error',
    //             progress: {
    //                 total: 0,
    //                 loaded: 0,
    //                 percent: 100
    //             },
    //             result: success ? <ApiResultData<T>>res : undefined,
    //             error: success ? undefined : <ErrorResult>res
    //         });
    //         subject.complete();
    //     }

    //     xhr.send(formData);
    //     return subject.asObservable();
    // }

    return {
        get,
        post,
        put,
        del: deletefn,
        download,
        onFinish,
        toPromise,
        toPromiseApi,
        unwrap,
        check,
        //upload
    }
}
