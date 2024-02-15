<template>
    <div class="input flex row margin-top margin-bottom">
        <p class="margin-bottom">{{ label }}:</p>
        <div class="tags in-line margin-bottom">
            <button
                v-for="tag in value"
                @click="() => remove(tag)"
            >
                &times;
                <template v-if="hasSlot">
                    <slot v-bind:data="tag" />
                </template>
                <template v-else>{{ tag }}</template>
            </button>
        </div>
        <InputGroup v-model="newItem" :icon="icon ?? ''" :placeholder="label" @search="add" />
        <p class="mute margin-top" v-if="note">{{ note }}</p>
    </div>
</template>

<script setup lang="ts">
const slots = useSlots();

const emits = defineEmits<{
    (e: 'update:modelValue', v: string[]): void;
    (e: 'added', v: string): void;
    (e: 'removed', v: string): void;
}>();

const props = defineProps<{
    modelValue: string[];
    label: string;
    icon?: string;
    note?: string;
}>();

const newItem = ref('');
const hasSlot = computed(() => !!slots['default']);
const value = computed({
    get: () => props.modelValue,
    set: (val: string[]) => emits('update:modelValue', val)
});

const remove = (tag: string) => {
    const index = value.value.indexOf(tag);
    if (index === -1) return;

    let values = [...value.value];

    const [removed] = values.splice(index, 1);
    value.value = [...values];
    emits('removed', removed);
};

const add = () => {
    if (!newItem.value) return;

    let input = newItem.value;
    if (value.value.includes(input)) return;

    emits('added', input);
    value.value = [...value.value, input];
    newItem.value = '';
}
</script>

<style lang="scss" scoped>

</style>
