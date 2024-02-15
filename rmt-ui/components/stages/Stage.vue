<template>
    <div class="stage" :class="{ 'active': isActive, 'dark': isTrue(dark) }">
        <div
            class="stage-panel"
            :class="classes"
            v-if="keepAlive || isActive"
        >
            <slot />
        </div>
    </div>
</template>

<script lang="ts" setup>
import type { ClassOptions, booleanish, StageEvents, StageProps } from '~/models';
const { serClasses, isTrue } = useUtils();

type Props = {
    scrollable?: booleanish;
    keepAlive?: booleanish;
    dark?: booleanish;
    'class'?: ClassOptions;
    title?: string;
    icon?: string;
    ordinal?: number | string;
    valid?: booleanish;
    disableMessage?: string;
};

const props = withDefaults(defineProps<Props>(), {
    keepAlive: true
});

const emits = defineEmits<{
    (e: 'activated'): void;
    (e: 'deactivated'): void;
    (e: 'close'): void;
    (e: 'mounted'): void;
    (e: 'unmounted'): void;
}>();

const classes = computed(() => serClasses(
    props.class,
    { 'scroll-y': isTrue(props.scrollable) }
));

const refProps: { [key: string]: any } = {};

for (const key in props) {
    refProps[key] = toRef(props, key as keyof Props);
}
const stageData = reactive(<StageProps>refProps);

const stageEvents: StageEvents = {
    activated: () => emits('activated'),
    deactivated: () => emits('deactivated'),
    close: () => emits('close'),
    mounted: () => emits('mounted'),
    unmounted: () => emits('unmounted'),
};

const { isActive } = useStage(stageData, stageEvents);
</script>
