<template>
    <div class="flex row" :class="classes">
        <div class="flex">
            <h3 class="center-vert pad-right">{{ title ?? 'What do you want?' }}</h3>
            <CheckBox class="center-vert margin-right" v-model="all">All</CheckBox>
        </div>
        <div class="grid by-auto">
            <CheckBox
                v-for="t in props.options"
                :key="t"
                :model-value="hasType(t)"
                @update:model-value="() => toggleType(t)"
            >
                <template v-if="!hasSlot">{{ t }}</template>
                <slot v-else :data="t" />
            </CheckBox>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { ClassOptions } from '~/models';

const { serClasses } = useUtils();
const slots = useSlots();

const props = defineProps<{
    options: string[],
    modelValue: string[],
    title?: string,
    'class'?: ClassOptions
}>();

const emits = defineEmits<{
    (e: 'update:modelValue', v: string[]): void;
}>();


const hasSlot = computed(() => !!slots.default);
const value = computed({
    get: () => props.modelValue,
    set: (v: string[]) => emits('update:modelValue', v)
});

const all = computed({
    get: () => value.value.length === props.options.length,
    set: (v: boolean) => {
        if (v) {
            value.value = [...props.options];
        } else {
            value.value = [];
        }
    }
})

const classes = computed(() => serClasses(props['class']));

const hasType = (type: string) => value.value.indexOf(type) !== -1;
const toggleType = (type: string) => {
    value.value = hasType(type)
        ? value.value.filter(t => t !== type)
        : [...value.value, type];
}

</script>
