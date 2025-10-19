namespace HMS.Module.Patient.Features.Patient.Queries
{
    public sealed record PatientQuery(string? Search, int Page, int PageSize)
    {
        // Default: no search, first page, 20 per page
        public PatientQuery() : this(null, 1, 20) { }
    }
}
