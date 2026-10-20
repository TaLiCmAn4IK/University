using System.Collections.Generic;

namespace Flux.DTOs;

public class CS2MatchDetails
{
    public string Map { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public int ScoreTeam1 { get; set; }
    public int ScoreTeam2 { get; set; }
    public int PickerTeam { get; set; }      
    public int ServerPickerTeam { get; set; }
}

public class ChessMatchDetails
{
    public List<string> Moves { get; set; } = new();
    public string Result { get; set; } = string.Empty;   
    public string EndReason { get; set; } = string.Empty; 
    public string FinalFen { get; set; } = string.Empty;
}
