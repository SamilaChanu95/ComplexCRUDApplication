namespace ComplexCRUDApplication.Models
{
    public class ApiResponse
    {
        public int ResponseCode { get; set; }
        public string? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
