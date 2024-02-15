<template>
    <div class="stage-controls fill-parent" :class="classes">
        <div class="stage-header flex">
            <div class="stage-buttons center-vert">
                <IconBtn
                    :disabled="!previous"
                    :icon="prevBtn.icon"
                    :text="prevBtn.text"
                    :color="prevBtn.color"
                    v-show="!prevBtn.hide"
                    title="Prevous Step"
                    @click="stageBack()"
                />
            </div>
            <div class="stage-crumbs center flex" v-if="!isTrue(noCrumbs) && current">
                <Icon>{{ current.props.icon }}</Icon>
                <p class="center-vert margin-left">
                    {{ current.props.title }}
                </p>
                <p class="center-vert margin-left mute">
                    ({{ currentIndex + 1 }} / {{ ordered.length }})
                </p>
            </div>
            <div class="stage-buttons flex">
                <IconBtn
                    v-if="next && current"
                    v-show="!nextBtn.hide"
                    :disabled="!current.props.valid"
                    :icon="nextBtn.icon"
                    :text="nextBtn.text"
                    :color="nextBtn.color"
                    :title="!current?.props.valid ? (current.props.disableMessage ?? 'Requirement missing') : 'Next Step'"
                    class="center-vert"
                    @click="stageNext()"
                />
                <IconBtn
                    v-else
                    v-show="!okBtn.hide"
                    :disabled="!next && current && !current.props.valid"
                    :icon="okBtn.icon"
                    :text="okBtn.text"
                    :color="okBtn.color"
                    :title="!next && current && current.props.valid ? (current.props.disableMessage ?? 'Requirement missing') : 'Ok'"
                    class="center-vert"
                    @click="stageOk()"
                />
            </div>
        </div>
        <div class="stage-content">
            <slot />
        </div>
    </div>
</template>

<script lang="ts" setup>
import type { booleanish, ClassOptions, StageData } from '~/models';

type ButtonDets = {
    icon?: string;
    text?: string;
    color?: 'primary' | 'secondary' | 'danger' | 'warning' | 'shade' | 'inline' | 'success';
    hide?: booleanish;
};

const { serClasses, isTrue } = useUtils();
const {
    moveNext, moveBack,
    next, previous, current, ordered, currentIndex
} = useStages();

const props = defineProps<{
    noMargins?: booleanish,
    'class'?: ClassOptions,
    noCrumbs?: booleanish,
    next?: ButtonDets,
    prev?: ButtonDets,
    ok?: ButtonDets
}>();

const emits = defineEmits<{
    (e: 'stage', v: StageData): void;
    (e: 'ok'): void;
}>();

const nextBtn = computed<ButtonDets>(() => props.next || { icon: 'arrow_forward' });
const prevBtn = computed<ButtonDets>(() => props.prev || { icon: 'arrow_back' });
const okBtn = computed<ButtonDets>(() => props.ok || { icon: 'check_circle', text: 'Submit', color: 'primary' });

const classes = computed(() => serClasses(props.class, {
    'no-margins': isTrue(props.noMargins),
}));

const stageOk = () => {
    emits('ok');
}

const stageNext = () => {
    const value = moveNext();
    if (!value) return;
    emits('stage', value);
}

const stageBack = () => {
    const value = moveBack();
    if (!value) return;
    emits('stage', value);
}
</script>

<style lang="scss">
$margin: 0.75rem;
$transition: 150ms;
$background: var(--bg-color-accent);
$darkbackground: var(--bg-color);

.stage-controls {
    position: relative;
    height: auto;
    min-width: 100%;
    min-height: 100%;
    overflow: hidden;
    display: flex;
    flex-flow: column;

    .stage-header {
        display: flex;
        flex-flow: row;
        margin: #{$margin} #{$margin} 0 #{$margin};
        overflow: hidden;

        .stage-crumbs {
            overflow-y: hidden;
            overflow-x: auto;
        }
    }

    .stage-content {
        position: relative;
        flex: 1;
        overflow: hidden;
        margin: 0 #{$margin} #{$margin} #{$margin};

        .stage {
            position: absolute;
            top: 0;
            left: 0;
            opacity: 0;
            z-index: -1;
            transition: opacity #{$transition};
            width: 100%;
            height: 100%;
            max-height: 100%;
            max-width: 100%;
            overflow: hidden;
            background-color: $background;
            border-radius: $margin;
            display: flex;

            &.active {
                opacity: 1;
                z-index: 0;
            }

            &.dark {
                background-color: $darkbackground;
            }

            &:first-child {
                border-top-left-radius: 0;
            }

            &:last-child {
                border-top-right-radius: 0;
            }

            .stage-panel {
                margin: $margin;
                flex: 1;
                overflow: hidden;

                &.scroll {
                    overflow-y: auto;
                }
            }
        }
    }

    &.no-margins {
        .stage-header {
            margin: 0;
        }

        .stage-content {
            margin: 0;
        }
    }
}
</style>
