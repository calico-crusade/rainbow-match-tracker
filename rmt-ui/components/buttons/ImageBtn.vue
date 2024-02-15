<template>
    <NuxtLink
        v-if="link && !isTrue(external)"
        :to="!isTrue(isDisabled) ? link : undefined"
        :active-class="active"
        class="btn"
        :class="classes"
        @click="() => $emit('click')"
        :style="styles"
        :title="text"
    >
        <div
            class="image-wrapper"
            :class="{
                'unsize': isTrue(unsize),
                'spin': isSpinning,
                'flip': isTrue(flip)
            }"
            :style="{
                'max-width': acIconSize,
                'max-height': acIconSize,
                'width': acIconSize,
                'height': acIconSize,
            }"
        >
            <Image
                :src="src"
                :style="{
                    transform: `rotate(${rotate || 0}deg) scaleX(${isTrue(flip) ? -1 : 1})`,
                    'animation-duration': speed,
                }"
            />
        </div>
        <p v-if="text">{{ text }}</p>
    </NuxtLink>
    <a
        v-else-if="link"
        target="_blank"
        :href="!isTrue(isDisabled) ? link : undefined"
        class="btn"
        :class="classes"
        @click="() => $emit('click')"
        :style="styles"
        :title="text"
    >
        <div
            class="image-wrapper"
            :class="{
                'unsize': isTrue(unsize),
                'spin': isSpinning,
                'flip': isTrue(flip)
            }"
            :style="{
                'max-width': acIconSize,
                'max-height': acIconSize,
                'width': acIconSize,
                'height': acIconSize,
            }"
        >
            <Image
                :src="src"
                :style="{
                    transform: `rotate(${rotate || 0}deg) scaleX(${isTrue(flip) ? -1 : 1})`,
                    'animation-duration': speed,
                }"
            />
        </div>
        <p v-if="text">{{ text }}</p>
    </a>
    <button
        v-else class="btn"
        :class="classes"
        @click="() => $emit('click')"
        :style="styles"
        :disabled="isTrue(isDisabled)"
        :title="text"
    >
        <div
            class="image-wrapper"
            :class="{
                'unsize': isTrue(unsize),
                'spin': isSpinning,
                'flip': isTrue(flip)
            }"
            :style="{
                'max-width': acIconSize,
                'max-height': acIconSize,
                'width': acIconSize,
                'height': acIconSize,
            }"
        >
            <Image
                :src="src"
                :style="{
                    transform: `rotate(${rotate || 0}deg) scaleX(${isTrue(flip) ? -1 : 1})`,
                    'animation-duration': speed,
                }"
            />
        </div>
        <p v-if="text">{{ text }}</p>
    </button>
</template>

<script setup lang="ts">
import type { booleanish } from '~/models';
const { isTrue } = useUtils();

const props = withDefaults(defineProps<{
    text?: string;
    src: string;
    color?: 'primary' | 'secondary' | 'danger' | 'warning' | 'shade' | 'inline';
    spin?: booleanish;
    size?: string;
    iconSize?: string;
    speed?: string;
    rotate?: number;
    unsize?: booleanish;
    link?: string;
    external?: booleanish;
    active?: string;
    disabled?: booleanish;
    padLeft?: booleanish;
    breakpoint?: booleanish;
    loading?: booleanish;
    inline?: booleanish;
    otherClasses?: string | string[];
    flip?: booleanish;
    rounded?: booleanish;
}>(), {
    size: '16px',
    iconSize: '24px'
});

defineEmits<{
    (e: 'click'): void
}>();

const isSpinning = computed(() => isTrue(props.spin || props.loading));
const isDisabled = computed(() => isTrue(props.disabled || props.loading));
const acIconSize = computed(() => props.iconSize);
const acColor = computed(() => isTrue(props.inline) ? 'inline' : props.color);

const classes = computed(() => [
    acColor.value,
    isTrue(props.inline) ? 'inline' : '',
    props.text ? 'has-text' : 'no-text',
    isDisabled.value ? 'disabled' : '',
    isTrue(props.padLeft) ? 'pad-left' : '',
    isTrue(props.breakpoint) ? 'breakpoint' : '',
    isTrue(props.rounded) ? 'rounded' : '',
    ...(typeof props.otherClasses === 'string' ? [props.otherClasses ?? ''] : props.otherClasses ?? [])
].join(' '));

const styles = computed(() => {
    const styles: { [key: string]: string } = {
        'font-size': props.size
    };

    if (!props.link) styles['--icon-size'] = props.iconSize;

    return styles;
});
</script>

<style lang="scss" scoped>
$icon-padding: 10px;
$icon-height: calc(var(--icon-size) + #{$icon-padding * 2} + 2px);

.btn {
    background-color: transparent;
    border: 1px solid transparent;
    display: inline-flex;
    flex-flow: row;
    align-items: center;
    border-radius: var(--brd-radius);
    padding: 5px;
    height: auto;

    .image-wrapper {
        display: flex;

        &.unsize {
            height: unset;
            width: unset;
        }

        &.spin { animation: spin 10s linear infinite; }

        &.flip { transform: scaleX(-1); }

        &.spin.flip {
            animation-name: spinflip;
        }

        img {
            max-width: 100%;
            max-height: 100%;
            margin: auto;
        }
    }

    p {
        margin-left: 5px;
        margin-right: 5px;
    }

    &.no-text {
        padding: $icon-padding;
        height: $icon-height;
    }

    &.has-text {
        padding: 5px 10px;
    }

    &.primary,
    &.secondary,
    &.danger,
    &.warning,
    &.shade {
        border-color: var(--bg-color-accent);
        background-color: var(--color-primary);
    }

    &.secondary {
        background-color: var(--color-secondary);
    }

    &.danger {
        background-color: var(--color-warning);
    }

    &.warning {
        background-color: var(--color-actual-warning);
    }

    &.shade {
        background-color: var(--bg-color-accent);
        border-color: transparent;
    }

    &.inline {
        background-color: transparent;
        margin: 0;
        padding: 0;
    }

    &:hover:not(.disabled) {
        cursor: pointer;

        &:not(.inline) {
            background-color: var(--bg-color-accent);
        }

        &.shade {
            background-color: var(--bg-color-accent-dark);
        }
    }

    &.breakpoint {
        padding: $icon-padding;
        height: $icon-height;

        p {
            display: none;
            margin: 0;
        }
    }

    &.active {
        border-color: var(--color-primary);
    }

    &.rounded {
        img {
            border-radius: 50%;
        }
    }
}

@media only screen and (max-width: 400px) {

    .breakpoint.has-text {
        flex: 1;
        padding: 5px 10px;
        height: unset;

        p {
            display: block;
            margin-left: 5px;
            margin-right: 5px;
        }
    }
}
</style>
