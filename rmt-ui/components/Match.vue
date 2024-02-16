<template>
    <NuxtLink
        class="card match flex"
        :class="classes"
        @click="() => $emit('click', match)"
        :to="actLink"
        :target="isExternal(actLink) ? '_blank' : undefined"
    >
        <Placeholder v-if="loading || !match" height="85px" width="100%" round="var(--brd-radius)" />
        <template v-else-if="!isCompact">
            <div class="match-details flex">
                <div class="fill flex center-vert">
                    <NuxtLink
                        class="pad-left center-vert"
                        :to="`/leagues/${match.league.id}`"
                    >
                        <span>{{ match.league.display }}</span>
                    </NuxtLink>
                    <Image class="margin-left" shadow :src="match.league.image" :alt="match.league.display" size="25px" fit="contain" />
                </div>
                <div class="sep margin-left margin-right"></div>
                <div class="fill flex center-vert">
                    <div class="data">
                        <Date :value="match.match.startTime" format="r" utc />
                        <span>&nbsp;- BO{{ match.match.bestOf }}</span>
                    </div>
                </div>
            </div>
            <div class="team-details flex">
                <template v-for="(team, index) in teams">
                    <NuxtLink
                        class="team flex center-vert"
                        :class="{
                            'left': index % 2 === 0,
                            'right': index % 2 !== 0,
                            'won': team.type === 'won',
                            'lost': team.type === 'lost',
                            'draw': team.type === 'draw',
                            'active': team.type === 'active',
                        }"
                        :to="`/teams/${team.id}`"
                    >
                        <div class="background" />
                        <Image :src="team.image" :alt="team.name" size="50px" fit="contain" shadow />
                        <h3 class="center-vert name">{{ team.name }}</h3>
                        <p class="center-vert code">{{ team.code }}</p>
                    </NuxtLink>
                    <div
                        v-if="index % 2 === 0"
                        class="vs"
                    >
                        <div class="line">
                            <Image src="line.svg" alt="vs" size="50px" />
                        </div>
                        <span
                            class="text"
                            v-if="match.match.status === MatchStatus.Upcoming ||
                                match.match.status === MatchStatus.Active">VS</span>
                        <span
                            class="text"
                            v-else>
                            {{ teams[0].score }}:{{ teams[1].score }}
                        </span>
                    </div>
                </template>
            </div>
        </template>
        <template v-else>

        </template>
    </NuxtLink>
</template>

<script setup lang="ts">
import { type ExtendedMatch, type Match, type booleanish, type ClassOptions, MatchStatus, type Team } from '~/models';

const { toPromise } = useApiHelper();
const { serClasses, isTrue, isExternal } = useUtils();
const { v1: api } = useRmtApi();

const props = defineProps<{
    id?: string;
    match?: ExtendedMatch | Match;
    link?: string | '' | boolean;
    compact?: booleanish;
    noTopMargin?: booleanish;
    hover?: booleanish;
    noRounded?: booleanish;
    noBorder?: booleanish;
    'class'?: ClassOptions;
}>();

const emit = defineEmits<{
    (e: 'click', v?: ExtendedMatch): void;
}>();

const loading = ref(false);
const match = ref<ExtendedMatch>();
const isCompact = computed(() => isTrue(props.compact));

const teams = computed(() => {
    if (!match.value) return [];

    const [team1, team2] = match.value.teams;
    const scores = match.value.match.teams;

    const status = match.value.match.status;
    const determineType = (team: Team) => {
        const score = scores.find(t => t.id === team.id)?.score ?? 0;
        const other = scores.find(t => t.id !== team.id)?.score ?? 0;

        switch(status) {
            case MatchStatus.TeamTwoWon:
            case MatchStatus.TeamOneWon: return { type: score > other ? 'won' : 'lost', score };
            case MatchStatus.Draw: return { type: 'draw', score };
            case MatchStatus.Active: return { type: 'active', score };
            case MatchStatus.Upcoming: return { type: 'upcoming', score };
            case MatchStatus.Unknown: return { type: 'unknown', score };
        }
    }

    return [
        { ...team1, ...determineType(team1) },
        { ...team2, ...determineType(team2) },
    ];
});

const classes = computed(() => serClasses(props.class, {
    'margin-top': !isTrue(props.noTopMargin),
    'hover': isTrue(props.hover),
    'rounded': !isTrue(props.noRounded),
    'no-border': isTrue(props.noBorder),
    'compact': isCompact.value,
    'row': !isCompact.value,
}));

const actLink = computed(() => {
    if (props.link === true || props.link === '')
        return `/matches/${match.value?.match.id}`;

    if (props.link === false) return '';

    return props.link || '';
});

const marry = async () => {

    if (props.match && 'match' in props.match) {
        match.value = props.match;
        return;
    }

    const id = props.id ?? props.match?.id;

    if (!id ||
        (match.value && match.value.match.id === id))
        return;

    if (!process.client) return;

    loading.value = true;
    match.value = (await toPromise(api.matches.get(id)))?.data;
    loading.value = false;
}

watch(() => props, () => marry(), { immediate: true, deep: true });
</script>

<style lang="scss">
@mixin back-fade($color, $left: false) {
    @if $left {
        background-image: linear-gradient(90deg, transparent, #{$color});
    } @else {
        background-image: linear-gradient(90deg, #{$color}, transparent);
    }
}


.match {
    .match-details {
        .fill {
            max-width: calc(50% - var(--margin));
            a, .data {
                text-align: right;
                white-space: nowrap;
                overflow: hidden;
                text-overflow: ellipsis;
            }
        }


        .sep {
            height: 25px;
            border: 1px solid var(--color);
        }
    }

    .team-details {
        margin-top: 5px;
        .team {
            flex: 1;
            position: relative;

            img {
                margin-left: 5px;
                margin-right: 5px;
            }

            .code {
                display: none;
            }

            .background {
                position: absolute;
                top: 50%;
                left: 0;
                width: 100%;
                height: 90%;
                transform: translateY(-50%);
                z-index: -1;
            }

            &.left {
                flex-direction: row-reverse;
                .background {
                    @include back-fade(#000, $left: true);
                }

                &.won {
                    .background {
                        @include back-fade(green, $left: true);
                    }
                }
                &.lost {
                    .background {
                        @include back-fade(red, $left: true);
                    }
                }
                &.draw {
                    .background {
                        @include back-fade(#383838, $left: true);
                    }
                }
            }

            &.right {
                .background {
                    @include back-fade(#000, $left: false);
                }

                &.won {
                    .background {
                        @include back-fade(green, $left: false);
                    }
                }
                &.lost {
                    .background {
                        @include back-fade(red, $left: false);
                    }
                }
                &.draw {
                    .background {
                        @include back-fade(#383838, $left: false);
                    }
                }
            }
        }

        .vs {
            width: 50px;
            height: 50px;
            position: relative;
            margin: 0 5px;

            .text {
                position: absolute;
                font-size: 14px;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
            }
        }
    }
}

@media only screen and (max-width: 650px) {
    .match {
        .team-details {
            position: relative;
            flex-flow: column;
            .team {
                flex-direction: row-reverse !important;

                .name { display: none; }
                .code {
                    display: block;
                    margin-right: auto;
                    margin-left: 5px;
                }

                .background {
                    height: 100%;
                }
            }

            .vs {
                position: absolute;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);

                img { display: none; }
            }
        }
    }
}
</style>
