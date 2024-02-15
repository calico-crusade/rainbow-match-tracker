import type { StageData, StageEvents, StageProps } from '~/models';

const injectionKey = Symbol('stage') as InjectionKey<{
    register: (identifier: symbol, props: StageProps, events: StageEvents) => void,
    deregister: (identifier: symbol) => void,
    active: Readonly<Ref<symbol | undefined>>,
}>;

export const useStages = () => {
    const stages = reactive(new Map<symbol, StageData>());
    const active = ref<symbol | undefined>();

    const ordered = computed(() => [...stages.values()].sort((a, b) => a.ordinal - b.ordinal));
    const current = computed(() => active.value ? stages.get(active.value) : undefined);
    const currentIndex = computed(() => ordered.value.findIndex(s => s.identifier === active.value));
    const next = computed(() => currentIndex.value === -1 ? undefined : ordered.value[currentIndex.value + 1]);
    const previous = computed(() => currentIndex.value === -1 ? undefined : ordered.value[currentIndex.value - 1]);

    const register = (identifier: symbol, props: StageProps, events: StageEvents) => {
        let stageData: StageData = {
            props,
            events,
            active: false,
            ordinal: +(props.ordinal ?? '0'),
            identifier,
        }

        stages.set(identifier, stageData);
        stageData.events.mounted();

        if (stages.size === 1) {
            setActive(identifier);
        }
    }

    const deregister = (identifier: symbol) =>  {
        const data = stages.get(identifier);
        if (!data) return;

        data.events.unmounted();
        stages.delete(identifier);
    }

    const setActive = (identifier: symbol) =>  {
        active.value = identifier

        for (const [id, data] of stages) {
            const active = id === identifier;

            if (active === data.active) continue;

            data.active = id === identifier;
            (active ? data.events.activated : data.events.deactivated)();
        }
    }

    const moveNext = () =>  {
        if (!next.value) return undefined;
        setActive(next.value.identifier);
        return next.value;
    }

    const moveBack = () =>  {
        if (!previous.value) return undefined;
        setActive(previous.value.identifier);
        return previous.value;
    }

    provide(injectionKey, {
        register,
        deregister,
        active
    });

    return {
        stages,
        ordered,
        current,
        next,
        previous,
        currentIndex,

        setActive: setActive,
        moveNext,
        moveBack,
    }
};

export const useStage = (props: StageProps, events: StageEvents) => {
    const injection = inject(injectionKey);

    if (!injection) throw new Error('Stage was not provided');

    const { register, deregister, active } = injection;

    const identifier = Symbol(props.title);

    register(identifier, props, events);

    onUnmounted(() => {
        deregister(identifier);
    });

    const isActive = computed(() => active.value === identifier);

    return {
        isActive,
    }
};
