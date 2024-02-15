<template>
    <ClientOnly>
        <label
            :tabindex="disabled ? -1 : 0"
            class="checkbox"
            role="checkbox"
            :for="id"
            :aria-disabled="isTrue(disabled)"
            :class="allClasses"
            :aria-checked="value"
            @keydown.space.prevent="toggle"
            @click.stop.prevent="toggle"
        >
            <slot name="pre" />
            <span class="wrap">
                <Icon>{{ value ? 'check_box' : 'check_box_outline_blank' }}</Icon>
                <input type="checkbox" :id="id" hidden :disabled="isTrue(disabled)" :modelValue="modelValue"
                    @update:modelValue="(e: any) => value = e.target.checked" />
            </span>
            <span v-if="hasLabel">
                <slot v-if="!label" />
                <template v-else>{{ labelText }}</template>
            </span>
        </label>
    </ClientOnly>
</template>

<script setup lang="ts">
import type { ClassOptions, booleanishext } from '~/models';
const { isTrue, serClasses } = useUtils();
const slots = useSlots();

interface Emits {
    (e: 'update:modelValue', value: boolean): void;
}

interface Props {
    label?: string;
    modelValue: boolean;
    disabled?: booleanishext;
    classes?: ClassOptions;
    'class'?: ClassOptions;
}

const emit = defineEmits<Emits>();
const props = defineProps<Props>();

const id = ref(`chk-${Math.random().toString().replace('.', '')}`);
const value = computed({
    get() {
        return props.modelValue;
    },
    set(value: boolean) {
        emit("update:modelValue", value);
    },
});

const toggle = () => {
    if (!props.disabled) { value.value = !value.value; }
}

const labelText = computed(() => props.label || slots.default);
const hasLabel = computed(() => !!labelText.value);

const allClasses = computed(() => serClasses(props.class, props.classes, {
    disabled: isTrue(props.disabled),
}));

</script>

<style lang="scss" scoped>
label {
    position: relative;
    display: flex;
    align-items: center;
    user-select: none;

    &.disabled {
        color: var(--color-muted);
    }

    .wrap {
        display: inline-flex;
        align-items: center;
    }
}
</style>
