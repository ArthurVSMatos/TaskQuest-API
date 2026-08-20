namespace TaskQuest.API.DTOs;

public class TarefaConcluidaResponseDto
{
    public string Mensagem { get; set; } = "Tarefa concluída com sucesso!";
    public int XpGanho { get; set; }
    public int XpTotalAtual { get; set; }
    public int NivelAtual { get; set; }
    public bool SubiuDeNivel { get; set; }
    public int TempoRealMin { get; set; }
}
