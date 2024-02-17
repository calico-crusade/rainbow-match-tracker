import type { DbObject } from "./base";

export interface League extends DbObject {
    name: string;
    display: string;
    url?: string;
    image?: string;
}
