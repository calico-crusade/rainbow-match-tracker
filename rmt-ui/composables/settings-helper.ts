export const useSettingsHelper = () => {
    const config = useRuntimeConfig();

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

    function getSet<T>(key: string, def?: T, fn?: (val: T | undefined) => void) {
        const fetch = () => getStore<T>(key, def);
        const state = useState<T | undefined>(key, () => fetch());

        return computed({
            get: () => state.value ?? fetch(),
            set: (val: T | undefined) => {
                state.value = val;
                setStore(key, val);
                if (fn) fn(val);
            }
        });
    }

    return {
        token: getSet<string>('auth-token'),
        apiUrl: config.public.apiUrl,
        env: config.public.env,

        local: getSet,
        accessibilityMode
    };
}
