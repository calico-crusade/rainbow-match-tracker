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
            <h3 class="center-vert mute">{{ codeDisplay }}'s Matches</h3>
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
import type { MatchSearch, MatchStatus } from '~/models';

const { v1: api } = useRmtApi();
const route = useRoute();
const { setMeta } = useUtils();

const id = computed(() => route.params.id?.toString());
const codes = computed(() => route.query.codes?.toString().split(',') ?? []);
const ids = computed(() => route.query.ids?.toString().split(',') ?? []);
const statuses = computed(() => route.query.statuses?.toString().split(',').map(t => <MatchStatus>Number(t)) ?? []);
const search = computed(() => <MatchSearch>{
    league: id.value,
    codes: codes.value,
    ids: ids.value,
    statuses: statuses.value
});
const { data, pending, error, refresh } = await api.matches.search(search.value);
const { data: leagueData, pending: leaguePending, error: leagueError } = await api.leagues.get(id.value);

const league = computed(() => leagueData.value?.data);
const matches = computed(() => data.value?.data ?? []);
const loading = computed(() => pending.value || leaguePending.value);
const err = computed(() => error.value ?? leagueError.value);
const codeDisplay = computed(() => {
    const values = codes.value.map(c => c.toUpperCase());

    if (values.length === 0) return 'Unknown';
    if (values.length === 1) return values[0];

    let output = '';
    for (let i = 0; i < values.length; i++) {
        output += values[i];
        if (i < values.length - 2) {
            output += ', ';
            continue;
        }

        if (i === values.length - 2) {
            output += ' and ';
            continue;
        }
    }
    return output;
});
</script>

<style lang="scss" scoped>

</style>
