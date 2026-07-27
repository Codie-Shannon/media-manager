using System;
using System.Collections.Generic;

namespace Media_Manager.Data
{
    public sealed class BackupManifest
    {
        public int FormatVersion { get; set; } = 1;
        public int DatabaseSchemaVersion { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string ApplicationVersion { get; set; }
        public List<BackupFileRecord> Files { get; set; }
            = new List<BackupFileRecord>();
    }

    public sealed class BackupFileRecord
    {
        public string RelativePath { get; set; }
        public long Length { get; set; }
        public string Sha256 { get; set; }
    }

    public sealed class LibraryHealthIssue
    {
        public string Table { get; set; }
        public int Id { get; set; }
        public string Kind { get; set; }
        public string Path { get; set; }
    }

    public sealed class LibraryHealthReport
    {
        public DateTime CheckedAtUtc { get; set; }
        public int TotalRecords { get; set; }
        public int CheckedPaths { get; set; }
        public int MissingPaths { get; set; }
        public int DuplicatePaths { get; set; }
        public List<LibraryHealthIssue> Issues { get; set; }
            = new List<LibraryHealthIssue>();

        public bool IsHealthy => MissingPaths == 0 && DuplicatePaths == 0;

        public string Summary =>
            $"Checked {CheckedPaths:N0} paths across {TotalRecords:N0} records. "
            + $"Missing: {MissingPaths:N0}. Duplicates: {DuplicatePaths:N0}.";
    }

    public sealed class LibraryDataException : Exception
    {
        public LibraryDataException(string message)
            : base(message)
        {
        }

        public LibraryDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
