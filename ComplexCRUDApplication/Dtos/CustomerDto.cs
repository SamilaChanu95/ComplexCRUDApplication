namespace ComplexCRUDApplication.Dtos
{
    public class CustomerDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public Decimal CreditLimit { get; set; }
        public int TaxCode { get; set; }
        public string? StatusName { get; set; }
    }
}
