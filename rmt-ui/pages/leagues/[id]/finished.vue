<template>
    <Container
        :loading="pending"
        :error="error?.message ?? error?.toString()"
        scroll-y
        class="flex row"
        no-heading-backing
    >
        <template #before>
            <h3 class="fill center-vert margin-left">Finished Matches</h3>
        </template>
        <template #header>
            <League :id="id" class="fill fill-bg">
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

        <Pager
            :page="page"
            :size="size"
            :total="total"
            :pages="pages"
            :url="`/leagues/${id}/finished`"
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();
const route = useRoute();

const id = computed(() => route.params.id?.toString());
const page = computed(() => +(route.query.page?.toString() ?? '1'));
const size = computed(() => +(route.query.size?.toString() ?? '10'));
const { data, pending, error, refresh } = await api.leagues.matches.finished(id.value, page, size);

const matches = computed(() => data.value?.data.results ?? []);
const total = computed(() => data.value?.data.count ?? 0);
const pages = computed(() => data.value?.data.pages ?? 0);
</script>

<style lang="scss" scoped>
.fill-bg {
    background-color: var(--bg-color-accent-dark);
}
</style>
