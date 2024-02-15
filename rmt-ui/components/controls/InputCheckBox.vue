<template>
    <div class="input flex margin-top">
        <div class="input center-vert">
            <CheckBox v-model="value" />
        </div>
        <h3 class="center-vert margin">{{ label }}</h3>
        <p class="mute center-vert" v-if="note">{{ note }}</p>
    </div>
</template>

<script setup lang="ts">
const emits = defineEmits<{
    (e: 'update:modelValue', v: boolean): void;
}>();

const props = defineProps<{
    modelValue: boolean;
    label: string;
    note?: string;
}>();

const value = computed({
    get: () => props.modelValue,
    set: (val: boolean) => emits('update:modelValue', val)
});
</script>

<style lang="scss" scoped>
.input {
    .mute a {
        color: var(--color-muted);
        text-decoration: underline;
    }
}
</style>
