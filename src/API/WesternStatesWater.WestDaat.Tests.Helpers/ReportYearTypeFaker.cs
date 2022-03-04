using WesternStatesWater.WestDaat.Accessors.EntityFramework;

namespace WesternStatesWater.WestDaat.Tests.Helpers
{
    internal class ReportYearTypeFaker : Faker<ReportYearType>
    {
        public ReportYearTypeFaker()
        {
            this.RuleFor(a => a.Name, b => b.Random.String(10, 'A', 'z'))
                .RuleFor(a => a.Term, b => b.Random.String(10, 'A', 'z'));
        }
    }
}