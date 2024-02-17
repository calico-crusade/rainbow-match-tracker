<template>
    <Container
        :loading="loading"
        :error="err?.message ?? err?.toString()"
        scroll-y
        class="flex row"
    >
        <template #header>
            <Image :src="league?.image" size="24px" class="center-vert margin-left" shadow />
            <h3 class="center-vert margin">{{ league?.display }}</h3>
            <h3 class="center-vert mute">Finished Matches</h3>
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
            <Pager
                :result="data"
                :url="`/leagues/${id}/finished`"
                no-top-margin
                class="center-vert"
            />
        </div>

        <Match
            v-for="match in data?.data.results"
            :key="match.id"
            :match="match"
            link
            hover
        />

        <Pager
            :result="data"
            :url="`/leagues/${id}/finished`"
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();
const route = useRoute();
const { setMeta } = useUtils();

const id = computed(() => route.params.id?.toString());
const page = computed(() => +(route.query.page?.toString() ?? '1'));
const { data, pending, error, refresh } = await api.leagues.matches.finished(id.value, page);
const { data: leagueData, pending: leaguePending, error: leagueError } = await api.leagues.get(id.value);

const league = computed(() => leagueData.value?.data);
const loading = computed(() => pending.value || leaguePending.value);
const err = computed(() => error.value ?? leagueError.value);

const title = computed(() => `${league.value?.display ?? 'League'} - ${data.value?.data.count ?? ''} Finished Matches`);
setMeta(title.value, league.value?.name ?? 'Finished league matches', league.value?.image);
</script>
