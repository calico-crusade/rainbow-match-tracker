<template>
    <Container
        :loading="pending"
        :error="error?.message ?? error?.toString()"
        scroll-y
        class="flex row"
        no-heading-backing
    >
        <template #before>
            <h3 class="fill center-vert margin-left">Team Matches</h3>
        </template>
        <template #header>
            <Team :id="id" class="fill fill-bg">
                <IconBtn
                    icon="sync"
                    @click="refresh"
                    title="Refresh"
                />
            </Team>
        </template>

        <Match
            v-for="match in matches"
            :key="match.id"
            :match="match"
            link
            hover
        />

        <Pager
            :page="page"
            :size="size"
            :total="total"
            :pages="pages"
            :url="`/teams/${id}`"
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();
const route = useRoute();

const id = computed(() => route.params.id?.toString());
const page = computed(() => +(route.query.page?.toString() ?? '1'));
const size = computed(() => +(route.query.size?.toString() ?? '10'));
const { data, pending, error, refresh } = await api.teams.matches(id.value, page, size);

const matches = computed(() => data.value?.data.results ?? []);
const total = computed(() => data.value?.data.count ?? 0);
const pages = computed(() => data.value?.data.pages ?? 0);
</script>

<style lang="scss" scoped>
.fill-bg {
    background-color: var(--bg-color-accent-dark);
}
</style>
