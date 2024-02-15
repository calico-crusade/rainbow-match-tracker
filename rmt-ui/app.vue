<template>
    <NuxtLayout>
        <NuxtPage />
    </NuxtLayout>
</template>

<script setup lang="ts">
const { accessibilityMode } = useSettingsHelper();

// onMounted(() => nextTick(() => setTimeout(async () => {

// }, 100)));

const ACCESS_STYLES = [
    {
        tag: '--color-muted',
        normal: '#555',
        access: '#fff'
    }, {
        tag: '--color',
        normal: '#dcddde',
        access: '#fff'
    }, {
        tag: '--font-size',
        normal: '14px',
        access: '16px'
    }
]

const setAccessibility = (value: boolean) => {
    if (!process.client) return;

    for(let style of ACCESS_STYLES) {
        document.documentElement.style.setProperty(style.tag, style[value ? 'access' : 'normal']);
    }
}

useSeoMeta({
    title: 'R6 Match Tracker',
    ogTitle: 'R6 Match Tracker',
    description: 'Rainbow Six Siege match tracker',
    ogDescription: 'Rainbow Six Siege match tracker',
    keywords: 'Rainbow Six Siege, R6, Match Tracker, R6 Match Tracker, R6MT',
    ogImageUrl: '/logos/logo.png',

})

watch(() => accessibilityMode.value, setAccessibility, { immediate: true });
</script>
