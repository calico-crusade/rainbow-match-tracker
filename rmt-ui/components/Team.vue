<template>
    <NuxtLink
        class="card flex"
        :class="classes"
        @click="() => $emit('click', data)"
        :to="actLink"
        :target="isExternal(actLink) ? '_blank' : undefined"
    >
        <div class="image">
            <Image shadow v-if="data" :src="data.image" :alt="data.name" size="50px" fit="contain" />
            <Placeholder v-else size="50px" round="50%" />
        </div>

        <div class="main fill center-vert margin-left margin-right flex">
            <template v-if="data">
                <h3 class="center-vert margin-right fill">{{ data.name }}</h3>
                <p class="center-vert mute">({{ data.code }})</p>
            </template>
            <Placeholder v-else height="103px" width="100%" round="var(--brd-radius)" />
        </div>

        <div class="actions flex row center-vert">
            <slot />
        </div>
    </NuxtLink>
</template>

<script setup lang="ts">
import type { Team, booleanish, ClassOptions } from '~/models';

const { check } = useApiHelper();
const { serClasses, isTrue, isExternal } = useUtils();
const { v1: api } = useRmtApi();

const props = defineProps<{
    team?: Team,
    id?: string,
    link?: string | '' | 'external' | boolean;
    noTopMargin?: booleanish;
    hover?: booleanish;
    noRounded?: booleanish;
    noBorder?: booleanish;
    'class'?: ClassOptions;
}>();

const emit = defineEmits<{
    (e: 'click', v?: Team): void;
}>();

const loading = ref(false);
const data = ref<Team>();

const classes = computed(() => serClasses(props.class, {
    'margin-top': !isTrue(props.noTopMargin),
    'hover': isTrue(props.hover),
    'rounded': !isTrue(props.noRounded),
    'no-border': isTrue(props.noBorder),
}));

const actLink = computed(() => {
    if (props.link === true || props.link === '')
        return `/teams/${data.value?.id}`;

    if (props.link === 'external')
        return data.value?.url;

    if (props.link === false) return '';

    return props.link || '';
});

const marry = async () => {
    if (!process.client) return;

    if (props.team) {
        data.value = props.team;
        return;
    }

    if (!props.id ||
        (data.value && data.value.id === props.id))
        return;

    loading.value = true;
    const { data: team } = check(await api.teams.get(props.id));
    data.value = team;
    loading.value = false;
}

watch(() => props, () => marry(), { immediate: true, deep: true });
</script>
