using System;
using System.Collections.Generic;

namespace Chess {
	
	class board {
		public cell[,] chessboard;
		public bool white_move;
		bool [] a1h1e1a8h8e8;    // castling checker
		int [] white_king, black_king;  // position of kings
		struct movement {
			public string square;
			public cell.status beaten;
		}
		List<movement> history;
		public board() {
			chessboard = new cell[8, 8];
			a1h1e1a8h8e8 = new bool[6];
			white_king = new int[2];
			black_king = new int[2];
			history = new List<movement>();
			ResetBoard();
		}
		public void ResetBoard() {
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					chessboard[i, j] = new cell();
				}
			}

			chessboard[0, 0].state = cell.status.BlackRook;
			chessboard[0, 1].state = cell.status.BlackKnight;
			chessboard[0, 2].state = cell.status.BlackBishop;
			chessboard[0, 3].state = cell.status.BlackQueen;
			chessboard[0, 4].state = cell.status.BlackKing;
			chessboard[0, 5].state = cell.status.BlackBishop;
			chessboard[0, 6].state = cell.status.BlackKnight;
			chessboard[0, 7].state = cell.status.BlackRook;

			chessboard[7, 0].state = cell.status.WhiteRook;
			chessboard[7, 1].state = cell.status.WhiteKnight;
			chessboard[7, 2].state = cell.status.WhiteBishop;
			chessboard[7, 3].state = cell.status.WhiteQueen;
			chessboard[7, 4].state = cell.status.WhiteKing;
			chessboard[7, 5].state = cell.status.WhiteBishop;
			chessboard[7, 6].state = cell.status.WhiteKnight;
			chessboard[7, 7].state = cell.status.WhiteRook;

			for (int j = 0; j < 8; j++) {
				chessboard[1, j].state = cell.status.BlackPawn;
				chessboard[6, j].state = cell.status.WhitePawn;
			}
			for (int i = 0; i < 6; i++) {  // reset castling restrictions
				a1h1e1a8h8e8[i] = true;
			}
			history.Clear();
			white_move = true;
			show();
		}

		bool legal_move(int x1, int y1, int x2, int y2) {
			if (x1 > 7 || x1 < 0 || x2 > 7 || x2 < 0 || y1 > 7 || y1 < 0 || y2 > 7 || y2 < 0 || x1 == x2 && y1 == y2) { // range check
				return false;
			}
			if (chessboard[y1, x1].state == cell.status.empty) { // piece presence check
				return false;
			}
			if (white_move == chessboard[y1, x1].state > cell.status.WhiteKing) { // color move check
				return false;
			}
			if (chessboard[y1, x1].state > cell.status.WhiteKing) {     // friendly fire check
				if (chessboard[y2, x2].state > cell.status.WhiteKing) {
					return false;
				}
			}

			if (chessboard[y1, x1].state == cell.status.BlackKing || chessboard[y1, x1].state == cell.status.WhiteKing) { // king moves
				if (Math.Abs(x1 - x2) == 2) {  // castling
					if (y1 != y2) {
						return false;
					}
					if (chessboard[y1, x1].state == cell.status.WhiteKing) {
						if (x2 - x1 == 2) {
							if (!a1h1e1a8h8e8[1] || !a1h1e1a8h8e8[2]) { // if h1 rook or king had moved
								return false;
							}
							if (chessboard[7, 5].state != cell.status.empty || chessboard[7, 6].state != cell.status.empty) {
								return false;
							}
							if (under_attack(7, 5, true) || under_attack(7, 6, true)) {
								return false;
							}
						}
						if (x2 - x1 == -2) {
							if (!a1h1e1a8h8e8[0] || !a1h1e1a8h8e8[2]) { // if a1 rook or king had moved
								return false;
							}
							if (chessboard[7, 3].state != cell.status.empty || chessboard[7, 2].state != cell.status.empty || chessboard[7, 1].state != cell.status.empty) {
								return false;
							}
							if (under_attack(7, 3, true) || under_attack(7, 2, true) || under_attack(7, 1, true)) {
								return false;
							}
						}
					}
					else if (chessboard[y1, x1].state == cell.status.BlackKing) {
						if (x2 - x1 == 2) {
							if (!a1h1e1a8h8e8[4] || !a1h1e1a8h8e8[5]) { // if h8 rook or king had moved
								return false;
							}
							if (chessboard[0, 5].state != cell.status.empty || chessboard[0, 6].state != cell.status.empty) {
								return false;
							}
							if (under_attack(0, 5, false) || under_attack(0, 6, false)) {
								return false;
							}
						}
						if (x2 - x1 == -2) {
							if (!a1h1e1a8h8e8[3] || !a1h1e1a8h8e8[5]) { // if a8 rook or king had moved
								return false;
							}
							if (chessboard[0, 3].state != cell.status.empty || chessboard[0, 2].state != cell.status.empty || chessboard[0, 1].state != cell.status.empty) {
								return false;
							}
							if (under_attack(0, 3, false) || under_attack(0, 2, false) || under_attack(0, 1, false)) {
								return false;
							}
						}
					}
				}
				else if (Math.Abs(x1 - x2) > 1 || Math.Abs(y1 - y2) > 1) {
					return false;
				}


			}

			else if (chessboard[y1, x1].state == cell.status.BlackQueen || chessboard[y1, x1].state == cell.status.WhiteQueen){ // queen moves
				if (!(x1 == x2 || y1 == y2 || Math.Abs(x1 - x2) == Math.Abs(y1 - y2))) {
					return false;
				}
				if (x1 == x2 || y1 == y2) {
					if (x1 != x2) {
						for (int i = x1 + (x2 - x1) / Math.Abs(x2 - x1); i != x2; i += (x2 - x1) / Math.Abs(x2 - x1)) {
							if (chessboard[y1, i].state != cell.status.empty) {
								return false;
							}
						}
					}
					else if (y1 != y2) {
						for (int i = y1 + (y2 - y1) / Math.Abs(y2 - y1); i != y2; i += (y2 - y1) / Math.Abs(y2 - y1)) {
							if (chessboard[i, x1].state != cell.status.empty) {
								return false;
							}
						}
					}
				}
				else {
					int i = y1 + (y2 - y1) / Math.Abs(y2 - y1);
					int j = x1 + (x2 - x1) / Math.Abs(x2 - x1);
					while (i != y2) {
						i += (y2 - y1) / Math.Abs(y2 - y1);
						j += (x2 - x1) / Math.Abs(x2 - x1);
						if (chessboard[i, j].state != cell.status.empty) {
							return false;
						}
					}
				}
			}

			else if (chessboard[y1, x1].state == cell.status.BlackRook || chessboard[y1, x1].state == cell.status.WhiteRook){ // rook moves
				if (x1 != x2 && y1 != y2) {
					return false;
				}
				if (x1 != x2) {
					for (int i = x1 + (x2 - x1) / Math.Abs(x2 - x1); i != x2; i += (x2 - x1) / Math.Abs(x2 - x1)) {
						if (chessboard[y1, i].state != cell.status.empty) {
							return false;
						}
					}
				}
				else if (y1 != y2) {
					for (int i = y1 + (y2 - y1) / Math.Abs(y2 - y1); i != y2; i += (y2 - y1) / Math.Abs(y2 - y1)) {
						if (chessboard[i, x1].state != cell.status.empty) {
							return false;
						}
					}
				}
			}

			else if (chessboard[y1, x1].state == cell.status.BlackBishop || chessboard[y1, x1].state == cell.status.WhiteBishop){ // bishop moves
				if (Math.Abs(x1 - x2) != Math.Abs(y1 - y2)) {
					return false;
				}
				int i = y1 + (y2 - y1) / Math.Abs(y2 - y1);
				int j = x1 + (x2 - x1) / Math.Abs(x2 - x1);
				while (i != y2) {
					i += (y2 - y1) / Math.Abs(y2 - y1);
					j += (x2 - x1) / Math.Abs(x2 - x1);
					if (chessboard[i, j].state != cell.status.empty) {
						return false;
					}
				}

			}

			else if (chessboard[y1, x1].state == cell.status.BlackKnight || chessboard[y1, x1].state == cell.status.WhiteKnight){ // knight moves
				if (!(Math.Abs(x1 - x2) == 1 && Math.Abs(y1 - y2) == 2 || Math.Abs(x1 - x2) == 2 && Math.Abs(y1 - y2) == 1)) {
					return false;
				}
			}

			else if (chessboard[y1, x1].state == cell.status.WhitePawn) { // white pawn moves
				if (y2 >= y1) {   // no moving back
					return false;
				}
				if (x1 == x2 && chessboard[y2, x2].state != cell.status.empty) { // no forward attacks
					return false;
				}
				if (y1 == 6 && (y2 < 4 || chessboard[y1 - 1, x1].state != cell.status.empty)) { // first move
					return false;
				}
				if (y1 - y2 > 1 && y1 != 6 ) { // one cell per move
					return false;
				}
				if (x1 != x2) { // attack
					if (Math.Abs(x1 - x2) > 1 || y1 - y2 > 1) {
						return false;
					}
					if (x2 == x1 - 1) { // nothing to attack
						if (chessboard[y2, x2].state == cell.status.empty) {
							return false;
						}
					}
					if (x2 == x1 + 1) {
						if (chessboard[y2, x2].state == cell.status.empty) {
							return false;
						}
					}
				}
			}

			else if (chessboard[y1, x1].state == cell.status.BlackPawn) { // black pawn moves
				if (y2 <= y1) {   // no moving back
					return false;
				}
				if (x1 == x2 && chessboard[y2, x2].state != cell.status.empty) { // no forward attacks
					return false;
				}
				if (y1 == 1 && (y2 > 3 || chessboard[y1 + 1, x1].state != cell.status.empty)) { // first move
					return false;
				}
				if (y2 - y1 > 1 && y1 != 1) { // one cell per move
					return false;
				}
				if (x1 != x2) { // attack
					if (Math.Abs(x1 - x2) > 1 || y2 - y1 > 1) {
						return false;
					}
					if (x2 == x1 - 1) { // nothing to attack
						if (chessboard[y2, x2].state == cell.status.empty) {
							return false;
						}
					}
					if (x2 == x1 + 1) {
						if (chessboard[y2, x2].state == cell.status.empty) {
							return false;
						}
					}
				}
			}

			return true;
		}

		bool under_attack(int y, int x, bool white) {
			if (white) {
				// pawn threats
				if (chessboard[y - 1, x + 1].state == cell.status.BlackPawn || chessboard[y - 1, x - 1].state == cell.status.BlackPawn) {
					return true;
				}
				// rook and queen threats
				for (int i = x + 1; i <= 7; i++) { // right
					if (chessboard[y, i].state == cell.status.BlackRook || chessboard[y, i].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[y, i].state != cell.status.empty) {
						break;
					}
				}
				for (int i = x - 1; i >= 0; i--) { // left
					if (chessboard[y, i].state == cell.status.BlackRook || chessboard[y, i].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[y, i].state != cell.status.empty) {
						break;
					}
				}
				for (int i = y + 1; i <= 7; i++) { // down
					if (chessboard[i, x].state == cell.status.BlackRook || chessboard[y, i].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[i, x].state != cell.status.empty) {
						break;
					}
				}
				for (int i = y - 1; i >= 0; i--) { // up
					if (chessboard[i, x].state == cell.status.BlackRook || chessboard[y, i].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[i, x].state != cell.status.empty) {
						break;
					}
				}
				// bishop and queen threat
				int i1 = y + 1, j1 = x + 1;
				while (i1 <= 7 && j1 <= 7) {  // down right
					if (chessboard[i1, j1].state == cell.status.BlackBishop || chessboard[i1, j1].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1++; j1++;
				}
				i1 = y - 1; j1 = x + 1;
				while (i1 >= 0 && j1 <= 7) {  // up right
					if (chessboard[i1, j1].state == cell.status.BlackBishop || chessboard[i1, j1].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1--; j1++;
				}
				i1 = y - 1; j1 = x - 1;
				while (i1 >= 0 && j1 >= 0) {  // up left
					if (chessboard[i1, j1].state == cell.status.BlackBishop || chessboard[i1, j1].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1--; j1--;
				}
				i1 = y + 1; j1 = x - 1;
				while (i1 <= 7 && j1 <= 7) {  // down left
					if (chessboard[i1, j1].state == cell.status.BlackBishop || chessboard[i1, j1].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1++; j1--;
				}
				// knight threats
				if (x < 6 && y < 7 && chessboard[y + 1, x + 2].state == cell.status.BlackKnight) {
					return true;
				}
				if (x < 6 && y > 0 && chessboard[y - 1, x + 2].state == cell.status.BlackKnight) {
					return true;
				}
				if (x < 7 && y > 1 && chessboard[y - 2, x + 1].state == cell.status.BlackKnight) {
					return true;
				}
				if (x > 0 && y > 1 && chessboard[y - 2, x - 1].state == cell.status.BlackKnight) {
					return true;
				}
				if (x > 1 && y > 0 && chessboard[y - 1, x - 2].state == cell.status.BlackKnight) {
					return true;
				}
				if (x < 1 && y < 7 && chessboard[y + 1, x - 2].state == cell.status.BlackKnight) {
					return true;
				}
				if (x > 0 && y < 6 && chessboard[y + 2, x - 1].state == cell.status.BlackKnight) {
					return true;
				}
				if (x < 7 && y < 6 && chessboard[y + 2, x + 1].state == cell.status.BlackKnight) {
					return true;
				}
			}


			if (!white) { ////////////////////////// black ////////////////////
				// pawn threats
				if (chessboard[y + 1, x + 1].state == cell.status.WhitePawn || chessboard[y + 1, x - 1].state == cell.status.WhitePawn) {
					return true;
				}
				// rook and queen threats
				for (int i = x + 1; i <= 7; i++) { // right
					if (chessboard[y, i].state == cell.status.WhiteRook || chessboard[y, i].state == cell.status.BlackQueen) {
						return true;
					}
					if (chessboard[y, i].state != cell.status.empty) {
						break;
					}
				}
				for (int i = x - 1; i >= 0; i--) { // left
					if (chessboard[y, i].state == cell.status.WhiteRook || chessboard[y, i].state == cell.status.WhiteQueen) {
						return true;
					}
					if (chessboard[y, i].state != cell.status.empty) {
						break;
					}
				}
				for (int i = y + 1; i <= 7; i++) { // down
					if (chessboard[i, x].state == cell.status.WhiteRook || chessboard[y, i].state == cell.status.WhiteQueen) {
						return true;
					}
					if (chessboard[i, x].state != cell.status.empty) {
						break;
					}
				}
				for (int i = y - 1; i >= 0; i--) { // up
					if (chessboard[i, x].state == cell.status.WhiteRook || chessboard[y, i].state == cell.status.WhiteQueen) {
						return true;
					}
					if (chessboard[i, x].state != cell.status.empty) {
						break;
					}
				}
				// bishop and queen threat
				int i1 = y + 1, j1 = x + 1;
				while (i1 < 7 && j1 < 7) {  // down right
					if (chessboard[i1, j1].state == cell.status.WhiteBishop || chessboard[i1, j1].state == cell.status.WhiteQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1++; j1++;
				}
				i1 = y - 1; j1 = x + 1;
				while (i1 > 0 && j1 < 7) {  // up right
					if (chessboard[i1, j1].state == cell.status.WhiteBishop || chessboard[i1, j1].state == cell.status.WhiteQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1--; j1++;
				}
				i1 = y - 1; j1 = x - 1;
				while (i1 > 0 && j1 > 0) {  // up left
					if (chessboard[i1, j1].state == cell.status.WhiteBishop || chessboard[i1, j1].state == cell.status.WhiteQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1--; j1--;
				}
				i1 = y + 1; j1 = x - 1;
				while (i1 < 7 && j1 < 7) {  // down left
					if (chessboard[i1, j1].state == cell.status.WhiteBishop || chessboard[i1, j1].state == cell.status.WhiteQueen) {
						return true;
					}
					if (chessboard[i1, j1].state != cell.status.empty) {
						break;
					}
					i1++; j1--;
				}
				// knight threats
				if (x < 6 && y < 7 && chessboard[y + 1, x + 2].state == cell.status.WhiteKnight) {
					return true;
				}
				if (x < 6 && y > 0 && chessboard[y - 1, x + 2].state == cell.status.WhiteKnight) {
					return true;
				}
				if (x < 7 && y > 1 && chessboard[y - 2, x + 1].state == cell.status.WhiteKnight) {
					return true;
				}
				if (x > 0 && y > 1 && chessboard[y - 2, x - 1].state == cell.status.WhiteKnight) {
					return true;
				}
				if (x > 1 && y > 0 && chessboard[y - 1, x - 2].state == cell.status.WhiteKnight) {
					return true;
				}
				if (x < 1 && y < 7 && chessboard[y + 1, x - 2].state == cell.status.WhiteKnight) {
					return true;
				}
				if (x > 0 && y < 6 && chessboard[y + 2, x - 1].state == cell.status.WhiteKnight) {
					return true;
				}
				if (x < 7 && y < 6 && chessboard[y + 2, x + 1].state == cell.status.WhiteKnight) {
					return true;
				}
			}
			if (Math.Abs(black_king[0] - white_king[0]) == 1 && Math.Abs(black_king[1] - white_king[1]) == 1) {
				return true;
			}
			return false;
		}
		public void reverse() {
			if (history.Count == 0) {
				return;
			}
			int x1, y1, x2, y2;
			x2 = Convert.ToInt32(history[history.Count - 1].square[0] - 'a');
			y2 = 8 - Convert.ToInt32(history[history.Count - 1].square[1] - '0');
			x1 = Convert.ToInt32(history[history.Count - 1].square[2] - 'a');
			y1 = 8 - Convert.ToInt32(history[history.Count - 1].square[3] - '0');
			if ((chessboard[y1, x1].state == cell.status.BlackKing || chessboard[y1, x1].state == cell.status.WhiteKing) && (Math.Abs(x1 - x2) == 2)) {
				if (chessboard[y1, x1].state == cell.status.BlackKing) {  // reversing castling
					if (x1 == 6) {
						chessboard[0, 4].state = cell.status.BlackKing;
						chessboard[0, 5].state = cell.status.empty;
						chessboard[0, 6].state = cell.status.empty;
						chessboard[0, 7].state = cell.status.BlackRook;
					}
					if (x1 == 2) {
						chessboard[0, 0].state = cell.status.BlackRook;
						chessboard[0, 1].state = cell.status.empty;
						chessboard[0, 2].state = cell.status.empty;
						chessboard[0, 3].state = cell.status.empty;
						chessboard[0, 4].state = cell.status.BlackKing;
					}
				}
				if (chessboard[y1, x1].state == cell.status.WhiteKing) {
					if (x1 == 6) {
						chessboard[7, 4].state = cell.status.BlackKing;
						chessboard[7, 5].state = cell.status.empty;
						chessboard[7, 6].state = cell.status.empty;
						chessboard[7, 7].state = cell.status.BlackRook;
					}
					if (x1 == 2) {
						chessboard[7, 0].state = cell.status.BlackRook;
						chessboard[7, 1].state = cell.status.empty;
						chessboard[7, 2].state = cell.status.empty;
						chessboard[7, 3].state = cell.status.empty;
						chessboard[7, 4].state = cell.status.BlackKing;
					}
				}
			}
			else {
				chessboard[y2, x2].state = chessboard[y1, x1].state;
				chessboard[y1, x1].state = history[history.Count - 1].beaten;
			}
			history.RemoveAt(history.Count - 1);
			white_move = !white_move;
		}



		public void move(string coords) {
			if (coords.Length != 4) {
				Console.WriteLine("Invalid command");
				return;
			}
			int x1, y1, x2, y2;
			x1 = Convert.ToInt32(coords[0] - 'a');
			y1 = 8 - Convert.ToInt32(coords[1] - '0');
			x2 = Convert.ToInt32(coords[2] - 'a');
			y2 = 8 - Convert.ToInt32(coords[3] - '0');
			if (legal_move(x1, y1, x2, y2)) {

				if (y1 == 0) {       // if rooks or kings moved
					if (x1 == 0) {
						a1h1e1a8h8e8[3] = false;
					}
					else if (x1 == 4) {
						a1h1e1a8h8e8[5] = false;
					}
					else if (x1 == 7) {
						a1h1e1a8h8e8[4] = false;
					}
				}
				if (y1 == 7) {
					if (x1 == 0) {
						a1h1e1a8h8e8[0] = false;
					}
					else if (x1 == 4) {
						a1h1e1a8h8e8[2] = false;
					}
					else if (x1 == 7) {
						a1h1e1a8h8e8[1] = false;
					}
				}
				if (y1 == white_king[0] && x1 == white_king[1]) {  // looking for castling
					if (x2 - x1 == 2) {    // white 0-0
						chessboard[7, 5].state = cell.status.BlackRook;
						chessboard[7, 7].state = cell.status.empty;
						a1h1e1a8h8e8[1] = false;
					}
					if (x2 - x1 == -2) {   // white 0-0-0
						chessboard[7, 3].state = cell.status.BlackRook;
						chessboard[7, 0].state = cell.status.empty;
						a1h1e1a8h8e8[0] = false;
					}
				}
				if (y1 == black_king[0] && x1 == black_king[1]) {
					if (x2 - x1 == 2) {    // black 0-0
						chessboard[0, 5].state = cell.status.WhiteRook;
						chessboard[0, 7].state = cell.status.empty;
						a1h1e1a8h8e8[4] = false;
					}
					if (x2 - x1 == -2) {   // black 0-0-0
						chessboard[0, 3].state = cell.status.WhiteRook;
						chessboard[0, 0].state = cell.status.empty;
						a1h1e1a8h8e8[3] = false;
					}
				}

				movement temp = new movement();
				temp.square = coords;
				temp.beaten = chessboard[y2, x2].state;
				history.Add(temp);

				cell temp_cell = new cell();
				temp_cell.state = chessboard[y1, x1].state;
				chessboard[y2, x2] = temp_cell;
				chessboard[y1, x1].state = cell.status.empty;

				for (int i = 0; i < 8; i++) {        // searching for the kings
					for (int j = 0; j < 8; j++) {
						if (chessboard[i, j].state == cell.status.BlackKing) {
							black_king[0] = i;
							black_king[1] = j;
						}
						if (chessboard[i, j].state == cell.status.WhiteKing) {
							white_king[0] = i;
							white_king[1] = j;
						}
					}
				}

				if (white_move) {
					white_move = !white_move;
					if (under_attack(white_king[0], white_king[1], true)) { // if king is under attack after move, we reverse it
						reverse();
						Console.WriteLine("Illegal move: king under attack");
						return;
					}
				}
				else {
					white_move = !white_move;
					if (under_attack(black_king[0], black_king[1], false)) {
						reverse();
						Console.WriteLine("Illegal move: king under attack");
						return;
					}
				}

				if (y2 == 0 && chessboard[y2, x2].state == cell.status.WhitePawn) {         // white pawn promotion
					Console.WriteLine("Promote to what? (Queen, Rook, Bishop or Knight)");
					while (chessboard[y2, x2].state == cell.status.WhitePawn) {
						string answer = Console.ReadLine().ToLower();
						switch (answer) {
							case "queen":
								chessboard[y2, x2].state = cell.status.WhiteQueen;
								break;
							case "rook":
								chessboard[y2, x2].state = cell.status.WhiteRook;
								break;
							case "bishop":
								chessboard[y2, x2].state = cell.status.WhiteBishop;
								break;
							case "knight":
								chessboard[y2, x2].state = cell.status.WhiteKnight;
								break;
							default:
								Console.WriteLine("No such piece");
								break;
						}
					}
				}
				if (y2 == 7 && chessboard[y2, x2].state == cell.status.BlackPawn) {       // black pawn promotion
					Console.WriteLine("Promote to what? (Queen, Rook, Bishop or Knight)");
					while (chessboard[y2, x2].state == cell.status.BlackPawn) {
						string answer = Console.ReadLine().ToLower();
						switch (answer) {
							case "queen":
								chessboard[y2, x2].state = cell.status.BlackQueen;
								break;
							case "rook":
								chessboard[y2, x2].state = cell.status.BlackRook;
								break;
							case "bishop":
								chessboard[y2, x2].state = cell.status.BlackBishop;
								break;
							case "knight":
								chessboard[y2, x2].state = cell.status.BlackKnight;
								break;
							default:
								Console.WriteLine("No such piece");
								break;
						}
					}
				}

				show();
			}
			else {
				Console.WriteLine("Illegal move!");
			}
		}

		public void show() {
			Console.Clear();
			if (white_move) {
				Console.WriteLine("        white move");
			}
			else {
				Console.WriteLine("        black move");
			}
			Console.WriteLine();
			Console.WriteLine("    a  b  c  d  e  f  g  h");
			Console.WriteLine();
			for (int i = 0; i < 8; i++) {
				Console.Write(8 - i);
				Console.Write("  ");
				for (int j = 0; j < 8; j++) {
					switch (chessboard[i, j].state) {
						case cell.status.empty:
							Console.Write("   ");
							break;
						case cell.status.WhitePawn:
							Console.Write(" wp");
							break;
						case cell.status.WhiteKnight:
							Console.Write(" wN");
							break;
						case cell.status.WhiteBishop:
							Console.Write(" wB");
							break;
						case cell.status.WhiteRook:
							Console.Write(" wR");
							break;
						case cell.status.WhiteQueen:
							Console.Write(" wQ");
							break;
						case cell.status.WhiteKing:
							Console.Write(" wK");
							break;
						case cell.status.BlackPawn:
							Console.Write(" bp");
							break;
						case cell.status.BlackKnight:
							Console.Write(" bN");
							break;
						case cell.status.BlackBishop:
							Console.Write(" bB");
							break;
						case cell.status.BlackRook:
							Console.Write(" bR");
							break;
						case cell.status.BlackQueen:
							Console.Write(" bQ");
							break;
						case cell.status.BlackKing:
							Console.Write(" bK");
							break;
						default:
							break;
					}

				}
				Console.WriteLine();
			}
		}
	}

	class cell {

		public status state;
		public cell() {
			state = new status();
			state = status.empty;
		}
		public enum status {
			empty,
			WhitePawn,
			WhiteKnight,
			WhiteBishop,
			WhiteRook,
			WhiteQueen,
			WhiteKing,
			BlackPawn,
			BlackKnight,
			BlackBishop,
			BlackRook,
			BlackQueen,
			BlackKing
		}
	}



	class Program {
		static void Main(string[] args) {
			//Console.OutputEncoding = System.Text.Encoding.Unicode;
			string command = "";

			board current_board = new board();

			while (true) {
				command = Console.ReadLine().ToLower();
				if (command == "exit") {
					break;
				}
				else if (command == "reset" || command == "new game") {
					current_board.ResetBoard();
				}
				else if (command == "0-0" || command == "o-o") {
					if (current_board.white_move) {
						current_board.move("e1g1");
					}
					else {
						current_board.move("e8g8");
					}
				}
				else if (command == "0-0-0" || command == "o-o-o") {
						if (current_board.white_move) {
							current_board.move("e1c1");
						}
						else {
							current_board.move("e8c8");
						}
					}
				else if (command == "back") {
					current_board.reverse();
					current_board.show();				}
				else {
					current_board.move(command);
				}

			}
		}
	}
}