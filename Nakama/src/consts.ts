
const TickRate = 16;
const DurationLobby = 10;
const DurationRoundResults = 10;
const DurationBattleEnding = 3;
const NecessaryWins = 3;
const MaxPlayers = 4;
const PlayerNotFound = -1;

const MessagesLogic: { [opCode: number]: (message: nkruntime.MatchMessage, state: GameState, dispatcher: nkruntime.MatchDispatcher) => void } =
{
    3: playerWon,
    4: draw
}
