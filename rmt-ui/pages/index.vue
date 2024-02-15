<template>
    <Container
        :loading="pending"
        :error="error?.message ?? error?.toString()"
        scroll-y
    >
        <Match
            v-for="match in data?.data.results"
            :key="match.id"
            :match="match"
            no-border
        />
    </Container>
</template>

<script setup lang="ts">
const { v1: api } = useRmtApi();

const { data, pending, error } = await api.matches.all(1, 20);
</script>

<style lang="scss" scoped>

</style>
