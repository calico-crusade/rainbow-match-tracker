<template>
    <Container
        :loading="loading"
        :error="err?.message ?? err?.toString()"
        scroll-y
        class="flex row"
        no-heading-backing
    >
        <template #before>
            <h3 class="fill center-vert margin-left">Team Matches</h3>
        </template>
        <template #header>
            <Team :team="team" class="fill fill-bg">
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
            :total="total"
            :pages="pages"
            :url="`/teams/${id}`"
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();
const route = useRoute();
const { setMeta } = useUtils();

const id = computed(() => route.params.id?.toString());
const page = computed(() => +(route.query.page?.toString() ?? '1'));
const { data, pending, error, refresh } = await api.teams.matches(id.value, page);
const { data: teamData, pending: teamPending, error: teamError } = await api.teams.get(id.value);

const team = computed(() => teamData.value?.data);
const loading = computed(() => pending.value || teamPending.value);
const err = computed(() => error.value ?? teamError.value);

const matches = computed(() => data.value?.data.results ?? []);
const total = computed(() => data.value?.data.count ?? 0);
const pages = computed(() => data.value?.data.pages ?? 0);

const title = computed(() => `${team.value?.name ?? 'Team'} - All Matches`);
setMeta(title.value, team.value?.code ?? 'All team matches', team.value?.image);
</script>

<style lang="scss" scoped>
.fill-bg {
    background-color: var(--bg-color-accent-dark);
}
</style>
