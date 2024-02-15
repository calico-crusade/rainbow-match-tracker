import type { ClassOptions, booleanish } from "./polyfill";

export interface StageProps {
    title?: string;
    icon?: string;
    ordinal?: number | string;
    valid?: booleanish;
    disableMessage?: string;
}

export interface StagePropsExt extends StageProps {
    scrollable?: booleanish;
    keepAlive?: booleanish;
    dark?: booleanish;
    'class'?: ClassOptions;
}

export interface StageEvents {
    activated: () => void;
    deactivated: () => void;
    close: () => void;
    mounted: () => void;
    unmounted: () => void;
}

export interface StageData {
    identifier: any;
    active: boolean;
    ordinal: number;
    props: StageProps;
    events: StageEvents;
}
