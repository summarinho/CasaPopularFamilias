using System.Collections.Generic;

namespace Domain.DTOs
{
    public class FamilyInformationExternal
    {
        public int Id { get; set; }
        public decimal Income { get; set; }
        public IEnumerable<FamilyMembersExternal> FamilyMembers { get; set; }
    }
}
