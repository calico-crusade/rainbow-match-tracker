<template>
    <div class="pager flex pad-left" :class="classes">
        <p class="center-vert margin-right mute">
            {{ start }} - {{ end }} of {{ actTotal }}
        </p>
        <IconBtn
            :disabled="1 == actPage || actTotal == 0"
            icon="keyboard_double_arrow_left"
            :link="link(1)"
            @click="goNext(1)"
            no-boarder
        />
        <IconBtn
            icon="chevron_left"
            :disabled="actPage <= 1 || actTotal == 0"
            :link="link(actPage - 1)"
            @click="goNext(actPage - 1)"
            no-boarder
        />
        <IconBtn
            v-for="i in options"
            icon=""
            :text="i.toString()"
            :link="link(i)"
            @click="goNext(i)"
            :no-boarder="i != actPage"
            :other-classes="i == actPage ? 'router-link-active' : ''"
        />
        <IconBtn
            icon="chevron_right"
            :disabled="actPage >= actPages || actTotal == 0"
            :link="link(actPage + 1)"
            @click="goNext(actPage + 1)"
            no-boarder
        />
        <IconBtn
            :disabled="actPages == actPage || actTotal == 0"
            icon="keyboard_double_arrow_right"
            :link="link(actPages)"
            @click="goNext(actPages)"
            no-boarder
        />
    </div>
</template>

<script setup lang="ts">
import type { ApiResultData, ClassOptions, PaginatedResult, booleanish } from '~/models';

const PAGED_BUTTONS = 2;
const { pageSize } = useSettingsHelper();
const route = useRoute();
const { serClasses, isTrue } = useUtils();

const props = defineProps<{
    page?: number;
    pages?: number;
    size?: number;
    total?: number;
    url?: string;
    params?: {
        [key: string]: any;
    },
    result?: PaginatedResult<any> | ApiResultData<PaginatedResult<any>> | null;
    noTopMargin?: booleanish;
    'class'?: ClassOptions;
}>();

const emits = defineEmits<{
    (e: 'load-page', v: number): void;
}>();

const actPage = ref(1);
const actSize = ref(20);

const actPages = ref(0);
const actTotal = ref(0);
const start = ref(0);
const end = ref(0);
const options = ref<number[]>([]);

const classes = computed(() => serClasses({
    'margin-top': !isTrue(props.noTopMargin)
}, props.class));

const link = (page: number) => {
    if (!props.url) return undefined;

    const params = props.params ?? {};
    params.page = page;
    return `${props.url}?${Object.entries(params).map(([k, v]) => `${k}=${v}`).join('&')}`;
}

const goNext = (page: number) => {
    emits('load-page', page);
}

watch(() => props, () => {

    const triggers = [
        () => { actPage.value = props.page ?? +(route.query.page?.toString() ?? '1'); },
        () => { actSize.value = props.size ?? pageSize.value; },
        () => {
            if (props.total) {
                actTotal.value = props.total;
                return;
            }

            if (!props.result) {
                actTotal.value = 0;
                return;
            }

            const res = 'data' in props.result ? props.result.data : props.result;
            actTotal.value = res.count;
        },
        () => {
            if (props.pages) {
                actPages.value = props.pages;
                return;
            }

            if (!props.result) {
                actPages.value = 0;
                return;
            }

            const res = 'data' in props.result ? props.result.data : props.result;
            actPages.value = res.pages;
        },
        () => {
            start.value = Math.max((actPage.value - 1) * actSize.value, 1);
            end.value = Math.min(actPage.value * actSize.value, actTotal.value);
            options.value = Array(actPages.value)
                .fill(0)
                .map((_, i) => i + 1)
                .filter(i =>
                    i >= actPage.value - PAGED_BUTTONS &&
                    i <= actPage.value + PAGED_BUTTONS
                );
        }
    ];

    triggers.forEach(t => t());

}, { immediate: true, deep: true });
</script>

<style scoped lang="scss">

</style>
