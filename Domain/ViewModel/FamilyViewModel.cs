using System.Collections.Generic;

namespace Domain.ViewModel
{
    public  class FamilyViewModel
    {
        public int Id { get; set; }
        public decimal Income { get; set; }
        public IEnumerable<FamilyMemberViewModel> FamilyMembers { get; set; }
    }
}
