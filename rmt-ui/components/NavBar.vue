<template>
    <header class="flex">
        <Image src="/logos/logo.png" class="center-vert" height="45px" />
        <NuxtLink to="/" class="center-vert margin-left pad-right">R6 Match Tracker</NuxtLink>

        <IconBtn
            icon="menu"
            class="center-vert"
            @click="toggle"
        />
    </header>

    <div class="fade" :class="{'open': !closed}" @click="toggle" />

    <aside class="flex row scroll-y" :class="{'open': !closed}">
        <section class="title flex">
            <Image src="/logos/logo.png" alt="Logo" class="logo" />
        </section>

        <section class="links flex row">
            <IconBtn icon="person" text="Teams" link="/teams" @click="toggle" />
            <IconBtn icon="groups" text="Leagues" link="/leagues" @click="toggle" />
            <IconBtn icon="sports_esports" text="Matches" link="/matches" @click="toggle" />
        </section>

        <span class="fill" />

        <section class="links flex row">
            <ImageBtn src="/logos/discord-logo.svg" external text="Discord" link="https://discord.gg/RV9MvvYXsp" />
            <CheckBox class="center-horz" v-model="accessibilityMode" label="Accessibility Mode" />
        </section>
    </aside>
</template>

<script setup lang="ts">
const { accessibilityMode } = useSettingsHelper();

const closed = ref(true);
const toggle = () => closed.value = !closed.value;

</script>

<style lang="scss" scoped>
$navheight: 50px;
$asidewidth: 300px;

header {
    height: $navheight;
    max-height: $navheight;
    background-color: var(--bg-color-accent-dark);

    .logo {
        transition: all 250ms;
        a {
            display: flex;
            height: #{ $navheight * 1.5 };
            width: #{ $navheight * 1.5 };
            background: var(--bg-image);
            border-bottom-left-radius: 50%;
            border-bottom-right-radius: 50%;
            transform: translateY(-25%);
            transition: all 250ms;
            position: relative;
            z-index: 5;

            img {
                margin: auto;
                height: $navheight;
                width: $navheight;
                transform: translateY(12.5%);
                transition: all 250ms;
            }

            span {
                position: absolute;
                color: white;
                bottom: 20px;
                right: 8px;
                font-size: 12px;
            }

            &:hover {
                transform: translateY(0);
            }
        }
    }

    .links {
        display: flex;
        flex-flow: row;

        a, button {
            margin: auto 5px;
        }

        &.internal {
            justify-content: flex-end;
        }
    }

    &.after-dark {

        .links.internal,
        .links.external {
            display: none;
        }

        .logo {
            a {
                height: $navheight;
                border-radius: 0;
                transform: translateY(0);
                background: none;

                img {
                    transform: translateY(0);
                }

                span {
                    bottom: 10px;
                }
            }
        }

        .links:first-child {
            margin-right: 0 !important;
        }
    }
}

.fade {
    position: fixed;
    top: 0;
    left: 0;
    width: var(--full-width);
    height: var(--full-height);
    background-color: var(--bg-color-accent-dark);
    z-index: -1;
    opacity: 0;
    transition: all 250ms;
    cursor: pointer;

    &.open {
        z-index: 5;
        opacity: 1;
    }
}

aside {
    position: fixed;
    top: 0;
    right: #{ -$asidewidth };
    height: var(--full-height);
    width: $asidewidth;
    max-width: 90vw;
    background-color: var(--bg-color);
    z-index: 6;
    transition: all 250ms;

    &.open {
        right: 0;
    }

    section {
        margin: 10px;

        &.title {
            img {
                height: 100px;
                width: 100px;
                margin: auto;
            }
        }

        &.links {
            flex-flow: column;

            a, button {
                margin: 5px 0;
            }

            hr {
                width: 100%;
            }
        }
    }
}

@media only screen and (max-width: 1050px) {
    header {
        .links.internal,
        .links.external {
            display: none;
        }
    }
}
</style>
