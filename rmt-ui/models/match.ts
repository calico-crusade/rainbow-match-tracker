import type { DbObject } from "./base";
import type { League } from "./league";
import type { Team } from "./team";

export enum MatchStatus {
    Unknown = 0,
    TeamOneWon = 1,
    TeamTwoWon = 2,
    Draw = 3,
    Active = 4,
    Upcoming = 5
}

export interface Match extends DbObject {
    hash: string;
    teams: {
        id: string;
        score: number;
    }[];
    leagueId: string;
    status: MatchStatus;
    startTime: Date;
    bestOf: number;
    lastBatchTime: Date;
}

export interface ExtendedMatch  {
    match: Match;
    league: League;
    teams: Team[];
}