export const useSettingsHelper = () => {
    const config = useRuntimeConfig();
    const route = useRoute();

    const DEFAULT_PAGE_SIZE = 10;

    const accessibilityModeState = useState<boolean>('accessibility-mode', () => !!process.client && !!localStorage.getItem('accessibility-mode'));
    const accessibilityMode = computed({
        get: () => accessibilityModeState.value,
        set: (value: boolean) => {
            accessibilityModeState.value = value;
            if (value) localStorage.setItem('accessibility-mode', 'true');
            else localStorage.removeItem('accessibility-mode');
        }
    });

    function getStore<T>(key: string, def?: T) {
        if (!process.client) return def;
        return <T><any>localStorage.getItem(key) ?? def;
    }

    function setStore<T>(key: string, value?: T) {
        if (!process.client) return;
        if (value) localStorage.setItem(key, (<any>value).toString());
        else localStorage.removeItem(key);
    }

    function getSet<T>(key: string, def: T, fn?: (val: T) => void) {
        const fetch = () => getStore<T>(key, def) ?? def;
        const state = useState<T>(key, () => fetch());

        return computed({
            get: () => state.value ?? fetch(),
            set: (val: T) => {
                state.value = val;
                setStore(key, val);
                if (fn) fn(val);
            }
        });
    }

    function getPage() {
        if (route.query.size) {
            const size = parseInt(route.query.size as string);
            if (!isNaN(size)) return size;
        }

        if (!process.client) return DEFAULT_PAGE_SIZE;

        const strSize = localStorage.getItem('page-size');
        if (!strSize) return DEFAULT_PAGE_SIZE;

        const size = parseInt(strSize as string);
        if (!isNaN(size)) return size;

        return DEFAULT_PAGE_SIZE;
    }

    function setPage(value: number) {
        if (!process.client) return;

        localStorage.setItem('page-size', value.toString());
    }

    return {
        token: getSet<string>('auth-token', ''),
        pageSize: computed({
            get: getPage,
            set: setPage
        }),
        apiUrl: config.public.apiUrl,
        env: config.public.env,

        local: getSet,
        accessibilityMode
    };
}
