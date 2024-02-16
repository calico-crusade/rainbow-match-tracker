<template>
    <Container
        :loading="pending"
        :error="error?.message ?? error?.toString()"
        scroll-y
        class="flex row"
    >
        <Match :match="match" />
    </Container>
</template>

<script setup lang="ts">
const route = useRoute();
const { v1: api } = useRmtApi();
const { setMeta } = useUtils();

const id = computed(() => route.params.id?.toString());
const { data, pending, error } = await api.matches.get(id.value);
const match = computed(() => data.value?.data ?? undefined);

const title = computed(() => {
    if (!match.value) return undefined;
    return `${match.value.teams[0].name} vs ${match.value.teams[1].name} - R6 Match Tracker`;
});

setMeta(title.value, undefined, match.value?.league.image);
</script>
