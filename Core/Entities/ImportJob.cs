using Core.Enums;
namespace Core.Entities;

public class ImportJob { public Guid Id { get; set; } public Guid TenantId { get; set; } public Guid CreatedByUserId { get; set; } public ImportJobStatus Status { get; set; } = ImportJobStatus.PENDING; public string FileName { get; set; } = null!; public int ProcessedOffset { get; set; } public int TotalRows { get; set; } public int SuccessRows { get; set; } public int FailedRows { get; set; } public string? ErrorMessage { get; set; } public DateTime CreatedAt { get; set; } public DateTime? StartedAt { get; set; } public DateTime? CompletedAt { get; set; } public Tenant Tenant { get; set; } = null!; public AppUser CreatedByUser { get; set; } = null!; }


