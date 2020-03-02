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
            int[] positionBonuses = getPositionBonuses(g);
            int[] oddEven = GetOddEven(g);
            int result = (tuplets[0][0] - tuplets[0][1]) * 5 +
                ((isFirstPlayer ? 1 : 100) * tuplets[1][0] - (isFirstPlayer ? 1 : 100) * tuplets[1][1]) * 10 +
                ((isFirstPlayer ? 1 : 100) * tuplets[2][0] - (isFirstPlayer ? 1 : 100) * tuplets[2][1]) * 10 +
                (oddEven[0] - oddEven[1]) +
                ((isFirstPlayer ? 1 : 3) * positionBonuses[0] - (isFirstPlayer ? 1 : 3) * positionBonuses[1]);
            return result;
        }        

        private int[] GetOddEven(Game g) {
            if (g.sizeY % 2 == 1) {
                return new int[] { 0, 0 };
            }
            int[] oddEvenCount = new int[2];
            for (int i = 0; i < g.sizeX; i++) {
                for (int j = 0; j < g.sizeY; j++) {
                    if (j % 2 == 0 && g.board[i, j] == true) {
                        oddEvenCount[0]++;
                    }
                    if (j % 2 == 0 && g.board[i, j] == false) {
                        oddEvenCount[1]++;
                    }
                }
            }
            return oddEvenCount;
        }

        public List<int[]> GetTuplets(Game g) {            
            int[] triplets = new int[2];
            int[] sureWin = new int[2];
            int[] wins = new int[2];
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
                                    wins[side ? 0 : 1]++;
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
                                    wins[side ? 0 : 1]++;
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
                                    wins[side ? 0 : 1]++;
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
                                    wins[side ? 0 : 1]++;
                                }
                            }
                        }
                    }
                }
            }

            sureWin[0] = countSureWin(firstPlayerWiningPosition);
            sureWin[1] = countSureWin(secondPlayerWiningPosition);
            return new List<int[]> { triplets, sureWin, wins  };
        }

        private int countSureWin(List<int[]> positions) {
            int count = 0;
            for (int i = 0; i < positions.Count - 1; i++) {
                for (int j = i + 1; j < positions.Count; j++) {
                    // two in row in y axis
                    if (positions[i][0] == positions[j][0]) {
                        if (positions[i][1] + 1 == positions[j][1] || positions[i][1] - 1 == positions[j][1]) {
                            count++;
                        }
                    }

                    // two in row in x axis
                    if (positions[i][1] == positions[j][1]) {
                        if(positions[i][0] + 1 == positions[j][0] || positions[i][0] - 1 == positions[j][0]) {
                            count++;
                        }
                    }
                }
            }
            return count;
        }       

        public override string getName() {
            return "Experimental " + base.getName() + " by Peter Hojnos";
        }
    }
}
