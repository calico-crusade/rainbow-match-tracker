import { Observable, Subscription } from 'rxjs';

class Subscriptions {
    private _subs: Subscription[] = [];

    subscribe<T>(ob: Observable<T>, next: (value: T) => void, error?: (error: any) => void, complete?: () => void): Subscriptions {
        this._subs.push(ob.subscribe({
            next: next,
            error: error,
            complete: complete
        }));
        return this;
    }

    unsubscribe(): void {
        this._subs.forEach(s => s.unsubscribe());
    }
}

export const useRxjs = (): Subscriptions => {
    return new Subscriptions();
};
