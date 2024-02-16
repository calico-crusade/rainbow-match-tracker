<template>
    <Container
        :loading="loading"
        :error="err?.message ?? err?.toString()"
        scroll-y
        class="flex row"
        no-heading-backing
    >
        <template #before>
            <h3 class="fill center-vert margin-left">Upcoming Matches</h3>
        </template>
        <template #header>
            <League :league="league" class="fill fill-bg">
                <IconBtn
                    icon="sync"
                    @click="refresh"
                    title="Refresh"
                />
            </League>
        </template>

        <div class="flex margin-top">
            <IconBtn
                class="pad-left"
                color="shade"
                icon="calendar_today"
                text="Upcoming"
                active="primary"
                :link="`/leagues/${id}/upcoming`"
            />
            <IconBtn
                color="shade"
                class="margin-left"
                icon="calendar_today"
                text="Finished"
                active="primary"
                :link="`/leagues/${id}/finished`"
            />
            <IconBtn
                color="shade"
                class="margin-left"
                icon="history"
                text="All"
                active="primary"
                :link="`/leagues/${id}`"
            />
        </div>

        <Match
            v-for="match in matches"
            :key="match.id"
            :match="match"
            link
            hover
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();
const { setMeta } = useUtils();
const route = useRoute();

const id = computed(() => route.params.id?.toString());
const { data, pending, error, refresh } = await api.leagues.matches.active(id.value);
const { data: leagueData, pending: leaguePending, error: leagueError } = await api.leagues.get(id.value);

const league = computed(() => leagueData.value?.data);
const loading = computed(() => pending.value || leaguePending.value);
const err = computed(() => error.value ?? leagueError.value);

const matches = computed(() => data.value?.data ?? []);
const title = computed(() => `${league.value?.display ?? 'League'} - Upcoming Matches`);

setMeta(title.value, league.value?.name ?? 'Upcoming matches for the league', league.value?.image);
</script>

<style lang="scss" scoped>
.fill-bg {
    background-color: var(--bg-color-accent-dark);
}
</style>
