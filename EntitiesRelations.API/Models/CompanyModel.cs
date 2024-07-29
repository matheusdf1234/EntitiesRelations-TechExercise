namespace EntitiesRelations.API.Models
{
    public class CompanyModel : Entity
    {
        public CompanyModel(long id, string name) : base(id, name)
        {
            WhoOwnsMeList = new Dictionary<long, int>();
            AvailableShares = 100;
            IsControlled = false;
        }
        
        public int AvailableShares { get; set; }
        public Dictionary<long, int> WhoOwnsMeList { get; set; }
        public bool IsControlled { get; set; }
        
    }
}
