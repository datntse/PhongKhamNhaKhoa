namespace Clinic.Core.Models
{
    public class ClinicDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public int SlotDuration { get; set; }
        public int MaxPatientsPerSlot { get; set; }
        public int MaxTreatmentPerSlot { get; set; }
        public DateTime CreateAt { get; set; }
        public string? OnwerId { get; set; } // Nullable OnwerId
    }
    public class UpdateClinic
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public int SlotDuration { get; set; }
        public int MaxPatientsPerSlot { get; set; }
        public int MaxTreatmentPerSlot { get; set; }
        public int Status { get; set; }
        public string? OnwerId { get; set; }
    }
}
