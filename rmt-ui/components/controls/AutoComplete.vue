<template>
    <div class="autocomplete" :class="classes" ref="parent">
        <input
            :type="type"
            v-model="selection"
            @keydown.enter="enter"
            @keydown.down="down"
            @keydown.up="up"
            @focus="onFocus"
            @blur="onBlur"
            :disabled="isDisabled"
            :placeholder="placeholder"
        />

        <ul class="dropdown" v-if="filtered.length !== 0">
            <li
                v-for="(opt, index) in filtered"
                @click="select(index)"
                :class="{ active: current === index }"
            >{{ opt }}</li>
        </ul>
    </div>
    </template>

    <script setup lang="ts">
    import type { ClassOptions, booleanishext } from '~/models';
    const { serClasses, isTrue } = useUtils();

    const props = withDefaults(defineProps<{
        type?: string;
        modelValue?: any;
        options?: string[] | string;
        splitChar?: string;
        placeholder?: string;
        disabled?: booleanishext;
        transparent?: booleanishext;
        'class'?: ClassOptions;
    }>(), {
        type: 'text'
    });

    const emits = defineEmits<{
        (e: 'update:modelValue', v: string | undefined): void;
        (e: 'selected', v: string | undefined): void;
    }>();

    const isDisabled = computed(() => isTrue(props.disabled));
    const classes = computed(() => serClasses(props.class, {
        open: open.value,
        transparent: isTrue(props.transparent),
        disabled: isDisabled.value
    }));

    const actOptions = computed(() => {
        if (!props.options) return [];
        if (typeof props.options !== 'string') return props.options;
        return props.options.split(props.splitChar ?? ',').map(o => o.trim());
    });

    const filtered = computed(() => {
        if (!props.modelValue) return actOptions.value;
        return actOptions.value.filter(o => o.toLowerCase().includes(props.modelValue?.toLowerCase() ?? ''));
    });

    const selection = computed({
        get: () => props.modelValue,
        set: v => { emits('update:modelValue', v); onFocus(); }
    });

    const open = ref(false);
    const current = ref(-1);
    const parent = ref<HTMLDivElement>();

    const up = () => { if (current.value > 0) current.value -= 1; }
    const down = () => { if (current.value < filtered.value.length - 1) current.value += 1; }

    const enter = () => {
        const selected = filtered.value[current.value] ?? props.modelValue;
        selection.value = selected;
        emits('selected', selected);
        open.value = false;
    }

    const select = (index: number) => {
        current.value = index;
        enter();
    }

    const onFocus = () => {
        if (isDisabled.value) return;
        open.value = true;
    }

    const onBlur = () => {
        setTimeout(() => {
            open.value = false;
        }, 100);
    }
    </script>

    <style lang="scss" scoped>
    $ctrl-bg: var(--bg-color);

    .autocomplete {
        position: relative;
        background-color: $ctrl-bg;
        color: var(--color);
        padding: 10px;
        border-radius: var(--brd-radius);
        outline: none;
        border: 1px solid #{$ctrl-bg};
        font-family: var(--font-family);
        transition: all 250ms;

        input {
            background-color: transparent;
            -moz-appearance: none;
            -webkit-appearance: none;
            appearance: none;
            padding: 0;
            margin: 0;
            width: 100%;
            height: 100%;
            border: none;
        }

        ul {
            display: none;
            flex-flow: column;
            position: absolute;
            border-radius: var(--brd-radius);
            border: 1px solid var(--color-primary);
            top: 100%;
            left: 0;
            background-color: $ctrl-bg;
            list-style-type: none;
            margin: 0;
            padding: 0;
            min-width: min(150px, 98vw);
            z-index: 999;
            overflow-y: auto;
            max-height: 150px;

            li {
                margin: 0;
                padding: 5px 10px;
                cursor: pointer;

                &.active {
                    background-color: var(--color-primary);
                }
            }
        }

        &.transparent {
            background-color: transparent;
            border: 1px solid transparent;
        }

        &.open {
            ul {
                display: flex;
            }
        }
    }
    </style>
