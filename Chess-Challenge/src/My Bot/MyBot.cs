using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
  // May need to turn this back into a tuple, depends how many tokens this saves/loses
  struct BoardWeight 
  { 
    public BoardWeight(int w, int b)
    {
      White = w;
      Black = b;
    }
    public int White; 
    public int Black; 
  };
  static readonly byte[] PieceWeights = new byte[] { 0, 1, 3, 3, 5, 9, 10 };
  bool IsWhite = false;

  public Move Think(Board board, Timer timer)
  {
    IsWhite = board.IsWhiteToMove;

    Console.WriteLine($"Thinking...");
    Move[] moves = board.GetLegalMoves();
    BoardWeight startingWeight = GetBoardWeight(board);
    int bestMove = -1;
    int bestDelta = 0;
    int index = 0;
    foreach (Move move in moves)
    {
      if (MoveIsCheckmate(board, move))
      {
        bestMove = index;
      }

      // Depth search of 0 currently, this needs to become recursive/tree-like
      int delta = SearchForBestMove(board, move, startingWeight, 0);
      if (delta > bestDelta)
      {
        bestMove = index;
        bestDelta = delta;
      }
      ++index;
    }

    if (bestMove != -1)
    {
      return moves[bestMove];
    }
    else
    {
      Random rnd = new();
      return moves[rnd.Next(moves.Length)];
    }
  }

  BoardWeight GetBoardWeight(Board board)
  {
    int w = 0, b = 0;
    foreach (var pieces in board.GetAllPieceLists())
    {
      var calcWeights = (PieceList list) => { return list.Count * PieceWeights[(int)list.TypeOfPieceInList]; };
      if (pieces.IsWhitePieceList)
      {
        w += calcWeights(pieces);
      }
      else
      {
        b += calcWeights(pieces);
      }
    }
    return new BoardWeight(w, b);
  }

  bool MoveIsCheckmate(Board board, Move move)
  {
    board.MakeMove(move);
    bool isMate = board.IsInCheckmate();
    board.UndoMove(move);
    return isMate;
  }

  bool MoveIsCheck(Board board, Move move)
  {
    board.MakeMove(move);
    bool isCheck = board.IsInCheck();
    board.UndoMove(move);
    return isCheck;
  }

  int SearchForBestMove(in Board board, in Move move, in BoardWeight boardWeight, int depth)
  {
    board.MakeMove(move);
    BoardWeight moveWeight = GetBoardWeight(board);
    // Black's board weight minus white's board weight gives a basic idea of how balanced things are
    // Moves that remove white's pieces will increase the delta, making them "better" moves
    int delta = board.IsWhiteToMove ? (boardWeight.White - boardWeight.Black) - (moveWeight.White - moveWeight.Black)
                                    : (boardWeight.Black - boardWeight.White) - (moveWeight.Black - moveWeight.White);
    
    Console.WriteLine($"{move}: {delta}");
    if (depth > 0)
    {
      // TODO
      //// Generate new moves from this current board state
      //Move[] newMoves = board.GetLegalMoves();
      //if (newMoves == 0)
      //{
      //  // Check for check
      //}
    }
    board.UndoMove(move);
    return delta;
  }
}