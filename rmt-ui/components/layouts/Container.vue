<template>
    <Error
        v-if="error"
        :message="error"
        :icon="errorIcon"
    />
    <Loading
        v-else-if="isTrue(loading)"
        :icon="loadingIcon"
        :message="loadingMessage"
    />
    <div
        v-else
        class="component-container fill-parent flex row overflow"
        :class="{
            'scroll-y': isTrue(scrollY),
            'scroll-x': isTrue(scrollX),
            'margin-top': isTrue(marginTop),
            'padding-top': isTrue(paddingTop)
        }"
        ref="scroller"
        @scroll="onScroll"
    >
        <div
            class="flex row margin center-horz fill"
            :class="{
                'overflow': !noOverflow
            }"
        >
            <div class="flex row" :class="compClasses" v-if="hasBefore">
                <slot name="before" />
            </div>
            <header
                ref="stickyheader"
                class="flex margin-top"
                :class="{ 'max-width': !isTrue(unwidth) }"
                v-if="hasHeader"
                :style="headerStyles"
            >
                <slot name="header" />
            </header>
            <div
                class="component-content fill center"
                :class="compClasses"
            >
                <slot />
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { booleanish } from '~/models';
const slots = useSlots();
const { isTrue, scrollPercent, debounce } = useUtils();

const props = defineProps<{
    unwidth?: booleanish;
    marginTop?: booleanish;
    paddingTop?: booleanish;

    loading?: booleanish;
    loadingIcon?: string;
    loadingMessage?: string;
    noLoginCheck?: booleanish;

    error?: string;
    errorIcon?: string;

    scrollY?: booleanish;
    scrollX?: booleanish;
    autoScrollPercent?: number;
    scrollWatcher?: any;
    scrollWait?: number | string;
    grid?: ('by-2' | 'by-3' | 'by-4' | 'by-auto' | 'responsive')[] | string;

    noHeadingBacking?: booleanish;
}>();

const emits = defineEmits<{
    (e: 'onscrolled'): void;
    (e: 'auto-scrolled'): void;
}>();

const hasHeader = computed(() => !!slots['header']);
const hasBefore = computed(() => !!slots['before']);
const noOverflow = computed(() => isTrue(props.scrollX) || isTrue(props.scrollY));
const actScrollWait = computed(() => (props.scrollWait ? +props.scrollWait : 500) ?? 500);

const scroller = ref<HTMLElement>();
const stickyheader = ref<HTMLElement>();
const compClasses = computed(() => {
    const output = [];

    if (!isTrue(props.unwidth)) output.push('max-width');
    else output.push('fill fill-parent');

    if (!props.grid) output.push('flex row');
    else if (typeof props.grid === 'string') output.push(props.grid);
    else output.push(...props.grid);

    return output.join(' ');
});

const headerStyles = computed(() => {
    const output: {[key: string]: string} = {};

    if (!isTrue(props.noHeadingBacking)) output.backgroundColor = 'var(--bg-color-accent-dark)';

    return output;
});

const onScroll = () => {
    const element = scroller.value;
    if (!element) return;

    const bottom =
        element.scrollTop + element.clientHeight
        >= element.scrollHeight;
    if (!bottom) return;

    emits('onscrolled');
}

const actScrollToBottom = () => {
    if (!scroller.value) return;

    scroller.value.scrollTo({ top: scroller.value.scrollHeight, behavior: 'smooth' });
    emits('auto-scrolled');
}

const scrollBottom = debounce<void>(actScrollToBottom, actScrollWait.value);

watch(() => props.scrollWatcher, () => {
    if (!scroller.value) return;

    const target = (props.autoScrollPercent ?? 80);
    const percent = scrollPercent(scroller.value);
    const isWithin = percent >= target;
    if (!isWithin) return;

    scrollBottom();
});
</script>

<style lang="scss" scoped>
.component-container {
    header {
        position: sticky;
        top: 0px;
        z-index: 1;
        border-radius: var(--brd-radius);

        &[stuck] { padding-top: 5px; }
    }
}
</style>
