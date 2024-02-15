<template>
    <div class="input flex margin-top margin-bottom">
        <div class="avatar center-vert margin-right" v-if="value">
            <Mark :input="value" size="60px" />
        </div>
        <div class="flex row center-vert fill">
            <p class="margin-bottom">{{ label }}:</p>
            <InputGroup v-model="value" :placeholder="label" :icon="icon ?? ''" />
            <p class="mute margin-top" v-if="!note">
                Can be a discord emote,
                <a href="https://fonts.google.com/icons" taget="_blank">google material icon</a>
                , image url, or a
                <a href="/admin/config" target="_blank">configuration emote</a>
                (sans the "ui_" prefix)
            </p>
            <p class="mute margin-top" v-else>{{ note }}</p>
        </div>
    </div>
</template>

<script setup lang="ts">
const emits = defineEmits<{
    (e: 'update:modelValue', v: any): void;
}>();

const props = defineProps<{
    modelValue?: any;
    icon?: string;
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
