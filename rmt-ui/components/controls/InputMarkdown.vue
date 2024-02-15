<template>
    <div
        class="input-group"
        :class="aClasses"
    >
        <div class="input-header control fill no-top group center-items">
            <input
                class="fill"
                :type="type"
                :placeholder="placeholder"
                v-model="search"
                :min="min"
                :max="max"
                :step="step"
                readonly
                @keyup.enter="doSearch()"
            />
            <IconBtn icon="close" inline icon-size="16px" @click="() => search = ''" :disabled="isTrue(inputDisabled)" />
            <IconBtn icon="content_paste" inline icon-size="16px" @click="fromClipboard" :disabled="isTrue(inputDisabled)" />
            <IconBtn icon="edit" inline icon-size="16px" @click="showPopup = !showPopup" :disabled="isTrue(inputDisabled)" />
            <div class="sep" />
            <slot name="input" />

            <IconBtn v-if="isDrawer" :icon="openIcon" inline @click="() => open = !open" />
            <IconBtn v-if="icon" :icon="icon" inline :link="link" @click="doSearch(true)" :disabled="isDisabled" />
        </div>
        <div class="input-drawer">
            <div class="drawer-content">
                <slot />
            </div>
        </div>

        <Popup
            :open="showPopup"
            :title="placeholder || 'Edit Markdown Content'"
            @close="showPopup = !showPopup"
            size="lg-mw"
        >
            <div class="popup-content fill flex">
                <Tabs>
                    <Tab title="Edit" icon="edit" keep-alive>
                        <div class="fill-parent flex row">
                            <textarea class="fill" v-model="search" />
                        </div>
                    </Tab>
                    <Tab title="Preview" icon="preview" scrollable>
                        <Markdown :content="search" styled />
                    </Tab>
                </Tabs>
            </div>
        </Popup>
    </div>
</template>

<script setup lang="ts">
import type { booleanishext } from '~/models';
const slots = useSlots();
const { isTrue } = useUtils();

const props = withDefaults(defineProps<{
    placeholder?: string,
    modelValue?: string,
    link?: string,
    icon?: string,
    openIcon?: string,
    stuck?: booleanishext,
    disabled?: booleanishext,
    inputDisabled?: booleanishext,
    disableDrawer?: booleanishext,
    type?: string,
    classes?: string | string[],
    min?: number | string,
    max?: number | string,
    step?: number | string,
}>(), {
    icon: 'save',
    openIcon: 'tune',
    type: 'text'
});

const showPopup = ref(false);
const isDrawer = computed(() => !!slots['default'] && !isTrue(props.disableDrawer));
const isStuck = computed(() => isTrue(props.stuck));
const isDisabled = computed(() => isTrue(props.disabled));

const emits = defineEmits<{
    (e: 'update:modelValue', v: string | undefined): void;
    (e: 'search'): void;
    (e: 'open', v: boolean): void;
}>();
const _open = ref(false);

const open = computed({
    get: () => isDrawer.value && _open.value,
    set: (value: boolean) => {
        _open.value = value
        emits('open', value);
    }
});

const search = computed({
    get: () => props.modelValue,
    set: (value: string | undefined) => emits('update:modelValue', value)
});

const fromClipboard = async () => {
    const data = await navigator.clipboard.readText();
    if (data) search.value = data;
}

const doSearch = (trigger?: boolean) => {
    if (props.link && !trigger) navigateTo(props.link);
    emits('search');
}

const aClasses = computed(() => {
    const classes: { [key: string]: boolean } = {
        'open': open.value,
        'stuck': isStuck.value
    };

    if (!props.classes) return classes;

    if (typeof props.classes === 'string') {
        props.classes.split(' ').forEach(c => classes[c] = true);
        return classes;
    }

    props.classes.forEach(c => classes[c] = true);
    return classes;
});
</script>

<style lang="scss" scoped>
.input-group {
    background-color: var(--bg-color-accent);
    border: 1px solid var(--bg-color-accent);
    border-radius: calc(var(--brd-radius) * 2);
    overflow: hidden;

    .input-header {
        background-color: transparent !important;
        border-color: transparent !important;
        margin-top: 0;

        button,
        a {
            margin: auto 5px !important;
        }

        .select-styled {
            background-color: transparent;
            border-color: transparent;
            min-width: 100px;
        }
    }

    .input-drawer {
        overflow: hidden;
        max-height: 0;
        transition: all 250ms;
        padding: 0;
        border: 1px solid transparent;
        position: relative;

        .drawer-content {
            overflow-x: hidden;
            overflow-y: auto;
            max-height: calc(80vh - 20px);
            padding: 10px;
        }
    }

    &:focus-within {
        border-color: var(--color-primary);
    }

    &.stuck {
        background-color: var(--bg-color-accent-dark);
    }

    &.open .input-drawer {
        max-height: 80vh;
        border-top-color: var(--bg-color-accent);
    }
}
</style>
