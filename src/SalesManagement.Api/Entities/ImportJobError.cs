using System;

namespace SalesManagement.Api.Entities
{
    public class ImportJobError
    {
        public long Id { get; set; }

        public Guid JobId { get; set; }
        public ImportJob? Job { get; set; }

        public int RowNumber { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RawData { get; set; }
    }
}
