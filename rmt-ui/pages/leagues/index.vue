<template>
    <Container
        :loading="pending"
        :error="error?.message ?? error?.toString()"
        scroll-y
        class="flex row"
    >
        <template #header>
            <h3 class="margin fill center-vert">All Leagues</h3>
            <IconBtn
                icon="sync"
                @click="refresh"
                title="Refresh"
            />
        </template>

        <League
            v-for="league in leagues"
            :key="league.id"
            :league="league"
            link
            hover
        />

        <Pager
            :page="page"
            :total="total"
            :pages="pages"
            url="/leagues"
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();
const route = useRoute();
const { setMeta } = useUtils();

const page = computed(() => +(route.query.page?.toString() ?? '1'));
const { data, pending, error, refresh } = await api.leagues.all(page);

const leagues = computed(() => data.value?.data.results ?? []);
const total = computed(() => data.value?.data.count ?? 0);
const pages = computed(() => data.value?.data.pages ?? 0);

setMeta();

</script>
