<template>
    <div class="flex" :class="{ 'fill-parent': !isTrue(inline) }">
        <div class="center flex row center-items error-comp">
            <Mark :input="icon || '<a:ohnosweat:837610177796374549>'" />
            <h2 class="text-center">An error occurred!</h2>
            <p class="text-center">{{ message }}</p>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { booleanish } from '~/models';
const { isTrue } = useUtils();

defineProps<{
    inline?: booleanish
    message?: string,
    icon?: string
}>();
</script>

<style lang="scss" scoped>
.error-comp img {
    height: 75px;
    width: 75px;
}
</style>
