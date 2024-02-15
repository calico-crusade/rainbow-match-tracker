<template>
    <Container>
        <Match :id="id" />
    </Container>
</template>

<script setup lang="ts">
const route = useRoute();

const id = computed(() => route.params.id?.toString());
</script>
