<template>
    <Container
        :loading="loading"
        :error="err?.message ?? err?.toString()"
        scroll-y
        class="flex row"
    >
        <template #before>
            <div class="flex">
                <Image :src="league?.image" size="24px" class="center-vert margin-left" shadow />
                <h3 class="center-vert margin-left margin-right">{{ league?.display }}</h3>
                <h3 class="center-vert mute">{{ teams.length }} Teams</h3>
            </div>
            <div class="flex margin-top page-container-btns">
                <div class="buttons flex">
                    <label class="center-vert margin-left margin-right">Matches: </label>
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
                    <div class="sep" />
                    <IconBtn
                        color="shade"
                        class="center-vert pad-right"
                        icon="groups"
                        text="Teams"
                        active="primary"
                        :link="`/leagues/${id}`"
                    />
                </div>

                <div class="pager mute center-vert pad-left">
                    {{ teamsData?.data.length ?? 0 }} Teams
                </div>
            </div>

            <h2 class="margin-left margin-top">Search Team Matches: </h2>
        </template>

        <template #header>
            <InputGroup
                v-model="search"
                placeholder="Search Teams"
                icon=""
                class="fill center-vert margin-left"
            />
            <IconBtn
                icon="sports_esports"
                color="success"
                :disabled="!link"
                :link="link"
                text="View Matches"
                class="margin"
            />
        </template>


        <div class="border flex row margin-top margin-bottom">
            <h3>Match Statuses: </h3>
            <div class="grid by-2 margin-top">
                <CheckBox v-for="status in statuses" :key="status.text" v-model="status.selected">
                    {{ status.text }}
                </CheckBox>
            </div>
        </div>

        <Team
            v-for="team in teams"
            :key="team.id"
            :team="team"
            @click="toggleSelected(team)"
            hover
        >
            <template #before>
                <CheckBox :model-value="isSelected(team)" @update:model-value="toggleSelected(team)" />
            </template>
        </Team>

    </Container>
</template>

<script setup lang="ts">
import { MatchStatus, type Team } from '~/models';

const { v1: api } = useRmtApi();
const { setMeta } = useUtils();
const route = useRoute();

const id = computed(() => route.params.id?.toString());
const { data: teamsData, pending: teamsPending, error: teamsError } = await api.leagues.teams(id.value);
const { data: leagueData, pending: leaguePending, error: leagueError } = await api.leagues.get(id.value);
const title = computed(() => `${league.value?.display ?? 'League'} - ${teamsData.value?.data.length ?? ''} Teams`);

const statuses = reactive([
    { text: 'In Progress', value: [MatchStatus.Active], selected: true, },
    { text: 'Finished', value: [MatchStatus.Draw, MatchStatus.TeamOneWon, MatchStatus.TeamTwoWon], selected: true },
    { text: 'Upcoming', value: [MatchStatus.Upcoming], selected: true },
    { text: 'Unknown', value: [MatchStatus.Unknown], selected: true }
]);

const league = computed(() => leagueData.value?.data);
const teams = computed(() => {
    const teams = teamsData.value?.data ?? [];
    if (!search.value) return teams;
    return teams.filter(t =>
        t.name.toLowerCase().includes(search.value.toLowerCase()) ||
        t.code.toLowerCase().includes(search.value.toLowerCase()));
});
const loading = computed(() => teamsPending.value || leaguePending.value);
const err = computed(() => leagueError.value ?? teamsError.value);

const codes = ref<string[]>([]);
const search = ref<string>('');
const link = computed(() => {
    const status = statuses.filter(s => s.selected).map(s => s.value).flat();
    const values = codes.value.join(',');

    if (status.length === 0 || !values) return undefined;

    const output = [ `codes=${values}` ];
    if (status.length !== 6)
        output.push(`statuses=${status.join(',')}`);

    return `/leagues/${id.value}/teams?` + output.join('&');
});

const isSelected = (team: Team) => {
    return codes.value.includes(team.code);
}

const toggleSelected = (team: Team) => {
    if (isSelected(team)) {
        codes.value = codes.value.filter(c => c !== team.code);
        return;
    }

    codes.value.push(team.code);
};

setMeta(title.value, league.value?.name ?? 'Search league games', league.value?.image);
</script>

<style lang="scss" scoped>
.border {
    border: 1px solid var(--color-primary);
    border-radius: var(--brd-radius);
    padding: var(--margin);
}

.sep {
    height: 100%;
    border: 1px solid var(--color);
    margin-left: var(--margin);
    margin-right: var(--margin);
}
</style>
