namespace ArloBrau.Models
{
    public class Save
    {
        public int SaveId { get; set; }
        public string SaveName { get; set; }
        public DateTime SaveDate { get; set; }

        public string SaveDateFormatted
        {
            get
            {
                return SaveDate.ToString("dd/MM/yyyy 'às' HH:mm");
            }
        }
    }
}
