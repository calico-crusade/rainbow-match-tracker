<template>
    <NuxtImg
        :src="imageSrc"
        :placeholder="placeholder"
        :width="width ?? size"
        :height="height ?? size"
        :loading="lazyLoading"
        :style="styles"
        :class="classes"
        :preload="isPreload"
        :fit="fit ?? 'cover'"
    />
</template>

<script setup lang="ts">
import type { booleanish, ClassOptions, StyleOptions } from '~/models';
const { serClasses, serStyles, isTrue, proxy } = useUtils();
const DEFAULT_IMAGE = '/logos/logo.png';

const props = defineProps<{
    src?: string;
    default?: string;
    'class'?: ClassOptions;
    style?: StyleOptions;
    noLazy?: booleanish;
    round?: booleanish;
    rounded?: booleanish;
    preload?: booleanish;
    width?: string | number;
    height?: string | number;
    size?: string | number;
    fit?: string;
    shadow?: booleanish;
}>();

const classes = computed(() => serClasses(props.class, {
    'round': isTrue(props.round),
    'rounded': isTrue(props.rounded),
    'shadow': isTrue(props.shadow)
}));

const styles = computed(() => serStyles(props.style, {
    // 'width': (props.width || props.size)?.toString(),
    // 'height': (props.height || props.size)?.toString()
}));

const lazyLoading = computed(() => isTrue(props.noLazy) ? 'eager' : 'lazy');
const placeholder = computed(() => props.default || DEFAULT_IMAGE);
const imageSrc = computed(() => massageUrl(props.src));
const isPreload = computed(() => isTrue(props.preload));

const massageUrl = (url?: string) => {
    if (!url) return placeholder.value;
    if (url.toLocaleLowerCase().indexOf('liquipedia.net') !== -1)
        return proxy(url);

    return url;
};

</script>

<style lang="scss" scoped>
.round { border-radius: 50%; }
.rounded { border-radius: var(--brd-radius); }
.shadow {
    filter: drop-shadow(1px 1px 0 white) drop-shadow(-1px -1px 0 white);
}
</style>
