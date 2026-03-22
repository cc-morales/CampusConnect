namespace Domain
{
    public class PaginationRequestModel
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        
        public Guid? MyOrganizationId { get; set; }
        
        public bool IsGuest { get; set; }
    }
}
