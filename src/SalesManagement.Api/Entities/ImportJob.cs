using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class ImportJob : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public string FilePath { get; set; } = null!;
        public ImportJobStatus Status { get; set; } = ImportJobStatus.PENDING;
        public int TotalRows { get; set; } = 0;
        public int ProcessedOffset { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ImportJobError> Errors { get; set; } = new List<ImportJobError>();
    }
}
