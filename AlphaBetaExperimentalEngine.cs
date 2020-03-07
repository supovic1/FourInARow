using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FourInARow {
    class AlphaBetaExperimentalEngine : AlphaBetaEngine {
        public AlphaBetaExperimentalEngine(int sizeX, int sizeY, int depth, bool isFirstPlayer) : base(sizeX, sizeY, depth, isFirstPlayer) {
        }

        protected override int eval(Game g) {
            List<int[]> tuplets = GetTuplets(g);
            int[] goodPositions = GetGoodPositionCount(g);
            int[] oddEvenBonuses = GetOddEvenBonuses(g);
            int[] positionBonuses = getPositionBonuses(g);
            int getOver = -GetOver(g);

            int result = (tuplets[0][0] - (isFirstPlayer ? 1 : 1) * tuplets[0][1]) * 5 +
                (tuplets[1][0] - (isFirstPlayer ? 10 : 100) * tuplets[1][1]) * 1000 +
                (tuplets[2][0] - (isFirstPlayer ? 10 : 100) * tuplets[2][1]) * 100 +
                (goodPositions[0] - goodPositions[1]) +
                // getOver +
                (oddEvenBonuses[0] - oddEvenBonuses[1]) * 1  +
                (positionBonuses[0] - positionBonuses[1]) * 3;
            return result;
        }

        private int[] GetOddEvenBonuses(Game g) {
            if (g.sizeY % 2 == 1) {
                return new int[] { 0, 0 };
            }
            int[] oddEvenBonuses = new int[2];
            for (int i = 0; i < g.sizeX; i++) {
                for (int j = 0; j < g.sizeY; j++) {
                    if (j % 2 == 0 && g.board[i, j] == true) {
                        oddEvenBonuses[0]++;
                        continue;
                    }
                    if (j % 2 == 1 && g.board[i, j] == true) {
                        oddEvenBonuses[0]--;
                        continue;
                    }
                    if (j % 2 == 0 && g.board[i, j] == false) {
                        oddEvenBonuses[1]++;
                        continue;
                    }
                    if (j % 2 == 1 && g.board[i, j] == false) {
                        oddEvenBonuses[1]--;
                        continue;
                    }
                    //if (j == g.sizeY / 2 - 1 && g.board[i, j] == true) {
                    //   oddEvenBonuses[0]++;
                    //}
                }
            }
            return oddEvenBonuses;
        }

        public List<int[]> GetTuplets(Game g) {
            int[] triplets = new int[2];
            int[] neighboringWinningPositionsCount = new int[2];
            int[] quadruplets = new int[2];
            List<int[]> firstPlayerWiningPosition = new List<int[]>();
            List<int[]> secondPlayerWiningPosition = new List<int[]>();

            for (int s = 0; s < 2; s++) {
                bool side = s % 2 == 0;
                for (int i = 0; i < g.sizeX; i++) {
                    for (int j = 0; j < g.sizeY; j++) {
                        int[] position = null;
                        // y axis
                        bool hasBeenError = false;
                        for (int k = 0; k < 4; k++) {
                            if (j + k >= g.sizeY || g.board[i, j + k] == !side)
                                break;
                            if (g.board[i, j + k] == null && hasBeenError)
                                break;
                            if (g.board[i, j + k] == null)
                                position = new int[] { i, j + k };
                            hasBeenError = true;
                            if (k == 3) {
                                if (hasBeenError) {
                                    if (side) {
                                        firstPlayerWiningPosition.Add(position);
                                    } else {
                                        secondPlayerWiningPosition.Add(position);
                                    }
                                    triplets[side ? 0 : 1]++;
                                } else {
                                    quadruplets[side ? 0 : 1]++;
                                }
                            }
                        }

                        // bottom left to upper right
                        hasBeenError = false;
                        for (int k = 0; k < 4; k++) {
                            if (j + k >= g.sizeY || i + k >= g.sizeX || g.board[i + k, j + k] == !side)
                                break;
                            if (g.board[i + k, j + k] == null && hasBeenError)
                                break;
                            if (g.board[i + k, j + k] == null) {
                                position = new int[] { i + k, j + k };
                                hasBeenError = true;
                            }
                            if (k == 3) {
                                if (hasBeenError) {
                                    if (side) {
                                        firstPlayerWiningPosition.Add(position);
                                    } else {
                                        secondPlayerWiningPosition.Add(position);
                                    }
                                    triplets[side ? 0 : 1]++;
                                } else {
                                    quadruplets[side ? 0 : 1]++;
                                }
                            }
                        }

                        // x axis
                        hasBeenError = false;
                        for (int k = 0; k < 4; k++) {
                            if (i + k >= g.sizeX || g.board[i + k, j] == !side)
                                break;
                            if (g.board[i + k, j] == null && hasBeenError)
                                break;
                            if (g.board[i + k, j] == null) {
                                position = new int[] { i + k, j };
                                hasBeenError = true;
                            }
                            if (k == 3) {
                                if (hasBeenError) {
                                    if (side) {
                                        firstPlayerWiningPosition.Add(position);
                                    } else {
                                        secondPlayerWiningPosition.Add(position);
                                    }
                                    triplets[side ? 0 : 1]++;
                                } else {
                                    quadruplets[side ? 0 : 1]++;
                                }
                            }
                        }

                        // upper left to bottom right
                        hasBeenError = false;
                        for (int k = 0; k < 4; k++) {
                            if (j - k < 0 || i + k >= g.sizeX || g.board[i + k, j - k] == !side)
                                break;
                            if (g.board[i + k, j - k] == null && hasBeenError)
                                break;
                            if (g.board[i + k, j - k] == null) {
                                position = new int[] { i + k, j - k };
                                hasBeenError = true;
                            }
                            if (k == 3) {
                                if (hasBeenError) {
                                    if (side) {
                                        firstPlayerWiningPosition.Add(position);
                                    } else {
                                        secondPlayerWiningPosition.Add(position);
                                    }
                                    triplets[side ? 0 : 1]++;
                                } else {
                                    quadruplets[side ? 0 : 1]++;
                                }
                            }
                        }
                    }
                }
            }
            quadruplets[1] += GetSureWinCount(g, secondPlayerWiningPosition);
            neighboringWinningPositionsCount[0] = GetWiningSituationCount(firstPlayerWiningPosition);
            neighboringWinningPositionsCount[1] = GetWiningSituationCount(secondPlayerWiningPosition);
            return new List<int[]> { triplets, neighboringWinningPositionsCount, quadruplets };
        }

        private int GetWiningSituationCount(List<int[]> winningPositions) {
            int count = 0;
            for (int i = 0; i < winningPositions.Count - 1; i++) {
                for (int j = i + 1; j < winningPositions.Count; j++) {
                    // detect two neighboring winning positions in columns (y axis)
                    if (winningPositions[i][0] == winningPositions[j][0]) {
                        if (winningPositions[i][1] + 1 == winningPositions[j][1] || winningPositions[i][1] - 1 == winningPositions[j][1]) {
                            count++;
                        }
                    }

                    // detect two neighboring winning positions in rows (x axis)
                    if (winningPositions[i][1] == winningPositions[j][1]) {
                        if (winningPositions[i][0] + 1 == winningPositions[j][0] || winningPositions[i][0] - 1 == winningPositions[j][0]) {
                            count++;
                        }
                    }

                    if (winningPositions[i][0] + 4 == winningPositions[j][0] || winningPositions[i][0] - 4 == winningPositions[j][0]) {
                        // horizontally
                        if (winningPositions[i][1] == winningPositions[j][1]) {
                            count++;
                        }

                        // diagonally
                        if (winningPositions[i][1] + 4 == winningPositions[j][1] || winningPositions[i][1] - 4 == winningPositions[j][1]) {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private int GetSureWinCount(Game g, List<int[]> winningPositions) {
            int count = 0;
            for (int i = 0; i < winningPositions.Count; i++) {
                if (winningPositions[i][1] == 0 || g.board[winningPositions[i][0], winningPositions[i][1] - 1] != null) {
                    count++;
                }
            }
            return count;
        }

        private int[] GetGoodPositionCount(Game g) {
            int[] goodPositions = new int[2];
            for (int s = 0; s < 2; s++) {
                bool side = s % 2 == 0;
                for (int i = 0; i < g.sizeX - 3; i++) {
                    for (int j = 0; j < g.sizeY; j++) {
                        if (g.board[i, j] == side) {
                            bool myToken = false;
                            for (int k = 1; k < 4; k++) {
                                if (g.board[i + k, j] == !side) {
                                    break;
                                }
                                if (g.board[i + k, j] == side) {
                                    myToken = true;
                                }
                                if (k == 3 && myToken) {
                                    goodPositions[side ? 0 : 1]++;
                                }
                            }

                            myToken = false;
                            for (int k = 1; k < 4; k++) {
                                if (j + k >= g.sizeY) {
                                    break;
                                }
                                if (g.board[i + k, j + k] == !side) {
                                    break;
                                }
                                if (g.board[i + k, j + k] == side) {
                                    myToken = true;
                                }
                                if (k == 3 && myToken) {
                                    goodPositions[side ? 0 : 1]++;
                                }
                            }

                            myToken = false;
                            for (int k = 1; k < 4; k++) {
                                if (j - k < 0) {
                                    break;
                                }
                                if (g.board[i + k, j - k] == !side) {
                                    break;
                                }
                                if (g.board[i + k, j - k] == side) {
                                    myToken = true;
                                }
                                if (k == 3 && myToken) {
                                    goodPositions[side ? 0 : 1]++;
                                }
                            }
                        }
                    }
                }
            }
            return goodPositions;
        }

        private int GetOver(Game g) {
            int occupied = 0;
            for (int i = 0; i < g.sizeX; i++) {
                for (int j = 0; j < g.sizeY; j++) {
                    if (g.board[i, j] != null) {
                        occupied++;
                    }
                }
            }
            int over = 0;
            for (int i = 0; i < g.sizeX; i++) {
                for (int j = Math.Max(0, occupied / g.sizeX); j < g.sizeY; j++) {
                    if (g.board[i, j] != null) {
                        occupied++;
                    }
                }
            }
            return over;
        }

        public override string getName() {
            return "Experimental " + base.getName() + " by Peter Hojnos";
        }
    }
}
