<template>
    <div
        class="input flex row margin-top margin-bottom"
        :class="classes"
    >
        <p class="margin-bottom">{{ label }}: </p>
        <InputGroup
            v-if="type !== 'markdown'"
            v-model="value"
            :placeholder="label"
            :icon="icon || ''"
            :type="type || 'text'"
            :min="min"
            :max="max"
            :step="step"
            :disabled="readonly"
            :input-disabled="readonly"
            :options="options"
            :split-char="splitChar"
        />
        <MarkdownInput
            v-else
            v-model="value"
            :placeholder="label"
            :icon="icon || ''"
            :min="min"
            :max="max"
            :step="step"
            :disabled="readonly"
            :input-disabled="readonly"
            :options="options"
            :split-char="splitChar"
        />
        <p class="mute margin-top" v-if="note">{{ note }}</p>
    </div>
    </template>

    <script setup lang="ts">
    import type { ClassOptions, booleanishext } from '~/models';

    const { serClasses } = useUtils();
    const emits = defineEmits<{
        (e: 'update:modelValue', v: any): void;
    }>();

    const props = defineProps<{
        modelValue?: any;
        icon?: string;
        label: string;
        note?: string;
        type?: string;
        min?: number | string;
        max?: number | string;
        step?: number | string;
        readonly?: booleanishext;
        options?: string | string[];
        splitChar?: string;
        'class'?: ClassOptions
    }>();

    const value = computed({
        get: () => props.modelValue,
        set: (val: any) => emits('update:modelValue', val)
    });

    const classes = computed(() => serClasses(props.class, {
        'os-visible': props.type === 'autocomplete'
    }));
    </script>

    <style lang="scss" scoped>

    </style>
