<template>
    <div class="flex" :class="{
        'fill-parent': !isTrue(inline),
        'margin-top': isTrue(marginTop),
    }">
        <div class="center flex loading-comp">
            <Mark :input="icon || '<a:loading:466940188849995807>'" />
            <p>{{ message || 'Loading...' }}</p>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { booleanish } from '~/models';
const { isTrue } = useUtils();
defineProps<{
    inline?: booleanish,
    message?: string,
    icon?: string,
    marginTop?: booleanish
}>();
</script>

<style lang="scss">
.loading-comp {
    img {
        height: 75px;
    }

    p {
        margin: auto 10px;
    }
}</style>
