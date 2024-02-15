<template>
    <div class="app-fix flex row overflow">
        <NavBar />
        <main class="fill flex content overflow margin">
            <slot />
        </main>
    </div>
</template>

<script setup lang="ts"></script>

<style lang="scss" scoped>
@media only screen and (max-width: 1050px) {
    .app-fix {
        flex-flow: column;

        main { overflow: hidden; }
    }
}
</style>
