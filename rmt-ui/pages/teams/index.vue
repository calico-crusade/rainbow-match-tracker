<template>
    <Container
        :loading="pending"
        :error="error?.message ?? error?.toString()"
        scroll-y
        class="flex row"
    >
        <template #header>
            <h3 class="margin fill center-vert">All Teams</h3>
            <IconBtn
                icon="sync"
                @click="refresh"
                title="Refresh"
            />
        </template>

        <Team
            v-for="team in teams"
            :key="team.id"
            :team="team"
            link
            hover
        />

        <Pager
            :page="page"
            :size="size"
            :total="total"
            :pages="pages"
            url="/teams"
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();
const route = useRoute();
const { setMeta } = useUtils();

const page = computed(() => +(route.query.page?.toString() ?? '1'));
const size = computed(() => +(route.query.size?.toString() ?? '10'));
const { data, pending, error, refresh } = await api.teams.all(page, size);

const teams = computed(() => data.value?.data.results ?? []);
const total = computed(() => data.value?.data.count ?? 0);
const pages = computed(() => data.value?.data.pages ?? 0);

setMeta();
</script>
