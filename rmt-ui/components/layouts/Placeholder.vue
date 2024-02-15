<template>
    <div class="placeholder" :style="styles" :class="allClasses">
        <div class="shimmer" />
    </div>
</template>

<script setup lang="ts">
import type { ClassOptions } from '~/models';

const { serClasses } = useUtils();

const props = defineProps<{
    width?: string;
    height?: string;
    round?: string;
    size?: string;
    classes?: ClassOptions;
    'class'?: ClassOptions;
}>();

const aWidth = computed(() => props.width || props.size);
const aHeight = computed(() => props.height || props.size);

const styles = computed(() => ({
    width: aWidth.value,
    height: aHeight.value,
    borderRadius: props.round
}));

const allClasses = computed(() => serClasses(props.class, props.classes));

</script>

<style lang="scss">
$background: #ffffff5e;
$background-accent: rgba(236, 8, 8, 0);

.placeholder {
    position: relative;
    overflow: hidden;

    .shimmer {
        animation-duration: 1s;
        animation-fill-mode: forwards;
        animation-iteration-count: infinite;
        animation-name: shimmer;
        animation-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
        position: absolute;
        height: 100%;
        width: 100%;
        top: 0;
        left: 0;
        background: linear-gradient(to right,
                #{$background-accent} 0%,
                #{$background} 50%,
                #{$background-accent} 100%);
        background-blend-mode: lighten;
        border-radius: var(--brd-radius);
    }
}

@keyframes shimmer {
    0% {
        left: -100%;
    }

    100% {
        left: 100%;
    }
}
</style>
