<template>
    <ClientOnly>
        <div
            class="markdown"
            v-html="markdown"
            :class="actClasses"
        />
    </ClientOnly>
</template>

<script setup lang="ts">
import type { ClassOptions, booleanish } from '~/models';
import * as marked from 'marked';

const { isTrue, serClasses } = useUtils();

const props = defineProps<{
    content?: string,
    styled?: booleanish,
    classes?: string | string[],
    'class'?: ClassOptions
}>();

const markdown = ref<string | undefined>('');
const actClasses = computed(() => serClasses(props.class, props.classes, {
    styled: isTrue(props.styled)
}));

watch(() => props.content, async () => {
    try {
        markdown.value = await marked.parse(props.content || '');
    } catch {
        markdown.value = props.content;
    }
}, { immediate: true });
</script>
