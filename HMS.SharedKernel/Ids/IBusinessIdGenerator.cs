namespace HMS.SharedKernel.Ids;

public interface IBusinessIdGenerator
{
    long Next(string seqName);
    string NewMrn();                              // e.g., "P20250819..." 
    string NewEncounterNo(DateTime utc);
    string NewAppointmentNo(DateTime whenUtc);    // e.g., "AP20250819..."
    string NewLabOrderNo(DateTime utc);
    string NewAccessionNumber(DateTime whenUtc);  // for Lab later, "ACC2025..."
    string NewInvoiceNo(DateTime utc);
    string NewCommInboundNo(DateTime utc);
}