<template>
    <template v-if="!type"><Icon :class="allClasses" :size="size" :title="title">{{ input }}</Icon></template>
    <Date v-else-if="type.type === 'date'" :input="type.value" :format="type.format ?? undefined" :classes="allClasses" />
    <Image
        v-else-if="type.type === 'image'"
        :src="type.value ?? undefined"
        :default="default"
        :style="styles"
        :class="allClasses"
        :title="title"
        fit="contain"
    />
    <NuxtLink
        v-else-if="type.type === 'href'"
        :to="type.value ?? undefined"
        :target="type.value && isExternal(type.value) ? '_blank' : undefined"
        :alt="type.alt ?? type.text"
        :title="type.text"
    >{{ type.text }}</NuxtLink>
    <div :class="allClasses" v-else v-html="type.value"></div>
</template>

<script setup lang="ts">
import { DateTime } from 'luxon';
import type { ClassOptions, booleanish } from '~/models';
const { keys, isTrue, serClasses, isExternal } = useUtils();

type Result = {
    type: 'html' | 'image' | 'date' | 'href';
    value?: string | null;
    format?: string | null;
    alt?: string | null;
    classes?: ClassOptions;
    text?: string | null;
}

const props = defineProps<{
    input?: string;
    size?: string;
    width?: string;
    height?: string;
    default?: string;
    round?: booleanish;
    classes?: ClassOptions;
    'class'?: ClassOptions;
    title?: string;
}>();

const styles = computed(() => {
    const styles: { [key: string]: string | undefined } = {
        'max-width': props.width || props.size,
        'max-height': props.height || props.size,
        'border-radius': isTrue(props.round) ? '50%' : undefined
    };

    if (props.input?.toLowerCase().endsWith('.svg') || !props.input || type.value?.type === 'image') {
        styles['height'] = props.height || props.size;
        styles['width'] = props.width || props.size;
    }

    return keys(styles)
        .filter(k => styles[k])
        .map(k => `${k}: ${styles[k]}`).join('; ');
});

const allClasses = computed(() => serClasses(props.class, props.classes, type.value?.classes));

const resolveType = (data?: string) => {
    const input = (data || props.default) ?? '/logos/logo.png';
    const resolvers: (() => Result | undefined)[] = [
        () => {
            const match = /^<t:([0-9]{1,}):?(t|T|d|D|f|F|r|R)?>/.exec(input);
            if (!match) return undefined;
            const { format, ts } = { format: match[2], ts: match[1] };
            const dt = DateTime.fromSeconds(+ts);
            return { type: 'date', value: dt.toJSON(), format }
        },
        () => {
            const match = /^<d:([0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}\.?[0-9]{0,}Z?):?(t|T|d|D|f|F|r|R)?>/.exec(input);
            if (!match) return undefined;
            let { format, ts } = { format: match[2], ts: match[1] };
            if (!ts.endsWith('Z')) ts += 'Z';
            return { type: 'date', value: ts, format }
        },
        () => {
            const match =  /^<(a)?:(.*?):([0-9]{1,})>/.exec(input);
            if (!match) return undefined;
            const { animated, name, id } = { animated: match[1], name: match[2], id: match[3] };
            return {
                type: 'image',
                value: `https://cdn.discordapp.com/emojis/${id}.${animated ? 'gif' : 'png'}`,
                alt: name
            }
        },
        () => {
            const match = /^<@(!)?([0-9]{1,})>/.exec(input);
            if (!match) return undefined;

            const id = match[2];
            return {
                type: 'href',
                value: `https://discord.com/users/${id}`,
                classes: 'discord-ref-link',
                text: `@${id}`
            }
        },
        () => {
            const match = /^(https?:?\/\/(.*?))$/.exec(input);
            if (!match) return undefined;
            return { type: 'image', value: input }
        },
        () => {
            const match = /^\/(.*?)$/.exec(input);
            if (!match) return undefined;
            return { type: 'image', value: input }
        }
    ];

    for(const resolver of resolvers) {
        const result = resolver();
        if (!result) continue;

        return result;
    }
}

const type = ref(resolveType(props.input));

watch(() => props, () => type.value = resolveType(props.input), { immediate: true, deep: true });
</script>

<style lang="scss" scoped>
img { display: block; }
</style>
