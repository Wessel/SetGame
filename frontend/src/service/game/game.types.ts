export type GameResponse = {
  deck: number[];
  hand: number[];
  found: number[];
  startedAt: Date;
  finishedAt?: Date;
  fails: number;
  hints: number;
  id: number;
}

export type SetCheckResponse = {
  isSet: boolean;
  newState: GameResponse;
}

export type HintResponse = number[];
