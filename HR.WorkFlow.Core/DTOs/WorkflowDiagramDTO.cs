namespace HR.WorkFlow.Core.DTOs;

public class WorkflowDiagramDTO
{
    public long WorkFlowTypeId { get; set; }
    public string? WorkFlowTypeTitle { get; set; }
    public long? SelectedWorkFlowId { get; set; }
    public List<WorkflowDiagramWorkFlowItemDTO> WorkFlows { get; set; } = [];
    public List<WorkflowDiagramNodeDTO> Nodes { get; set; } = [];
    public List<WorkflowDiagramEdgeDTO> Edges { get; set; } = [];
    public List<WorkflowDiagramPaletteNodeDTO> PaletteNodes { get; set; } = [];
    public List<WorkflowDiagramActionDTO> Actions { get; set; } = [];
}

public class WorkflowDiagramWorkFlowItemDTO
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public bool IsActive { get; set; }
}

public class WorkflowDiagramNodeDTO
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int Priority { get; set; }
    public double? X { get; set; }
    public double? Y { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }
}

public class WorkflowDiagramEdgeDTO
{
    public long Id { get; set; }
    public long WorkFlowId { get; set; }
    public long? FromNodeId { get; set; }
    public long? ToNodeId { get; set; }
    public string? FromNodeTitle { get; set; }
    public string? ToNodeTitle { get; set; }
    public long ActionId { get; set; }
    public string? ActionTitle { get; set; }
    public bool? AllowComment { get; set; }
    public bool? IsCommentRequired { get; set; }
    public bool NeedSignature { get; set; }
    public bool IsFinalTransition { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public class WorkflowDiagramPaletteNodeDTO
{
    public long Id { get; set; }
    public string? Title { get; set; }
}

public class WorkflowDiagramActionDTO
{
    public long Id { get; set; }
    public string? Title { get; set; }
}

public class WorkflowDiagramLayoutDTO
{
    public long WorkFlowId { get; set; }
    public List<WorkflowDiagramNodePositionDTO> Positions { get; set; } = [];
}

public class WorkflowDiagramNodePositionDTO
{
    public long NodeId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}
