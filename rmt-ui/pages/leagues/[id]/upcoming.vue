<template>
    <Container
        :loading="loading"
        :error="err?.message ?? err?.toString()"
        scroll-y
        class="flex row"
    >
        <template #header>
            <Image :src="league?.image" size="24px" class="center-vert margin-left" shadow />
            <h3 class="center-vert margin">{{ league?.display }} </h3>
            <h3 class="center-vert mute">Upcoming Matches</h3>
            <IconBtn
                icon="sync"
                @click="refresh"
                title="Refresh"
                class="pad-left center-vert"
            />
        </template>

        <div class="flex margin-top page-container-btns">
            <div class="buttons flex">
                <IconBtn
                    class="center-vert"
                    color="shade"
                    icon="calendar_today"
                    text="Upcoming"
                    active="primary"
                    :link="`/leagues/${id}/upcoming`"
                />
                <IconBtn
                    color="shade"
                    class="margin-left center-vert"
                    icon="event_available"
                    text="Finished"
                    active="primary"
                    :link="`/leagues/${id}/finished`"
                />
                <IconBtn
                    color="shade"
                    class="margin-left center-vert"
                    icon="history"
                    text="All"
                    active="primary"
                    :link="`/leagues/${id}/all`"
                />
                <IconBtn
                    color="shade"
                    class="margin-left center-vert pad-right"
                    icon="groups"
                    text="Teams"
                    active="primary"
                    :link="`/leagues/${id}`"
                />
            </div>

            <div class="pager mute center-vert pad-left">
                {{ matches.length }} Upcoming Matches
            </div>
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
