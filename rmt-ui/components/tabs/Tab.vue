<template>
    <div class="tab" :class="{ 'active': isActive, dark }">
        <div class="tab-panel" :class="classes" v-if="keepAlive || isActive">
            <slot />
        </div>
    </div>
</template>

<script lang="ts" setup>
import { toRef, reactive } from 'vue'
import type { ClassOptions } from '~/models';
interface TabData {
    title?: string;
    icon?: string;
    scrollable?: boolean;
    keepAlive?: boolean;
    dark?: boolean;
    className?: string;
    'class'?: ClassOptions
}

const { serClasses, isTrue } = useUtils();
const props = defineProps<TabData>();

const refProps: { [key: string]: any } = {};

for (const key in props) {
    refProps[key] = toRef(props, key as keyof TabData);
}

const tabData = reactive(<TabData>refProps);
const { isActive } = useTab(tabData);

const classes = computed(() => serClasses(props.class, props.className, {
    'scroll': isTrue(props.scrollable)
}));
</script>

<style lang="scss">
.scroll {
    overflow-y: auto;
}
</style>
