<template>
    <div class="popup-fade" :class="{ 'open': isTrue(open) }" />
    <div
        class="popup-popup flex row center"
        :class="{
            'open': isTrue(open)
        }"
    >
        <div class="popup-header flex">
            <p class="fill center-vert">{{ title }}</p>
            <IconBtn icon="close" @click="() => emit('close', false)" />
        </div>
        <div
            class="popup-content flex row margin-top"
            :class="{ 'scrollable': isTrue(props.scrollable) }"
            :style="styles">
            <slot />
        </div>
    </div>
</template>

<script setup lang="ts">
import type { booleanish } from '~/models';
const { keys, isTrue } = useUtils();

const emit = defineEmits<{
    (e: 'close', cancel: boolean): void;
}>();

const DEFAULT_SIZES = {
    'lg': { w: '100vw', h: '100vh' },
    'lg-mw': { w: 'min(1050px, 98vw)', h: '100vh' },
    'sm': { w: '200px', h: '300px' }
}

const props = defineProps<{
    open?: booleanish,
    title: string,
    size?: keyof typeof DEFAULT_SIZES,
    width?: string,
    height?: string,
    scrollable?: booleanish
}>();

const styles = computed(() => {
    const output: { [key: string]: string | undefined } = {};

    let { size, width, height } = props;

    if (size && DEFAULT_SIZES[size]) {
        width = width ?? DEFAULT_SIZES[size].w;
        height = height ?? DEFAULT_SIZES[size].h;
    }

    if (size || width) output['width'] = width ?? size;
    if (size || height) output['height'] = height ?? size;

    return keys(output)
        .filter(t => !!t)
        .map(t => `${t}: ${output[t]}`)
        .join('; ');
});
</script>

<style lang="scss" scoped>
.popup-fade {
    position: fixed;
    top: -100%;
    left: 0;
    z-index: 100;
    opacity: 0;
    width: var(--full-width);
    height: var(--full-height);
    background-color: var(--bg-color-accent-dark);
    transition: all 250ms ease-in-out;

    &.open {
        top: 0;
        opacity: 1;
    }
}

.popup-popup {
    position: fixed;
    top: -100%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 101;
    opacity: 0;
    transition: all 250ms ease-in-out;
    max-width: 98vw;
    max-height: 98vh;
    border: 1px solid var(--bg-color-accent);
    background-color: var(--bg-color);
    border-radius: var(--brd-radius);

    &.open {
        opacity: 1;
        top: 50%;
    }

    .popup-header {
        padding-bottom: 5px;
        margin: 20px;
        margin-bottom: 5px !important;
        border-bottom: 1px solid var(--color);
    }

    .popup-content {
        min-height: min(200px, 10vh);
        min-width: min(300px, 30vw);
        max-height: 80vh;
        max-width: 90vw;
        overflow-y: auto;
        margin: 20px;
        margin-top: 5px !important;
    }
}

@media only screen and (max-width: 450px) {
    .popup-popup {
        max-width: min(100vw, 100%);
        max-height: min(100vh, 100%);
        .popup-content, .popup-header {
            margin: 10px;
        }
    }
}
</style>
