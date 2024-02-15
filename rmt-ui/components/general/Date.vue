<template>
    <span
        class="date-time"
        :class="classes"
        :title="datetime?.toLocaleString(DateTime.DATETIME_FULL)"
    >{{ formatted }}</span>
</template>

<script setup lang="ts">
import { DateTime } from 'luxon';
import type { booleanish } from '~/models';
const { isTrue, trigger } = useUtils();

const props = defineProps<{
    value?: string | Date;
    format?: string;
    muted?: booleanish;
    utc?: booleanish;
    'class'?: string | string[];
}>();

const refreshKey = ref(0);
const classes = computed(() => [
    isTrue(props.muted) ? 'mute' : '',
    ...(typeof props.class === 'string' ? props.class.split(' ') : props.class ?? [])
].join(' '));

const datetime = computed(() => {
    if (!props.value) return undefined;
    if (props.value instanceof Date) return DateTime.fromJSDate(props.value);
    let value = props.value.endsWith('Z') || !isTrue(props.utc) ? props.value : `${props.value}Z`;
    return DateTime.fromJSDate(new Date(value));
})

const formatted = computed(() => {
    refreshKey.value;
    const date = datetime.value;
    if (!date) return undefined;

    const format = props.format ?? 'f';
    if (format === 'r' || format === 'R') return date.toRelative();


    let formats: { [key: string]: any } = {
        't': DateTime.TIME_24_SIMPLE,
        'T': DateTime.TIME_24_WITH_SECONDS,
        'd': DateTime.DATE_SHORT,
        'D': DateTime.DATE_MED_WITH_WEEKDAY,
        'f': DateTime.DATETIME_MED,
        'F': DateTime.DATETIME_FULL
    };

    let fo = formats[format];
    return fo ? date.toLocaleString(fo) : date.toFormat(format);
});

watch(() => trigger.value, () => {
    const relatives = ['r', 'R'];
    if (relatives.indexOf(props.format ?? 'f') === -1) return;

    refreshKey.value = refreshKey.value === 10 ? 0 : refreshKey.value + 1;
});
</script>

<style lang="scss" scoped>
.bold {
    font-weight: bold;
}

.hover:hover {
    cursor: pointer;
}
</style>
