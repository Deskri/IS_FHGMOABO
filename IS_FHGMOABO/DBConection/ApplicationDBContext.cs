using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IS_FHGMOABO.DAL;

namespace IS_FHGMOABO.DBConection
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<House> Houses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<LegalPerson> LegalPersons { get; set; }
        public DbSet<NaturalPerson> NaturalPersons { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Chairperson> Chairpersons { get; set; }
        public DbSet<CountingCommitteeMember> CountingCommitteeMembers { get; set; }
        public DbSet<Question> Questions { get; set; }

    }
}
