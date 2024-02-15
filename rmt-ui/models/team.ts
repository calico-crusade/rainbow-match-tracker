import type { DbObject } from "./base";

export interface Team extends DbObject {
    name: string;
    code: string;
    url?: string;
    image?: string;
}