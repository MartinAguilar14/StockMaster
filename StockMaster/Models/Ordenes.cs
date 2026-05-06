namespace StockMaster.Models
{
    public class Ordenes
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<OrdenDetalle> Detalles { get; set; }
    }
}
