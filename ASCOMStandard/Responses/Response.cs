namespace ASCOM.Alpaca.Responses
{
    public class Response : IResponse
    {
        public int ClientTransactionID { get; set; }
        public int ServerTransactionID { get; set; }
        public int ErrorNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}