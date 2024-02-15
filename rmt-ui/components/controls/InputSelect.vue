<template>
    <div class="input flex row margin-top margin-bottom">
        <p class="margin-bottom">{{ label }}: </p>
        <SelectBox v-model="value">
            <slot />
        </SelectBox>
        <p class="mute margin-top" v-if="note">{{note}}</p>
    </div>
</template>

<script setup lang="ts">
const emits = defineEmits<{
    (e: 'update:modelValue', v: any): void;
}>();

const props = defineProps<{
    modelValue?: any;
    label: string;
    note?: string;
}>();

const value = computed({
    get: () => props.modelValue,
    set: (val: any) => emits('update:modelValue', val)
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
