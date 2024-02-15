<template>
    <div
        class="input-group"
        :class="aClasses"
    >
        <div
            class="input-header control fill flex no-top group center-items"
            :class="{ 'ov-visible': type === 'autocomplete' }"
        >
            <template v-if="type === 'file'">
                <input
                    hidden="true"
                    type="file"
                    @change="handleFileInputChange($event)"
                    ref="fileInput"
                    :accept="accept"
                    :multiple="false"
                />
                <input
                    type="text"
                    :value="fileData?.name ?? placeholder ?? 'Searh for a file...'"
                    readonly
                    class="fill"
                    @click="handleFileClick()"
                />
                <IconBtn icon="close" inline icon-size="16px" @click="search = undefined" title="Cancel Upload" :disabled="isTrue(inputDisabled) || !search" />
                <IconBtn icon="folder_open" inline icon-size="16px" @click="handleFileClick()" title="Upload file" :disabled="isTrue(inputDisabled)" />
            </template>
            <template v-else-if="type === 'color'">
                <input
                    class="hidden"
                    type="color"
                    v-model="search"
                    ref="colorInput"
                />
                <p class="center-vert fill color-picker" :style="{ color: <any>search }" @click="handleColorClick">
                    {{ placeholder }}! Click here to select a color!
                </p>
                <IconBtn class="hide-sm" icon="close" inline icon-size="16px" @click="() => search = ''" :disabled="isTrue(inputDisabled)" />
                <IconBtn class="hide-sm" icon="content_paste" inline icon-size="16px" @click="fromClipboard" :disabled="isTrue(inputDisabled)" />
            </template>
            <template v-else>
                <AutoComplete
                    v-if="type === 'autocomplete'"
                    transparent
                    :placeholder="placeholder"
                    v-model="search"
                    :options="options"
                    :split-char="splitChar"
                    :disabled="isTrue(inputDisabled)"
                    @selected="doSearch()"
                    class="fill ov-visible"
                />
                <input
                    v-else
                    class="fill"
                    :type="type"
                    :placeholder="placeholder"
                    v-model="search"
                    :min="min"
                    :max="max"
                    :step="step"
                    :disabled="isTrue(inputDisabled)"
                    @keyup.enter="doSearch()"
                />
                <IconBtn class="hide-sm" icon="close" inline icon-size="16px" @click="() => search = ''" :disabled="isTrue(inputDisabled)" />
                <IconBtn class="hide-sm" icon="content_paste" inline icon-size="16px" @click="fromClipboard" :disabled="isTrue(inputDisabled)" />
            </template>
            <div class="sep" />
            <slot name="input" />

            <IconBtn v-if="isDrawer" :icon="openIcon" inline @click="() => open = !open" />
            <IconBtn v-if="icon" :icon="icon" inline :link="link" @click="doSearch(true)" :disabled="isDisabled" no-boarder />
        </div>
        <div class="input-drawer">
            <div class="drawer-content">
                <slot />
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type { ClassOptions, booleanishext } from '~/models';
const slots = useSlots();
const { serClasses, isTrue } = useUtils();

type InputType = string | number | Date | undefined | File;

const props = withDefaults(defineProps<{
    placeholder?: string,
    modelValue?: InputType,
    link?: string,
    icon?: string,
    openIcon?: string,
    stuck?: booleanishext,
    disabled?: booleanishext,
    inputDisabled?: booleanishext,
    disableDrawer?: booleanishext,
    type?: string,
    classes?: ClassOptions,
    min?: number | string,
    max?: number | string,
    step?: number | string,
    textAreaAutoResize?: booleanishext,
    textAreaMaxHeight?: string,
    textAreaMinHeight?: string,
    options?: string | string[],
    splitChar?: string,
    accept?: string,
}>(), {
    icon: 'search',
    openIcon: 'tune',
    type: 'text'
});

const isDrawer = computed(() => !!slots['default'] && !isTrue(props.disableDrawer));
const isStuck = computed(() => isTrue(props.stuck));
const isDisabled = computed(() => isTrue(props.disabled));
const fileData = computed(() => props.type === 'file' && !!search.value ? <File>search.value : undefined);

const emits = defineEmits<{
    (e: 'update:modelValue', v: InputType): void;
    (e: 'search'): void;
    (e: 'open', v: boolean): void;
}>();
const _open = ref(false);
const fileInput = ref<HTMLInputElement>();
const colorInput = ref<HTMLInputElement>();

const open = computed({
    get: () => isDrawer.value && _open.value,
    set: (value: boolean) => {
        _open.value = value
        emits('open', value);
    }
});

const search = computed({
    get: () => props.modelValue,
    set: (value: InputType) => emits('update:modelValue', value)
});

const fromClipboard = async () => {
    const data = await navigator.clipboard.readText();
    if (data) search.value = data;
}

const doSearch = (trigger?: boolean) => {
    if (props.link && !trigger) navigateTo(props.link);
    emits('search');
}

const aClasses = computed(() => serClasses(props.classes, {
    'open': open.value,
    'stuck': isStuck.value,
    'ov-visible': props.type === 'autocomplete'
}));

const handleFileInputChange = (e: Event) => {
    const target = e.target as HTMLInputElement;
    if (target.files) search.value = target.files[0];
}

const handleFileClick = () => {
    fileInput.value?.click();
}

const handleColorClick = () => {
    colorInput.value?.focus();
    colorInput.value?.click();
}
</script>

<style lang="scss">
.input-group {
    background-color: var(--bg-color-accent);
    border: 1px solid var(--bg-color-accent);
    border-radius: calc(var(--brd-radius) * 2);
    overflow: hidden;
    min-height: 40px;

    .input-header {
        background-color: transparent !important;
        border-color: transparent !important;
        margin-top: 0;
        min-height: 100%;

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

    .hidden {
        width: 1px;
        height: 1px;
        max-width: 1px !important;
        min-width: 1px !important;
        overflow: hidden;
    }
}
</style>
