using System.Text;
using System.Threading.Tasks;
using ClearCare.Models.Entities;
using ClearCare.DataSource;
using Google.Cloud.Firestore;
using ClearCare.Models.Interface;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ClearCare.Models.Control
{
    public class ViewMedicalRecord : IMedicalRecord
    {
        private MedicalRecordGateway MedicalRecordGateway;
        private readonly UserGateway UserGateway;
        private readonly IEncryption encryptionService;
        private readonly ErratumManagement erratumManagement;

        public ViewMedicalRecord(IEncryption encryptionService, ErratumManagement erratumManagement)
        {
            MedicalRecordGateway = new MedicalRecordGateway();
            UserGateway = new UserGateway();
            this.encryptionService = encryptionService;
            this.erratumManagement = erratumManagement;
        }

        // Retrieve all medical records and process them for display
        public async Task<List<dynamic>> getAllMedicalRecords()
        {
            var medicalRecords = await MedicalRecordGateway.retrieveAllMedicalRecords();
            var processedRecords = new List<dynamic>();

            foreach (var record in medicalRecords)
            {
                var recordDetails = record.getRecordDetails();
                string patientName = await UserGateway.findUserNameByID((string)recordDetails["PatientID"]);
                string doctorName = await UserGateway.findUserNameByID((string)recordDetails["DoctorID"]);

                processedRecords.Add(new
                {
                    MedicalRecordID = recordDetails["MedicalRecordID"],
                    PatientID = patientName,
                    CreatedBy = doctorName,
                    RecordID = recordDetails["MedicalRecordID"]
                });
            }
            return processedRecords;
        }

        // Retrieve medical record by ID
        public async Task<dynamic> getAdjustedRecordByID(string recordID)
        {
            var medicalRecord = await MedicalRecordGateway.retrieveMedicalRecordById(recordID);
            if (medicalRecord == null)
            {
                return null;
            }

            var recordDetails = medicalRecord.getRecordDetails();
            string patientName = await UserGateway.findUserNameByID((string)recordDetails["PatientID"]);
            string doctorName = await UserGateway.findUserNameByID((string)recordDetails["DoctorID"]);

            // Decrypt the doctor note before returning it
            string decryptedDoctorNote = encryptionService.decryptMedicalData((string)recordDetails["DoctorNote"]);

            return new
            {
                MedicalRecordID = recordDetails["MedicalRecordID"],
                PatientID = patientName,
                CreatedBy = doctorName,
                Date = recordDetails["Date"],
                DoctorNote = decryptedDoctorNote,
                AttachmentName = recordDetails["AttachmentName"],
                HasAttachment = recordDetails["HasAttachment"]
            };
        }

        public async Task<MedicalRecord> getOriginalRecordByID(string recordID)
        {
            return await MedicalRecordGateway.retrieveMedicalRecordById(recordID);
        }

        public async Task<string> exportMedicalRecord(string recordID, string format = "csv")
        {
            var medicalRecord = await getAdjustedRecordByID(recordID);
            if (medicalRecord == null)
            {
                return "Medical record not found.";
            }

            var allErratums = await erratumManagement.getAllErratum();
            var filteredErratums = allErratums.Where(e => e.MedicalRecordID == recordID).ToList();

            string filePath;
            if (format.ToLower() == "csv")
            {
                // CSV export code remains the same
                StringBuilder csvContent = new StringBuilder();
                csvContent.AppendLine("MedicalRecordID,PatientID,CreatedBy,Date,DoctorNote,AttachmentName");
                csvContent.AppendLine($"{medicalRecord.MedicalRecordID},{medicalRecord.PatientID},{medicalRecord.CreatedBy},{medicalRecord.Date},{medicalRecord.DoctorNote},{medicalRecord.AttachmentName}");

                if (filteredErratums.Count > 0)
                {
                    csvContent.AppendLine();
                    csvContent.AppendLine("ErratumID,MedicalRecordID,FiledBy,DateFiled,ErratumDetails");
                    foreach (var erratum in filteredErratums)
                    {
                        csvContent.AppendLine($"{erratum.ErratumID},{erratum.MedicalRecordID},{erratum.CreatedBy},{erratum.Date},{erratum.ErratumDetails}");
                    }
                }

                filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{recordID}_MedicalRecord.csv");
                await File.WriteAllTextAsync(filePath, csvContent.ToString());
            }
            else if (format.ToLower() == "pdf")
            {
                // Updated PDF export code with tables
                filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{recordID}_MedicalRecord.pdf");

                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var doc = new Document())
                    {
                        PdfWriter.GetInstance(doc, fs);
                        doc.Open();

                        // Add a title
                        Paragraph title = new Paragraph($"Medical Record: {medicalRecord.MedicalRecordID}",
                            new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                        title.Alignment = Element.ALIGN_CENTER;
                        title.SpacingAfter = 20f;
                        doc.Add(title);

                        // Create table for medical record details
                        PdfPTable detailsTable = new PdfPTable(2);
                        detailsTable.WidthPercentage = 100;
                        detailsTable.SpacingAfter = 20f;

                        // Set column widths
                        float[] columnWidths = new float[] { 1f, 3f };
                        detailsTable.SetWidths(columnWidths);

                        // Add header row
                        PdfPCell headerCell = new PdfPCell(new Phrase("Medical Record Details",
                            new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                        headerCell.Colspan = 2;
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        headerCell.BackgroundColor = new BaseColor(220, 220, 220);
                        headerCell.Padding = 8f;
                        detailsTable.AddCell(headerCell);

                        
                        // Medical Record ID row
                        PdfPCell labelCell = new PdfPCell(new Phrase("Medical Record ID", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        labelCell.BackgroundColor = new BaseColor(240, 240, 240);
                        labelCell.Padding = 5f;
                        detailsTable.AddCell(labelCell);

                        PdfPCell valueCell = new PdfPCell(new Phrase(medicalRecord.MedicalRecordID.ToString()));
                        valueCell.Padding = 5f;
                        detailsTable.AddCell(valueCell);

                        // Patient row
                        labelCell = new PdfPCell(new Phrase("Patient", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        labelCell.BackgroundColor = new BaseColor(240, 240, 240);
                        labelCell.Padding = 5f;
                        detailsTable.AddCell(labelCell);

                        valueCell = new PdfPCell(new Phrase(medicalRecord.PatientID));
                        valueCell.Padding = 5f;
                        detailsTable.AddCell(valueCell);

                        // Created By row
                        labelCell = new PdfPCell(new Phrase("Created By", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        labelCell.BackgroundColor = new BaseColor(240, 240, 240);
                        labelCell.Padding = 5f;
                        detailsTable.AddCell(labelCell);

                        valueCell = new PdfPCell(new Phrase(medicalRecord.CreatedBy));
                        valueCell.Padding = 5f;
                        detailsTable.AddCell(valueCell);

                        // Date row
                        labelCell = new PdfPCell(new Phrase("Date", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        labelCell.BackgroundColor = new BaseColor(240, 240, 240);
                        labelCell.Padding = 5f;
                        detailsTable.AddCell(labelCell);

                        valueCell = new PdfPCell(new Phrase(medicalRecord.Date.ToString()));
                        valueCell.Padding = 5f;
                        detailsTable.AddCell(valueCell);

                        // Doctor Note row
                        labelCell = new PdfPCell(new Phrase("Doctor Note", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        labelCell.BackgroundColor = new BaseColor(240, 240, 240);
                        labelCell.Padding = 5f;
                        detailsTable.AddCell(labelCell);

                        valueCell = new PdfPCell(new Phrase(medicalRecord.DoctorNote));
                        valueCell.Padding = 5f;
                        detailsTable.AddCell(valueCell);

                        // Attachment row
                        labelCell = new PdfPCell(new Phrase("Attachment", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                        labelCell.BackgroundColor = new BaseColor(240, 240, 240);
                        labelCell.Padding = 5f;
                        detailsTable.AddCell(labelCell);
                    

                        doc.Add(detailsTable);

                        // Add erratum table if there are any erratums
                        if (filteredErratums.Count > 0)
                        {
                            // Create table for erratums
                            PdfPTable erratumTable = new PdfPTable(4);
                            erratumTable.WidthPercentage = 100;

                            // Set column widths for erratum table
                            float[] erratumColumnWidths = new float[] { 1f, 1f, 1f, 3f };
                            erratumTable.SetWidths(erratumColumnWidths);

                            // Add header row
                            PdfPCell erratumHeaderCell = new PdfPCell(new Phrase("Filed Erratums",
                                new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                            erratumHeaderCell.Colspan = 4;
                            erratumHeaderCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            erratumHeaderCell.BackgroundColor = new BaseColor(220, 220, 220);
                            erratumHeaderCell.Padding = 8f;
                            erratumTable.AddCell(erratumHeaderCell);

                            // Add column headers - inline implementation of AddHeaderCell
                            // Erratum ID header
                            PdfPCell headerCellErratum = new PdfPCell(new Phrase("Erratum ID", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                            headerCellErratum.BackgroundColor = new BaseColor(240, 240, 240);
                            headerCellErratum.Padding = 5f;
                            headerCellErratum.HorizontalAlignment = Element.ALIGN_CENTER;
                            erratumTable.AddCell(headerCellErratum);

                            // Filed By header
                            headerCellErratum = new PdfPCell(new Phrase("Filed By", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                            headerCellErratum.BackgroundColor = new BaseColor(240, 240, 240);
                            headerCellErratum.Padding = 5f;
                            headerCellErratum.HorizontalAlignment = Element.ALIGN_CENTER;
                            erratumTable.AddCell(headerCellErratum);

                            // Date Filed header
                            headerCellErratum = new PdfPCell(new Phrase("Date Filed", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                            headerCellErratum.BackgroundColor = new BaseColor(240, 240, 240);
                            headerCellErratum.Padding = 5f;
                            headerCellErratum.HorizontalAlignment = Element.ALIGN_CENTER;
                            erratumTable.AddCell(headerCellErratum);

                            // Details header
                            headerCellErratum = new PdfPCell(new Phrase("Details", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                            headerCellErratum.BackgroundColor = new BaseColor(240, 240, 240);
                            headerCellErratum.Padding = 5f;
                            headerCellErratum.HorizontalAlignment = Element.ALIGN_CENTER;
                            erratumTable.AddCell(headerCellErratum);

                            // Add data rows for each erratum
                            foreach (var erratum in filteredErratums)
                            {
                                erratumTable.AddCell(new PdfPCell(new Phrase(erratum.ErratumID.ToString())));
                                erratumTable.AddCell(new PdfPCell(new Phrase(erratum.CreatedBy)));
                                erratumTable.AddCell(new PdfPCell(new Phrase(erratum.Date.ToString())));
                                erratumTable.AddCell(new PdfPCell(new Phrase(erratum.ErratumDetails)));
                            }

                            doc.Add(erratumTable);
                        }

                        doc.Close();
                    }
                }
            }
            else
            {
                return "Unsupported format.";
            }

            // Handle medical record attachment
            if (medicalRecord.HasAttachment)
            {
                var originalRecord = await getOriginalRecordByID(recordID);
                (byte[] fileBytes, string fileName) = originalRecord.retrieveAttachment();
                string attachmentPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                await File.WriteAllBytesAsync(attachmentPath, fileBytes);
            }

            return $"Medical record exported to {filePath}.";
        }


    }

}